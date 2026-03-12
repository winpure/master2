using System.Data;

namespace WinPure.DataService.Senzing.Models.Pipeline;

public class SaveBlock : BaseBlock
{
    public DataTable Data { get; set; }
}