using System.Collections.Generic;

using static H2Randomizer.Levels.RegretCharacters;
using static H2Randomizer.Levels.RegretWeapons;

namespace H2Randomizer.Levels;

public enum RegretCharacters
{
    elite = 0,
    elite_honor_guard = 1,
    grunt = 2,
    jackal = 3,
    hunter = 4,
    bugger = 5,
    marine = 6,
    marine_female = 7,
    elite_ranger = 8,
    jackal_sniper = 9,
    prophet_regret = 10,
    elite_stealth = 11,
    marine_sgt = 12,
}
public enum RegretWeapons
{
    energy_blade = 0,
    plasma_pistol = 1,
    needler = 2,
    battle_rifle = 3,
    beam_rifle = 4,
    covenant_carbine = 5,
    plasma_rifle = 6,
    smg = 7,
    sniper_rifle = 8,
    shotgun = 9,
    hunter_particle_cannon = 10,
    rocket_launcher = 11,
    flak_cannon = 12,
    c_turret_mp_item = 13,
    head_sp = 14,
    magnum = 15,
}
public class RegretData : BaseLevelData<RegretCharacters, RegretWeapons>
{
    public override RegretCharacters[] ValidCharacters => new[] { elite, elite_honor_guard, grunt, jackal, hunter, bugger, marine, marine_female, elite_ranger, jackal_sniper, prophet_regret, elite_stealth, marine_sgt };
    public override RegretWeapons[] ValidWeapons => new[] { energy_blade, plasma_pistol, needler, battle_rifle, beam_rifle, covenant_carbine, plasma_rifle, smg, sniper_rifle, shotgun, hunter_particle_cannon, rocket_launcher, flak_cannon, magnum };
}
