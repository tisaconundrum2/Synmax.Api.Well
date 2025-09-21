using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using HtmlAgilityPack;

namespace Synmax.Api.Well.Services;

public class WellDetailsParser
{
    private static readonly HttpClient client = new HttpClient();

    public async Task<string> GetWellDetails(string apiNumber)
    {
        var url = $"https://wwwapps.emnrd.nm.gov/OCD/OCDPermitting/Data/WellDetails.aspx?api={apiNumber}"; // shouldn't be hardcoded
        return await client.GetStringAsync(url);
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
            {"LocationText", ParseField(response, "ctl00_ctl00__main_main_ucGeneralWellInformation_Location_lblText")},
            {"Lot", ParseField(response, "ctl00_ctl00__main_main_ucGeneralWellInformation_Location_lblLot")},
            {"FootageNSH", ParseField(response, "ctl00_ctl00__main_main_ucGeneralWellInformation_Location_lblFootageNSH")},
            {"FootageEW", ParseField(response, "ctl00_ctl00__main_main_ucGeneralWellInformation_Location_lblFootageEW")},
            {"Coordinates", ParseField(response, "ctl00_ctl00__main_main_ucGeneralWellInformation_Location_lblCoordinates")},

            // Elevation Information
            {"GLElevation", ParseField(response, "ctl00_ctl00__main_main_ucGeneralWellInformation_lblGLElevation")},
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