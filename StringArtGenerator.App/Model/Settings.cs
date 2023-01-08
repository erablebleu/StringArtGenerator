using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StringArtGenerator.App.Model;

public class Settings
{
    public GifSettings Gif { get; set; } = new();

    public class GifSettings
    {
        public int StepPerFrame { get; set; } = 100;
        public int FrameDuration { get; set; } = 50;
        public int LastFrameDuration { get; set; } = 5000;
    }
}
