namespace WinPure.DataService.Senzing.Models.G2;

public class EntityResult
{
    public ResolvedEntity Resolved_Entity { get; set; }
    public List<RelatedEntity> Related_Entities { get; set; }
}