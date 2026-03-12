namespace WinPure.DataService.Senzing.Models.G2;

public class RelatedEntity : SenzingEntity
{
    public string Match_Level_Code { get; set; }

    public int Match_Level => (int)(Match_Level_Code == "POSSIBLY_SAME" ? PossibilityType.Duplicated
        : Match_Level_Code == "POSSIBLY_RELATED" ? PossibilityType.Related : PossibilityType.None);
    
    public string Match_Key { get; set; }

    public override bool Equals(object obj)
    {
        var item = obj as RelatedEntity;

        if (item == null)
        {
            return false;
        }

        return item.Entity_Id == Entity_Id;
    }
}