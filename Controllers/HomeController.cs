using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MultipleDataGenerator.Models;
using MultipleDataGenerator.Services;
using OfficeOpenXml;

namespace MultipleDataGenerator.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult About() 
    {
        return View();
    }
}
