using System.Collections.Generic;

using static H2Randomizer.Levels.CairoStationCharacters;
using static H2Randomizer.Levels.CairoStationWeapons;

namespace H2Randomizer.Levels;

public enum CairoStationCharacters
{
    elite = 0,
    grunt = 1,
    marine = 2,
    marine_johnson = 3,
    bugger = 4,
    marine_female = 5,
    marine_odst = 6,
    marine_wounded = 7,
    miranda = 8,
    elite_ranger = 9,
    elite_specops = 10,
    grunt_heavy = 11,
    elite_stealth = 12,
    cortana = 13,
    marine_dress = 14,
    elite_zealot = 15,
    elite_ultra = 16,
    grunt_ultra = 17,
    elite_major = 18,
    marine_johnson_dress = 19,
}
public enum CairoStationWeapons
{
    battle_rifle = 0,
    smg = 1,
    magnum = 2,
    plasma_pistol = 3,
    needler = 4,
    plasma_rifle = 5,
    h_turret_mp_item = 6,
    c_turret_mp_item = 7,
    shotgun = 8,
    energy_blade = 9,
    head_sp = 10,
}
public class CairoStationData : BaseLevelData<CairoStationCharacters, CairoStationWeapons>
{
    public override CairoStationCharacters[] ValidCharacters => new[] { elite, grunt, marine, bugger, marine_female, marine_odst, marine_wounded, elite_ranger, elite_specops, grunt_heavy, elite_stealth, marine_dress, elite_zealot, elite_ultra, grunt_ultra, elite_major };
    public override CairoStationWeapons[] ValidWeapons => new[] { battle_rifle, smg, magnum, plasma_pistol, needler, plasma_rifle, shotgun, energy_blade };
}
