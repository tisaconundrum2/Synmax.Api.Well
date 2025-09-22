//2) A geospatial polygon search endpoint that will return a list of well API numbers that are within a polygon.
// Write a GET endpoint where the customer can pass a list of (latitude, longitude) pairs that define a polygon to retrieve all API numbers located within the polygon.
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using NetTopologySuite.Geometries;
using Synmax.Api.Well.Data;
using Synmax.Api.Well.Models;
using Coordinate = Synmax.Api.Well.Models.Coordinate;

namespace Synmax.Api.Well.Controllers;

public class GeoSpatialController : Controller
{
    private readonly ILogger<GeoSpatialController> _logger;
    private readonly ApplicationDbContext _context;

    public GeoSpatialController(ILogger<GeoSpatialController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    [HttpGet]
    public IActionResult PolygonSearch()
    {
        return Redirect("/swagger");
    }

    [HttpPost]
    [Route("GeoSpatial/PolygonSearch")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult PolygonSearch([FromBody] List<Coordinate> polygon)
    {
        if (polygon == null || polygon.Count < 3)
        {
            return BadRequest(Json(new { message = "A valid polygon with at least 3 coordinates is required." }));
        }

        try
        {
            var apiNumbers = GetApiNumbersWithinPolygon(polygon);
            return Json(apiNumbers);
        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occurred while processing the polygon search: {ex.Message}");
            return StatusCode(500, Json(new { message = "An error occurred while processing your request.", details = ex.Message }));
        }
    }

    public List<PolygonSearchResult> GetApiNumbersWithinPolygon(List<Coordinate> polygon)
    {
        var geometryFactory = new GeometryFactory();
        
        // Create the polygon from input coordinates
        var coordinates = polygon.Select(p =>
            new NetTopologySuite.Geometries.Coordinate(p.Longitude, p.Latitude)).ToArray();
        var polygonGeometry = geometryFactory.CreatePolygon(coordinates);
    
        // Because you need SQL for spatial queries, we will need to do the following instead
        var wellData = _context.WellDetails
            .Where(w => w.Latitude != null && w.Longitude != null)
            .Select(w => new 
            {
                w.API,
                w.Latitude,
                w.Longitude
            })
            .ToList();

        // Then perform spatial operations in memory
        // This is not efficient for large datasets but works for demonstration purposes
        // TODO: Figure out how to do this in SQL directly
        var polygonSearchResults = wellData
            .Select(w => new
            {
                w.API,
                Point = geometryFactory.CreatePoint(
                    new NetTopologySuite.Geometries.Coordinate((double)w.Longitude, (double)w.Latitude))
            })
            .Where(w => polygonGeometry.Contains(w.Point))
            .Select(w => new PolygonSearchResult
            {
                ApiNumbers = w.API,
                Latitude = w.Point.Y,
                Longitude = w.Point.X
            })
            .ToList();
    
        return polygonSearchResults;
    }    
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

