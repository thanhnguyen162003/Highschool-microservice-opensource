using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Domain.Enumerations
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum AdmissionMethod
    {
        DGNL,
        THPTQG,
        HighschoolTranscript,
        //SAT,
        //IELTS,
        //TSA,
        //AptitudeTest,
        //TalentAssessment
    }
}
