using System.Collections.Generic;
using WinPure.Common.Exceptions;

namespace WinPure.Common.Helpers;

public static class EnumExtension
{
    private static readonly List<Tuple<Enum, Attribute>> Cache = new List<Tuple<Enum, Attribute>>();
    private static readonly Dictionary<object, object> DisplayNameDictionary = new Dictionary<object, object>();

    /// <summary>
    /// Gets an attribute on an enum field value
    /// </summary>
    /// <typeparam name="T">The type of the attribute you want to retrieve</typeparam>
    /// <param name="enumVal">The enum value</param>
    /// <returns>The attribute of type T that exists on the enum value</returns>
    public static T GetAttributeOfType<T>(this Enum enumVal) where T : Attribute
    {
        if (Cache.Any(x => Equals(x.Item1, enumVal) && x.Item2 is T))
        {
            return Cache.Where(x => Equals(x.Item1, enumVal) && x.Item2 is T).Select(x => x.Item2 as T).FirstOrDefault();
        }

        var type = enumVal.GetType();
        var memInfo = type.GetMember(enumVal.ToString());
        var attributes = memInfo[0].GetCustomAttributes(typeof(T), false);
        var attr = (attributes.Length > 0) ? (T)attributes[0] : null;
        Cache.Add(new Tuple<Enum, Attribute>(enumVal, attr));
        return attr;
    }

    public static Dictionary<string, T> GetDisplayNameDictionary<T>() where T : IComparable, IFormattable, IConvertible
    {
        if (!typeof(T).IsEnum)
        {
            throw new WinPureArgumentException("en must be enum type");
        }

        if (DisplayNameDictionary.ContainsKey(typeof(T).FullName))
        {
            return DisplayNameDictionary[typeof(T).FullName] as Dictionary<string, T>;
        }

        var res = new Dictionary<string, T>();
        foreach (T op in (T[])Enum.GetValues(typeof(T)))
        {
            res.Add((op as Enum).GetAttributeOfType<DisplayNameAttribute>().DisplayName, op);
        }

        DisplayNameDictionary.Add(typeof(T).FullName, res);
        return res;
    }

    public static class EnumHelper
    {
        public static List<string> GetEnumNames<T>() where T : Enum
        {
            return Enum.GetNames(typeof(T)).ToList();
        }
    }
}