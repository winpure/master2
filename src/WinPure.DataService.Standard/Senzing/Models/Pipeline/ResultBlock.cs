using WinPure.DataService.Senzing.Models.G2;

namespace WinPure.DataService.Senzing.Models.Pipeline;

public class ResultBlock : BaseBlock
{
    //public List<long> EntityIds { get; set; }
    public List<EntityResult> Entities { get; set; }
}