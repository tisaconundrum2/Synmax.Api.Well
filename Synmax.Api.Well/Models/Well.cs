namespace Synmax.Api.Well.Models;

public class Well
{
    public string Operator { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string WellType { get; set; } = string.Empty;
    public string WorkType { get; set; } = string.Empty;
    public string DirectionalStatus { get; set; } = string.Empty;
    public string MultiLateral { get; set; } = string.Empty;
    public string MineralOwner { get; set; } = string.Empty;
    public string SurfaceOwner { get; set; } = string.Empty;
    public string SurfaceLocation { get; set; } = string.Empty;
    public double GLElevation { get; set; }
    public double KBElevation { get; set; }
    public double DFElevation { get; set; }
    public string SingleMultipleCompletion { get; set; } = string.Empty;
    public string PotashWaiver { get; set; } = string.Empty;
    public DateTime SpudDate { get; set; }
    public DateTime LastInspection { get; set; }
    public double TVD { get; set; }
    public string API { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string CRS { get; set; } = string.Empty;
}
