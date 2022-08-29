using System.Collections.Generic;

using static H2Randomizer.Levels.TheArbiterCharacters;
using static H2Randomizer.Levels.TheArbiterWeapons;

namespace H2Randomizer.Levels;

public enum TheArbiterCharacters
{
    heretic = 0,
    sentinel_aggressor_halo1 = 1,
    elite_specops = 2,
    grunt_specops = 3,
    heretic_leader = 4,
    heretic_grunt = 5,
}
public enum TheArbiterWeapons
{
    plasma_rifle = 0,
    needler = 1,
    plasma_pistol = 2,
    sentinel_aggressor_beam = 3,
    energy_blade = 4,
    beam_rifle = 5,
    covenant_carbine = 6,
    flak_cannon = 7,
    sentinel_aggressor_welder = 8,
    c_turret_mp_item = 9,
    head_sp = 10,
}
public class TheArbiterData : BaseLevelData<TheArbiterCharacters, TheArbiterWeapons>
{
    public override TheArbiterCharacters[] ValidCharacters => new[] { heretic, sentinel_aggressor_halo1, elite_specops, grunt_specops, heretic_grunt };
    public override TheArbiterWeapons[] ValidWeapons => new[] { plasma_rifle, needler, plasma_pistol, sentinel_aggressor_beam, energy_blade, beam_rifle, covenant_carbine, flak_cannon };
    public override int[] BannedSquadIndexes => new int[] { 0, 1, 2, 3, 4, 37, 44, 71, 95 }; // sentinels for hangar and scripted dudes
}
