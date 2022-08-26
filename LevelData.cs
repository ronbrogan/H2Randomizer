using Superintendent.CommandSink;
using Superintendent.Core.Remote;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;

namespace H2Randomizer
{
    public interface ILevelData
    {
        int MaxCharacter { get; }

        int[] ValidCharacters { get; }

        int MaxWeapon { get; }

        int[] ValidWeapons { get; }

        public Dictionary<int, int[]> ValidCharacterWeapons { get; }

        public int[] BannedSquadIndexes => Array.Empty<int>();

    }

    public class LevelData
    {
        private static Dictionary<string, ILevelData> levels = new()
        {
            ["01a_tutorial"] = new CairoStationData(),
            ["01b_spacestation"] = new CairoStationData(),
            ["03a_oldmombasa"] = new OutskirtsData(),
            ["03b_newmombasa"] = new MetropolisData(),
            ["04a_gasgiant"] = new TheArbiterData(),
            ["04b_floodlab"] = new OracleData(),
            ["05a_deltaapproach"] = new DeltaHaloData(),
            ["05b_deltatowers"] = new RegretData(),
            ["06a_sentinelwalls"] = new SacredIconData(),
            ["06b_floodzone"] = new QuarantineZoneData(),
            ["07a_highcharity"] = new GravemindData(),
            ["07b_forerunnership"] = new HighCharityData(),
            ["08a_deltacliffs"] = new UprisingData(),
            ["08b_deltacontrol"] = new TheGreatJourneyData()
        };

        public static bool TryGet(string name, [NotNullWhen(true)] out ILevelData? data)
        {
            return levels.TryGetValue(name, out data);
        }

        public static string GetCharName(ILevelData level, int value)
        {
            var fields = level.GetType().GetFields();

            var charField = fields.FirstOrDefault(f => f.GetRawConstantValue() is int constInt && constInt == value);

            return charField?.Name ?? value.ToString();
        }

        public static string GetWeapName(ILevelData level, int value)
        {
            var fields = level.GetType().GetFields();

            var charField = fields.LastOrDefault(f => f.GetRawConstantValue() is int constInt && constInt == value);

            return charField?.Name ?? value.ToString();
        }

        public unsafe static LevelDataAllocation Write(ILevelData data, MemoryBlock mem, ICommandSink h2)
        {
            var alloc = new LevelDataAllocation();

            // characters
            alloc.CharCount = data.ValidCharacters.Length;

            var allowLookup = new int[data.MaxCharacter + 1];
            for (int i = 0; i <= data.MaxCharacter; i++)
                allowLookup[i] = data.ValidCharacters.Contains(i) ? 1 : 0;

            mem.Allocate(sizeof(int) * allowLookup.Length, out alloc.AllowedChars, alignment: 1);
            mem.Allocate(sizeof(int) * data.ValidCharacters.Length, out alloc.CharIndexes, alignment: 1);

            h2.WriteAt(alloc.AllowedChars, MemoryMarshal.AsBytes<int>(allowLookup));
            h2.WriteAt(alloc.CharIndexes, MemoryMarshal.AsBytes<int>(data.ValidCharacters));

            // banned squad
            alloc.BannedSquadCount = data.BannedSquadIndexes.Length;
            mem.Allocate(sizeof(int) * data.BannedSquadIndexes.Length, out alloc.BannedSquads, alignment: 1);
            h2.WriteAt(alloc.BannedSquads, MemoryMarshal.AsBytes<int>(data.BannedSquadIndexes));

            // weapons
            //alloc.WeapCount = data.ValidWeapons.Length;

            var allowLookup2 = new int[data.MaxWeapon + 1];
            for (int i = 0; i <= data.MaxWeapon; i++)
                allowLookup2[i] = data.ValidWeapons.Contains(i) ? 1 : 0;

            mem.Allocate(sizeof(int) * allowLookup2.Length, out alloc.AllowedWeaps, alignment: 1);
            //mem.Allocate(sizeof(int) * data.ValidWeapons.Length, out alloc.WeapIndexes, alignment: 1);

            h2.WriteAt(alloc.AllowedWeaps, MemoryMarshal.AsBytes<int>(allowLookup2));
            //h2.WriteAt(alloc.WeapIndexes, MemoryMarshal.AsBytes<int>(data.ValidWeapons));


            var chars = data.ValidCharacterWeapons.Keys.ToArray();
            var lookupLength = chars.Max()+1;

            var lookupTableEntrySize = sizeof(int) + sizeof(nint); // count + pointer

            mem.Allocate(lookupTableEntrySize * lookupLength, out alloc.CharWeaponsLookup, alignment: 1);

            foreach(var (key, weaps) in data.ValidCharacterWeapons)
            {
                var spot = alloc.CharWeaponsLookup + lookupTableEntrySize * key;
                mem.Allocate(sizeof(int) * weaps.Length, out var slotData, alignment: 1);

                h2.WriteAt(spot, weaps.Length);
                h2.WriteAt(spot + sizeof(int), slotData);
                h2.WriteAt(slotData, MemoryMarshal.AsBytes<int>(weaps));
            }

            return alloc;
        }
    }

    public struct LevelDataAllocation
    {
        public int CharCount;
        public nint CharIndexes;
        public nint AllowedChars;
        public int BannedSquadCount;
        public nint BannedSquads;
        public nint AllowedWeaps;

        public nint CharWeaponsLookup;
    }


