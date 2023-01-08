using System;
using System.IO;
using System.Windows.Media.Imaging;

namespace StringArtGenerator.App.Tools.Serialization;

public class BitmapConverter : System.Text.Json.Serialization.JsonConverter<BitmapImage>
{
    public override BitmapImage Read(ref System.Text.Json.Utf8JsonReader reader, Type typeToConvert, System.Text.Json.JsonSerializerOptions options)
    {
        
        using MemoryStream stream = new(reader.GetBytesFromBase64());
        BitmapImage bitmapImage = new();
        bitmapImage.BeginInit();
        bitmapImage.StreamSource = stream;
        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
        bitmapImage.EndInit();
        return bitmapImage;
    }

    public override void Write(System.Text.Json.Utf8JsonWriter writer, BitmapImage value, System.Text.Json.JsonSerializerOptions options)
    {
        using MemoryStream stream = new();
        BitmapEncoder enc = new PngBitmapEncoder();
        enc.Frames.Add(BitmapFrame.Create(value));
        enc.Save(stream);
        writer.WriteBase64StringValue(stream.ToArray());
    }
}