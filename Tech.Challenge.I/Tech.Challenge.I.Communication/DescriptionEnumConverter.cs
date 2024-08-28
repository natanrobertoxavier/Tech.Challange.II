using Newtonsoft.Json;
using System.ComponentModel;

namespace Tech.Challenge.I.Communication;

public class DescriptionEnumConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return objectType.IsEnum;
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        var enumText = reader.Value.ToString();
        foreach (var field in objectType.GetFields())
        {
            if (Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) is DescriptionAttribute attribute)
            {
                if (attribute.Description == enumText)
                {
                    return Enum.Parse(objectType, field.Name);
                }
            }
            else
            {
                if (field.Name == enumText)
                {
                    return Enum.Parse(objectType, field.Name);
                }
            }
        }
        throw new ArgumentException($"Unknown enum value: {enumText}");
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        var field = value.GetType().GetField(value.ToString());
        if (Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) is DescriptionAttribute attribute)
        {
            writer.WriteValue(attribute.Description);
        }
        else
        {
            writer.WriteValue(value.ToString());
        }
    }
}