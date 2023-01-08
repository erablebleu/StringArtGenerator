using StringArtGenerator.App.Adapters;
using StringArtGenerator.App.Resources.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace StringArtGenerator.App.Controls;

/// <summary>
/// Interaction logic for NailMapper.xaml
/// </summary>
public partial class Stepper : UserControl
{
    public static readonly DependencyProperty InstructionsProperty =
       DependencyProperty.RegisterAttached("Instructions", typeof(ObservableCollection<Model.ThreadInstruction>), typeof(Stepper),
           new FrameworkPropertyMetadata(null, OnLinesChanged));

    public static readonly DependencyProperty NailsProperty =
               DependencyProperty.RegisterAttached("Nails", typeof(ObservableCollection<NailAdapter>), typeof(Stepper),
           new FrameworkPropertyMetadata(null, OnNailsChanged));

    public static readonly DependencyProperty StepProperty =
       DependencyProperty.RegisterAttached("Step", typeof(int), typeof(Stepper),
           new FrameworkPropertyMetadata(1, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnStepChanged));

    public static readonly DependencyProperty ThreadsProperty =
           DependencyProperty.RegisterAttached("Threads", typeof(ObservableCollection<ThreadAdapter>), typeof(Stepper),
           new FrameworkPropertyMetadata(null, OnThreadsChanged));

    private readonly Dictionary<Line, Model.ThreadInstruction> _lines = new();
    private readonly Dictionary<ThreadAdapter, Brush> _brushes = new();

    public Stepper()
    {
        InitializeComponent();
    }

    public ObservableCollection<Model.ThreadInstruction> Instructions { get => (ObservableCollection<Model.ThreadInstruction>)GetValue(InstructionsProperty); set => SetValue(InstructionsProperty, value); }
    public ObservableCollection<NailAdapter> Nails { get => (ObservableCollection<NailAdapter>)GetValue(NailsProperty); set => SetValue(NailsProperty, value); }
    public int Step { get => (int)GetValue(StepProperty); set => SetValue(StepProperty, value); }
    public ObservableCollection<ThreadAdapter> Threads { get => (ObservableCollection<ThreadAdapter>)GetValue(ThreadsProperty); set => SetValue(ThreadsProperty, value); }

    private static LinearGradientBrush GetLinearGradient(Line l) =>
        new(Color.FromArgb(25, 0, 0, 0), Colors.Red, new Point(l.X1 > l.X2 ? 1 : 0, l.Y1 > l.Y2 ? 1 : 0), new Point(l.X1 < l.X2 ? 1 : 0, l.Y1 < l.Y2 ? 1 : 0));

