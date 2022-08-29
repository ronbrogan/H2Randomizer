using System.Collections.Generic;

using static H2Randomizer.Levels.QuarantineZoneCharacters;
using static H2Randomizer.Levels.QuarantineZoneWeapons;

namespace H2Randomizer.Levels;

public enum QuarantineZoneCharacters
{
    sentinel_enforcer = 0,
    elite = 1,
    floodcombat_elite = 2,
    flood_carrier = 3,
    marine = 4,
    flood_infection = 5,
    elite_specops = 6,
    flood_combat_human = 7,
    sentinel_aggressor_eliminator = 8,
    marine2 = 9,
    sentinel_aggressor = 10,
    sentinel_aggressor_major = 11,
}
public enum QuarantineZoneWeapons
{
    plasma_rifle = 0,
    covenant_carbine = 1,
    needler = 2,
    shotgun = 3,
    smg = 4,
    battle_rifle = 5,
    rocket_launcher = 6,
    beam_rifle = 7,
    sentinel_aggressor_beam = 8,
    energy_blade = 9,
    sent_agg_beam_elim = 10,
    sniper_rifle = 11,
    head_sp = 12,
}
public class QuarantineZoneData : BaseLevelData<QuarantineZoneCharacters, QuarantineZoneWeapons>
{
    public override QuarantineZoneCharacters[] ValidCharacters => new[] { elite, floodcombat_elite, flood_carrier, marine, elite_specops, flood_combat_human, sentinel_aggressor_eliminator, marine, sentinel_aggressor, sentinel_aggressor_major };
    public override QuarantineZoneWeapons[] ValidWeapons => new[] { plasma_rifle, covenant_carbine, needler, shotgun, smg, battle_rifle, rocket_launcher, beam_rifle, sentinel_aggressor_beam, energy_blade, sniper_rifle };
}
