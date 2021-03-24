using Lightaplusplus.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lightaplusplus.BisLogic
{
    /// <summary>
    /// This is a data class for organizing the information related to a section
    /// </summary>
    public class SectionRegistrationData
    {
        /// <summary>
        /// This is the Section associated with this section
        /// </summary>
        public Sections Section { get; set; }

        /// <summary>
        /// This is the registry of students related to this section
        /// </summary>
        public List<SectionStudents> StudentRegistry { get; set; }

        /// <summary>
        /// This is the current registration status for this particular section
        /// R -> Registered; F -> Full capacity;  N -> Not registered
        /// </summary>
        public char RegistrationStatus { get; set; }

        public SectionRegistrationData(Sections section, List<SectionStudents> sectionStudents, char registrationStatus)
        {
            Section = section;
            StudentRegistry = sectionStudents;
            RegistrationStatus = registrationStatus;
        }
    }
}
