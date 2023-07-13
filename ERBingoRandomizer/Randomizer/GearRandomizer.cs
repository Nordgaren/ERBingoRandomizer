using FSParam;
using System.Collections.Generic;
using System.Linq;
using static ERBingoRandomizer.Const;

namespace ERBingoRandomizer.Randomizer; 

public partial class BingoRandomizer {
    private int getRandomWeapon(int id) {
        while (true) {
            int newWeapon = _weaponDictionary.Keys.ElementAt(_random.Next(_weaponDictionary.Keys.Count));
            if (_weaponDictionary.ContainsKey(newWeapon) && newWeapon != id) {
                return newWeapon;
            }
        }
    }
    private object chanceGetRandomWeapon(int id) {
        if (ReturnNoItem(id)) {
            return NoItem;
        }

        return getRandomWeapon(id);
    }
    private int getRandomHelm(int id) {
        while (true) {
            List<Param.Row> helms = _armorTypeDictionary[HelmType];
            int newHelm = helms[_random.Next(helms.Count)].ID;
            if (newHelm != id) {
                return newHelm;
            }
        }
    }
    private object chanceGetRandomHelm(int id) {
        if (ReturnNoItem(id)) {
            return NoItem;
        }

        return getRandomHelm(id);
    }
    private int getRandomBody(int id) {
        while (true) {
            List<Param.Row> bodies = _armorTypeDictionary[BodyType];
            int newBody = bodies[_random.Next(bodies.Count)].ID;
            if (newBody != id) {
                return newBody;
            }
        }
    }
    private object chanceGetRandomBody(int id) {
        if (ReturnNoItem(id)) {
            return NoItem;
        }

        return getRandomBody(id);
    }
    private int getRandomArms(int id) {
        while (true) {
            List<Param.Row> arms = _armorTypeDictionary[ArmType];
            int newArms = arms[_random.Next(arms.Count)].ID;
            if (newArms != id) {
                return newArms;
            }
        }
    }
    private object chanceGetRandomArms(int id) {
        if (ReturnNoItem(id)) {
            return NoItem;
        }

        return getRandomArms(id);
    }
    private int getRandomLegs(int id) {
        while (true) {
            List<Param.Row> legs = _armorTypeDictionary[LegType];
            int newLegs = legs[_random.Next(legs.Count)].ID;
            if (newLegs != id) {
                return newLegs;
            }
        }
    }
    private object chanceGetRandomLegs(int id) {
        if (ReturnNoItem(id)) {
            return NoItem;
        }

        return getRandomLegs(id);
    }
    private bool ReturnNoItem(int id) {
        float target = _random.NextSingle();
        
        // If the entry is -1, return -1 99.99% of the time. If it's not, return -1 0.01% of the time
        // This makes it a small chance for a no item to become an item, and a small chance for an item to become no item.
        if (id == -1) {
            if (target > Chance) {
                return true;
            }
        } else {
            if (target < Chance) {
                return true;
            }
        }

        return false;
    }
}
