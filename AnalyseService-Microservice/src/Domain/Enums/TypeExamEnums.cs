using System.ComponentModel;

namespace Domain.Enums;

public enum TypeExamEnums
{
    [Description("1 tiết")]
    T1H,
    [Description("cuối kì")]
    FIE,
    [Description("THPTQG")]
    NHE,
    [Description("DGNL")]
    CAP,
    [Description("Other")]
    OTHER
}
