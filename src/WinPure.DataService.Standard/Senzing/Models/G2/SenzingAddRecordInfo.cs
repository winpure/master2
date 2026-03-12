namespace WinPure.DataService.Senzing.Models.G2;

public class SenzingAddRecordInfo
{
    public string Data_Source { get; set; }
    public string Record_Id { get; set; }
    public List<SenzingEntity> Affected_Entities { get; set; }
}