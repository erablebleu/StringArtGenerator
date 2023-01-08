using AutoMapper;
using StringArtGenerator.App.Adapters;
using StringArtGenerator.App.Model;
using StringArtGenerator.App.Resources.Enums;
using StringArtGenerator.App.Resources.Extensions;
using StringArtGenerator.App.Tools;
using StringArtGenerator.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;

namespace StringArtGenerator.App.ApplicationServices;
public class CalculatorApplicationService : ApplicationServiceBase
{
    private CalculationResultAdapter _result = new();
    private ICommand? _cancelCommand;
    private ICommand? _calculateCommand;
    private bool _isCalculating;
    private System.Threading.CancellationTokenSource _cts = new();

    public ICommand CalculateCommand => _calculateCommand ??= new RelayCommand(OnCalculateCommand);
    public ICommand CancelCommand => _cancelCommand ??= new RelayCommand(OnCancelCommand);
    public bool IsCalculating { get => _isCalculating; set => Set(ref _isCalculating, value); }
    public CalculationResultAdapter Result { get => _result; set => Set(ref _result, value); }

    [Injectable] public ProjectApplicationService ProjectApplicationService { get; set; }
    [Injectable] public SettingsApplicationService SettingsApplicationService { get; set; }

    private class ThreadData
    {
        public Thread Thread { get; set; }
        public ThreadAdapter ThreadAdapter { get; set; }
        public Line? CurrentLine { get; set; }
        public Nail? CurrentNail { get; set; }
        public List<Line> Sequence { get; set; } = new();
        public Queue<Nail> ReverseQueue { get; set; } = new();
        public double[,] Data { get; set; }
        public double[,] TargetData { get; set; }

        public ThreadData(Thread thread, ThreadAdapter adapter, System.Drawing.Bitmap bmp)
        {
            Thread = thread;
            ThreadAdapter = adapter;
            adapter.StepCount = 0;
            adapter.TotalLength = 0;

            byte[,] r, g, b, a;
            (r, g, b, a) = bmp.GetData2D();

            TargetData = (thread.ColorAdjustment.IsActive ? r : a).Select(v => 1d - (double)v / 255);
            Data = a.Select(v => 0d);
        }
    }
    
    private void OnCalculateCommand()
    {
        Dispatcher dispatcher = Dispatcher.CurrentDispatcher;
        Task.Factory.StartNew((arg) =>
        {
            _cts = new();
            IsCalculating = true;
            if (arg is not ProjectAdapter prj) return;
            Calculate(dispatcher, prj);
            IsCalculating = false;
        }, ProjectApplicationService.CurrentProject);
    }
    private void OnCancelCommand()
    {
        _cts.Cancel();
    }

