using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace H2Randomizer
{
    public class Offsets
    {
        public nint CharIndexPatch { get; init; }
        public nint WeaponIndexPatch { get; init; }
        public nint LevelName { get; init; }

        public static Offsets v1_2448()
        {
            return new Offsets
            {
                CharIndexPatch = 0x65842B,
                WeaponIndexPatch = 0x639fdc,
                LevelName = 0xE63F80
            };
        }

        public static Offsets v1_2904()
        {
            return new Offsets
            {
                CharIndexPatch = 0x65992b,
                WeaponIndexPatch = 0x63b4ec,
                //ScnrPointer = 0xD49528
                LevelName = 0xD4ABF8
            };
        }

        private static Dictionary<string, Func<Offsets>> lookup = new()
        {
            ["1.2448.0.0"] = v1_2448,
            ["1.2904.0.0"] = v1_2904
        };

        public static bool TryGetOffsets(ProcessModule dll, out Offsets offsets)
        {
            if(lookup.TryGetValue(dll.FileVersionInfo.FileVersion, out var factory))
            {
                offsets = factory();
                return true;
            }

            offsets = v1_2904();
            return false;
        }
    }
}
