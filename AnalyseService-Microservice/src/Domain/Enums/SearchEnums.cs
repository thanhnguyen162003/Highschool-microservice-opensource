using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Domain.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum SearchType
{
    All = 0,
    Flashcard = 1,
    Subject = 2,
    Document = 3,
    Name = 4,
    Folder = 5,
    News = 6
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum SearchCourseType
{
    Lesson = 4,
    Chapter = 3,
    SubjectCurriculum = 2,
    Subject = 1
}
