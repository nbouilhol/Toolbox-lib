using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SuiviCRA.Application;
using SuiviCRA.Domain;
using SuiviCRA.Web.Models;
using AutoMapper;
using SuiviCRA.Web.Helpers.Extension;

namespace Mvc.Helper.UI
{
    public class AuthentificationHelper
    {
        public static string FormatWindowsUser(string fullLogin)
        {
            if (fullLogin == null)
                throw new ArgumentNullException("fullLogin");

            var loginSplit = fullLogin.Split('\\');
            if (!loginSplit.Any())
                return fullLogin;
            string[] split = fullLogin.Split('\\');
            if (split.Count() > 1)
                return split[1];
            return null;
        }
    }
}