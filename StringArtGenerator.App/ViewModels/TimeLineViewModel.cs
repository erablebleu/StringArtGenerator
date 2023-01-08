using AutoMapper;
using Microsoft.VisualBasic.FileIO;
using StringArtGenerator.App.Adapters;
using StringArtGenerator.App.ApplicationServices;
using StringArtGenerator.App.Model;
using StringArtGenerator.App.Tools;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace StringArtGenerator.App.ViewModels;
public class TimeLineViewModel : ViewModelBase
{
    private ICommand? _addThreadCommand;
    private ICommand? _removeThreadCommand;
    private ICommand? _updateThreadCommand;
    private ICommand? _updateSizeCommand;
    private ICommand? _updateOpacityCommand;
    private ICommand? _updateBlurCommand;

    public ICommand AddThreadCommand => _addThreadCommand ??= new RelayCommand(OnAddThreadCommand);
    public ICommand RemoveThreadCommand => _removeThreadCommand ??= new RelayCommand<ThreadAdapter>(OnRemoveThreadCommand);
    public ICommand UpdateThreadCommand => _updateThreadCommand ??= new RelayCommand<ThreadAdapter>(OnUpdateThreadCommand);

    public ICommand UpdateSizeCommand => _updateSizeCommand ??= new RelayCommand(OnUpdateSizeCommand);
    public ICommand UpdateOpacityCommand => _updateOpacityCommand ??= new RelayCommand(OnUpdateOpacityCommand);
    public ICommand UpdateBlurCommand => _updateBlurCommand ??= new RelayCommand(OnUpdateBlurCommand);

    private void OnUpdateSizeCommand()
    {
        SizeOptionAdapter option = ProjectApplicationService.CurrentProject.ImageOptions.SizeOption;
        NailMap map = Mapper.Map<NailMap>(ProjectApplicationService.CurrentProject.NailMap);

        // Resize
        double scale = ProjectApplicationService.CurrentProject.NailMap.Scale;
        Point position = ProjectApplicationService.CurrentProject.NailMap.Position;
        Rect area = new(new Point(map.Nails.Min(n => n.Position.X), map.Nails.Min(n => n.Position.Y)),
                        new Point(map.Nails.Max(n => n.Position.X), map.Nails.Max(n => n.Position.Y)));

        System.Drawing.Bitmap bmp = ProjectApplicationService.CurrentProject.SourceImage.ToBitmap();
        bmp = bmp.Crop((int)position.X, (int)position.Y, (int)(area.Width * scale), (int)(area.Height * scale));

        if (option.IsEnabled && scale > option.MaxPPMM)
            bmp = bmp.Resize(option.MaxPPMM / scale);

        ProjectApplicationService.CurrentProject.ImageOptions.SizeOption.ResultImage = bmp.ToBitmapImage();
    }
    private void OnUpdateOpacityCommand()
    {
        OpacityOptionAdapter option = ProjectApplicationService.CurrentProject.ImageOptions.OpacityOption;
        SizeOptionAdapter sizeOption = ProjectApplicationService.CurrentProject.ImageOptions.SizeOption;

        double[,] dmap = GetNailDistance(option.Length, option.EvolutionPow);
        option.ResultImage = sizeOption.ResultImage.ToBitmap().OpacityFilter(dmap).ToBitmapImage();
    }
    private void OnUpdateBlurCommand()
    {
        BlurOptionAdapter option = ProjectApplicationService.CurrentProject.ImageOptions.BlurOption;
        SizeOptionAdapter sizeOption = ProjectApplicationService.CurrentProject.ImageOptions.SizeOption;

        double[,] dmap = GetNailDistance(option.Length, option.EvolutionPow);

        option.ResultImage = (ProjectApplicationService.CurrentProject.ImageOptions.OpacityOption.IsEnabled
                ? ProjectApplicationService.CurrentProject.ImageOptions.OpacityOption.ResultImage
                : sizeOption.ResultImage).ToBitmap().BlurFilter(dmap, option.Diameter).ToBitmapImage();
    }
    private double[,] GetNailDistance(double length, double evolutionPow)
    {
        SizeOptionAdapter sizeOption = ProjectApplicationService.CurrentProject.ImageOptions.SizeOption;
        NailMap map = Mapper.Map<NailMap>(ProjectApplicationService.CurrentProject.NailMap);
        double scale = ProjectApplicationService.CurrentProject.NailMap.Scale;
        if (sizeOption.IsEnabled && scale > sizeOption.MaxPPMM)
            scale *= sizeOption.MaxPPMM;

        return GetNailDistance(length, evolutionPow, map.Nails.Select(n => new Point(n.Position.X * scale, n.Position.Y * scale)).ToArray(), sizeOption.ResultImage.PixelWidth, sizeOption.ResultImage.PixelHeight);
    }
    private static double[,] GetNailDistance(double length, double evolutionPow, Point[] nails, int width, int height)
    {
        double[,] result = new double[height, width];

        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                result[y, x] = 1;

        foreach (Point p in nails)
        {
            for(int x = (int)(p.X - length); x <= p.X+ length; x++)
            {
                if (x < 0 || x >= width) continue;
                for (int y = (int)(p.Y - length); y <= p.Y + length; y++)
                {
                    if (y < 0 || y >= height) continue;

                    result[y, x] = Math.Min(result[y, x], Math.Pow(Math.Min(length, Math.Sqrt(Math.Pow(p.X - x, 2) + Math.Pow(p.Y - y, 2))) / length, evolutionPow));
                }
            }
        }

        return result;
    }

    private void OnAddThreadCommand()
    {
        ProjectApplicationService.CurrentProject.Threads.Add(new ThreadAdapter());
    }
    private void OnRemoveThreadCommand(ThreadAdapter adapter)
    {
        ProjectApplicationService.CurrentProject.Threads.Remove(adapter);
    }
    private void OnUpdateThreadCommand(ThreadAdapter adapter)
    {
        ImageOptionsAdapter imgOptions = ProjectApplicationService.CurrentProject.ImageOptions;
        BitmapImage img = imgOptions.BlurOption.IsEnabled ? imgOptions.BlurOption.ResultImage
                        : imgOptions.OpacityOption.IsEnabled ? imgOptions.OpacityOption.ResultImage
                        : imgOptions.SizeOption.ResultImage;
        System.Drawing.Bitmap? bmp = img?.ToBitmap();

        if (bmp is null) return;

        if (adapter.LuminosityAdjustment.IsActive)
            bmp = bmp.SetBrightnessContrast(adapter.LuminosityAdjustment.Brightness, adapter.LuminosityAdjustment.Contrast);
        if (adapter.ColorAdjustment.IsActive)
            bmp = bmp.SetMatrix(adapter.ColorAdjustment.ColorMatrix);
        else
            bmp = bmp.Filter(adapter.Color.ToDrawingColor(), adapter.FilterStrength);

        adapter.FilteredImage = bmp.ToBitmapImage();
    }
}
