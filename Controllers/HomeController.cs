﻿using Microsoft.AspNetCore.Mvc;

namespace Assignment2.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Error()
        {
            return View(); // Do you need to modify this line? Use your judgement based on the app you developed so far
        }


    }
}
