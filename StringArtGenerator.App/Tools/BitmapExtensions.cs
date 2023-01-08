using ColorMine.ColorSpaces;
using ColorMine.ColorSpaces.Comparisons;
using StringArtGenerator.Imaging;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Media.Imaging;

namespace StringArtGenerator.App.Tools;

public static class BitmapExtensions
{
    public static Bitmap Crop(this Bitmap img, int left, int top, int width, int height)
    {
        Bitmap result = new(img);
        return result.Clone(new Rectangle(left, top, width, height), result.PixelFormat);
    }

    public static Bitmap Resize(this Bitmap bmp, double scale)
    {
        // todo : corriger la perte d'alpha
        int width = (int)(bmp.Width * scale + 1);
        int height = (int)(bmp.Height * scale + 1);
        Bitmap result = new(width, height, bmp.PixelFormat);

        result.SetResolution(bmp.HorizontalResolution, bmp.VerticalResolution);

        using (var graphics = Graphics.FromImage(result))
        {
            graphics.Clear(Color.Transparent);
            //graphics.CompositingMode = CompositingMode.SourceCopy;
            //graphics.CompositingQuality = CompositingQuality.HighQuality;
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            //graphics.SmoothingMode = SmoothingMode.HighQuality;
            //graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

            //using ImageAttributes wrapMode = new ();
            //wrapMode.SetWrapMode(WrapMode.TileFlipXY);
            //graphics.DrawImage(bmp, new Rectangle(0, 0, width, height), 0, 0, bmp.Width, bmp.Height, GraphicsUnit.Pixel, wrapMode);
            graphics.DrawImage(bmp, new Rectangle(0, 0, width, height));
        }

        return result;
    }

    public static Bitmap ToBitmap<T>(this BitmapSource bs) where T : BitmapEncoder, new()
    {
        using MemoryStream outStream = new();
        T enc = new ();
        enc.Frames.Add(BitmapFrame.Create(bs));
        enc.Save(outStream);
        Bitmap bitmap = new(outStream);

        return new Bitmap(bitmap);
    }

    public static Bitmap ToBitmap(this BitmapSource bs) => ToBitmap<PngBitmapEncoder>(bs);
    public static BitmapImage ToBitmapImage(this Bitmap bmp)
    {
        using MemoryStream memory = new();
        bmp.Save(memory, ImageFormat.Png);
        memory.Position = 0;

        var bitmapImage = new BitmapImage();
        bitmapImage.BeginInit();
        bitmapImage.StreamSource = memory;
        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
        bitmapImage.EndInit();
        bitmapImage.Freeze();

        return bitmapImage;
    }

    public static Bitmap SetBrightnessContrast(this Bitmap bmp, double brightness, double contrast)
    {
        float b = (float)brightness;
        float c = (float)contrast;

        var bitmap = new Bitmap(bmp.Width, bmp.Height, PixelFormat.Format32bppArgb);

        using var g = Graphics.FromImage(bitmap);
        using var attributes = new ImageAttributes();
        float[][] matrix = {
            new float[] {c, 0, 0, 0, 0},
            new float[] {0, c, 0, 0, 0},
            new float[] {0, 0, c, 0, 0},
            new float[] {0, 0, 0, 1, 0},
            new float[] {b, b, b, 1, 1}
        };

        ColorMatrix colorMatrix = new(matrix);
        attributes.SetColorMatrix(colorMatrix);
        g.DrawImage(bmp, new Rectangle(0, 0, bitmap.Width, bitmap.Height), 0, 0, bitmap.Width, bitmap.Height, GraphicsUnit.Pixel, attributes);
        return bitmap;
    }

    public static Bitmap SetMatrix(this Bitmap bmp, double[,] d)
    {
        var bitmap = new Bitmap(bmp.Width, bmp.Height, PixelFormat.Format32bppArgb);

        using var graph = Graphics.FromImage(bitmap);
        using var attributes = new ImageAttributes();
        float[][] matrix = {
            new float[] { (float)d[0, 0], (float)d[0, 1], (float)d[0, 2], 0, 0},
            new float[] { (float)d[1, 0], (float)d[1, 1], (float)d[1, 2], 0, 0},
            new float[] { (float)d[2, 0], (float)d[2, 1], (float)d[2, 2], 0, 0},
            new float[] {0, 0, 0, 1, 0},
            new float[] {0, 0, 0, 0, 1}
        };

        ColorMatrix colorMatrix = new(matrix);
        attributes.SetColorMatrix(colorMatrix);
        graph.DrawImage(bmp, new Rectangle(0, 0, bitmap.Width, bitmap.Height), 0, 0, bitmap.Width, bitmap.Height, GraphicsUnit.Pixel, attributes);
        return bitmap;
    }

