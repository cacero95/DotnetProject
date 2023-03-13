using System.Text.Json.Serialization;
namespace Projects.Models
{
    [ JsonConverter(typeof(JsonStringEnumConverter)) ]
    public enum RpgClass
    {
        Knight = 1,
        Mage = 2,
        Cleric = 3,
        Archer = 4
    }
}