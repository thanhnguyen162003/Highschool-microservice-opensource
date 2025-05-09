using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Domain.Enumerations
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum UniversityType
    {
        [Description("Công lập")]
        Public,
        [Description("Tư thục")]
        Private,
    }
}
