using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace StringArtGenerator.App.Controls;

/// <summary>
/// Interaction logic for ImageComparator.xaml
/// </summary>
public partial class ImageComparator : UserControl
{
    public static readonly DependencyProperty Image1Property =
                   DependencyProperty.RegisterAttached("Image1", typeof(BitmapImage), typeof(ImageComparator),
           new FrameworkPropertyMetadata(null, OnImageChanged));

    public static readonly DependencyProperty Image2Property =
                   DependencyProperty.RegisterAttached("Image2", typeof(BitmapImage), typeof(ImageComparator),
           new FrameworkPropertyMetadata(null, OnImageChanged));

    public static readonly DependencyProperty SplittingOrientationProperty =
                   DependencyProperty.RegisterAttached("SplittingOrientation", typeof(Orientation), typeof(ImageComparator),
           new FrameworkPropertyMetadata(Orientation.Horizontal, SplittingOrientationChanged));

    private bool _drag;

    public ImageComparator()
    {
        InitializeComponent();
        SizeChanged += (s, o) => UpdateSplitterSize();
    }

    public BitmapImage Image1 { get => (BitmapImage)GetValue(Image1Property); set => SetValue(Image1Property, value); }
    public BitmapImage Image2 { get => (BitmapImage)GetValue(Image2Property); set => SetValue(Image2Property, value); }
    public Orientation SplittingOrientation { get => (Orientation)GetValue(SplittingOrientationProperty); set => SetValue(SplittingOrientationProperty, value); }

    private static void OnImageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not ImageComparator nm) return;
        nm.OnImageChanged();
    }

    private static void SplittingOrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not ImageComparator nm) return;
        nm.SplittingOrientationChanged();
    }

    private void EndDrag()
    {
        _drag = false;
    }

    private void OnImageChanged()
    {
        ctrlImg1.Source = Image1;
        ctrlImg2.Source = Image2;
        ctrlImg1.Width = Image1?.PixelWidth ?? 0;
        ctrlImg1.Width = Image1?.PixelWidth ?? 0;
        ctrlImg2.Width = Image1?.PixelWidth ?? 0;
        ctrlImg2.Width = Image1?.PixelWidth ?? 0;
        canvas.Width = Image1?.PixelWidth ?? Image2?.PixelWidth ?? 0;
        canvas.Height = Image1?.PixelHeight ?? Image2?.PixelHeight ?? 0;
        if (canvas.ActualHeight != Image1?.PixelHeight
            || canvas.ActualWidth != Image1?.PixelWidth)
            ResetSplitter();
        SetImageClip();
    }

    private void OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        _drag = true;
    }

    private void OnMouseLeave(object sender, MouseEventArgs e) => EndDrag();

    private void OnMouseMove(object sender, MouseEventArgs e)
    {
        if (!_drag) return;
        Point pos = e.GetPosition(canvas);

        switch (SplittingOrientation)
        {
            case Orientation.Horizontal:
                Canvas.SetTop(splitter, pos.Y);
                break;

            case Orientation.Vertical:
                Canvas.SetLeft(splitter, pos.X);
                break;
        }
        SetImageClip();
    }

    private void OnMouseUp(object sender, MouseButtonEventArgs e) => EndDrag();

    private void ResetSplitter()
    {
        switch (SplittingOrientation)
        {
            case Orientation.Horizontal:
                Canvas.SetTop(splitter, canvas.Height / 2);
                Canvas.SetLeft(splitter, 0);
                splitter.Width = canvas.Width;
                splitter.Cursor = Cursors.SizeNS;
                break;

            case Orientation.Vertical:
                Canvas.SetTop(splitter, 0);
                Canvas.SetLeft(splitter, canvas.Width / 2);
                splitter.Height = canvas.Height;
                splitter.Cursor = Cursors.SizeWE;
                break;
        }
        UpdateSplitterSize();
    }
    private void UpdateSplitterSize()
    {
        switch (SplittingOrientation)
        {
            case Orientation.Horizontal:
                splitter.Height = ActualHeight == 0 ? 0 : 5 * canvas.Height / ActualHeight;
                break;

            case Orientation.Vertical:
                splitter.Width = ActualWidth == 0 ? 0 : 5 * canvas.Width / ActualWidth;
                break;
        }
    }

    private void SetImageClip()
    {
        switch (SplittingOrientation)
        {
            case Orientation.Horizontal:
                double y = Canvas.GetTop(splitter);
                rectImg1.Rect = new Rect(0, 0, canvas.Width, y);
                rectImg2.Rect = new Rect(0, y, canvas.Width, canvas.Height - y);
                break;
            case Orientation.Vertical:
                double x = Canvas.GetLeft(splitter);
                rectImg1.Rect = new Rect(0, 0, x, canvas.Height);
                rectImg2.Rect = new Rect(x, 0, canvas.Width - x, canvas.Height);
                break;
        }
    }

    private void SplittingOrientationChanged()
    {
        if(!_isOrientationUpdating)
        {
            switch (SplittingOrientation)
            {
                case Orientation.Horizontal: rbHori.IsChecked = true; break;
                case Orientation.Vertical: rbVert.IsChecked = true; break;
            }
        }
        ResetSplitter();
        SetImageClip();
    }

    private void SetHorizontal(object sender, RoutedEventArgs e)
    {
        SplittingOrientation = Orientation.Horizontal;
    }

    private void SetVertical(object sender, RoutedEventArgs e)
    {
        SplittingOrientation = Orientation.Vertical;
    }

    private bool _isOrientationUpdating;

    private void ChangeOrientation(object sender, RoutedEventArgs e)
    {
        _isOrientationUpdating = true;

        if (sender == rbHori)
            SplittingOrientation = Orientation.Horizontal;
        if (sender == rbVert)
            SplittingOrientation=Orientation.Vertical;

        _isOrientationUpdating = false;
    }
}