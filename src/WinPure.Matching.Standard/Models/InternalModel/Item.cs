using System.Data;

namespace WinPure.Matching.Models.InternalModel;

internal class Item : IComparable
{
    public Item()
    {
        SimilarItems = new List<ItemSimilarity>();
        GroupValues = new List<GroupValue>();
    }

    public string TableName { get; set; }
    public DataRow DataRow { get; set; }
    public Item MainItem { get; set; }
    public List<ItemSimilarity> SimilarItems { get; set; }
    public List<GroupValue> GroupValues { get; }

    public int CountDuplicates { get; set; }
    public int? GroupId { get; set; }
    public bool IsMaster { get; set; }
    public long WinPurePK { get; set; }

    public void AddGroupValue(GroupValue matchGroupValue)
    {
        GroupValues.Add(matchGroupValue);
    }

    public int CompareTo(object obj)
    {
        if (obj == null)
        {
            return 1;
        }

        var itm = obj as Item;
        var tblCompare = string.Compare(TableName, itm.TableName, StringComparison.InvariantCultureIgnoreCase);

        if (tblCompare != 0)
        {
            return tblCompare;
        }

        if (WinPurePK == itm.WinPurePK)
        {
            return 0;
        }

        if (WinPurePK < itm.WinPurePK)
        {
            return -1;
        }
        return 1;
    }

    public override bool Equals(object obj)
    {
        var item = obj as Item;

        if (item == null)
        {
            return false;
        }

        return WinPurePK == item.WinPurePK && TableName == item.TableName;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public string HashCode()
    {
        return $"{TableName}_{WinPurePK}";
    }
}