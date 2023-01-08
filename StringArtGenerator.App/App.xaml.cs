using AutoMapper;
using StringArtGenerator.App.Adapters;
using StringArtGenerator.App.ApplicationServices;
using StringArtGenerator.App.Model;
using StringArtGenerator.App.Resources.Extensions;
using StringArtGenerator.App.ViewModels;
using StringArtGenerator.App.Views;
using StringArtGenerator.Injection;
using System.Globalization;
using System.Windows;
using System.Windows.Markup;

namespace StringArtGenerator.App;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private Loader? _loader;
    private Views.MainView? _mainView;
    private IMapper? _mapper;

    protected override void OnExit(ExitEventArgs e)
    {
        base.OnExit(e);
        _loader?.Clean();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
        System.Threading.Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
        FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement), new FrameworkPropertyMetadata(
                    XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));

        _mapper = InitMapper();
        _loader = InitLoader();

        MainViewModel viewModel = _loader.Resolve<MainViewModel>();
        _mainView = new()
        {
            DataContext = viewModel,
        };
        Current.MainWindow = _mainView;
        viewModel.Load();
        _mainView.Show();
        _mainView.Activate();

        base.OnStartup(e);
    }

    private Loader InitLoader()
    {
        Loader loader = new();

        // ApplicationServices
        loader.AddSingleton<NavigationApplicationService>();
        loader.AddSingleton<SettingsApplicationService>();
        loader.AddSingleton<ProjectApplicationService>();
        loader.AddSingleton<CalculatorApplicationService>();

        // ViewModels
        loader.AddSingleton<MainViewModel>();
        loader.AddSingleton<StepperViewModel>();
        loader.AddSingleton<MapperViewModel>();
        loader.AddSingleton<TimeLineViewModel>();
        loader.AddScoped<SettingsViewModel>();

        // Tools
        loader.AddSingleton(_mapper);

        loader.Build();

        return loader;
    }

    private static IMapper InitMapper() => new MapperConfiguration(c =>
            {
                c.CreateMap<object, AdapterBase>()
                    .ForMember(dest => dest.IsDirty, opt =>
                    {
                        opt.SetMappingOrder(99);
                        opt.MapFrom(src => false);
                    })
                    .IncludeAllDerived();

                c.CreateMapWithReverse<Nail, NailAdapter>();
                c.CreateMapWithReverse<Thread, ThreadAdapter>();
                c.CreateMapWithReverse<ColorAdjustment, ColorAdjustmentAdapter>();
                c.CreateMapWithReverse<LuminosityAdjustment, LuminosityAdjustmentAdapter>();

                c.CreateMap<NailMap, NailMapAdapter>()
                 .ForMember(dest => dest.Lines, opt => opt.Ignore())
                 .AfterMap((m, v) =>
                 {
                     foreach (LineInstruction line in m.Lines)
                         v.Lines.Add(new LineAdapter { Nail1 = v.Nails[line.Nail1Index], Nail2 = v.Nails[line.Nail2Index] });
                 })                 
                 .ReverseMap()
                 .ForMember(dest => dest.Lines, opt => opt.Ignore())
                 .AfterMap((m, v) =>
                 {
                     foreach (LineAdapter line in m.Lines)
                         v.Lines.Add(new LineInstruction { Nail1Index = m.Nails.IndexOf(line.Nail1), Nail2Index = m.Nails.IndexOf(line.Nail2) });
                 });

                c.CreateMapWithReverse<CalculationOptions, CalculationOptionsAdapter>();
                c.CreateMapWithReverse<OpacityOption, OpacityOptionAdapter>();
                c.CreateMapWithReverse<BlurOption, BlurOptionAdapter>();
                c.CreateMapWithReverse<SizeOption, SizeOptionAdapter>();
                c.CreateMapWithReverse<CalculationResult, CalculationResultAdapter>();
                c.CreateMapWithReverse<ImageOptions, ImageOptionsAdapter>();
                c.CreateMapWithReverse<Stepper, StepperAdapter>();
                c.CreateMapWithReverse<Project, ProjectAdapter>();


                c.CreateExtendedMap<Settings, SettingsAdapter>();
                c.CreateExtendedMap<Settings.GifSettings, SettingsAdapter.GifSettingsAdapter>();
            }).CreateMapper();
}