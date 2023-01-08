using System;

namespace StringArtGenerator.App.Tools.Serialization;

public class DoubleConverter : System.Text.Json.Serialization.JsonConverter<double>
{
    private const string NaN = "NaN";
    private const string NegativeInf = "-Inf";
    private const string PositiveInf = "+Inf";

    public override double Read(ref System.Text.Json.Utf8JsonReader reader, Type typeToConvert, System.Text.Json.JsonSerializerOptions options) => reader.TokenType switch
    {
        System.Text.Json.JsonTokenType.Number => reader.GetDouble(),
        System.Text.Json.JsonTokenType.String => Read(reader.GetString()),
        _ => throw new NotImplementedException(),
    };

    public override void Write(System.Text.Json.Utf8JsonWriter writer, double value, System.Text.Json.JsonSerializerOptions options)
    {
        switch (value)
        {
            case double.NaN:
                writer.WriteStringValue(NaN);
                break;

            case double.PositiveInfinity:
                writer.WriteStringValue(PositiveInf);
                break;

            case double.NegativeInfinity:
                writer.WriteStringValue(NegativeInf);
                break;

            default:
                writer.WriteNumberValue(value);
                break;
        }
    }

    private static double Read(string value) => value switch
    {
        PositiveInf => double.PositiveInfinity,
        NegativeInf => double.NegativeInfinity,
        NaN => double.NaN,
        _ => throw new NotImplementedException(),
    };
}