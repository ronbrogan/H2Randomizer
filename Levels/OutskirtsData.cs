using System.Collections.Generic;

using static H2Randomizer.Levels.OutskirtsCharacters;
using static H2Randomizer.Levels.OutskirtsWeapons;

namespace H2Randomizer.Levels;

public enum OutskirtsCharacters
{
    elite = 0,
    marine = 1,
    grunt = 2,
    jackal = 3,
    bugger = 4,
    hunter = 5,
    marine_johnson = 6,
    elite_ultra = 7,
    jackal_sniper = 8,
    marine_sgt = 9,
    elite_zealot = 10,
    elite_stealth = 11,
    elite_major = 12,
    grunt_heavy = 13,
    marine2 = 14,
}
public enum OutskirtsWeapons
{
    plasma_pistol = 0,
    plasma_rifle = 1,
    battle_rifle = 2,
    smg = 3,
    sniper_rifle = 4,
    magnum = 5,
    hunter_particle_cannon = 7,
    rocket_launcher = 8,
    needler = 9,
    beam_rifle = 10,
    h_turret_mp_item = 11,
    c_turret_mp_item = 12,
    scarab_main_gun_handheld = 13,
    energy_blade = 14,
    head_sp = 15,
    big_needler_handheld = 16,
}
public class OutskirtsData : BaseLevelData<OutskirtsCharacters, OutskirtsWeapons>
{
    public override OutskirtsCharacters[] ValidCharacters => new[] { elite, marine, grunt, jackal, bugger, hunter, elite_ultra, jackal_sniper, marine_sgt, elite_zealot, elite_stealth, elite_major, grunt_heavy };
    public override OutskirtsWeapons[] ValidWeapons => new[] { plasma_pistol, plasma_rifle, battle_rifle, smg, sniper_rifle, magnum, rocket_launcher, needler, beam_rifle, energy_blade };
}