    public class ArmoryData : ILevelData
    {
        public const int marine_johnson = 0;
        public const int crewman = 1;
        public const int marine = 2;
        public const int marine_massive = 3;
        public const int marine_johnson_dress = 4;
        public int MaxCharacter => marine_johnson_dress;
        public const int head_sp = 0;
        public int MaxWeapon => head_sp;
        public int[] ValidCharacters => new[] { marine_johnson, crewman, marine, marine_massive, marine_johnson_dress };
        public int[] ValidWeapons => new[] { head_sp };
        public Dictionary<int, int[]> ValidCharacterWeapons => new Dictionary<int, int[]>
        {
            [marine_johnson] = ValidWeapons,
            [crewman] = ValidWeapons,
            [marine] = ValidWeapons,
            [marine_massive] = ValidWeapons,
            [marine_johnson_dress] = ValidWeapons,
        };
    }



    public class CairoStationData : ILevelData
    {
        public const int elite = 0;
        public const int grunt = 1;
        public const int marine = 2;
        public const int marine_johnson = 3;
        public const int bugger = 4;
        public const int marine_female = 5;
        public const int marine_odst = 6;
        public const int marine_wounded = 7;
        public const int miranda = 8;
        public const int elite_ranger = 9;
        public const int elite_specops = 10;
        public const int grunt_heavy = 11;
        public const int elite_stealth = 12;
        public const int cortana = 13;
        public const int marine_dress = 14;
        public const int elite_zealot = 15;
        public const int elite_ultra = 16;
        public const int grunt_ultra = 17;
        public const int elite_major = 18;
        public const int marine_johnson_dress = 19;
        public int MaxCharacter => marine_johnson_dress;
        public const int battle_rifle = 0;
        public const int smg = 1;
        public const int magnum = 2;
        public const int plasma_pistol = 3;
        public const int needler = 4;
        public const int plasma_rifle = 5;
        public const int h_turret_mp_item = 6;
        public const int c_turret_mp_item = 7;
        public const int shotgun = 8;
        public const int energy_blade = 9;
        public const int head_sp = 10;
        public int MaxWeapon => head_sp;
        public int[] ValidCharacters => new[] { elite, grunt, marine, bugger, marine_female, marine_odst, marine_wounded, elite_ranger, elite_specops, grunt_heavy, elite_stealth, marine_dress, elite_zealot, elite_ultra, grunt_ultra, elite_major };
        public int[] ValidWeapons => new[] { battle_rifle, smg, magnum, plasma_pistol, needler, plasma_rifle, shotgun, energy_blade };
        
        public Dictionary<int, int[]> ValidCharacterWeapons => new Dictionary<int, int[]>
        {
            [elite] = ValidWeapons,
            [grunt] = ValidWeapons.Without(battle_rifle, energy_blade, shotgun, smg),
            [marine] = ValidWeapons.Without(energy_blade),
            [marine_johnson] = ValidWeapons,
            [bugger] = ValidWeapons.Without(energy_blade),
            [marine_female] = ValidWeapons.Without(energy_blade),
            [marine_odst] = ValidWeapons.Without(energy_blade),
            [marine_wounded] = ValidWeapons.Without(energy_blade),
            [miranda] = ValidWeapons.Without(energy_blade),
            [elite_ranger] = ValidWeapons.Without(energy_blade),
            [elite_specops] = ValidWeapons,
            [grunt_heavy] = ValidWeapons.Without(energy_blade, battle_rifle, shotgun, smg),
            [elite_stealth] = ValidWeapons,
            [cortana] = Array.Empty<int>(),
            [marine_dress] = ValidWeapons.Without(energy_blade),
            [elite_zealot] = ValidWeapons,
            [elite_ultra] = ValidWeapons,
            [grunt_ultra] = ValidWeapons.Without(energy_blade, battle_rifle, shotgun, smg),
            [elite_major] = ValidWeapons,
            [marine_johnson_dress] = ValidWeapons.Without(energy_blade),
        };
    }



    public class OutskirtsData : ILevelData
    {
        public const int elite = 0;
        public const int marine = 1;
        public const int grunt = 2;
        public const int jackal = 3;
        public const int bugger = 4;
        public const int hunter = 5;
        public const int marine_johnson = 6;
        public const int elite_ultra = 7;
        public const int jackal_sniper = 8;
        public const int marine_sgt = 9;
        public const int elite_zealot = 10;
        public const int elite_stealth = 11;
        public const int elite_major = 12;
        public const int grunt_heavy = 13;
        public const int marine2 = 14;
        public int MaxCharacter => marine2;
        public const int plasma_pistol = 0;
        public const int plasma_rifle = 1;
        public const int battle_rifle = 2;
        public const int smg = 3;
        public const int sniper_rifle = 4;
        public const int magnum = 5;
        public const int hunter_particle_cannon = 7;
        public const int rocket_launcher = 8;
        public const int needler = 9;
        public const int beam_rifle = 10;
        public const int h_turret_mp_item = 11;
        public const int c_turret_mp_item = 12;
        public const int scarab_main_gun_handheld = 13;
        public const int energy_blade = 14;
        public const int head_sp = 15;
        public const int big_needler_handheld = 16;
        public int MaxWeapon => big_needler_handheld;
        public int[] ValidCharacters => new[] { elite, marine, grunt, jackal, bugger, hunter, elite_ultra, jackal_sniper, marine_sgt, elite_zealot, elite_stealth, elite_major, grunt_heavy, marine };
        public int[] ValidWeapons => new[] { plasma_pistol, plasma_rifle, battle_rifle, smg, sniper_rifle, magnum, rocket_launcher, needler, beam_rifle, energy_blade };
        public Dictionary<int, int[]> ValidCharacterWeapons => new Dictionary<int, int[]>
        {
            [elite] = ValidWeapons,
            [marine] = ValidWeapons.Without(energy_blade),
            [grunt] = ValidWeapons.Without(energy_blade, battle_rifle, smg),
            [jackal] = ValidWeapons.Without(energy_blade, rocket_launcher),
            [bugger] = ValidWeapons.Without(energy_blade),
            [hunter] = ValidWeapons,
            [marine_johnson] = ValidWeapons,
            [elite_ultra] = ValidWeapons,
            [jackal_sniper] = ValidWeapons.Without(energy_blade, rocket_launcher),
            [marine_sgt] = ValidWeapons,
            [elite_zealot] = ValidWeapons,
            [elite_stealth] = ValidWeapons,
            [elite_major] = ValidWeapons,
            [grunt_heavy] = ValidWeapons.Without(energy_blade, battle_rifle, smg),
            [marine] = ValidWeapons.Without(energy_blade),
        };
    }



