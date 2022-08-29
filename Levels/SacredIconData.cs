using System.Collections.Generic;

using static H2Randomizer.Levels.SacredIconCharacters;
using static H2Randomizer.Levels.SacredIconWeapons;

namespace H2Randomizer.Levels;

public enum SacredIconCharacters
{
    sentinel_aggressor = 0,
    sentinel_enforcer = 1,
    floodcombat_elite = 2,
    flood_carrier = 3,
    marine = 4,
    jackal_major = 5,
    flood_infection = 6,
    elite_specops = 7,
    flood_infection2 = 8,
    flood_combat_human = 9,
    sentinel_aggressor_major = 10,
    sentinel_aggressor_eliminator = 11,
    elite_specops_commander = 12,
    sentinel_constructor = 13,
    grunt_major = 14,
    grunt_ultra = 15,
    grunt_heavy = 16,
    jackal_sniper = 17,
    brute = 18,
    elite_honor_guard = 19,
}
public enum SacredIconWeapons
{
    plasma_rifle = 0,
    covenant_carbine = 1,
    plasma_pistol = 2,
    needler = 3,
    shotgun = 4,
    smg = 5,
    battle_rifle = 6,
    sentinel_aggressor_beam = 7,
    energy_blade = 8,
    sentinel_aggressor_welder = 9,
    sent_agg_beam_elim = 10,
    magnum = 11,
    phantom_turret_handheld = 12,
    rocket_launcher = 13,
    head_sp = 14,
}
public class SacredIconData : BaseLevelData<SacredIconCharacters, SacredIconWeapons>
{
    public override SacredIconCharacters[] ValidCharacters => new[] { sentinel_aggressor, floodcombat_elite, flood_carrier, marine, jackal_major, elite_specops, flood_combat_human, sentinel_aggressor_major, sentinel_aggressor_eliminator, grunt_major, grunt_ultra, grunt_heavy, jackal_sniper, brute, elite_honor_guard };
    public override SacredIconWeapons[] ValidWeapons => new[] { plasma_rifle, covenant_carbine, plasma_pistol, needler, shotgun, smg, battle_rifle, sentinel_aggressor_beam, energy_blade, magnum, rocket_launcher };

    public override int[] BannedSquadIndexes => new int[] { 83, 84, 85, 86, 87, 88, 90, 91, 92, 93, 94, 95, 96, 97, 98, 107, 108, 109, 110, 111, 112 }; // end fight squads, cutscene
}
