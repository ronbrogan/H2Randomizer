using System.Collections.Generic;

using static H2Randomizer.Levels.ArmoryCharacters;
using static H2Randomizer.Levels.ArmoryWeapons;

namespace H2Randomizer.Levels;

public enum ArmoryCharacters
{
    marine_johnson = 0,
    crewman = 1,
    marine = 2,
    marine_massive = 3,
    marine_johnson_dress = 4,
}
public enum ArmoryWeapons
{
    head_sp = 0,
}
public class ArmoryData : BaseLevelData<ArmoryCharacters, ArmoryWeapons>
{
    public override ArmoryCharacters[] ValidCharacters => new[] { marine_johnson, crewman, marine, marine_massive, marine_johnson_dress };
    public override ArmoryWeapons[] ValidWeapons => new[] { head_sp };
}
