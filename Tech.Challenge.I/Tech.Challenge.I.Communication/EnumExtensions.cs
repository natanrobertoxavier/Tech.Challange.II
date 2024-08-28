using System.ComponentModel;
using System.Reflection;

namespace Tech.Challenge.I.Communication;
public static class EnumExtensions
{
    public static string GetDescription(this Enum value)
    {
        FieldInfo field = value.GetType().GetField(value.ToString());
        DescriptionAttribute attribute = (DescriptionAttribute)field.GetCustomAttribute(typeof(DescriptionAttribute));
        return attribute == null ? value.ToString() : attribute.Description;
    }

    public static T GetEnumValueFromDescription<T>(string description) where T : Enum
    {
        Dictionary<string, string> mapRegioes = new Dictionary<string, string>
        {
            { "Norte", "Norte" },
            { "Sul", "Sul" },
            { "CentroOeste", "Centro-Oeste" },
            { "Nordeste", "Nordeste" },
            { "Sudeste", "Sudeste" }
        };

        if (mapRegioes.TryGetValue(description, out string descriptionCorrected))
        {
            foreach (var field in typeof(T).GetFields())
            {
                var attribute = (DescriptionAttribute)field.GetCustomAttribute(typeof(DescriptionAttribute));
                if (attribute != null && attribute.Description == descriptionCorrected)
                {
                    return (T)field.GetValue(null);
                }
            }

            throw new ArgumentException($"No enum value found for description '{descriptionCorrected}'", nameof(descriptionCorrected));
        }
        else
        {
            throw new ArgumentException($"Error getting enum value");
        }
    }
}
