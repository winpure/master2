using System.Collections.Generic;

namespace WinPure.AddressVerification.Models.Dto;

internal class AddressRequestDto
{
    public string Key { get; set; }
    public bool Geocode { get; set; }
    public OptionsDto Options { get; set; }

    // limit is 1000 addresses. but currently we have to use only 200 for one request. 
    public List<AddressVerificationData> Addresses { get; set; }
}