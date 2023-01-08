using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StringArtGenerator.App.Adapters;

public  class SettingsAdapter : AdapterBase
{
    private GifSettingsAdapter _gif = new();

    public GifSettingsAdapter Gif { get => _gif; set => Set(ref _gif, value); }

    public class GifSettingsAdapter : AdapterBase
    {
        private int _stepPerFrame = 100;
        private int _frameDuration = 50;
        private int _lastFrameDuration = 5000;

        public int StepPerFrame { get => _stepPerFrame; set => Set(ref _stepPerFrame, value); }

        public int FrameDuration { get => _frameDuration; set => Set(ref _frameDuration, value); }
        public int LastFrameDuration { get => _lastFrameDuration; set => Set(ref _lastFrameDuration, value); }
    }    
}
