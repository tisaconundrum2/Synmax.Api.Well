using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using HtmlAgilityPack;

namespace Synmax.Api.Well.Services;

public class WellDetailsParser
{
    private static readonly HttpClient client = new HttpClient();
    private const int MaxRetries = 5;
    private const int InitialDelayMs = 1000;

    private bool IsRateLimited(string html)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(html);
        var rateLimitNode = doc.DocumentNode.SelectSingleNode("//*[@id=\"mainContentContainer\"]/h1");
        return rateLimitNode?.InnerText?.Trim() == "Site Busy - Rate Limit Reached";
    }

    public async Task<string> GetWellDetails(string apiNumber)
    {
        var url = $"https://wwwapps.emnrd.nm.gov/OCD/OCDPermitting/Data/WellDetails.aspx?api={apiNumber}"; // shouldn't be hardcoded
        var currentDelay = InitialDelayMs;
        var attempts = 0;

        while (attempts < MaxRetries)
        {
            try
            {
                var response = await client.GetStringAsync(url);
                if (!IsRateLimited(response))
                {
                    return response;
                }

                attempts++;
                if (attempts >= MaxRetries)
                {
                    throw new Exception("Maximum retry attempts reached while handling rate limit");
                }

                await Task.Delay(currentDelay);
                currentDelay *= 2; // Exponential backoff
            }
            catch (Exception ex) when (ex is HttpRequestException || ex is TaskCanceledException)
            {
                attempts++;
                if (attempts >= MaxRetries)
                {
                    throw;
                }

                await Task.Delay(currentDelay);
                currentDelay *= 2;
            }
        }

        throw new Exception("Failed to get well details after maximum retries");
    }

    public string ParseField(string html, string fieldId)
    {
        try
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            var node = doc.DocumentNode.SelectSingleNode($"//*[@id='{fieldId}']");
            return node?.InnerText?.Trim();
        }
        catch
        {
            return null;
        }
    }

    public async Task<Dictionary<string, string>> ParseWellDetails(string apiNumber)
    {
        /*
        * We are using the XPath to directly target the HTML elements containing the data we need.
        * This is better than scraping the entire page and trying to parse it manually.
        * Each field is identified by its unique ID, which I've extracted from the page's HTML before implementing this.
        */
        var response = await GetWellDetails(apiNumber);
        var fields = new Dictionary<string, string>
        {
            // General Well Information
            {"Operator", ParseField(response, "ctl00_ctl00__main_main_ucGeneralWellInformation_lblOperator")},
            {"Status", ParseField(response, "ctl00_ctl00__main_main_ucGeneralWellInformation_lblStatus")},
            {"WellType", ParseField(response, "ctl00_ctl00__main_main_ucGeneralWellInformation_lblWellType")},
            {"WorkType", ParseField(response, "ctl00_ctl00__main_main_ucGeneralWellInformation_lblWorkType")},
            {"DirectionalStatus", ParseField(response, "ctl00_ctl00__main_main_ucGeneralWellInformation_lblDirectionalStatus")},
            {"MultiLateral", ParseField(response, "ctl00_ctl00__main_main_ucGeneralWellInformation_lblMultiLateral")},
            {"MineralOwner", ParseField(response, "ctl00_ctl00__main_main_ucGeneralWellInformation_lblMineralOwner")},
            {"SurfaceOwner", ParseField(response, "ctl00_ctl00__main_main_ucGeneralWellInformation_lblSurfaceOwner")},

            // Location Information
            {"Location", ParseField(response, "ctl00_ctl00__main_main_ucGeneralWellInformation_Location_lblLocation")},
            {"Lot", ParseField(response, "ctl00_ctl00__main_main_ucGeneralWellInformation_Location_lblLot")},
            {"FootageNSH", ParseField(response, "ctl00_ctl00__main_main_ucGeneralWellInformation_Location_lblFootageNSH")},
            {"FootageEW", ParseField(response, "ctl00_ctl00__main_main_ucGeneralWellInformation_Location_lblFootageEW")},

            {"LocationText", ParseField(response, "ctl00_ctl00__main_main_ucGeneralWellInformation_Location_lblText")},
            {"Coordinates", ParseField(response, "ctl00_ctl00__main_main_ucGeneralWellInformation_Location_lblCoordinates")},

            // Elevation Information
            { "GLElevation", ParseField(response, "ctl00_ctl00__main_main_ucGeneralWellInformation_lblGLElevation")},
            {"KBElevation", ParseField(response, "ctl00_ctl00__main_main_ucGeneralWellInformation_lblKBElevation")},
            {"DFElevation", ParseField(response, "ctl00_ctl00__main_main_ucGeneralWellInformation_lblDFElevation")},

            // Well Details
            {"Completions", ParseField(response, "ctl00_ctl00__main_main_ucGeneralWellInformation_lblCompletions")},
            {"PotashWaiver", ParseField(response, "ctl00_ctl00__main_main_ucGeneralWellInformation_lblPotashWaiver")},
            {"ProposedFormation", ParseField(response, "ctl00_ctl00__main_main_ucGeneralWellInformation_lblProposedFormation")},
            {"ProposedDepth", ParseField(response, "ctl00_ctl00__main_main_ucGeneralWellInformation_lblProposedDepth")},
            {"MeasuredVerticalDepth", ParseField(response, "ctl00_ctl00__main_main_ucGeneralWellInformation_lblMeasuredVerticalDepth")},
            {"TrueVerticalDepth", ParseField(response, "ctl00_ctl00__main_main_ucGeneralWellInformation_lblTrueVerticalDepth")},
            {"PlugbackMeasuredDepth", ParseField(response, "ctl00_ctl00__main_main_ucGeneralWellInformation_lblPlugbackMeasuredDpth")},

            // Dates
            {"ApdInitialApprovalDate", ParseField(response, "ctl00_ctl00__main_main_ucGeneralWellInformation_lblApdInitialApprovalDate")},
            {"ApdEffectiveDate", ParseField(response, "ctl00_ctl00__main_main_ucGeneralWellInformation_lblApdEffectiveDate")},
            {"ApdCancellationDate", ParseField(response, "ctl00_ctl00__main_main_ucGeneralWellInformation_lblApdCancellationDate")},
            {"ApdExtensionApprovalEffectiveDate", ParseField(response, "ctl00_ctl00__main_main_ucGeneralWellInformation_lblApdExtensionApprovalEffectiveDate")},
            {"SpudDate", ParseField(response, "ctl00_ctl00__main_main_ucGeneralWellInformation_lblSpudDate")},
            {"TADate", ParseField(response, "ctl00_ctl00__main_main_ucGeneralWellInformation_lblTADate")},
            {"ShutInWaitingForPipelineDate", ParseField(response, "ctl00_ctl00__main_main_ucGeneralWellInformation_lblShutInWaitingForPipelineDate")},
            {"PluggedAbandonedDateIntent", ParseField(response, "ctl00_ctl00__main_main_ucGeneralWellInformation_lblPluggedAbandonedDateIntent")},
            {"PluggedDate", ParseField(response, "ctl00_ctl00__main_main_ucGeneralWellInformation_lblPluggedDate")},
            {"SiteReleaseDate", ParseField(response, "ctl00_ctl00__main_main_ucGeneralWellInformation_lblSiteReleaseDate")},
            {"LastInspectionDate", ParseField(response, "ctl00_ctl00__main_main_ucGeneralWellInformation_lblLastInspectionDate")},
            {"LastInspectionDateLabel", ParseField(response, "ctl00_ctl00__main_main_ucGeneralWellInformation_lblLastInspectionDateLable")},
            {"ApdExpirationDate", ParseField(response, "ctl00_ctl00__main_main_ucGeneralWellInformation_lblApdExpirationDate")},
            {"GasCapturePlanDate", ParseField(response, "ctl00_ctl00__main_main_ucGeneralWellInformation_lblGasCapturePlanDate")},
            {"TAExpirationDate", ParseField(response, "ctl00_ctl00__main_main_ucGeneralWellInformation_lblTAExpirationDate")},
            {"PluggedNotReleasedExpirationDate", ParseField(response, "ctl00_ctl00__main_main_ucGeneralWellInformation_lblPluggedNotReleasedExpirationDate")},
            {"LastMitDate", ParseField(response, "ctl00_ctl00__main_main_ucGeneralWellInformation_lblLastMitDate")}
        };

        return fields;
    }
}