using System.Collections.Generic;

using static H2Randomizer.Levels.OracleCharacters;
using static H2Randomizer.Levels.OracleWeapons;

namespace H2Randomizer.Levels;

public enum OracleCharacters
{
    heretic = 0,
    sentinel_aggressor_halo1 = 1,
    floodcombat_elite = 2,
    elite_specops = 3,
    grunt_specops = 4,
    heretic_leader = 5,
    heretic_leader_hologram = 6,
    flood_infection = 7,
    flood_carrier = 8,
    monitor = 9,
    elite_specops_commander = 10,
    heretic_grunt = 11,
    dervish = 12,
}
public enum OracleWeapons
{
    plasma_rifle = 0,
    needler = 1,
    plasma_pistol = 2,
    sentinel_aggressor_beam = 3,
    energy_blade = 4,
    covenant_carbine = 5,
    head_sp = 6,
}
public class OracleData : BaseLevelData<OracleCharacters, OracleWeapons>
{
    public override OracleCharacters[] ValidCharacters => new[] { heretic, sentinel_aggressor_halo1, floodcombat_elite, elite_specops, grunt_specops, heretic_leader_hologram, flood_carrier, heretic_grunt };
    public override OracleWeapons[] ValidWeapons => new[] { plasma_rifle, needler, plasma_pistol, sentinel_aggressor_beam, energy_blade, covenant_carbine };

    public override int[] BannedSquadIndexes => new int[] { 29, 30, 31, 32, 38, 39, 40, 132 }; // lab fight and boss fight
}
