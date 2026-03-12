namespace WinPure.AddressVerification.Models.Dto;

internal class ServerOptionsDto
{
    public int? MinimumMatchScore { get; set; }
    public int? MaximumGeoDistance { get; set; }
    public int? MinimumGeoAccuracyLevel { get; set; }
    public int? MinimumVerificationLevel { get; set; }
    public int? MaxResults { get; set; }
    public string ForceCountry { get; set; }
}