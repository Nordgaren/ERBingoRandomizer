using FSParam;
using SoulsFormats;
using System.Collections.Generic;

namespace ERBingoRandomizer.Utility;

public static class ExtensionMethods {
    public static bool ApplyParamDefsCarefully(this Param param, List<PARAMDEF> defs) {
        foreach (PARAMDEF def in defs) {
            if (param.ParamType == def.ParamType) {
                param.ApplyParamdef(def);
                return true;
            }
        }

        return false;
    }
}
