using System.Collections.Generic;

using static H2Randomizer.Levels.GravemindCharacters;
using static H2Randomizer.Levels.GravemindWeapons;

namespace H2Randomizer.Levels;

public enum GravemindCharacters
{
    bugger = 0,
    jackal_major = 1,
    jackal = 2,
    elite_zealot = 4,
    elite = 5,
    elite_honor_guard = 6,
    elite_major = 7,
    elite_specops = 8,
    elite_stealth = 9,
    elite_stealth_major = 10,
    elite_ultra = 11,
    grunt_ultra = 12,
    grunt = 13,
    grunt_major = 14,
    grunt_specops = 15,
    hunter = 16,
    marine = 17,
    brute_honor_guard = 18,
    brute = 19,
    brute_captain = 20,
    cortana = 21,
    elite_ranger = 22,
    jackal_sniper = 23,
    grunt_heavy = 24,
    flood_infection = 26,
    flood_infection2 = 27,
}
public enum GravemindWeapons
{
    flak_cannon = 0,
    brute_shot = 1,
    covenant_carbine = 2,
    plasma_rifle = 3,
    plasma_cannon = 4,
    energy_blade = 5,
    needler = 6,
    plasma_pistol = 7,
    beam_rifle = 8,
    brute_plasma_rifle = 9,
    pike = 10,
    c_turret_mp_item = 11,
    head = 12,
}
public class GravemindData : BaseLevelData<GravemindCharacters, GravemindWeapons>
{
    public override GravemindCharacters[] ValidCharacters => new[] { bugger, jackal_major, jackal, elite_zealot, elite, elite_honor_guard, elite_major, elite_specops, elite_stealth, elite_stealth_major, elite_ultra, grunt_ultra, grunt, grunt_major, grunt_specops, hunter, marine, brute_honor_guard, brute, brute_captain, elite_ranger, jackal_sniper, grunt_heavy };
    public override GravemindWeapons[] ValidWeapons => new[] { flak_cannon, brute_shot, covenant_carbine, plasma_rifle, plasma_cannon, energy_blade, needler, plasma_pistol, beam_rifle, brute_plasma_rifle };

    public override int[] BannedSquadIndexes => new[] { 84, 87 }; // imprisoned marines, causes softlock if the starting profile for them isn't supported by actor
}