    private static void OnLinesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not Stepper nm) return;
        if (e.OldValue is ObservableCollection<Model.ThreadInstruction> oldC) oldC.CollectionChanged -= nm.OnLinesCollectionChanged;
        if (e.NewValue is ObservableCollection<Model.ThreadInstruction> newC) newC.CollectionChanged += nm.OnLinesCollectionChanged;
        nm.OnLinesCollectionChanged(null, null);
    }

    private static void OnNailsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not Stepper nm) return;
        if (e.OldValue is ObservableCollection<NailAdapter> oldC) oldC.CollectionChanged -= nm.OnNailsCollectionChanged;
        if (e.NewValue is ObservableCollection<NailAdapter> newC) newC.CollectionChanged += nm.OnNailsCollectionChanged;
        nm.OnNailsCollectionChanged(null, null);
    }

    private static void OnStepChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not Stepper nm) return;
        nm.OnStepChanged();
    }

    private static void OnThreadsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not Stepper nm) return;
        nm.OnThreadsChanged();
    }

    private void SetLineProperties(Line line, int idx)
    {
        if(Threads is not null && Threads.Count > idx && idx >= 0)
        {
            ThreadAdapter thread = Threads[idx];
            line.Stroke = _brushes[thread];
            line.StrokeThickness = thread.PreviewThickness;
        }
        else
        {
            line.Stroke = Brushes.Black;
            line.StrokeThickness = 0.1;
        }
    }

    private double GetThreadThickness(int idx) => Threads is null || Threads.Count > idx || idx >= 0 ? 0.1 : Threads[idx].PreviewThickness;

    private void AddLine(Model.ThreadInstruction adapter)
    {
        Line line = new()
        {
            X1 = adapter.Nail1RealPos.X,
            Y1 = adapter.Nail1RealPos.Y,
            X2 = adapter.Nail2RealPos.X,
            Y2 = adapter.Nail2RealPos.Y,
        };
        SetLineProperties(line, adapter.ThreadIndex);
        lineGroup.Children.Add(line);
        _lines.Add(line, adapter);
    }

    private Rect GetMapArea() => new(new Point(Nails.Min(n => n.Position.X), Nails.Min(n => n.Position.Y)), new Point(Nails.Max(n => n.Position.X), Nails.Max(n => n.Position.Y)));

    private void GoToFirstStep(object sender, RoutedEventArgs e)
    {
        Step = 0;
    }

    private void GoToLastStep(object sender, RoutedEventArgs e)
    {
        Step = Instructions.Count - 1;
    }

    private void GoToNextStep(object sender, RoutedEventArgs e)
    {
        Step++;
    }

    private void GoToPreviousStep(object sender, RoutedEventArgs e)
    {
        Step--;
    }

    private void OnLinesCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e is null || e.Action == NotifyCollectionChangedAction.Reset)
        {
            lineGroup.Children.Clear();
            _lines.Clear();
            if (Instructions is null) return;
            foreach (Model.ThreadInstruction line in Instructions)
                AddLine(line);
        }
        else
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (Model.ThreadInstruction line in e.NewItems)
                        AddLine(line);
                    break;

                case NotifyCollectionChangedAction.Remove:
                    break;

                case NotifyCollectionChangedAction.Replace:
                    break;

                case NotifyCollectionChangedAction.Move:
                    break;

                case NotifyCollectionChangedAction.Reset:
                    break;
            }
        }
    }

    private void OnNailsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        nailsGroup.ItemsSource = Nails;

        if (!Nails.Any())
            return;

        Rect area = GetMapArea();
        double margin = Math.Max(area.Width, area.Height) / 20;
        canvas.RenderTransform = new TranslateTransform(-area.X + margin, -area.Y + margin);
        canvas.Width = area.Width + 2 * margin;
        canvas.Height = area.Height + 2 * margin;
    }

    private void OnStepChanged()
    {
        Stopwatch sw = new();
        sw.Start();
        int step = Math.Min(Step, _lines.Count - 1);
        for (int i = 0; i < step + 1; i++)
        {
            KeyValuePair<Line, Model.ThreadInstruction> kv = _lines.ElementAt(i);
            Line l = kv.Key;
            if(i == step)
            {
                l.Stroke = GetLinearGradient(l);
                l.StrokeThickness = 10 * GetThreadThickness(kv.Value.ThreadIndex);
            }
            else
                SetLineProperties(l, kv.Value.ThreadIndex);

            l.Visibility = Visibility.Visible;
        }
        for (int i = step + 1; i < _lines.Count; i++)
        {
            Line l = _lines.Keys.ElementAt(i);
            l.Visibility = Visibility.Hidden;
        }

        if (Step < Instructions?.Count && Step > 0)
        {
            Model.ThreadInstruction instruction = Instructions[Step];

            tbSource.Text = $"{instruction.Nail1Index} ({instruction.Nail1Out})";
            tbDestination.Text = $"{instruction.Nail2Index} ({instruction.Nail2In})";
            tbStep.Text = $"{Step + 1}";

            double angle = Vector.AngleBetween(new Vector(0, -1), instruction.Nail2RealPos - instruction.Nail1RealPos);

            switch (instruction.Nail2In)
            {
                case TangentSide.Left:
                    LeftArrowTranslate.X = instruction.Nail2RealPos.X;
                    LeftArrowTranslate.Y = instruction.Nail2RealPos.Y;
                    LeftArrow.Visibility = Visibility.Visible;
                    RightArrow.Visibility = Visibility.Collapsed;
                    LeftArrowRotation.Angle = angle;
                    break;

                case TangentSide.Right:
                    RightArrowTranslate.X = instruction.Nail2RealPos.X;
                    RightArrowTranslate.Y = instruction.Nail2RealPos.Y;
                    LeftArrow.Visibility = Visibility.Collapsed;
                    RightArrow.Visibility = Visibility.Visible;
                    RightArrowRotation.Angle = angle;
                    break;
            }
        }

        buttonFirstStep.IsEnabled = Step > 0;
        buttonPreviousStep.IsEnabled = Step > 0;
        buttonNextStep.IsEnabled = Step < Instructions?.Count;
        buttonLastStep.IsEnabled = Step < Instructions?.Count;

        sw.Stop();
        log.Text = $"{sw.ElapsedMilliseconds} ms";
    }

    private void OnThreadsChanged()
    {
        _brushes.Clear();
        foreach (ThreadAdapter thread in Threads)
            _brushes.Add(thread, new SolidColorBrush(thread.Color));

        foreach(KeyValuePair<Line, Model.ThreadInstruction> kv in _lines)
            SetLineProperties(kv.Key, kv.Value.ThreadIndex);
    }
}