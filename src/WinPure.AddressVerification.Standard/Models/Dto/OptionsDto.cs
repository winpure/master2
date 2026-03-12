using Newtonsoft.Json;

namespace WinPure.AddressVerification.Models.Dto;

[JsonObject(Title = "Options")]
internal class OptionsDto
{
    public ServerOptionsDto ServerOptions { get; set; }
    public string Process { get; set; }
    public bool? Certify { get; set; }
}