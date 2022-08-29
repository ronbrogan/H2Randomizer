using System.Collections.Generic;

using static H2Randomizer.Levels.MetropolisCharacters;
using static H2Randomizer.Levels.MetropolisWeapons;

namespace H2Randomizer.Levels;

public enum MetropolisCharacters
{
    elite = 0,
    marine = 1,
    grunt = 2,
    jackal = 3,
    elite_ultra = 4,
    marine_female = 5,
    elite_major = 6,
    grunt_major = 7,
    grunt_ultra = 8,
    marine_johnson = 9,
    jackal_sniper = 10,
    marine_sgt = 11,
    elite_stealth = 12,
    elite_zealot = 13,
    grunt_heavy = 14,
}
public enum MetropolisWeapons
{
    plasma_pistol = 0,
    plasma_rifle = 1,
    battle_rifle = 2,
    smg = 3,
    sniper_rifle = 4,
    rocket_launcher = 6,
    shotgun = 7,
    needler = 8,
    beam_rifle = 9,
    c_turret_mp_item = 10,
    h_turret_mp_weapon = 11,
    scarab_main_gun_handheld = 12,
    energy_blade = 13,
    head_sp = 14,
}
public class MetropolisData : BaseLevelData<MetropolisCharacters, MetropolisWeapons>
{
    public override MetropolisCharacters[] ValidCharacters => new[] { elite, marine, grunt, jackal, elite_ultra, marine_female, elite_major, grunt_major, grunt_ultra, jackal_sniper, marine_sgt, elite_stealth, elite_zealot, grunt_heavy };
    public override MetropolisWeapons[] ValidWeapons => new[] { plasma_pistol, plasma_rifle, battle_rifle, smg, sniper_rifle, rocket_launcher, shotgun, needler, beam_rifle, energy_blade };
}
