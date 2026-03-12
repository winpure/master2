namespace WinPure.Matching.Algorithm;

internal interface IFuzzyComparison
{
    double CompareString(string firstWord, string secondWord);
    StringCondition CreateStringCondition(string value, MatchConditionBase condition, IDictionaryService dictionaryService);
}