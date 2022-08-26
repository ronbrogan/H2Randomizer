using Superintendent.CommandSink;
using Superintendent.Core.Native;
using Superintendent.Core.Remote;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace H2Randomizer
{
    public class Randomizer
    {
        private readonly Offsets offsets;
        private readonly RpcRemoteProcess process;
        private readonly MainContext context;
        private readonly ICommandSink h2;
        private readonly MemoryBlock mem;
        private readonly Action<string> AppendLog;

        private bool hookedChars = false;
        private bool hookedWeaps = false;

        public RandomizerAllocation Alloc;
        public ILevelData Level { get; private set; }
        
        private LevelDataAllocation levelData;


        public Randomizer(Offsets offsets, RpcRemoteProcess process, MainContext context, ICommandSink h2, MemoryBlock mem, Action<string> logger)
        {
            this.offsets = offsets;
            this.process = process;
            this.context = context;
            this.h2 = h2;
            this.mem = mem;
            this.AppendLog = logger;
        }

        public bool TryHook()
        {
            this.AppendLog($"Hooking on {context.Level}");

            if (!LevelData.TryGet(context.Level, out var levelData))
            {
                this.AppendLog("Hook failed");
                context.Level = "Unknown level";
                return false;
            }

            this.SetupData(levelData);

            this.WriteCharIndexRng();
            this.WriteCharIndexCall();
            this.AppendLog($"Randomizing AI placement on {context.Level}");
            this.hookedChars = true;

            if (context.ShouldRandomizeWeapons)
            {
                this.WriteWeapIndexRng();
                this.WriteWeapIndexCall();
                this.AppendLog($"Randomizing AI weapons on {context.Level}");
                this.hookedWeaps = true;
            }

            return true;
        }

        private void SetupData(ILevelData levelData)
        {
            this.Level = levelData;

            int seed = 0;

            if (!string.IsNullOrWhiteSpace(context.SeedText))
            {
                if (!int.TryParse(context.SeedText, out seed))
                {
                    var hash = MD5.HashData(Encoding.UTF8.GetBytes(context.SeedText));
                    seed = MemoryMarshal.Read<int>(hash);

                    this.AppendLog($"Using seed {context.SeedText} as {seed:x}");
                }
            }

            if (seed == 0)
            {
                seed = Random.Shared.Next(int.MaxValue);
                this.AppendLog($"No seed, using {seed:x}");
            }

            mem.Allocate(4, out Alloc.SeedAddress, alignment: 1);
            this.h2.WriteAt(Alloc.SeedAddress, seed);

            // squadIndex and squadSpawnIndex is to store the spawn info during char roll for use in weap roll
            mem.Allocate(4, out Alloc.SquadIndex, alignment: 1);
            mem.Allocate(4, out Alloc.SquadSpawnIndex, alignment: 1);

            mem.Allocate(4100, out Alloc.LogIndex, alignment: 64);
            Alloc.Logs = Alloc.LogIndex + 4;

            this.levelData = LevelData.Write(levelData, mem, this.h2);
        }

        private void WriteCharIndexRng()
        {
            var asm = RngCodeGen.GenerateCharRng(Alloc, this.levelData);

            if (Alloc.charRng == default)
            {
                mem.Allocate(asm.Length, out Alloc.charRng);
            }

            this.h2.WriteAt(Alloc.charRng, asm);
        }

        private void WriteCharIndexCall()
        {
            var rip = this.h2.GetBaseOffset() + offsets.CharIndexPatch;
            var bytes = RngCodeGen.GenerateCharHook(Alloc.charRng, rip);
            Debug.Assert(bytes.Length == 25);

            this.process.SetProtection(rip, bytes.Length, MemoryProtection.ExecuteReadWrite);
            this.h2.Write(offsets.CharIndexPatch, bytes.AsSpan());
        }

        private void WriteWeapIndexRng()
        {
            var bytes = RngCodeGen.GenerateWeapRng(this.Alloc, this.levelData);

            if (Alloc.weapRng == default)
            {
                mem.Allocate(bytes.Length, out Alloc.weapRng);
            }

            this.h2.WriteAt(Alloc.weapRng, bytes);
        }

        private void WriteWeapIndexCall()
        {
            var rip = this.h2.GetBaseOffset() + offsets.WeaponIndexPatch;

            var bytes = RngCodeGen.GenerateWeapHook(Alloc.weapRng, rip);

            Debug.Assert(bytes.Length == 18);

            this.process.SetProtection(rip, bytes.Length, MemoryProtection.ExecuteReadWrite);
            this.h2.Write(offsets.WeaponIndexPatch, bytes.AsSpan());
        }
    }

    public struct RandomizerAllocation
    {
        public nint SeedAddress;
        public nint SquadIndex;
        public nint SquadSpawnIndex;
        public nint charRng;
        public nint weapRng;

        public nint Logs;
        public nint LogIndex;
    }
}
