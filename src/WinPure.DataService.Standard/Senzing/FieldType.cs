namespace WinPure.DataService.Senzing;

public class FieldType
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int ParentId { get; set; }
    public string SystemName { get; set; }
    public string Description { get; set; }
    public string Example { get; set; }
    public bool IsCompany { get; set; }
    public bool IsPerson { get; set; }
}