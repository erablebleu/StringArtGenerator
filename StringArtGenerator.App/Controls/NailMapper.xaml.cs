using StringArtGenerator.App.Adapters;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace StringArtGenerator.App.Controls;

/// <summary>
/// Interaction logic for NailMapper.xaml
/// </summary>
public partial class NailMapper : UserControl
{
    public static readonly DependencyProperty LineThicknessProperty =
       DependencyProperty.RegisterAttached("LineThickness", typeof(double), typeof(NailMapper),
           new FrameworkPropertyMetadata(0.1, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnLineThicknessChanged));

    public static readonly DependencyProperty NailsProperty =
       DependencyProperty.RegisterAttached("Nails", typeof(ObservableCollection<NailAdapter>), typeof(NailMapper),
           new FrameworkPropertyMetadata(null, OnNailsChanged));

    public static readonly DependencyProperty LinesProperty =
       DependencyProperty.RegisterAttached("Lines", typeof(ObservableCollection<LineAdapter>), typeof(NailMapper),
           new FrameworkPropertyMetadata(null, OnLinesChanged));

    public static readonly DependencyProperty SelectedNailProperty =
       DependencyProperty.RegisterAttached("SelectedNail", typeof(NailAdapter), typeof(NailMapper),
           new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedNailChanged));

    public static readonly DependencyProperty ScaleProperty =
       DependencyProperty.RegisterAttached("Scale", typeof(double), typeof(NailMapper),
           new FrameworkPropertyMetadata(1d, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnScaleChanged));

    public static readonly DependencyProperty PositionProperty =
       DependencyProperty.RegisterAttached("Position", typeof(Point), typeof(NailMapper),
           new FrameworkPropertyMetadata(new Point(), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnPositionChanged));

    public static readonly DependencyProperty SrcImageProperty =
                   DependencyProperty.RegisterAttached("SrcImage", typeof(BitmapImage), typeof(NailMapper),
           new FrameworkPropertyMetadata(null, OnImageChanged));

    private readonly Rectangle[] _mapArea;
    private readonly ScaleTransform _scaleTransform;
    private Point _cvPos;
    private object? _dragged;
    private double _scale = 1;
    private double _svHorizontalOffset;
    private Point _svPos;
    private double _svVerticalOffset;
    private Point _initialPosition;
    private Point _lastPos;

    public NailMapper()
    {
        InitializeComponent();
        _scaleTransform = new() { ScaleX = 1, ScaleY = 1 };
        canvas.LayoutTransform = _scaleTransform;
        _mapArea = new Rectangle[] { mapArea0, mapArea1, mapArea2, mapArea3 };
    }

    public double LineThickness { get => (double)GetValue(LineThicknessProperty); set => SetValue(LineThicknessProperty, value); }
    public ObservableCollection<NailAdapter> Nails { get => (ObservableCollection<NailAdapter>)GetValue(NailsProperty); set => SetValue(NailsProperty, value); }
    public ObservableCollection<LineAdapter> Lines { get => (ObservableCollection<LineAdapter>)GetValue(LinesProperty); set => SetValue(LinesProperty, value); }
    public NailAdapter SelectedNail { get => (NailAdapter)GetValue(SelectedNailProperty); set => SetValue(SelectedNailProperty, value); }
    public BitmapImage SrcImage { get => (BitmapImage)GetValue(SrcImageProperty); set => SetValue(SrcImageProperty, value); }
    public double Scale { get => (double)GetValue(ScaleProperty); set => SetValue(ScaleProperty, value); }
    public Point Position { get => (Point)GetValue(PositionProperty); set => SetValue(PositionProperty, value); }

    private static void OnImageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not NailMapper nm) return;
        nm.srcImage.Source = nm.SrcImage;
        nm.canvas.Width = nm.SrcImage?.PixelWidth ?? 0;
        nm.canvas.Height = nm.SrcImage?.PixelHeight ?? 0;

        nm.srcImage.Width = nm.SrcImage?.PixelWidth ?? 0;
        nm.srcImage.Height = nm.SrcImage?.PixelHeight ?? 0;
    }

    private static void OnLineThicknessChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not NailMapper nm) return;
        nm.OnLineThicknessChanged();
    }

    private static void OnNailsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not NailMapper nm) return;
        if (e.OldValue is ObservableCollection<NailAdapter> oldC) oldC.CollectionChanged -= nm.OnNailsCollectionChanged;
        if (e.NewValue is ObservableCollection<NailAdapter> newC) newC.CollectionChanged += nm.OnNailsCollectionChanged;
        nm.OnNailsCollectionChanged(null, null);
    }

    private static void OnLinesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not NailMapper nm) return;
        if (e.OldValue is ObservableCollection<LineAdapter> oldC) oldC.CollectionChanged -= nm.OnLinesCollectionChanged;
        if (e.NewValue is ObservableCollection<LineAdapter> newC) newC.CollectionChanged += nm.OnLinesCollectionChanged;
        nm.OnLinesCollectionChanged(null, null);
    }

    

    private static void OnSelectedNailChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not NailMapper nm) return;
        nm.OnSelectedNailChanged();
    }
    private static void OnScaleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not NailMapper nm) return;
        nm.OnScaleChanged();
    }
    private static void OnPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not NailMapper nm) return;
        nm.OnPositionChanged();
    }
    private void OnPositionChanged()
    {
        UpdateMapArea();
    }
    private void OnScaleChanged()
    {
        UpdateMapArea();
    }
    private void EndDrag()
    {
        _dragged = null;
    }

    private Rect GetMapArea(double scale) => new(
        new Point(Position.X + scale * Nails.Min(n => n.Position.X), Position.Y + scale * Nails.Min(n => n.Position.Y)), 
        new Point(Position.X + scale * Nails.Max(n => n.Position.X), Position.Y + scale * Nails.Max(n => n.Position.Y)));

    private void MoveNail(Ellipse e, Point p)
    {
        //nailsGroup.ItemsPanel
        NailAdapter? nail = e.DataContext as NailAdapter;
        if(nail is null) return;
        nail.Position = p;
    }

    private void OnEndDrag(object sender, MouseEventArgs e) => EndDrag();

    private void OnEndDrag(object sender, MouseButtonEventArgs e) => EndDrag();

    private void OnLineThicknessChanged()
    {
        //foreach (var kv in _lines)
        //    kv.Key.StrokeThickness = LineThickness;
    }

    private void OnMouseWheel(object sender, MouseWheelEventArgs e)
    {
        if (e.Delta > 0)
            _scale *= 1.5;
        else
            _scale /= 1.5;

        _scaleTransform.ScaleX = _scale;
        _scaleTransform.ScaleY = _scale;
    }

    private void OnNailsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        nailsGroup.ItemsSource = Nails;
        UpdateMapArea();
    }

    private void OnLinesCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        OnSelectedNailChanged();
    }

    private void OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        _dragged = Mouse.DirectlyOver;
        _initialPosition = Position;
        _svPos = e.GetPosition(sv);
        _cvPos = e.GetPosition(canvas);
        _svVerticalOffset = sv.VerticalOffset;
        _svHorizontalOffset = sv.HorizontalOffset;
        
        if(_dragged is Ellipse ellipse)
        {
            NailAdapter? nail = ellipse.DataContext as NailAdapter;
            if (nail is not null)
                SelectedNail = nail;
        }
    }

    private void SelectNail(NailAdapter nail)
    {
        if (nail is null)
            UnselectNail();
        else
        {
            Canvas.SetLeft(selectedNail, nail.Position.X);
            Canvas.SetTop(selectedNail, nail.Position.Y);
            selectedNail.Visibility = Visibility.Visible;
            selectedNail.Width = nail.Diameter + 5;
            selectedNail.Height = nail.Diameter + 5;
        }
    }

    private void UnselectNail()
    {
        selectedNail.Visibility = Visibility.Hidden;
    }

    private void OnPreviewMouseMove(object sender, MouseEventArgs e)
    {
        Point pos = e.GetPosition(canvas);
        if (_lastPos == pos) return;
        Ellipse? mo = Mouse.DirectlyOver as Ellipse;

        if (mo?.DataContext is not NailAdapter nail)
            UnselectNail();
        else
            SelectNail(nail);

        if (_dragged is null)
            return;

        if (_dragged == mover) // move nails
        {
            Vector d = e.GetPosition(canvas) - _cvPos;
            Position = new Point(_initialPosition.X + d.X, _initialPosition.Y + d.Y);
        }
        else if (_dragged == resizer) // resize nails
        {
            Point origin = Position;
            Rect area = GetMapArea(1);
            Point max = new (canvas.Width - Position.X, canvas.Height - Position.Y);
            if (pos.Y <= origin.Y) return;

            Scale = new double[] {
                //(pos.X - origin.X) / (max.X - origin.X),
               // (pos.Y - origin.Y) / (max.Y - origin.Y),
               //(canvas.Width - origin.X) / (max.X - origin.X),
               //(canvas.Height - origin.Y) / (max.Y - origin.Y),
               (pos.Y - origin.Y) / area.Height,
            }.Min();


        }
        else if (_dragged is Ellipse ellipse) // drag nail
        {
            MoveNail(ellipse, e.GetPosition(nailsGroup));
            UpdateMapArea();
        }
        else // drag canvas
        {
            Vector d = e.GetPosition(sv) - _svPos;
            sv.ScrollToVerticalOffset(_svVerticalOffset - d.Y);
            sv.ScrollToHorizontalOffset(_svHorizontalOffset - d.X);
        }
        _lastPos = pos;
    }

    private void OnSelectedNailChanged()
    {
        SelectNail(SelectedNail);
        linesGroup.ItemsSource = new ObservableCollection<LineAdapter>(Lines.Where(l => l.Nail1 == SelectedNail || l.Nail2 == SelectedNail));
    }

    private void UpdateMapArea()
    {
        if (Nails.Count == 0) return;
        Rect area = GetMapArea(Scale);

        SetMapArea(_mapArea[0], 0, 0, area.X, canvas.Height);
        SetMapArea(_mapArea[1], area.X, 0, area.Width, area.Y);
        SetMapArea(_mapArea[2], area.X, area.Y + area.Height, area.Width, canvas.Height - area.Height - area.Y);
        SetMapArea(_mapArea[3], area.X + area.Width, 0, canvas.Width - area.Width - area.X, canvas.Height);

        Canvas.SetLeft(resizer, area.X + 2 * area.Width / 3);
        Canvas.SetTop(resizer, area.Y + area.Height);

        Canvas.SetLeft(mover, area.X + area.Width / 3);
        Canvas.SetTop(mover, area.Y + area.Height);
    }

    private void SetMapArea(Rectangle rect, double x, double y, double width, double height)
    {
        if (width > 0) rect.Width = width;
        if (height > 0) rect.Height = height;
        Canvas.SetLeft(rect, x);
        Canvas.SetTop(rect, y);
        rect.Visibility = width > 0 && height > 0 ? Visibility.Visible : Visibility.Hidden;
    }
}