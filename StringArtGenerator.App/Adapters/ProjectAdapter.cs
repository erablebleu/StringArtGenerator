using StringArtGenerator.App.Tools;
using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Text.Json.Serialization;
using System.Windows.Media.Imaging;

namespace StringArtGenerator.App.Adapters;

public class ProjectAdapter : AdapterBase
{
    private CalculationOptionsAdapter _calculationOptions = new();
    private CalculationResultAdapter _calculationResult = new();
    private string? _filePath;
    private NailMapAdapter _nailMap = new();
    private ImageOptionsAdapter _imageOptions = new();
    private BitmapImage? _sourceImage;
    private StepperAdapter _stepper = new();
    private ObservableCollection<ThreadAdapter> _threads = new();

    public CalculationOptionsAdapter CalculationOptions { get => _calculationOptions; set => Set(ref _calculationOptions, value); }
    public CalculationResultAdapter CalculationResult { get => _calculationResult; set => Set(ref _calculationResult, value); }
    [JsonIgnore] public string? FilePath { get => _filePath; set => Set(ref _filePath, value); }
    public NailMapAdapter NailMap { get => _nailMap; set => Set(ref _nailMap, value); }

    public ImageOptionsAdapter ImageOptions { get => _imageOptions; set => Set(ref _imageOptions, value); }
    public BitmapImage? SourceImage { get => _sourceImage; set => Set(ref _sourceImage, value); }
    public StepperAdapter Stepper { get => _stepper; set => Set(ref _stepper, value); }
    public ObservableCollection<ThreadAdapter> Threads { get => _threads; set => Set(ref _threads, value); }
}