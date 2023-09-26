using System;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace SustiVest.Web.Controllers
{
    public enum AlertType { success, danger, warning, info }

    // Implements General functionality which is then accessible to any 
    // Controller inheriting from BaseController
    public class BaseController : Controller
    {
        // set alert message
        public void Alert(string message, AlertType type = AlertType.info)
        {

            if (TempData != null)
            {
                TempData["Alert.Message"] = message;
                TempData["Alert.Type"] = type.ToString();
                // }
                // TempData["Alert.Message"] = message;
                // TempData["Alert.Type"] = type.ToString();
            }

        }

    }
}