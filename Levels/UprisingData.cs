using System.Collections.Generic;

using static H2Randomizer.Levels.UprisingCharacters;
using static H2Randomizer.Levels.UprisingWeapons;

namespace H2Randomizer.Levels;

public enum UprisingCharacters
{
    elite = 0,
    brute = 1,
    jackal = 2,
    bugger = 3,
    hunter = 4,
    grunt = 5,
    elite_stealth = 6,
    elite_stealth_major = 7,
    elite_zealot = 8,
    elite_specops = 9,
    brute_captain = 10,
    jackal_sniper = 11,
}
public enum UprisingWeapons
{
    plasma_rifle = 0,
    plasma_pistol = 1,
    brute_shot = 2,
    covenant_carbine = 3,
    energy_blade = 4,
    beam_rifle = 5,
    needler = 6,
    rocket_launcher = 7,
    flak_cannon = 8,
    brute_plasma_rifle = 9,
    shotgun = 10,
    head_sp = 11,
}
public class UprisingData : BaseLevelData<UprisingCharacters, UprisingWeapons>
{
    public override UprisingCharacters[] ValidCharacters => new[] { elite, brute, jackal, bugger, hunter, grunt, elite_stealth, elite_stealth_major, elite_zealot, elite_specops, brute_captain, jackal_sniper };
    public override UprisingWeapons[] ValidWeapons => new[] { plasma_rifle, plasma_pistol, brute_shot, covenant_carbine, energy_blade, beam_rifle, needler, rocket_launcher, flak_cannon, brute_plasma_rifle, shotgun };
}
