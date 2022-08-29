using System.Collections.Generic;

using static H2Randomizer.Levels.HighCharityCharacters;
using static H2Randomizer.Levels.HighCharityWeapons;

namespace H2Randomizer.Levels;

public enum HighCharityCharacters
{
    floodcombat_elite = 0,
    brute = 1,
    jackal = 2,
    flood_infection = 3,
    flood_combat_human = 4,
    flood_juggernaut = 5,
    flood_carrier = 6,
    grunt = 7,
    grunt_major = 8,
    grunt_ultra = 9,
    bugger = 10,
    brute_honor_guard = 11,
    brute_major = 12,
    brute_captain = 13,
    jackal_major = 14,
    jackal_sniper = 15,
    floodcombat_elite_shielded = 16,
    cortana = 17,
}
public enum HighCharityWeapons
{
    energy_blade = 0,
    needler = 1,
    plasma_pistol = 2,
    plasma_rifle = 3,
    brute_plasma_rifle = 4,
    brute_shot = 5,
    battle_rifle = 6,
    shotgun = 7,
    smg = 8,
    magnum = 9,
    rocket_launcher = 10,
    sniper_rifle = 11,
    beam_rifle = 12,
    h_turret_mp_item = 13,
    flak_cannon = 14,
    covenant_carbine = 15,
    head_sp = 16,
}
public class HighCharityData : BaseLevelData<HighCharityCharacters, HighCharityWeapons>
{
    public override HighCharityCharacters[] ValidCharacters => new[] { floodcombat_elite, brute, jackal, flood_combat_human, flood_juggernaut, flood_carrier, grunt, grunt_major, grunt_ultra, bugger, brute_honor_guard, brute_major, brute_captain, jackal_major, jackal_sniper, floodcombat_elite_shielded };
    public override HighCharityWeapons[] ValidWeapons => new[] { energy_blade, needler, plasma_pistol, plasma_rifle, brute_plasma_rifle, brute_shot, battle_rifle, shotgun, smg, magnum, rocket_launcher, sniper_rifle, beam_rifle, flak_cannon, covenant_carbine };
}