    public class MetropolisData : ILevelData
    {
        public const int elite = 0;
        public const int marine = 1;
        public const int grunt = 2;
        public const int jackal = 3;
        public const int elite_ultra = 4;
        public const int marine_female = 5;
        public const int elite_major = 6;
        public const int grunt_major = 7;
        public const int grunt_ultra = 8;
        public const int marine_johnson = 9;
        public const int jackal_sniper = 10;
        public const int marine_sgt = 11;
        public const int elite_stealth = 12;
        public const int elite_zealot = 13;
        public const int grunt_heavy = 14;
        public int MaxCharacter => grunt_heavy;
        public const int plasma_pistol = 0;
        public const int plasma_rifle = 1;
        public const int battle_rifle = 2;
        public const int smg = 3;
        public const int sniper_rifle = 4;
        public const int rocket_launcher = 6;
        public const int shotgun = 7;
        public const int needler = 8;
        public const int beam_rifle = 9;
        public const int c_turret_mp_item = 10;
        public const int h_turret_mp_weapon = 11;
        public const int scarab_main_gun_handheld = 12;
        public const int energy_blade = 13;
        public const int head_sp = 14;
        public int MaxWeapon => head_sp;
        public int[] ValidCharacters => new[] { elite, marine, grunt, jackal, elite_ultra, marine_female, elite_major, grunt_major, grunt_ultra, jackal_sniper, marine_sgt, elite_stealth, elite_zealot, grunt_heavy };
        public int[] ValidWeapons => new[] { plasma_pistol, plasma_rifle, battle_rifle, smg, sniper_rifle, rocket_launcher, shotgun, needler, beam_rifle, energy_blade };
        public Dictionary<int, int[]> ValidCharacterWeapons => new Dictionary<int, int[]>
        {
            [elite] = ValidWeapons,
            [marine] = ValidWeapons,
            [grunt] = ValidWeapons.Without(energy_blade, battle_rifle, shotgun, smg),
            [jackal] = ValidWeapons.Without(energy_blade, rocket_launcher),
            [elite_ultra] = ValidWeapons,
            [marine_female] = ValidWeapons.Without(energy_blade),
            [elite_major] = ValidWeapons,
            [grunt_major] = ValidWeapons.Without(energy_blade, battle_rifle, shotgun, smg),
            [grunt_ultra] = ValidWeapons.Without(energy_blade, battle_rifle, shotgun, smg),
            [marine_johnson] = ValidWeapons,
            [jackal_sniper] = ValidWeapons.Without(energy_blade, rocket_launcher),
            [marine_sgt] = ValidWeapons.Without(energy_blade),
            [elite_stealth] = ValidWeapons,
            [elite_zealot] = ValidWeapons,
            [grunt_heavy] = ValidWeapons.Without(energy_blade, battle_rifle, shotgun, smg),
        };
    }



    public class TheArbiterData : ILevelData
    {
        public const int heretic = 0;
        public const int sentinel_aggressor_halo1 = 1;
        public const int elite_specops = 2;
        public const int grunt_specops = 3;
        public const int heretic_leader = 4;
        public const int heretic_grunt = 5;
        public int MaxCharacter => heretic_grunt;
        public const int plasma_rifle = 0;
        public const int needler = 1;
        public const int plasma_pistol = 2;
        public const int sentinel_aggressor_beam = 3;
        public const int energy_blade = 4;
        public const int beam_rifle = 5;
        public const int covenant_carbine = 6;
        public const int flak_cannon = 7;
        public const int sentinel_aggressor_welder = 8;
        public const int c_turret_mp_item = 9;
        public const int head_sp = 10;
        public int MaxWeapon => head_sp;
        public int[] ValidCharacters => new[] { heretic, sentinel_aggressor_halo1, elite_specops, grunt_specops, heretic_grunt };
        public int[] ValidWeapons => new[] { plasma_rifle, needler, plasma_pistol, sentinel_aggressor_beam, energy_blade, beam_rifle, covenant_carbine, flak_cannon };
        public Dictionary<int, int[]> ValidCharacterWeapons => new Dictionary<int, int[]>
        {
            [heretic] = ValidWeapons,
            [sentinel_aggressor_halo1] = ValidWeapons.Without(energy_blade),
            [elite_specops] = ValidWeapons,
            [grunt_specops] = ValidWeapons.Without(energy_blade),
            [heretic_leader] = ValidWeapons,
            [heretic_grunt] = ValidWeapons.Without(energy_blade),
        };

