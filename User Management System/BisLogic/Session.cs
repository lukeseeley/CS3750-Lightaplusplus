using Lightaplusplus.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lightaplusplus.BisLogic
{
    public static class Session
    {
        public static void setUser(this ISession session, Users user)
        {
            session.SetInt32("UserId", user.ID);
            session.SetString("UserType", user.usertype.ToString());
        }

        public static int? getUserId(this ISession session)
        {
            return session.GetInt32("UserId");
        }

        public static string getUserType(this ISession session)
        {
            return session.GetString("UserType");
        }
    }
}
