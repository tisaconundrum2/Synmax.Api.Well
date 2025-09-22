using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Synmax.Api.Well.Data;
using Synmax.Api.Well.Models;

namespace Synmax.Api.Well.Controllers;

public class WellController : Controller
{
    private readonly ILogger<WellController> _logger;
    private readonly ApplicationDbContext _context;

    public WellController(ILogger<WellController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return Redirect("/swagger");
    }

    [HttpGet]
    [Route("Well/{Api}")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult Index(string Api)
    {
        try
        {
            var wellDetail = _context.WellDetails.FirstOrDefault(w => w.API == Api);
            if (wellDetail == null)
            {
                return StatusCode(404, Json(new { message = $"Well with API {Api} not found." }));
            }
            return Json(wellDetail);
        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occurred while processing the request for API {Api}: {ex.Message}");
            return StatusCode(500, Json(new { message = "An error occurred while processing your request.", details = ex.Message }));
        }
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}