namespace WinPure.AddressVerification.Models.Dto;

public class VerificationErrorDto
{
    public int Number { get; set; }
    public string Description { get; set; }
    public string Cause { get; set; }
    public string Resolution { get; set; }
}