        public int[] BannedSquadIndexes => new int[] { 37, 44, }; // sentinels for hangar
    }



    public class OracleData : ILevelData
    {
        public const int heretic = 0;
        public const int sentinel_aggressor_halo1 = 1;
        public const int floodcombat_elite = 2;
        public const int elite_specops = 3;
        public const int grunt_specops = 4;
        public const int heretic_leader = 5;
        public const int heretic_leader_hologram = 6;
        public const int flood_infection = 7;
        public const int flood_carrier = 8;
        public const int monitor = 9;
        public const int elite_specops_commander = 10;
        public const int heretic_grunt = 11;
        public const int dervish = 12;
        public int MaxCharacter => dervish;
        public const int plasma_rifle = 0;
        public const int needler = 1;
        public const int plasma_pistol = 2;
        public const int sentinel_aggressor_beam = 3;
        public const int energy_blade = 4;
        public const int covenant_carbine = 5;
        public const int head_sp = 6;
        public int MaxWeapon => head_sp;
        public int[] ValidCharacters => new[] { heretic, sentinel_aggressor_halo1, floodcombat_elite, elite_specops, grunt_specops, heretic_leader_hologram, flood_carrier, heretic_grunt };
        public int[] ValidWeapons => new[] { plasma_rifle, needler, plasma_pistol, sentinel_aggressor_beam, energy_blade, covenant_carbine };
        public Dictionary<int, int[]> ValidCharacterWeapons => new Dictionary<int, int[]>
        {
            [heretic] = ValidWeapons,
            [sentinel_aggressor_halo1] = ValidWeapons.Without(energy_blade),
            [floodcombat_elite] = ValidWeapons,
            [elite_specops] = ValidWeapons,
            [grunt_specops] = ValidWeapons.Without(energy_blade),
            [heretic_leader] = new[] { plasma_rifle, needler, plasma_pistol, energy_blade },
            [heretic_leader_hologram] = ValidWeapons,
            [flood_infection] = Array.Empty<int>(),
            [flood_carrier] = Array.Empty<int>(),
            [monitor] = Array.Empty<int>(),
            [elite_specops_commander] = ValidWeapons,
            [heretic_grunt] = ValidWeapons.Without(energy_blade),
            [dervish] = Array.Empty<int>(),
        };

        public int[] BannedSquadIndexes => new int[] { 29, 30, 31, 32, 132 }; // lab fight and boss fight
    }



    public class DeltaHaloData : ILevelData
    {
        public const int marine = 0;
        public const int elite = 1;
        public const int bugger = 2;
        public const int grunt = 3;
        public const int jackal = 4;
        public const int elite_honor_guard = 5;
        public const int marine_odst = 6;
        public const int marine_female = 7;
        public const int elite_stealth = 8;
        public const int jackal_sniper = 9;
        public const int elite_ranger = 10;
        public const int grunt_heavy = 11;
        public const int marine_sgt = 12;
        public int MaxCharacter => marine_sgt;
        public const int plasma_rifle = 0;
        public const int battle_rifle = 1;
        public const int smg = 2;
        public const int sniper_rifle = 3;
        public const int covenant_carbine = 4;
        public const int beam_rifle = 5;
        public const int plasma_pistol = 6;
        public const int needler = 7;
        public const int energy_blade = 8;
        public const int rocket_launcher = 9;
        public const int magnum = 10;
        public const int c_turret_mp_item = 11;
        public const int big_needler_handheld = 12;
        public const int head_sp = 13;
        public const int jackal_shield = 14;
        public int MaxWeapon => jackal_shield;
        public int[] ValidCharacters => new[] { marine, elite, bugger, grunt, jackal, elite_honor_guard, marine_odst, marine_female, elite_stealth, jackal_sniper, elite_ranger, grunt_heavy, marine_sgt };
        public int[] ValidWeapons => new[] { plasma_rifle, battle_rifle, smg, sniper_rifle, covenant_carbine, beam_rifle, plasma_pistol, needler, energy_blade, rocket_launcher, magnum };
        public Dictionary<int, int[]> ValidCharacterWeapons => new Dictionary<int, int[]>
        {
            [marine] = ValidWeapons.Without(energy_blade),
            [elite] = ValidWeapons,
            [bugger] = ValidWeapons.Without(energy_blade),
            [grunt] = ValidWeapons.Without(energy_blade, battle_rifle, smg),
            [jackal] = ValidWeapons.Without(energy_blade, rocket_launcher),
            [elite_honor_guard] = ValidWeapons,
            [marine_odst] = ValidWeapons.Without(energy_blade),
            [marine_female] = ValidWeapons.Without(energy_blade),
            [elite_stealth] = ValidWeapons,
            [jackal_sniper] = ValidWeapons.Without(energy_blade, rocket_launcher),
            [elite_ranger] = ValidWeapons,
            [grunt_heavy] = ValidWeapons.Without(energy_blade, battle_rifle, smg),
            [marine_sgt] = ValidWeapons.Without(energy_blade),
        };
    }



