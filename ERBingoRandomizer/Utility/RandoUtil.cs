using ERBingoRandomizer.Params;

namespace ERBingoRandomizer.Utility;

public class RandoUtil {
    public static bool ChrCanUseWeapon(EquipParamWeapon wep, CharaInitParam chr) {
        return wep.properStrength <= chr.baseStr
            && wep.properAgility <= chr.baseDex
            && wep.properMagic <= chr.baseMag
            && wep.properFaith <= chr.baseFai
            && wep.properLuck <= chr.baseLuc;
    }

    public static bool ChrCanUseSpell(Magic spell, CharaInitParam chr) {
        return spell.requirementIntellect <= chr.baseMag
            && spell.requirementFaith <= chr.baseFai
            && spell.requirementLuck <= chr.baseLuc;
    }
}
