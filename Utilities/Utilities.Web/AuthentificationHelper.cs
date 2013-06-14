using System;
using System.Linq;

namespace Mvc.Helper.UI
{
    public class AuthentificationHelper
    {
        public static string FormatWindowsUser(string fullLogin)
        {
            if (fullLogin == null)
                throw new ArgumentNullException("fullLogin");

            string[] loginSplit = fullLogin.Split('\\');
            if (!loginSplit.Any())
                return fullLogin;
            string[] split = fullLogin.Split('\\');
            if (split.Count() > 1)
                return split[1];
            return null;
        }
    }
}