    public static Bitmap SetBW(this Bitmap bmp, float r = 0.30f, float g = 0.59f, float b = 0.11f)
    {
        var bitmap = new Bitmap(bmp.Width, bmp.Height, PixelFormat.Format32bppArgb);

        using var graph = Graphics.FromImage(bitmap);
        using var attributes = new ImageAttributes();
        float[][] matrix = {
            new float[] {r, r, r, 0, 0},
            new float[] {g, g, g, 0, 0},
            new float[] {b, b, b, 0, 0},
            new float[] {0, 0, 0, 1, 0},
            new float[] {0, 0, 0, 0, 1}
        };

        ColorMatrix colorMatrix = new(matrix);
        attributes.SetColorMatrix(colorMatrix);
        graph.DrawImage(bmp, new Rectangle(0, 0, bitmap.Width, bitmap.Height), 0, 0, bitmap.Width, bitmap.Height, GraphicsUnit.Pixel, attributes);
        return bitmap;
    }

    public static double[] GetSpectrum(this Bitmap bmp)
    {
        double[] spectrum = new double[256];

        byte[] r, g, b;
        (r, g, b, _) = bmp.GetData();

        for (int i = 0; i < bmp.Width * bmp.Height; i++)
            spectrum[ColorExtension.RGBToHSL(r[i], g[i], b[i]).h]++;

        double max = spectrum.Max();
        for (int i = 0; i < spectrum.Length; i++)
            spectrum[i] /= max;

        return spectrum;
    }

    public static Bitmap GetSpectrumBitmap(double[] spectrum)
    {
        int length = 4 * 256 * 256;
        byte[] data = new byte[length];
        for (int i = 0; i < 256; i++)
        {
            byte r, g, b;
            (r, g, b) = ColorExtension.HSLToRGB((byte)i, 255, 128);
            for (int j = 0; j < 256 * spectrum[i]; j++)
            {
                data[(j * 256 + i) * 4 + 2] = r;
                data[(j * 256 + i) * 4 + 1] = g;
                data[(j * 256 + i) * 4] = b;
                data[(j * 256 + i) * 4 + 3] = 255;
            }
        }

        Bitmap bmp = new (256, 256, PixelFormat.Format32bppArgb);
        BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, 256, 256), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
        Marshal.Copy(data, 0, bmpData.Scan0, length);
        bmp.UnlockBits(bmpData);

