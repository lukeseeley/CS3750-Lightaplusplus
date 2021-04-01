using Lightaplusplus.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lightaplusplus.BisLogic
{
    public static class Notifications
    {
        public static void SetUserObject(this ISession session, string key, Users user)
        {
            session.SetString(key, JsonConvert.SerializeObject(user));
        }

        public static Users GetUserObject<Users>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default : JsonConvert.DeserializeObject<Users>(value);
        }

        public static void SetGrades(this ISession session, string grades)
        {
            session.SetString("Grades", grades);
        }

        public static List<Grades> GetGrades(this ISession session)
        {
            string myGrades = session.GetString("Grades");
            List<Grades> gradesList = new List<Grades>();
            foreach (var grade in myGrades.Split(":::"))
            {
                try
                {
                    if (grade.Length > 0)
                    {
                        Grades myGrade = JsonConvert.DeserializeObject<Grades>(grade);
                        gradesList.Add(myGrade);
                    }
                }
                catch
                {
                    continue;
                }
            }

            return gradesList;
        }
    }
}