    public class RegretData : ILevelData
    {
        public const int elite = 0;
        public const int elite_honor_guard = 1;
        public const int grunt = 2;
        public const int jackal = 3;
        public const int hunter = 4;
        public const int bugger = 5;
        public const int marine = 6;
        public const int marine_female = 7;
        public const int elite_ranger = 8;
        public const int jackal_sniper = 9;
        public const int prophet_regret = 10;
        public const int elite_stealth = 11;
        public const int marine_sgt = 12;
        public int MaxCharacter => marine_sgt;
        public const int energy_blade = 0;
        public const int plasma_pistol = 1;
        public const int needler = 2;
        public const int battle_rifle = 3;
        public const int beam_rifle = 4;
        public const int covenant_carbine = 5;
        public const int plasma_rifle = 6;
        public const int smg = 7;
        public const int sniper_rifle = 8;
        public const int shotgun = 9;
        public const int hunter_particle_cannon = 10;
        public const int rocket_launcher = 11;
        public const int flak_cannon = 12;
        public const int c_turret_mp_item = 13;
        public const int head_sp = 14;
        public const int magnum = 15;
        public int MaxWeapon => magnum;
        public int[] ValidCharacters => new[] { elite, elite_honor_guard, grunt, jackal, hunter, bugger, marine, marine_female, elite_ranger, jackal_sniper, prophet_regret, elite_stealth, marine_sgt };
        public int[] ValidWeapons => new[] { energy_blade, plasma_pistol, needler, battle_rifle, beam_rifle, covenant_carbine, plasma_rifle, smg, sniper_rifle, shotgun, rocket_launcher, flak_cannon, magnum };
        public Dictionary<int, int[]> ValidCharacterWeapons => new Dictionary<int, int[]>
        {
            [elite] = ValidWeapons,
            [elite_honor_guard] = ValidWeapons,
            [grunt] = ValidWeapons.Without(energy_blade, shotgun, battle_rifle, smg),
            [jackal] = ValidWeapons.Without(energy_blade, rocket_launcher),
            [hunter] = Array.Empty<int>(),
            [bugger] = ValidWeapons.Without(energy_blade, flak_cannon),
            [marine] = ValidWeapons.Without(energy_blade, flak_cannon),
            [marine_female] = ValidWeapons.Without(energy_blade, flak_cannon),
            [elite_ranger] = ValidWeapons,
            [jackal_sniper] = ValidWeapons.Without(energy_blade, rocket_launcher),
            [prophet_regret] = ValidWeapons.Without(energy_blade, rocket_launcher),
            [elite_stealth] = ValidWeapons,
            [marine_sgt] = ValidWeapons.Without(energy_blade, flak_cannon),
        };
    }



    public class SacredIconData : ILevelData
    {
        public const int sentinel_aggressor = 0;
        public const int sentinel_enforcer = 1;
        public const int floodcombat_elite = 2;
        public const int flood_carrier = 3;
        public const int marine = 4;
        public const int jackal_major = 5;
        public const int flood_infection = 6;
        public const int elite_specops = 7;
        public const int flood_infection2 = 8;
        public const int flood_combat_human = 9;
        public const int sentinel_aggressor_major = 10;
        public const int sentinel_aggressor_eliminator = 11;
        public const int elite_specops_commander = 12;
        public const int sentinel_constructor = 13;
        public const int grunt_major = 14;
        public const int grunt_ultra = 15;
        public const int grunt_heavy = 16;
        public const int jackal_sniper = 17;
        public const int brute = 18;
        public const int elite_honor_guard = 19;
        public int MaxCharacter => elite_honor_guard;
        public const int plasma_rifle = 0;
        public const int covenant_carbine = 1;
        public const int plasma_pistol = 2;
        public const int needler = 3;
        public const int shotgun = 4;
        public const int smg = 5;
        public const int battle_rifle = 6;
        public const int sentinel_aggressor_beam = 7;
        public const int energy_blade = 8;
        public const int sentinel_aggressor_welder = 9;
        public const int sent_agg_beam_elim = 10;
        public const int magnum = 11;
        public const int phantom_turret_handheld = 12;
        public const int rocket_launcher = 13;
        public const int head_sp = 14;
        public int MaxWeapon => head_sp;
        public int[] ValidCharacters => new[] { sentinel_aggressor, floodcombat_elite, flood_carrier, marine, jackal_major, elite_specops, flood_combat_human, sentinel_aggressor_major, sentinel_aggressor_eliminator, grunt_major, grunt_ultra, grunt_heavy, jackal_sniper, brute, elite_honor_guard };
        public int[] ValidWeapons => new[] { plasma_rifle, covenant_carbine, plasma_pistol, needler, shotgun, smg, battle_rifle, sentinel_aggressor_beam, energy_blade, magnum, rocket_launcher };
        public Dictionary<int, int[]> ValidCharacterWeapons => new Dictionary<int, int[]>
        {
            [sentinel_aggressor] = ValidWeapons.Without(energy_blade),
            [sentinel_enforcer] = Array.Empty<int>(),
            [floodcombat_elite] = ValidWeapons,
            [flood_carrier] = Array.Empty<int>(),
            [marine] = ValidWeapons.Without(energy_blade),
            [jackal_major] = ValidWeapons.Without(energy_blade, rocket_launcher),
            [flood_infection] = Array.Empty<int>(),
            [elite_specops] = ValidWeapons,
            [flood_infection] = Array.Empty<int>(),
            [flood_combat_human] = ValidWeapons,
            [sentinel_aggressor_major] = ValidWeapons.Without(energy_blade),
            [sentinel_aggressor_eliminator] = ValidWeapons.Without(energy_blade),
            [elite_specops_commander] = ValidWeapons,
            [sentinel_constructor] = new[] { sentinel_aggressor_welder },
            [grunt_major] = ValidWeapons.Without(energy_blade, shotgun, battle_rifle, smg),
            [grunt_ultra] = ValidWeapons.Without(energy_blade, shotgun, battle_rifle, smg),
            [grunt_heavy] = ValidWeapons.Without(energy_blade, shotgun, battle_rifle, smg),
            [jackal_sniper] = ValidWeapons.Without(energy_blade, rocket_launcher),
            [brute] = ValidWeapons.Without(energy_blade),
            [elite_honor_guard] = ValidWeapons,
        };