        return bmp;
    }
    static double minD = double.MaxValue;
    static double maxD;
    public static Bitmap Filter(this Bitmap bmp, Color color, double strength = 1d) => Filter(bmp, c =>
        {

            //byte h0, s0, l0;
            //byte h1, s1, l1;

            //(h0, s0, l0) = color.GetHSL();
            //(h1, s1, l1) = c.GetHSL();

            //int dh = (255 + h1 - h0) % 255;
            //byte r, g, b;
            //(r, g, b) = ColorExtension.HSLToRGB((byte)dh, s1, l1);


            //int herr = Math.Min(Math.Min(Math.Abs(h1 - h0), Math.Abs(h1 - h0 + 256)), Math.Abs(h0 - h1 + 256));
            //int err = (Math.Abs(color.R - r) + Math.Abs(color.G - g) + Math.Abs(color.B - b))/3;

            //return Color.FromArgb(255 - err, color.R, color.G, color.B);



            //double d = Math.Sqrt(Math.Pow((color.R - c.R) / 255d, 2) + Math.Pow((color.G - c.G) / 255d, 2) + Math.Pow((color.B - c.B) / 255d, 2));
            //double md = Math.Sqrt(3);
            //return Color.FromArgb(255-(int)(255*d/md), color.R, color.G, color.B);

            ExtendedColor ec1 = new ExtendedColor(color);
            ExtendedColor ec2 = new ExtendedColor(c);

            double d = ec1.Compare(ec2);
            minD = Math.Min(minD, d);
            maxD = Math.Max(maxD, d);
            return Color.FromArgb(Math.Clamp((int)(255 - d * strength), 0, 255), color.R, color.G, color.B);




            int err = Math.Abs(c.R - color.R) + Math.Abs(c.G - color.G) + Math.Abs(c.B - color.B);
            return Color.FromArgb(Math.Max(0, Math.Min(255, 255 - err / 2)), color.R, color.G, color.B);

        });
    public static Bitmap OpacityFilter(this Bitmap bmp, double[,] filter)
        => Filter(bmp, (x, y, c) => Color.FromArgb(255, 
            255 - (int)(filter[y, x] * (255 - c.R)), 
            255 - (int)(filter[y, x] * (255 - c.G)),
            255 - (int)(filter[y, x] * (255 - c.B))));
    public static Bitmap BlurFilter(this Bitmap bmp, double[,] filter, double diameter)
    {
        Rectangle area = new(0, 0, bmp.Width, bmp.Height);
        BitmapData bitmapData = bmp.LockBits(area, ImageLockMode.ReadOnly, bmp.PixelFormat);
        int length = bitmapData.Width * bitmapData.Height;
        int[] srcData = new int[length];
        int[] dstData = new int[length];
        Marshal.Copy(bitmapData.Scan0, srcData, 0, length);
        bmp.UnlockBits(bitmapData);

        for (int x = 0; x < bitmapData.Width; x++)
        {
            for (int y = 0; y < bitmapData.Height; y++)
            {
                int i = GetIndex(x, y, bitmapData.Width);
                int cnt = 0;
                int r = 0;
                int g = 0;
                int b = 0;
                int diam = (int)(diameter / 2 * (1 - filter[y, x]));

                for(int x2 = x - diam; x2 <= x + diam; x2++)
                {
                    if (x2 < 0 || x2 >= bmp.Width) continue;

                    for (int y2 = y - diam; y2 <= y + diam; y2++)
                    {
                        if (y2 < 0 || y2 >= bmp.Height) continue;
                        int i2 = GetIndex(x2, y2, bitmapData.Width);
                        cnt++;
                        r += srcData[i2].R();
                        g += srcData[i2].G();
                        b += srcData[i2].B();
                    }
                }

                r /= cnt;
                g /= cnt;
                b /= cnt;
                dstData[i] = ARGB(srcData[i].A(), r, g, b);
            }
        }

        return FromData(bmp.Width, bmp.Height, dstData);
    }
    public static int GetIndex(int x, int y, int width) => y * width + x;
    public static int A(this int argb) => 0xff & (argb >> 24);
    public static int R(this int argb) => 0xff & (argb >> 16);
    public static int G(this int argb) => 0xff & (argb >> 8);
    public static int B(this int argb) => 0xff & argb;
    public static int ARGB(int a, int r, int g, int b) => (a << 24) | (r << 16) | (g << 8) | b;
    public static Bitmap Filter(this Bitmap bmp, Func<Color, Color> filter) => Filter(bmp, (x, y, c) => filter.Invoke(c));
    public static Bitmap Filter(this Bitmap bmp, Func<int, int, Color, Color> filter) => Filter(bmp, (int x, int y, int c) => filter.Invoke(x, y, Color.FromArgb(c)).ToArgb());
    public static Bitmap Filter(this Bitmap bmp, Func<int, int, int, int> filter)
    {
        Rectangle area = new(0, 0, bmp.Width, bmp.Height);
        BitmapData bitmapData = bmp.LockBits(area, ImageLockMode.ReadOnly, bmp.PixelFormat);
        int length = bitmapData.Width * bitmapData.Height;
        int[] srcData = new int[length];
        int[] dstData = new int[length];
        Marshal.Copy(bitmapData.Scan0, srcData, 0, length);
        bmp.UnlockBits(bitmapData);

        for (int x = 0; x < bitmapData.Width; x++)
        {
            for (int y = 0; y < bitmapData.Height; y++)
            {
                int i = y * bitmapData.Width + x;
                dstData[i] = filter.Invoke(x, y, srcData[i]);
            }
        }

        return FromData(bmp.Width, bmp.Height, dstData);
    }

    public static Bitmap FromData(int width, int height, int[] data, PixelFormat pixelFormat = PixelFormat.Format32bppArgb)
    {
        Bitmap bmp = new(width, height, pixelFormat);
        Rectangle area = new(0, 0, bmp.Width, bmp.Height);
        BitmapData bitmapData = bmp.LockBits(area, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
        int length = bitmapData.Width * bitmapData.Height;
        Marshal.Copy(data, 0, bitmapData.Scan0, length);
        bmp.UnlockBits(bitmapData);
        return bmp;
    }

    public static (byte[] r, byte[] g, byte[] b, byte[] a) GetData(this Bitmap bmp)
    {
        byte[] r, g, b, a;

        BitmapData bitmapData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, bmp.PixelFormat);
        int length = bitmapData.Stride * bitmapData.Height;
        byte[] data = new byte[length];
        Marshal.Copy(bitmapData.Scan0, data, 0, length);
        bmp.UnlockBits(bitmapData);

        r = new byte[bmp.Width * bmp.Height];        
        g = new byte[bmp.Width * bmp.Height];
        b = new byte[bmp.Width * bmp.Height];
        a = new byte[bmp.Width * bmp.Height];

        for (int i = 0; i < length; i += 4)
        {
            a[i / 4] = data[i + 3];
            r[i / 4] = data[i + 2];
            g[i / 4] = data[i + 1];
            b[i / 4] = data[i + 0];
        }

        return (r, g, b, a);
    }

    public static (byte[,] r, byte[,] g, byte[,] b, byte[,] a) GetData2D(this Bitmap bmp)
    {
        byte[,] r, g, b, a;

        BitmapData bitmapData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, bmp.PixelFormat);
        int length = bitmapData.Stride * bitmapData.Height;
        byte[] data = new byte[length];
        Marshal.Copy(bitmapData.Scan0, data, 0, length);
        bmp.UnlockBits(bitmapData);

        r = new byte[bmp.Height, bmp.Width];
        g = new byte[bmp.Height, bmp.Width];
        b = new byte[bmp.Height, bmp.Width];
        a = new byte[bmp.Height, bmp.Width];

        for (int i = 0; i < bmp.Height; i++)
            for (int j = 0; j < bmp.Width; j++)
            {
                a[i, j] = data[(i * bmp.Width + j) * 4 + 3];
                r[i, j] = data[(i * bmp.Width + j) * 4 + 2];
                g[i, j] = data[(i * bmp.Width + j) * 4 + 1];
                b[i, j] = data[(i * bmp.Width + j) * 4 + 0];
            }

        return (r, g, b, a);
    }
}