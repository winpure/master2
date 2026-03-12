namespace WinPure.Matching.Helpers;

internal static class Predicate
{
    public static Func<T, bool> OrElse<T>(
        this Func<T, bool> lhs, Func<T, bool> rhs)
    {
        return lhs == null ? rhs : obj => lhs(obj) || rhs(obj);
    }
    public static Func<T, bool> AndAlso<T>(
        this Func<T, bool> lhs, Func<T, bool> rhs)
    {
        return lhs == null ? rhs : obj => lhs(obj) && rhs(obj);
    }
}