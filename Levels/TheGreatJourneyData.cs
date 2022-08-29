using System.Collections.Generic;

using static H2Randomizer.Levels.TheGreatJourneyCharacters;
using static H2Randomizer.Levels.TheGreatJourneyWeapons;

namespace H2Randomizer.Levels;

public enum TheGreatJourneyCharacters
{
    elite = 0,
    jackal = 1,
    brute = 2,
    hunter = 3,
    elite_specops = 4,
    elite_zealot = 5,
    marine = 6,
    marine_johnson = 7,
    elite_specops_commander = 8,
    brute_tartarus = 9,
    brute_captain = 10,
    brute_honor_guard = 11,
    miranda = 12,
    monitor = 13,
    elite_councilor = 14,
    marine_sgt = 15,
    marine_johnson_boss = 16,
    jackal_sniper = 17,
    brute_major = 18,
    bugger = 19,
}
public enum TheGreatJourneyWeapons
{
    plasma_rifle = 0,
    plasma_pistol = 1,
    covenant_carbine = 2,
    brute_shot = 3,
    hunter_particle_cannon = 4,
    needler = 5,
    energy_blade = 6,
    brute_plasma_rifle = 7,
    shotgun = 8,
    beam_rifle = 9,
    gravity_hammer = 10,
    head_sp = 11,
    beam_rifle_noplayer = 12,
}
public class TheGreatJourneyData : BaseLevelData<TheGreatJourneyCharacters, TheGreatJourneyWeapons>
{
    public override TheGreatJourneyCharacters[] ValidCharacters => new[] { elite, jackal, brute, hunter, elite_specops, elite_zealot, marine, elite_specops_commander, brute_captain, brute_honor_guard, elite_councilor, marine_sgt, jackal_sniper, brute_major, bugger };
    public override TheGreatJourneyWeapons[] ValidWeapons => new[] { plasma_rifle, plasma_pistol, covenant_carbine, brute_shot, needler, energy_blade, brute_plasma_rifle, shotgun, beam_rifle };

    public override int[] BannedSquadIndexes => new int[] { 97, 102 }; // tartar sauce
}