        public int[] BannedSquadIndexes => new int[] { 83, 84, 85, 86, 87, 88, 90, 91, 92, 93, 94, 95, 96, 97, 98, 107, 108, 109, 110, 111, 112 }; // end fight squads, cutscene
    }



    public class QuarantineZoneData : ILevelData
    {
        public const int sentinel_enforcer = 0;
        public const int elite = 1;
        public const int floodcombat_elite = 2;
        public const int flood_carrier = 3;
        public const int marine = 4;
        public const int flood_infection = 5;
        public const int elite_specops = 6;
        public const int flood_combat_human = 7;
        public const int sentinel_aggressor_eliminator = 8;
        public const int marine2 = 9;
        public const int sentinel_aggressor = 10;
        public const int sentinel_aggressor_major = 11;
        public int MaxCharacter => sentinel_aggressor_major;
        public const int plasma_rifle = 0;
        public const int covenant_carbine = 1;
        public const int needler = 2;
        public const int shotgun = 3;
        public const int smg = 4;
        public const int battle_rifle = 5;
        public const int rocket_launcher = 6;
        public const int beam_rifle = 7;
        public const int sentinel_aggressor_beam = 8;
        public const int energy_blade = 9;
        public const int sent_agg_beam_elim = 10;
        public const int sniper_rifle = 11;
        public const int head_sp = 12;
        public int MaxWeapon => head_sp;
        public int[] ValidCharacters => new[] { elite, floodcombat_elite, flood_carrier, marine, elite_specops, flood_combat_human, sentinel_aggressor_eliminator, marine, sentinel_aggressor, sentinel_aggressor_major };
        public int[] ValidWeapons => new[] { plasma_rifle, covenant_carbine, needler, shotgun, smg, battle_rifle, rocket_launcher, beam_rifle, sentinel_aggressor_beam, energy_blade, sniper_rifle };
        public Dictionary<int, int[]> ValidCharacterWeapons => new Dictionary<int, int[]>
        {
            [sentinel_enforcer] = Array.Empty<int>(),
            [elite] = ValidWeapons,
            [floodcombat_elite] = ValidWeapons,
            [flood_carrier] = Array.Empty<int>(),
            [marine] = ValidWeapons.Without(energy_blade),
            [flood_infection] = Array.Empty<int>(),
            [elite_specops] = ValidWeapons,
            [flood_combat_human] = ValidWeapons,
            [sentinel_aggressor_eliminator] = ValidWeapons,
            [marine] = ValidWeapons.Without(energy_blade),
            [sentinel_aggressor] = ValidWeapons,
            [sentinel_aggressor_major] = ValidWeapons,
        };
    }



    public class GravemindData : ILevelData
    {
        public const int bugger = 0;
        public const int jackal_major = 1;
        public const int jackal = 2;
        public const int elite_zealot = 4;
        public const int elite = 5;
        public const int elite_honor_guard = 6;
        public const int elite_major = 7;
        public const int elite_specops = 8;
        public const int elite_stealth = 9;
        public const int elite_stealth_major = 10;
        public const int elite_ultra = 11;
        public const int grunt_ultra = 12;
        public const int grunt = 13;
        public const int grunt_major = 14;
        public const int grunt_specops = 15;
        public const int hunter = 16;
        public const int marine = 17;
        public const int brute_honor_guard = 18;
        public const int brute = 19;
        public const int brute_captain = 20;
        public const int cortana = 21;
        public const int elite_ranger = 22;
        public const int jackal_sniper = 23;
        public const int grunt_heavy = 24;
        public const int flood_infection = 26;
        public const int flood_infection2 = 27;
        public int MaxCharacter => flood_infection2;
        public const int flak_cannon = 0;
        public const int brute_shot = 1;
        public const int covenant_carbine = 2;
        public const int plasma_rifle = 3;
        public const int plasma_cannon = 4;
        public const int energy_blade = 5;
        public const int needler = 6;
        public const int plasma_pistol = 7;
        public const int beam_rifle = 8;
        public const int brute_plasma_rifle = 9;
        public const int pike = 10;
        public const int c_turret_mp_item = 11;
        public const int head = 12;
        public int MaxWeapon => head;
        public int[] ValidCharacters => new[] { bugger, jackal_major, jackal, elite_zealot, elite, elite_honor_guard, elite_major, elite_specops, elite_stealth, elite_stealth_major, elite_ultra, grunt_ultra, grunt, grunt_major, grunt_specops, hunter, marine, brute_honor_guard, brute, brute_captain, elite_ranger, jackal_sniper, grunt_heavy };
        public int[] ValidWeapons => new[] { flak_cannon, brute_shot, covenant_carbine, plasma_rifle, plasma_cannon, energy_blade, needler, plasma_pistol, beam_rifle, brute_plasma_rifle };
        public Dictionary<int, int[]> ValidCharacterWeapons => new Dictionary<int, int[]>
        {
            [bugger] = ValidWeapons.Without(energy_blade),
            [jackal_major] = ValidWeapons.Without(energy_blade),
            [jackal] = ValidWeapons.Without(energy_blade),
            [elite_zealot] = ValidWeapons,
            [elite] = ValidWeapons,
            [elite_honor_guard] = ValidWeapons,
            [elite_major] = ValidWeapons,
            [elite_specops] = ValidWeapons,
            [elite_stealth] = ValidWeapons,
            [elite_stealth_major] = ValidWeapons,
            [elite_ultra] = ValidWeapons,
            [grunt_ultra] = ValidWeapons.Without(energy_blade),
            [grunt] = ValidWeapons.Without(energy_blade),
            [grunt_major] = ValidWeapons.Without(energy_blade),
            [grunt_specops] = ValidWeapons.Without(energy_blade),
            [hunter] = Array.Empty<int>(),
            [marine] = ValidWeapons.Without(energy_blade),
            [brute_honor_guard] = ValidWeapons.Without(energy_blade),
            [brute] = ValidWeapons.Without(energy_blade),
            [brute_captain] = ValidWeapons.Without(energy_blade),
            [cortana] = ValidWeapons,
            [elite_ranger] = ValidWeapons,
            [jackal_sniper] = ValidWeapons.Without(energy_blade),
            [grunt_heavy] = ValidWeapons.Without(energy_blade),
            [flood_infection] = Array.Empty<int>(),
            [flood_infection] = Array.Empty<int>(),
        };
    }



