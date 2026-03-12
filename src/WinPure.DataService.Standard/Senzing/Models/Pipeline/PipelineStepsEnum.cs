namespace WinPure.DataService.Senzing.Models.Pipeline;

public enum PipelineStepsEnum
{
    Initialize = 0,
    Add = 1,
    Fetch = 2,
    ProcessPossibleRelated = 3,
    ProcessPossibleDuplicated = 4,
    SavePossibilities = 5,
    UpdatePossibilities = 6,
    CleanUp = 7,
}