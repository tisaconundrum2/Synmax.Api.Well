using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Synmax.Api.Well.Models;

namespace Synmax.Api.Well.Controllers;

public class WellController : Controller
{
    private readonly ILogger<WellController> _logger;

    public WellController(ILogger<WellController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return Redirect("/swagger");
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}