    public class HighCharityData : ILevelData
    {
        public const int floodcombat_elite = 0;
        public const int brute = 1;
        public const int jackal = 2;
        public const int flood_infection = 3;
        public const int flood_combat_human = 4;
        public const int flood_juggernaut = 5;
        public const int flood_carrier = 6;
        public const int grunt = 7;
        public const int grunt_major = 8;
        public const int grunt_ultra = 9;
        public const int bugger = 10;
        public const int brute_honor_guard = 11;
        public const int brute_major = 12;
        public const int brute_captain = 13;
        public const int jackal_major = 14;
        public const int jackal_sniper = 15;
        public const int floodcombat_elite_shielded = 16;
        public const int cortana = 17;
        public int MaxCharacter => cortana;
        public const int energy_blade = 0;
        public const int needler = 1;
        public const int plasma_pistol = 2;
        public const int plasma_rifle = 3;
        public const int brute_plasma_rifle = 4;
        public const int brute_shot = 5;
        public const int battle_rifle = 6;
        public const int shotgun = 7;
        public const int smg = 8;
        public const int magnum = 9;
        public const int rocket_launcher = 10;
        public const int sniper_rifle = 11;
        public const int beam_rifle = 12;
        public const int h_turret_mp_item = 13;
        public const int flak_cannon = 14;
        public const int covenant_carbine = 15;
        public const int head_sp = 16;
        public int MaxWeapon => head_sp;
        public int[] ValidCharacters => new[] { floodcombat_elite, brute, jackal, flood_combat_human, flood_juggernaut, flood_carrier, grunt, grunt_major, grunt_ultra, bugger, brute_honor_guard, brute_major, brute_captain, jackal_major, jackal_sniper, floodcombat_elite_shielded };
        public int[] ValidWeapons => new[] { energy_blade, needler, plasma_pistol, plasma_rifle, brute_plasma_rifle, brute_shot, battle_rifle, shotgun, smg, magnum, rocket_launcher, sniper_rifle, beam_rifle, flak_cannon, covenant_carbine };
        public Dictionary<int, int[]> ValidCharacterWeapons => new Dictionary<int, int[]>
        {
            [floodcombat_elite] = ValidWeapons,
            [brute] = ValidWeapons.Without(energy_blade),
            [jackal] = ValidWeapons.Without(energy_blade, rocket_launcher),
            [flood_infection] = Array.Empty<int>(),
            [flood_combat_human] = ValidWeapons,
            [flood_juggernaut] = Array.Empty<int>(),
            [flood_carrier] = Array.Empty<int>(),
            [grunt] = ValidWeapons.Without(energy_blade, shotgun, battle_rifle, smg),
            [grunt_major] = ValidWeapons.Without(energy_blade, shotgun, battle_rifle, smg),
            [grunt_ultra] = ValidWeapons.Without(energy_blade, shotgun, battle_rifle, smg),
            [bugger] = ValidWeapons.Without(energy_blade, flak_cannon),
            [brute_honor_guard] = ValidWeapons.Without(energy_blade),
            [brute_major] = ValidWeapons.Without(energy_blade),
            [brute_captain] = ValidWeapons.Without(energy_blade),
            [jackal_major] = ValidWeapons.Without(energy_blade, rocket_launcher),
            [jackal_sniper] = ValidWeapons.Without(energy_blade, rocket_launcher),
            [floodcombat_elite_shielded] = ValidWeapons,
            [cortana] = ValidWeapons,
        };
    }



