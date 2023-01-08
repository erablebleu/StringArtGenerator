using StringArtGenerator.App.Adapters;
using StringArtGenerator.App.Model;
using StringArtGenerator.App.Resources.Enums;
using StringArtGenerator.App.Tools;
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
public partial class Stepper2 : UserControl
{
    public static readonly DependencyProperty InstructionsProperty =
       DependencyProperty.RegisterAttached("Instructions", typeof(ObservableCollection<Model.ThreadInstruction>), typeof(Stepper2),
           new FrameworkPropertyMetadata(null, OnLinesChanged));

    public static readonly DependencyProperty NailsProperty =
               DependencyProperty.RegisterAttached("Nails", typeof(ObservableCollection<NailAdapter>), typeof(Stepper2),
           new FrameworkPropertyMetadata(null, OnNailsChanged));

    public static readonly DependencyProperty StepProperty =
       DependencyProperty.RegisterAttached("Step", typeof(int), typeof(Stepper2),
           new FrameworkPropertyMetadata(1, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnStepChanged));

    public static readonly DependencyProperty ThreadsProperty =
           DependencyProperty.RegisterAttached("Threads", typeof(ObservableCollection<ThreadAdapter>), typeof(Stepper2),
           new FrameworkPropertyMetadata(null, OnThreadsChanged));

    public Stepper2()
    {
        InitializeComponent();
    }

    public ObservableCollection<Model.ThreadInstruction> Instructions { get => (ObservableCollection<Model.ThreadInstruction>)GetValue(InstructionsProperty); set => SetValue(InstructionsProperty, value); }
    public ObservableCollection<NailAdapter> Nails { get => (ObservableCollection<NailAdapter>)GetValue(NailsProperty); set => SetValue(NailsProperty, value); }
    public int Step { get => (int)GetValue(StepProperty); set => SetValue(StepProperty, value); }
    public ObservableCollection<ThreadAdapter> Threads { get => (ObservableCollection<ThreadAdapter>)GetValue(ThreadsProperty); set => SetValue(ThreadsProperty, value); }

    private static void OnLinesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not Stepper2 nm) return;
        if (e.OldValue is ObservableCollection<Model.ThreadInstruction> oldC) oldC.CollectionChanged -= nm.OnLinesCollectionChanged;
        if (e.NewValue is ObservableCollection<Model.ThreadInstruction> newC) newC.CollectionChanged += nm.OnLinesCollectionChanged;
        nm.OnLinesCollectionChanged(null, null);
    }

    private static void OnNailsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not Stepper2 nm) return;
        if (e.OldValue is ObservableCollection<NailAdapter> oldC) oldC.CollectionChanged -= nm.OnNailsCollectionChanged;
        if (e.NewValue is ObservableCollection<NailAdapter> newC) newC.CollectionChanged += nm.OnNailsCollectionChanged;
        nm.OnNailsCollectionChanged(null, null);
    }

    private static void OnStepChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not Stepper2 nm) return;
        nm.OnStepChanged();
    }

    private static void OnThreadsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not Stepper2 nm) return;
        nm.OnThreadsChanged();
    }

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

    private void Refresh()
    {
        Stopwatch sw = new();
        sw.Start();
        img.Width = grid.ActualWidth;
        img.Height = grid.ActualHeight;
        canvas.Width = Nails?.Max(n => n.Position.X) ?? 0;
        canvas.Height = Nails?.Max(n => n.Position.Y) ?? 0;
        img.Source = PreviewBuilder.Build(Threads, Instructions?.Take(Step + 1).ToList(), (int)img.Width, (int)img.Height, true)?.ToBitmapImage();

        if (Step < Instructions?.Count && Step >= 0)
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

    private void OnLinesCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        Refresh();
    }

    private void OnNailsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        Refresh();
    }

    private void OnStepChanged()
    {
        Refresh();
    }

    private void OnThreadsChanged()
    {
        Refresh();
    }

    private void grid_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        Refresh();
    }
}