using FSParam;
using SoulsFormats;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Project.Utility;

public static class ExtensionMethods
{
    public static bool ApplyParamDefsCarefully(this Param param, List<PARAMDEF> defs)
    {
        foreach (PARAMDEF def in defs)
        {
            if (param.ParamType != def.ParamType)
            { continue; }

            param.ApplyParamdef(def);
            return true;
        }
        return false;
    }
    // This is a pop method for IList<T>, because I can't shuffle Queues.
    public static T Pop<T>(this IList<T> list)
    {
        T last = list.Last();
        list.RemoveAt(list.Count - 1);
        return last;
    }
    public static void Shuffle<T>(this IList<T> values, Random rand)
    {
        for (int i = 0; i < values.Count; ++i)
        {
            int k = rand.Next(i + 1);
            (values[k], values[i]) = (values[i], values[k]);
        }
    }
}
