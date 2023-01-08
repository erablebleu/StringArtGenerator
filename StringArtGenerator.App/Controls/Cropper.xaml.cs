using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace StringArtGenerator.App.Controls;

/// <summary>
/// Interaction logic for Cropper.xaml
/// </summary>
public partial class Cropper : UserControl
{
    public static readonly DependencyProperty CropHeightProperty =
       DependencyProperty.RegisterAttached("CropHeight", typeof(int), typeof(Cropper), new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnPositionChanged));

    public static readonly DependencyProperty CropLeftProperty =
       DependencyProperty.RegisterAttached("CropLeft", typeof(int), typeof(Cropper), new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnPositionChanged));

    public static readonly DependencyProperty CropTopProperty =
       DependencyProperty.RegisterAttached("CropTop", typeof(int), typeof(Cropper), new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnPositionChanged));

    public static readonly DependencyProperty CropWidthProperty =
       DependencyProperty.RegisterAttached("CropWidth", typeof(int), typeof(Cropper), new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnPositionChanged));

    public static readonly DependencyProperty ImageSourceProperty =
                       DependencyProperty.RegisterAttached("ImageSource", typeof(ImageSource), typeof(Cropper), new FrameworkPropertyMetadata(null, OnImageSourceChanged));

    private readonly Dictionary<object, double[]> _dragFactors;
    private double[] _currentFactors;
    private double _initHeight;
    private double _initLeft;
    private Point _initPos;
    private double _initTop;
    private double _initWidth;
    private bool _isDragging;

    public Cropper()
    {
        InitializeComponent();
        _dragFactors = new Dictionary<object, double[]>()
        {
            { Rect_Cropping, new double[] { 1, 1, 0, 0 } },
            { RedimW, new double[] { 1, 0, -1, 0 } },
            { RedimN, new double[] { 0, 1, 0, -1 } },
            { RedimE, new double[] { 0, 0, 1, 0 } },
            { RedimS, new double[] { 0, 0, 0, 1 } },
            { RedimNW, new double[] { 1, 1, -1, -1 } },
            { RedimNE, new double[] { 0, 1, 1, -1 } },
            { RedimSE, new double[] { 0, 0, 1, 1 } },
            { RedimSW, new double[] { 1, 0, -1, 1 } },
        };
    }

    public int CropHeight { get => (int)GetValue(CropHeightProperty); set => SetValue(CropHeightProperty, value); }
    public int CropLeft { get => (int)GetValue(CropLeftProperty); set => SetValue(CropLeftProperty, value); }
    public int CropTop { get => (int)GetValue(CropTopProperty); set => SetValue(CropTopProperty, value); }
    public int CropWidth { get => (int)GetValue(CropWidthProperty); set => SetValue(CropWidthProperty, value); }
    public ImageSource ImageSource { get => (ImageSource)GetValue(ImageSourceProperty); set => SetValue(ImageSourceProperty, value); }

    private static void MoveUIElement(Rectangle el, double x, double y)
    {
        Canvas.SetLeft(el, x);
        Canvas.SetTop(el, y);
    }

    private static void MoveUIElement(Rectangle el, double x, double y, double width, double height)
    {
        Canvas.SetLeft(el, x);
        Canvas.SetTop(el, y);
        el.Width = width;
        el.Height = height;
    }

    private static void OnImageSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        Cropper c = (Cropper)d;
        c.Image_Source.Source = c.ImageSource;
        c.Canvas_Cropping.Width = c.ImageSource.Width;
        c.Canvas_Cropping.Height = c.ImageSource.Height;
    }

    private static void OnPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((Cropper)d)?.UpdatePosition();

    private void Move(double left, double top, double width, double height)
    {
        if (width < 0)
        {
            left += width;
            width = -width;
        }
        if (height < 0)
        {
            top += height;
            height = -height;
        }

        left = Math.Min(Math.Max(left, 0), ImageSource.Width - width);
        top = Math.Min(Math.Max(top, 0), ImageSource.Height - height);
        CropLeft = (int)left;
        CropTop = (int)top;
        CropWidth = (int)width;
        CropHeight = (int)height;

        MoveUIElement(Rect_Cropping, left, top, width, height);

        // redim rects
        MoveUIElement(RedimW, left, top + height / 2);
        MoveUIElement(RedimN, left + width / 2, top);
        MoveUIElement(RedimE, left + width, top + height / 2);
        MoveUIElement(RedimS, left + width / 2, top + height);

        MoveUIElement(RedimNW, left, top);
        MoveUIElement(RedimNE, left + width, top);
        MoveUIElement(RedimSE, left + width, top + height);
        MoveUIElement(RedimSW, left, top + height);

        // back rect
        MoveUIElement(BackLeft, 0, 0, left, ImageSource.Height);
        MoveUIElement(BackTop, left, 0, width, top);
        MoveUIElement(BackRight, left + width, 0, ImageSource.Width - left - width, ImageSource.Height);
        MoveUIElement(BackBottom, left, top + height, width, ImageSource.Height - top - height);
    }

    private void OnEndDrag(object sender, System.Windows.Input.MouseEventArgs e)
    {
        _isDragging = false;
    }

    private void OnPreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        _isDragging = true;
        _currentFactors = _dragFactors[sender];
        _initPos = e.GetPosition(Canvas_Cropping);
        _initLeft = Canvas.GetLeft(Rect_Cropping);
        _initTop = Canvas.GetTop(Rect_Cropping);
        _initWidth = Rect_Cropping.Width;
        _initHeight = Rect_Cropping.Height;
    }

    private void OnPreviewMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
    {
        if (!_isDragging)
            return;

        Vector d = e.GetPosition(Canvas_Cropping) - _initPos;

        Move(_initLeft + _currentFactors[0] * d.X,
             _initTop + _currentFactors[1] * d.Y,
             _initWidth + _currentFactors[2] * d.X,
             _initHeight + _currentFactors[3] * d.Y);
    }

    private void UpdatePosition()
    {
        if (_isDragging) return;

        Move(CropLeft, CropTop, CropWidth, CropHeight);
    }
}