    public class UprisingData : ILevelData
    {
        public const int elite = 0;
        public const int brute = 1;
        public const int jackal = 2;
        public const int bugger = 3;
        public const int hunter = 4;
        public const int grunt = 5;
        public const int elite_stealth = 6;
        public const int elite_stealth_major = 7;
        public const int elite_zealot = 8;
        public const int elite_specops = 9;
        public const int brute_captain = 10;
        public const int jackal_sniper = 11;
        public int MaxCharacter => jackal_sniper;
        public const int plasma_rifle = 0;
        public const int plasma_pistol = 1;
        public const int brute_shot = 2;
        public const int covenant_carbine = 3;
        public const int energy_blade = 4;
        public const int beam_rifle = 5;
        public const int needler = 6;
        public const int rocket_launcher = 7;
        public const int flak_cannon = 8;
        public const int brute_plasma_rifle = 9;
        public const int shotgun = 10;
        public const int head_sp = 11;
        public int MaxWeapon => head_sp;
        public int[] ValidCharacters => new[] { elite, brute, jackal, bugger, hunter, grunt, elite_stealth, elite_stealth_major, elite_zealot, elite_specops, brute_captain, jackal_sniper };
        public int[] ValidWeapons => new[] { plasma_rifle, plasma_pistol, brute_shot, covenant_carbine, energy_blade, beam_rifle, needler, rocket_launcher, flak_cannon, brute_plasma_rifle, shotgun };
        public Dictionary<int, int[]> ValidCharacterWeapons => new Dictionary<int, int[]>
        {
            [elite] = ValidWeapons,
            [brute] = ValidWeapons.Without(energy_blade),
            [jackal] = ValidWeapons.Without(energy_blade, rocket_launcher),
            [bugger] = ValidWeapons.Without(energy_blade, flak_cannon),
            [hunter] = Array.Empty<int>(),
            [grunt] = ValidWeapons.Without(energy_blade, shotgun),
            [elite_stealth] = ValidWeapons,
            [elite_stealth_major] = ValidWeapons,
            [elite_zealot] = ValidWeapons,
            [elite_specops] = ValidWeapons,
            [brute_captain] = ValidWeapons.Without(energy_blade),
            [jackal_sniper] = ValidWeapons.Without(energy_blade, rocket_launcher),
        };
    }



    public class TheGreatJourneyData : ILevelData
    {
        public const int elite = 0;
        public const int jackal = 1;
        public const int brute = 2;
        public const int hunter = 3;
        public const int elite_specops = 4;
        public const int elite_zealot = 5;
        public const int marine = 6;
        public const int marine_johnson = 7;
        public const int elite_specops_commander = 8;
        public const int brute_tartarus = 9;
        public const int brute_captain = 10;
        public const int brute_honor_guard = 11;
        public const int miranda = 12;
        public const int monitor = 13;
        public const int elite_councilor = 14;
        public const int marine_sgt = 15;
        public const int marine_johnson_boss = 16;
        public const int jackal_sniper = 17;
        public const int brute_major = 18;
        public const int bugger = 19;
        public int MaxCharacter => bugger;
        public const int plasma_rifle = 0;
        public const int plasma_pistol = 1;
        public const int covenant_carbine = 2;
        public const int brute_shot = 3;
        public const int hunter_particle_cannon = 4;
        public const int needler = 5;
        public const int energy_blade = 6;
        public const int brute_plasma_rifle = 7;
        public const int shotgun = 8;
        public const int beam_rifle = 9;
        public const int gravity_hammer = 10;
        public const int head_sp = 11;
        public const int beam_rifle_noplayer = 12;
        public int MaxWeapon => beam_rifle_noplayer;
        public int[] ValidCharacters => new[] { elite, jackal, brute, hunter, elite_specops, elite_zealot, marine, elite_specops_commander, brute_captain, brute_honor_guard, elite_councilor, marine_sgt, jackal_sniper, brute_major, bugger };
        public int[] ValidWeapons => new[] { plasma_rifle, plasma_pistol, covenant_carbine, brute_shot, needler, energy_blade, brute_plasma_rifle, shotgun, beam_rifle };
        public Dictionary<int, int[]> ValidCharacterWeapons => new Dictionary<int, int[]>
        {
            [elite] = ValidWeapons,
            [jackal] = ValidWeapons.Without(energy_blade),
            [brute] = ValidWeapons.Without(energy_blade),
            [hunter] = Array.Empty<int>(),
            [elite_specops] = ValidWeapons,
            [elite_zealot] = ValidWeapons,
            [marine] = ValidWeapons.Without(energy_blade),
            [marine_johnson] = ValidWeapons.Without(energy_blade),
            [elite_specops_commander] = ValidWeapons,
            [brute_tartarus] = new[] { gravity_hammer },
            [brute_captain] = ValidWeapons.Without(energy_blade),
            [brute_honor_guard] = ValidWeapons.Without(energy_blade),
            [miranda] = ValidWeapons.Without(energy_blade),
            [monitor] = ValidWeapons.Without(energy_blade),
            [elite_councilor] = ValidWeapons,
            [marine_sgt] = ValidWeapons.Without(energy_blade),
            [marine_johnson_boss] = new[] { beam_rifle },
            [jackal_sniper] = ValidWeapons.Without(energy_blade),
            [brute_major] = ValidWeapons.Without(energy_blade),
            [bugger] = ValidWeapons.Without(energy_blade),
        };

        public int[] BannedSquadIndexes => new int[] { 97, 102 }; // tartar sauce
    }

    public static class Helpers
    {
        public static int[] Without(this int[] array, params int[] exclude)
        {
            return array.Where(i => !exclude.Contains(i)).ToArray();
        }
    }
}