    public void Calculate(Dispatcher dispatcher, ProjectAdapter project)
    {
        NailMap map = Mapper.Map<NailMap>(project.NailMap);
        Result = new();
        project.CalculationResult = Result;

        CalculationOptionsAdapter options = project.CalculationOptions;

        Result.SetStep("Scanning map");

        // Get scale
        System.Drawing.Bitmap bmp = project.ImageOptions.SizeOption.ResultImage.ToBitmap();
        double xMax = map.Nails.Max(n => n.Position.X);
        double yMax = map.Nails.Max(n => n.Position.Y);

        double scale = Math.Min(bmp.Width / xMax, bmp.Height / yMax);
        LineFactory lf = new(options.UseBresenham, scale, map.Nails, bmp.Width, bmp.Height);

        // init thread data
        ThreadData[] threadDatas = project.Threads.Select(adapter =>
        {
            Thread thread = Mapper.Map<Thread>(adapter);
            thread.CalculationThickness *= scale;
            return new ThreadData(thread, adapter, thread.FilteredImage.ToBitmap());
        }).ToArray();

        // Create Map
        Result.StartLocalProgress(0.2);
        Result.SetStep("Pixel lines generation");
        for (int i = 0; i < map.Nails.Count; i++)
        {
            Nail nail1 = map.Nails[i];
            List<int> targets = map.GetTargets(i).ToList();

            nail1.LinesL = lf.GetLines(targets, i, TangentSide.Left);
            nail1.LinesR = lf.GetLines(targets, i, TangentSide.Right);
            nail1.Lines = nail1.LinesL.Concat(nail1.LinesR).ToArray();
            Result.SetLocalProgress((double)i / map.Nails.Count);

            if (_cts.IsCancellationRequested)
                return;
        }
        Result.SetProgress(0.2);

        Result.StartLocalProgress(0.7);
        Result.SetStep("Threading");
        int totalLineCount = threadDatas.Sum(td => td.Thread.MaxLineCount);

        Parallel.ForEach(threadDatas, td =>
        {
            while (td.Sequence.Count < td.Thread.MaxLineCount)
            {
                if (td.CurrentNail is null)
                    td.CurrentNail = map.Nails.First();

                IEnumerable<Line> lines = /*options.Continuity &&*/ td.CurrentLine is null
                    ? map.Nails.SelectMany(n => n.Lines).Distinct()
                    : td.CurrentNail.GetLines(td.CurrentLine.Nail2In);

                // Best line
                td.CurrentLine = null;
                double ind = 0;
                foreach (Line l in lines)
                {
                    if (td.ReverseQueue.Contains(l.Nail2))
                        continue;
                    double ni = l.GetIndicator(options.MaximizationFunction, td.Data, td.TargetData, td.Thread.CalculationThickness);
                    if (ni <= ind)
                        continue;
                    td.CurrentLine = l;
                    ind = ni;
                }

                if (td.CurrentLine is null)
                    break;

                td.CurrentLine.Apply(td.Data, td.Thread.CalculationThickness);

                td.Sequence.Add(td.CurrentLine);
                td.ThreadAdapter.StepCount++;
                td.ThreadAdapter.TotalLength += (td.CurrentLine.Nail2RealPos - td.CurrentLine.Nail1RealPos).Length / 1000;
                //dispatcher.Invoke(() => project.CalculationResult.Instructions.Add(td.CurrentLine.GetInstruction(Array.IndexOf(threads, td.Thread)));
                td.CurrentNail = td.CurrentLine.Nail2;
                if (options.AntiReverse)
                {
                    td.ReverseQueue.Enqueue(td.CurrentLine.Nail2);
                    if (td.ReverseQueue.Count > options.AntiReverseQueueSize)
                        td.ReverseQueue.Dequeue();
                }

                Result.SetLocalProgress((double)threadDatas.Sum(dt => dt.ThreadAdapter.StepCount) / totalLineCount);

                if (_cts.IsCancellationRequested)
                    return;
            }
        });

        Result.SetProgress(0.9);
        Result.StartLocalProgress(0.1);
        Result.SetStep("Color mix");

        // Combine threads steps
        int idx = 0;
        int newAdd;
        do
        {
            newAdd = 0;
            foreach (ThreadData td in threadDatas)
            {
                if (idx >= td.Sequence.Count)
                    continue;

                newAdd++;
                ThreadInstruction instruction = td.Sequence[td.Sequence.Count - idx - 1].GetInstruction(project.Threads.IndexOf(td.ThreadAdapter)).Reverse();
                dispatcher.Invoke(() => Result.Instructions.Add(instruction));
            }

            Result.SetLocalProgress((double)Result.Instructions.Count / totalLineCount);
            idx++;
        }
        while (newAdd > 0);

        Result.SetProgress(1);
        Result.SetStep("Calculation is complete");

        /*
        if (options.Continuity)
            return sequence.Select(l => l.Nail2Index).ToArray();
        else // Rebuilt Continuity
        {
            // Search for min dist continuity
            List<(int, int)> lines = sequence.Select(l => (l.Nail1Index, l.Nail2Index)).ToList();
            List<int> result = new();
            int nail = 0;

            while (lines.Any())
            {
                int dst = int.MaxValue;
                bool reverse = false;
                (int, int) line = (0, 0);

                foreach ((int, int) l in lines)
                {
                    if (Math.Abs(nail - l.Item1) < dst)
                    {
                        dst = Math.Abs(nail - l.Item1);
                        line = l;
                        reverse = false;
                    }
                    if (Math.Abs(nail - l.Item2) < dst)
                    {
                        dst = Math.Abs(nail - l.Item2);
                        line = l;
                        reverse = true;
                    }
                }

                if ((reverse ? line.Item2 : line.Item1) != nail)
                    result.Add(reverse ? line.Item2 : line.Item1);
                nail = reverse ? line.Item1 : line.Item2;
                result.Add(nail);
                lines.Remove(line);
            }

            return result.ToArray();
        }*/

        /* GIF generation */
        GifExtension.ExportGif2($"{ProjectApplicationService.CurrentProject.FilePath}.gif", 
            Result.Instructions,
            project.Threads.ToList(),
            SettingsApplicationService.Settings.Gif.StepPerFrame,
            SettingsApplicationService.Settings.Gif.FrameDuration,
            SettingsApplicationService.Settings.Gif.LastFrameDuration);
        /* GIF generation */
    }
}
