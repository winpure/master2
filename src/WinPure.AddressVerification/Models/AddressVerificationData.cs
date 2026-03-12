namespace WinPure.AddressVerification.Models;

public class AddressVerificationData
{
    public long WinpureId { get; set; }
    public string Address { get; set; }
    public string Address1 { get; set; }
    public string Locality { get; set; }
    public string PostalCode { get; set; }
    public string AdministrativeArea { get; set; }
    public string Country { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
}