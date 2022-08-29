using System.Collections.Generic;

using static H2Randomizer.Levels.DeltaHaloCharacters;
using static H2Randomizer.Levels.DeltaHaloWeapons;

namespace H2Randomizer.Levels;

public enum DeltaHaloCharacters
{
    marine = 0,
    elite = 1,
    bugger = 2,
    grunt = 3,
    jackal = 4,
    elite_honor_guard = 5,
    marine_odst = 6,
    marine_female = 7,
    elite_stealth = 8,
    jackal_sniper = 9,
    elite_ranger = 10,
    grunt_heavy = 11,
    marine_sgt = 12,
}
public enum DeltaHaloWeapons
{
    plasma_rifle = 0,
    battle_rifle = 1,
    smg = 2,
    sniper_rifle = 3,
    covenant_carbine = 4,
    beam_rifle = 5,
    plasma_pistol = 6,
    needler = 7,
    energy_blade = 8,
    rocket_launcher = 9,
    magnum = 10,
    c_turret_mp_item = 11,
    big_needler_handheld = 12,
    head_sp = 13,
    jackal_shield = 14,
}
public class DeltaHaloData : BaseLevelData<DeltaHaloCharacters, DeltaHaloWeapons>
{
    public override DeltaHaloCharacters[] ValidCharacters => new[] { marine, elite, bugger, grunt, jackal, elite_honor_guard, marine_odst, marine_female, elite_stealth, jackal_sniper, elite_ranger, grunt_heavy, marine_sgt };
    public override DeltaHaloWeapons[] ValidWeapons => new[] { plasma_rifle, battle_rifle, smg, sniper_rifle, covenant_carbine, beam_rifle, plasma_pistol, needler, energy_blade, rocket_launcher, magnum };
}
