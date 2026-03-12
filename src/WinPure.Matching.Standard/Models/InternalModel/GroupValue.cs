namespace WinPure.Matching.Models;

internal class GroupValue
{
    private readonly MatchGroupBase _group;
    private string _hashCode;
    internal string HashCodeString { get; set; }

    public GroupValue(MatchGroupBase group)
    {
        _group = group;
        Values = new List<IConditionValue>();
    }

    public int GroupId => _group.GroupId;
    public double MinGroupRate => _group.GroupLevel;

    public List<IConditionValue> Values { get; }

    public void CalculateHashCode()
    {
        HashCodeString = string.Join("$", Values.Select(x => x.ValueWithNullHandling));

        _hashCode = HashHelper.GetHash(HashCodeString);
    }

    public override bool Equals(object obj)
    {
        var item = obj as GroupValue;

        if (item == null || string.IsNullOrWhiteSpace(HashCodeString) || string.IsNullOrWhiteSpace(item.HashCodeString))
        {
            return false;
        }

        return GroupId == item.GroupId && _hashCode == item.HashCode() && item.HashCodeString.Length == HashCodeString.Length;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public string HashCode()
    {
        return _hashCode;
    }
}