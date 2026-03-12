using System.Collections;

namespace WinPure.Matching.Models;

/// <summary>
/// Define common interface for matching conditions.
/// </summary>
internal interface IConditionValue : IEqualityComparer
{
    /// <summary>
    /// minimum rate, when the two values are considered equivalent
    /// </summary>
    double MinRate { get; }

    /// <summary>
    /// Matching condition
    /// </summary>
    MatchConditionBase Condition { get; }

    /// <summary>
    /// Matching value
    /// </summary>
    object Value { get; }

    /// <summary>
    /// Matching value with handling NULL values depends from IgnoreNullOption
    /// </summary>
    object ValueWithNullHandling { get; }

    /// <summary>
    /// Type of matching condition
    /// </summary>
    MatchType ConditionType { get; }

    /// <summary>
    /// Match two values and return rate. 
    /// </summary>
    /// <param name="obj">Second value to compare</param>
    /// <param name="coefficientDetail">Coefficient , how strong equality should be</param>
    /// <returns>Coefficient of equality</returns>
    double Compare(IConditionValue obj, int coefficientDetail);
}