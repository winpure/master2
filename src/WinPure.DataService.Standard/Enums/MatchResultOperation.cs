namespace WinPure.DataService.Enums;

internal enum MatchResultOperation
{
    Matching = 0,
    Update = 1,
    Delete = 2,
    SetMasterRecord = 3,
    Merge = 4,
    NotDuplicate = 6,
    Duplicate = 7
}