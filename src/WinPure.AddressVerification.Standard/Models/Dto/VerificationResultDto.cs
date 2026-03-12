using System.Collections.Generic;

namespace WinPure.AddressVerification.Models.Dto;

internal class VerificationResultDto
{
    public AddressVerificationData Input { get; set; }
    public List<MatchAddressDto> Matches { get; set; }
}