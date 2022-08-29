using Avalonia.Logging;
using H2Randomizer.Levels;
using Superintendent.CommandSink;
using Superintendent.Core.Remote;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace H2Randomizer
{
    public abstract class BaseLevelData<TChar, TWeap> : ILevelData<TChar, TWeap>
        where TChar : struct, Enum
        where TWeap : struct, Enum
    {
        public abstract TChar[] ValidCharacters { get; }
        public abstract TWeap[] ValidWeapons { get; }
        public Dictionary<TChar, TWeap[]> ValidCharacterWeapons { get; }
        public int MaxCharacter { get; }
        public int MaxWeapon { get; }

        public virtual int[] BannedSquadIndexes => Array.Empty<int>();

        public BaseLevelData()
        {
            this.ValidCharacterWeapons = LevelData.GetAllowedWeapons(this);
            this.MaxWeapon = Enum.GetValues<TWeap>().Cast<int>().Max();
            this.MaxCharacter = Enum.GetValues<TChar>().Cast<int>().Max();
        }

        public string GetCharacterName(int index)
        {
            var character = (TChar)(object)index;
            return character.ToString();
        }

        public string GetWeaponName(int index)
        {
            var weap = (TWeap)(object)index;
            return weap.ToString();
        }
    }

    public interface ILevelData<TChar, TWeap> : ILevelData
        where TChar : struct, Enum
        where TWeap : struct, Enum
    {
        // strongly typed for convenience
        TChar[] ValidCharacters { get; }
        TWeap[] ValidWeapons { get; }
        public Dictionary<TChar, TWeap[]> ValidCharacterWeapons { get; }

        // dangerous cast to int for ease of consumption
        int[] ILevelData.ValidCharacterIds => Unsafe.As<int[]>(ValidCharacters);
        int[] ILevelData.ValidWeaponIds => Unsafe.As<int[]>(ValidWeapons);
        Dictionary<int, int[]> ILevelData.ValidCharacterWeaponIds => Unsafe.As<Dictionary<int, int[]>>(ValidCharacterWeapons);
    }

    public interface ILevelData
    {
        int MaxCharacter { get; }

        int[] ValidCharacterIds { get; }

        int MaxWeapon { get; }

        int[] ValidWeaponIds { get; }

        public Dictionary<int, int[]> ValidCharacterWeaponIds { get; }

        int[] BannedSquadIndexes { get; }

        string GetCharacterName(int index);
        string GetWeaponName(int index);
    }

    public class LevelData
    {
        private static Dictionary<string, Func<ILevelData>> levels = new()
        {
            ["01a_tutorial"] = () => new CairoStationData(),
            ["01b_spacestation"] = () => new CairoStationData(),
            ["03a_oldmombasa"] = () => new OutskirtsData(),
            ["03b_newmombasa"] = () => new MetropolisData(),
            ["04a_gasgiant"] = () => new TheArbiterData(),
            ["04b_floodlab"] = () => new OracleData(),
            ["05a_deltaapproach"] = () => new DeltaHaloData(),
            ["05b_deltatowers"] = () => new RegretData(),
            ["06a_sentinelwalls"] = () => new SacredIconData(),
            ["06b_floodzone"] = () => new QuarantineZoneData(),
            ["07a_highcharity"] = () => new GravemindData(),
            ["07b_forerunnership"] = () => new HighCharityData(),
            ["08a_deltacliffs"] = () => new UprisingData(),
            ["08b_deltacontrol"] = () => new TheGreatJourneyData()
        };

        public static Dictionary<string, string[]> CharacterWeapons = new()
        {
            ["brute"] = new[] { "battle_rifle", "beam_rifle", "brute_plasma_rifle", "brute_shot", "covenant_carbine", "plasma_rifle", "sent_agg_beam_elim", "sentinel_aggressor_beam", "shotgun", "smg", "sniper_rifle" },
            ["brute_captain"] = new[] { "battle_rifle", "beam_rifle", "brute_plasma_rifle", "brute_shot", "covenant_carbine", "plasma_rifle", "shotgun", "smg", "sniper_rifle" },
            ["brute_honor_guard"] = new[] { "battle_rifle", "beam_rifle", "brute_plasma_rifle", "brute_shot", "covenant_carbine", "plasma_rifle", "shotgun", "smg", "sniper_rifle" },
            ["brute_major"] = new[] { "battle_rifle", "beam_rifle", "brute_plasma_rifle", "brute_shot", "covenant_carbine", "plasma_rifle", "shotgun", "smg", "sniper_rifle" },
            ["brute_tartarus"] = new[] { "gravity_hammer" },
            ["bugger"] = new[] { "magnum", "needler", "plasma_pistol" },
            ["cortana"] = Array.Empty<string>(),
            ["crewman"] = Array.Empty<string>(),
            ["dervish"] = new[] { "covenant_carbine", "energy_blade", "needler", "plasma_pistol", "plasma_rifle", "sentinel_aggressor_beam" },
            ["elite"] = new[] { "battle_rifle", "beam_rifle", "brute_plasma_rifle", "brute_shot", "covenant_carbine", "energy_blade", "flak_cannon", "magnum", "needler", "plasma_pistol", "plasma_rifle", "rocket_launcher", "sent_agg_beam_elim", "sentinel_aggressor_beam", "shotgun", "smg", "sniper_rifle" },
            ["elite_councilor"] = new[] { "beam_rifle", "brute_plasma_rifle", "brute_shot", "covenant_carbine", "energy_blade", "needler", "plasma_pistol", "plasma_rifle", "shotgun" },
            ["elite_honor_guard"] = new[] { "battle_rifle", "beam_rifle", "brute_plasma_rifle", "brute_shot", "covenant_carbine", "energy_blade", "flak_cannon", "magnum", "needler", "plasma_pistol", "plasma_rifle", "rocket_launcher", "sent_agg_beam_elim", "sentinel_aggressor_beam", "shotgun", "smg", "sniper_rifle" },
            ["elite_major"] = new[] { "battle_rifle", "beam_rifle", "brute_plasma_rifle", "brute_shot", "covenant_carbine", "energy_blade", "flak_cannon", "magnum", "needler", "plasma_pistol", "plasma_rifle", "rocket_launcher", "shotgun", "smg", "sniper_rifle" },
            ["elite_ranger"] = new[] { "battle_rifle", "beam_rifle", "brute_plasma_rifle", "brute_shot", "covenant_carbine", "energy_blade", "flak_cannon", "magnum", "needler", "plasma_pistol", "plasma_rifle", "rocket_launcher", "shotgun", "smg", "sniper_rifle" },
            ["elite_specops"] = new[] { "battle_rifle", "beam_rifle", "brute_plasma_rifle", "brute_shot", "covenant_carbine", "energy_blade", "flak_cannon", "magnum", "needler", "plasma_pistol", "plasma_rifle", "rocket_launcher", "sent_agg_beam_elim", "sentinel_aggressor_beam", "shotgun", "smg", "sniper_rifle" },
            ["elite_specops_commander"] = new[] { "battle_rifle", "beam_rifle", "brute_plasma_rifle", "brute_shot", "covenant_carbine", "energy_blade", "magnum", "needler", "plasma_pistol", "plasma_rifle", "rocket_launcher", "sent_agg_beam_elim", "sentinel_aggressor_beam", "shotgun", "smg" },
            ["elite_stealth"] = new[] { "battle_rifle", "beam_rifle", "brute_plasma_rifle", "brute_shot", "covenant_carbine", "energy_blade", "flak_cannon", "magnum", "needler", "plasma_pistol", "plasma_rifle", "rocket_launcher", "shotgun", "smg", "sniper_rifle" },
            ["elite_stealth_major"] = new[] { "beam_rifle", "brute_plasma_rifle", "brute_shot", "covenant_carbine", "energy_blade", "flak_cannon", "needler", "plasma_pistol", "plasma_rifle", "rocket_launcher", "shotgun" },
            ["elite_ultra"] = new[] { "battle_rifle", "beam_rifle", "brute_plasma_rifle", "brute_shot", "covenant_carbine", "energy_blade", "flak_cannon", "magnum", "needler", "plasma_pistol", "plasma_rifle", "rocket_launcher", "shotgun", "smg", "sniper_rifle" },
            ["elite_zealot"] = new[] { "battle_rifle", "beam_rifle", "brute_plasma_rifle", "brute_shot", "covenant_carbine", "energy_blade", "flak_cannon", "magnum", "needler", "plasma_pistol", "plasma_rifle", "rocket_launcher", "shotgun", "smg", "sniper_rifle" },
            ["flood_carrier"] = Array.Empty<string>(),
            ["flood_combat_human"] = new[] { "battle_rifle", "beam_rifle", "brute_plasma_rifle", "covenant_carbine", "energy_blade", "magnum", "needler", "plasma_pistol", "plasma_rifle", "shotgun", "smg", "sniper_rifle" },
            ["flood_juggernaut"] = Array.Empty<string>(),
            ["floodcombat_elite"] = new[] { "battle_rifle", "beam_rifle", "brute_plasma_rifle", "covenant_carbine", "energy_blade", "magnum", "needler", "plasma_pistol", "plasma_rifle", "shotgun", "smg", "sniper_rifle" },
            ["floodcombat_elite_shielded"] = new[] { "battle_rifle", "beam_rifle", "brute_plasma_rifle", "covenant_carbine", "energy_blade", "magnum", "needler", "plasma_pistol", "plasma_rifle", "shotgun", "smg", "sniper_rifle" },
            ["grunt"] = new[] { "flak_cannon", "magnum", "needler", "plasma_pistol", "rocket_launcher" },
            ["grunt_heavy"] = new[] { "flak_cannon", "magnum", "needler", "plasma_pistol", "rocket_launcher" },
            ["grunt_major"] = new[] { "flak_cannon", "magnum", "needler", "plasma_pistol", "rocket_launcher" },
            ["grunt_specops"] = new[] { "flak_cannon", "needler", "plasma_pistol" },
            ["grunt_ultra"] = new[] { "flak_cannon", "magnum", "needler", "plasma_pistol", "rocket_launcher" },
            ["heretic"] = new[] { "beam_rifle", "covenant_carbine", "energy_blade", "flak_cannon", "needler", "plasma_pistol", "plasma_rifle", "sentinel_aggressor_beam" },
            ["heretic_grunt"] = new[] { "flak_cannon", "needler", "plasma_pistol" },
            ["heretic_leader"] = new[] { "beam_rifle", "covenant_carbine", "energy_blade", "flak_cannon", "needler", "plasma_pistol", "plasma_rifle", "sentinel_aggressor_beam" },
            ["heretic_leader_hologram"] = new[] { "covenant_carbine", "energy_blade", "needler", "plasma_pistol", "plasma_rifle", "sentinel_aggressor_beam" },
            ["hunter"] = new[] { "hunter_particle_cannon" },
            ["jackal"] = new[] { "battle_rifle", "beam_rifle", "brute_plasma_rifle", "covenant_carbine", "magnum", "needler", "plasma_pistol", "plasma_rifle", "shotgun", "smg", "sniper_rifle" },
            ["jackal_major"] = new[] { "battle_rifle", "beam_rifle", "brute_plasma_rifle", "covenant_carbine", "magnum", "needler", "plasma_pistol", "plasma_rifle", "shotgun", "smg", "sniper_rifle" },
            ["jackal_sniper"] = new[] { "battle_rifle", "beam_rifle", "brute_plasma_rifle", "covenant_carbine", "magnum", "needler", "plasma_pistol", "plasma_rifle", "shotgun", "smg", "sniper_rifle" },
            ["marine"] = new[] { "battle_rifle", "beam_rifle", "brute_plasma_rifle", "covenant_carbine", "flak_cannon", "magnum", "needler", "plasma_pistol", "plasma_rifle", "rocket_launcher", "shotgun", "smg", "sniper_rifle" },
            ["marine_dress"] = new[] { "battle_rifle", "magnum", "needler", "plasma_pistol", "plasma_rifle", "shotgun", "smg" },
            ["marine_female"] = new[] { "battle_rifle", "beam_rifle", "brute_plasma_rifle", "covenant_carbine", "flak_cannon", "magnum", "needler", "plasma_pistol", "plasma_rifle", "rocket_launcher", "shotgun", "smg", "sniper_rifle" },
            ["marine_johnson"] = new[] { "battle_rifle", "beam_rifle", "brute_plasma_rifle", "covenant_carbine", "magnum", "needler", "plasma_pistol", "plasma_rifle", "rocket_launcher", "shotgun", "smg", "sniper_rifle" },
            ["marine_johnson_boss"] = new[] { "beam_rifle", "brute_plasma_rifle", "covenant_carbine", "needler", "plasma_pistol", "plasma_rifle", "shotgun" },
            ["marine_johnson_dress"] = new[] { "battle_rifle", "magnum", "needler", "plasma_pistol", "plasma_rifle", "shotgun", "smg" },
            ["marine_massive"] = Array.Empty<string>(),
            ["marine_odst"] = new[] { "battle_rifle", "beam_rifle", "covenant_carbine", "magnum", "needler", "plasma_pistol", "plasma_rifle", "rocket_launcher", "shotgun", "smg", "sniper_rifle" },
            ["marine_sgt"] = new[] { "battle_rifle", "beam_rifle", "brute_plasma_rifle", "covenant_carbine", "flak_cannon", "magnum", "needler", "plasma_pistol", "plasma_rifle", "rocket_launcher", "shotgun", "smg", "sniper_rifle" },
            ["marine_wounded"] = new[] { "battle_rifle", "magnum", "needler", "plasma_pistol", "plasma_rifle", "shotgun", "smg" },
            ["miranda"] = Array.Empty<string>(),
            ["monitor"] = Array.Empty<string>(),
            ["prophet_regret"] = new[] { "battle_rifle", "beam_rifle", "brute_plasma_rifle", "covenant_carbine", "flak_cannon", "magnum", "needler", "plasma_pistol", "plasma_rifle", "rocket_launcher", "shotgun", "smg", "sniper_rifle" },
            ["sentinel_aggressor"] = new[] { "battle_rifle", "beam_rifle", "brute_plasma_rifle", "covenant_carbine", "flak_cannon", "needler", "rocket_launcher", "sent_agg_beam_elim", "sentinel_aggressor_beam", "shotgun", "sniper_rifle" },
            ["sentinel_aggressor_eliminator"] = new[] { "battle_rifle", "beam_rifle", "brute_plasma_rifle", "covenant_carbine", "flak_cannon", "needler", "rocket_launcher", "sent_agg_beam_elim", "sentinel_aggressor_beam", "shotgun", "sniper_rifle" },
            ["sentinel_aggressor_halo1"] = new[] { "battle_rifle", "beam_rifle", "brute_plasma_rifle", "covenant_carbine", "flak_cannon", "needler", "rocket_launcher", "sent_agg_beam_elim", "sentinel_aggressor_beam", "shotgun", "sniper_rifle" },
            ["sentinel_aggressor_major"] = new[] { "battle_rifle", "beam_rifle", "brute_plasma_rifle", "covenant_carbine", "flak_cannon", "needler", "rocket_launcher", "sent_agg_beam_elim", "sentinel_aggressor_beam", "shotgun", "sniper_rifle" },
            ["sentinel_constructor"] = new[] { "sentinel_aggressor_welder" }
        };

        public static string[] HumanWeapons = new[] { "battle_rifle", "smg", "magnum", "shotgun", "sniper_rifle", "rocket_launcher" };
        public static string[] CovenantWeapons = new[] { "plasma_pistol", "needler", "plasma_rifle", "energy_blade", "hunter_particle_cannon", "beam_rifle", "covenant_carbine", "flak_cannon", "brute_shot", "plasma_cannon", "brute_plasma_rifle", "gravity_hammer" };
        public static string[] ForerunnerWeapons = new[] { "sentinel_aggressor_beam", "sentinel_aggressor_welder", "sent_agg_beam_elim" };

        public static Dictionary<string, string[]> NaturalCharacterWeapons = new()
        {
            ["brute"] = CharacterWeapons["brute"].Without(HumanWeapons),
            ["brute_captain"] = CharacterWeapons["brute_captain"].Without(HumanWeapons),
            ["brute_honor_guard"] = CharacterWeapons["brute_honor_guard"].Without(HumanWeapons),
            ["brute_major"] = CharacterWeapons["brute_major"].Without(HumanWeapons),
            ["brute_tartarus"] = CharacterWeapons["brute_tartarus"].Without(HumanWeapons),
            ["bugger"] = CharacterWeapons["bugger"].Without(HumanWeapons),
            ["cortana"] = Array.Empty<string>(),
            ["crewman"] = Array.Empty<string>(),
            ["dervish"] = CharacterWeapons["dervish"].Without(HumanWeapons),
            ["elite"] = CharacterWeapons["elite"].Without(HumanWeapons),
            ["elite_councilor"] = CharacterWeapons["elite_councilor"].Without(HumanWeapons),
            ["elite_honor_guard"] = CharacterWeapons["elite_honor_guard"].Without(HumanWeapons),
            ["elite_major"] = CharacterWeapons["elite_major"].Without(HumanWeapons),
            ["elite_ranger"] = CharacterWeapons["elite_ranger"].Without(HumanWeapons),
            ["elite_specops"] = CharacterWeapons["elite_specops"].Without(HumanWeapons),
            ["elite_specops_commander"] = CharacterWeapons["elite_specops_commander"].Without(HumanWeapons),
            ["elite_stealth"] = CharacterWeapons["elite_stealth"].Without(HumanWeapons),
            ["elite_stealth_major"] = CharacterWeapons["elite_stealth_major"].Without(HumanWeapons),
            ["elite_ultra"] = CharacterWeapons["elite_ultra"].Without(HumanWeapons),
            ["elite_zealot"] = CharacterWeapons["elite_zealot"].Without(HumanWeapons),
            ["flood_carrier"] = Array.Empty<string>(),
            ["flood_combat_human"] = CharacterWeapons["flood_combat_human"],
            ["flood_juggernaut"] = Array.Empty<string>(),
            ["floodcombat_elite"] = CharacterWeapons["floodcombat_elite"],
            ["floodcombat_elite_shielded"] = CharacterWeapons["floodcombat_elite_shielded"],
            ["grunt"] = CharacterWeapons["grunt"].Without(HumanWeapons),
            ["grunt_heavy"] = CharacterWeapons["grunt_heavy"].Without(HumanWeapons),
            ["grunt_major"] = CharacterWeapons["grunt_major"].Without(HumanWeapons),
            ["grunt_specops"] = CharacterWeapons["grunt_specops"].Without(HumanWeapons),
            ["grunt_ultra"] = CharacterWeapons["grunt_ultra"].Without(HumanWeapons),
            ["heretic"] = CharacterWeapons["heretic"].Without(HumanWeapons),
            ["heretic_grunt"] = CharacterWeapons["heretic_grunt"].Without(HumanWeapons),
            ["heretic_leader"] = CharacterWeapons["heretic_leader"].Without(HumanWeapons),
            ["heretic_leader_hologram"] = CharacterWeapons["heretic_leader_hologram"].Without(HumanWeapons),
            ["hunter"] = Array.Empty<string>(),
            ["jackal"] = CharacterWeapons["jackal"].Without(HumanWeapons),
            ["jackal_major"] = CharacterWeapons["jackal_major"].Without(HumanWeapons),
            ["jackal_sniper"] = CharacterWeapons["jackal_sniper"].Without(HumanWeapons),
            ["marine"] = CharacterWeapons["marine"].Without(CovenantWeapons).Without(ForerunnerWeapons),
            ["marine_dress"] = CharacterWeapons["marine_dress"].Without(CovenantWeapons).Without(ForerunnerWeapons),
            ["marine_female"] = Array.Empty<string>(),
            ["marine_johnson"] = CharacterWeapons["marine_johnson"].Without(CovenantWeapons).Without(ForerunnerWeapons),
            ["marine_johnson_boss"] = CharacterWeapons["marine_johnson_boss"].Without(CovenantWeapons).Without(ForerunnerWeapons),
            ["marine_johnson_dress"] = CharacterWeapons["marine_johnson_dress"].Without(CovenantWeapons).Without(ForerunnerWeapons),
            ["marine_massive"] = Array.Empty<string>(),
            ["marine_odst"] = CharacterWeapons["marine_odst"].Without(CovenantWeapons).Without(ForerunnerWeapons),
            ["marine_sgt"] = CharacterWeapons["marine_sgt"].Without(CovenantWeapons).Without(ForerunnerWeapons),
            ["marine_wounded"] = CharacterWeapons["marine_wounded"].Without(CovenantWeapons).Without(ForerunnerWeapons),
            ["miranda"] = Array.Empty<string>(),
            ["monitor"] = Array.Empty<string>(),
            ["prophet_regret"] = CharacterWeapons["prophet_regret"],
            ["sentinel_aggressor"] = CharacterWeapons["sentinel_aggressor"],
            ["sentinel_aggressor_eliminator"] = CharacterWeapons["sentinel_aggressor_eliminator"],
            ["sentinel_aggressor_halo1"] = CharacterWeapons["sentinel_aggressor_halo1"],
            ["sentinel_aggressor_major"] = CharacterWeapons["sentinel_aggressor_major"],
            ["sentinel_constructor"] = CharacterWeapons["sentinel_constructor"],
        };

        public static Dictionary<string, string[]> ActiveWeaponPool { get; private set; } = CharacterWeapons;

        public static bool TryGet(string name, [NotNullWhen(true)] out ILevelData? data)
        {
            if (levels.TryGetValue(name, out var factory))
            {
                data = factory();
                return true;
            }

            data = null;
            return false;
        }

        public static void UseNaturalWeapons()
        {
            ActiveWeaponPool = NaturalCharacterWeapons;
        }

        public static void UseFullyRandomWeapons()
        {
            ActiveWeaponPool = CharacterWeapons;
        }

        public unsafe static LevelDataAllocation Write(ILevelData data, MemoryBlock mem, ICommandSink h2)
        {
            var alloc = new LevelDataAllocation();

            // characters
            alloc.CharCount = data.ValidCharacterIds.Length;

            var allowLookup = new int[data.MaxCharacter + 1];
            for (var i = 0; i <= data.MaxCharacter; i++)
                allowLookup[i] = data.ValidCharacterIds.Contains(i) ? 1 : 0;

            mem.Allocate(sizeof(int) * allowLookup.Length, out alloc.AllowedChars, alignment: 1);
            mem.Allocate(sizeof(int) * data.ValidCharacterIds.Length, out alloc.CharIndexes, alignment: 1);

            h2.WriteAt(alloc.AllowedChars, MemoryMarshal.AsBytes<int>(allowLookup));
            h2.WriteAt(alloc.CharIndexes, MemoryMarshal.AsBytes<int>(data.ValidCharacterIds));

            // banned squad
            alloc.BannedSquadCount = data.BannedSquadIndexes.Length;
            mem.Allocate(sizeof(int) * data.BannedSquadIndexes.Length, out alloc.BannedSquads, alignment: 1);
            h2.WriteAt(alloc.BannedSquads, MemoryMarshal.AsBytes<int>(data.BannedSquadIndexes));

            // weapons
            //alloc.WeapCount = data.ValidWeapons.Length;

            var allowLookup2 = new int[data.MaxWeapon + 1];
            for (var i = 0; i <= data.MaxWeapon; i++)
                allowLookup2[i] = data.ValidWeaponIds.Contains(i) ? 1 : 0;

            mem.Allocate(sizeof(int) * allowLookup2.Length, out alloc.AllowedWeaps, alignment: 1);
            //mem.Allocate(sizeof(int) * data.ValidWeapons.Length, out alloc.WeapIndexes, alignment: 1);

            h2.WriteAt(alloc.AllowedWeaps, MemoryMarshal.AsBytes<int>(allowLookup2));
            //h2.WriteAt(alloc.WeapIndexes, MemoryMarshal.AsBytes<int>(data.ValidWeapons));


            var chars = data.ValidCharacterWeaponIds.Keys.ToArray();
            var lookupLength = chars.Max() + 1;

            var lookupTableEntrySize = sizeof(int) + sizeof(nint); // count + pointer

            mem.Allocate(lookupTableEntrySize * lookupLength, out alloc.CharWeaponsLookup, alignment: 1);

            foreach (var (key, weaps) in data.ValidCharacterWeaponIds)
            {
                var spot = alloc.CharWeaponsLookup + lookupTableEntrySize * key;
                mem.Allocate(sizeof(int) * weaps.Length, out var slotData, alignment: 1);

                h2.WriteAt(spot, weaps.Length);
                h2.WriteAt(spot + sizeof(int), slotData);
                h2.WriteAt(slotData, MemoryMarshal.AsBytes<int>(weaps));
            }

            return alloc;
        }

        public static Dictionary<TChar, TWeap[]> GetAllowedWeapons<TChar, TWeap>(ILevelData<TChar, TWeap> level)
            where TChar : struct, Enum
            where TWeap : struct, Enum
        {
            var pool = LevelData.ActiveWeaponPool;

            var availableWeapons = level.ValidWeapons.Select(w => Enum.GetName(typeof(TWeap), w)!).ToArray(); ;

            var result = new Dictionary<TChar, TWeap[]>();

            foreach (var character in level.ValidCharacters)
            {
                var charName = Enum.GetName(typeof(TChar), character);
                if (pool.TryGetValue(charName, out var allowedWeapons))
                {
                    result[character] = availableWeapons.Where(allowedWeapons.Contains).Select(Enum.Parse<TWeap>).ToArray();
                }
                else
                {
                    result[character] = Array.Empty<TWeap>();
                }
            }

            return result;
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


    public static class Helpers
    {
        public static T[] Without<T>(this T[] array, params T[] exclude)
        {
            return array.Where(i => !exclude.Contains(i)).ToArray();
        }
    }
}
