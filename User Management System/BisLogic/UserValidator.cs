using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lightaplusplus.BisLogic
{
    public static class UserValidator
    {
        /// <summary>
        /// A method for determining if a user should be able to access a specific page or not
        /// Current Options:
        /// 'I' || 'S' for only accessible to instructors or students;
        /// </summary>
        /// <param name="context">The current database context</param>
        /// <param name="session">The current user session</param>
        /// <param name="reqPermissions">A character for basic validation</param>
        /// <returns>A page path to redirect to, or an empty string if valid</returns>
        public static string validateUser(Lightaplusplus.Data.Lightaplusplus_SystemContext context, ISession session, char? reqPermissions = null)
        {
            var id = Session.getUserId(session);
            var userType = Session.getUserType(session);

            if(id == null)
            {
                return "/Index";
            }

            if (reqPermissions != null)
            {
                if ((reqPermissions == 'I' && userType == "S") || (reqPermissions == 'S' && userType == "I")) return "/Welcome";
            }

            return "";
        }

        /// <summary>
        /// A method for determining if a user should be able to access a specific page or not
        /// Current Options:
        /// "Sec : [section id]" for specific access to a given section.
        /// </summary>
        /// <param name="context">The current database context</param>
        /// <param name="session">The current user session</param>
        /// <param name="reqPermissions">A Key Pair Id object for advanced validation</param>
        /// <returns>A page path to redirect to, or an empty string if valid</returns>
        public static string validateUser(Lightaplusplus.Data.Lightaplusplus_SystemContext context, ISession session, KeyPairId reqPermissions)
        {
            var id = Session.getUserId(session);
            var userType = Session.getUserType(session);

            if (id == null)
            {
                return "/Index";
            }

            if (reqPermissions.Key == "Sec")
            {
                if (userType == "S")
                {
                    var registry = context.SectionStudents.FirstOrDefault(ss => ss.SectionId == reqPermissions.Id && ss.StudentId == id);
                    if (registry == null)
                    {
                        return "/Welcome";
                    }
                }
                else
                {
                    var section = context.Sections.FirstOrDefault(s => s.SectionId == reqPermissions.Id && s.InstructorId == id);
                    if (section == null)
                    {
                        return "/Courses/Index";
                    }
                }
            }

            return "";
        }

        /// <summary>
        /// A method for determining if a user should be able to access a specific page or not
        /// Current Options:
        /// "Sec : [section id]" for specific access to a given section.
        /// </summary>
        /// <param name="context">The current database context</param>
        /// <param name="session">The current user session</param>
        /// <param name="reqPermissions">A list of Key Pair Id objects for advanced validation</param>
        /// <returns>A page path to redirect to, or an empty string if valid</returns>
        public static string validateUser(Lightaplusplus.Data.Lightaplusplus_SystemContext context, ISession session, List<KeyPairId> reqPermissions)
        {
            var id = Session.getUserId(session);
            var userType = Session.getUserType(session);

            if (id == null)
            {
                return "/Index";
            }

            foreach (var item in reqPermissions)
            {
                var result = validateUser(context, session, item);
                if (result != "") return result;
            }

            return "";
        }
    }

    /// <summary>
    /// A data class intended for connecting a Key to an integer Id
    /// </summary>
    public class KeyPairId
    {
        public string Key { get; set; }
        public int Id { get; set; }

        public KeyPairId(string key, int id)
        {
            Key = key;
            Id = id;
        }

        public override string ToString()
        {
            return $"{Key}:{Id}";
        }
    }
}
