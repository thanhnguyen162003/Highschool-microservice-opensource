using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enumerations
{
    public enum ProgressStage
    {
        /// <summary>
        /// The account is in the new user stage.
        /// </summary>
        NewUser = 1,

        /// <summary>
        /// The account is in the subject information stage.
        /// </summary>
        SubjectInformation = 2,

        /// <summary>
        /// The account is in the MBTI or Holland assessment stage.
        /// </summary>
        PersonalityAssessment = 3,

        /// <summary>
        /// The account has completed all stages.
        /// </summary>
        Completion = 4,

        /// <summary>
        /// The account is in the verify teacher stage.
        /// </summary>
        VerifyTeacher = 5,
    }

}
