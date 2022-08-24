using Avalonia.Controls;
using Avalonia.Threading;
using PropertyChanged;
using Superintendent.CommandSink;
using Superintendent.Core.Native;
using Superintendent.Core.Remote;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace H2Randomizer
{
    [AddINotifyPropertyChangedInterface]
    public class MainContext
    {
        public string Process { get; set; } = "No game";

        public string Level { get; set; } = "";

        public string Logs { get; set; } = "Changes to seed or options take effect after main menu/changing levels\r\n=======================";

        public string? SeedText { get; set; } = "";

        public bool ShouldRandomizeWeapons { get; set; }
    }

    [PropertyChanged.DoNotNotify]
    public partial class MainWindow : Window
    {
        private TextBox logbox;

        public RpcRemoteProcess Process { get; private set; }

        public MainContext context;
        private Offsets offsets;
        private MemoryBlock mem;

        private nint seedAddress;
        private nint squadIndex;
        private nint squadSpawnIndex;

        private int charCount;
        private nint allowedChars;
        private nint charIndexes;
        private nint charRng;

        private int weapCount;
        private nint allowedWeaps;
        private nint weapIndexes;
        private nint weapRng;

        private int bannedSquadCount;
        private nint bannedSquads;

        private ICommandSink h2;

        // These have a separate max, because the max in our allowed may not be the same, and there
        // needs to be enough in the check array at runtime for all possible options
        private Dictionary<string, (int max, int[])> AllowedCharacters = new()
        {
            ["01a_tutorial"] = (4, new[] { 0, 1, 2, 3, 4 }),
            ["01b_spacestation"] = (19, new[] { 0, 1, 2, 4, 5, 6, 7, 9, 10, 11, 12, 14, 15, 16, 17, 18 }),
            ["03a_oldmombasa"] = (14, new[] { 0, 1, 2, 3, 4, 5, 7, 8, 9, 10, 11, 12, 13, 14 }),
            ["03b_newmombasa"] = (14, new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 10, 11, 12, 13, 14 }),
            ["04a_gasgiant"] = (5, new[] { 0, 1, 2, 3, 5 }),
            ["04b_floodlab"] = (12, new[] { 0, 1, 2, 3, 4, 6, 8, 11 }),
            ["05a_deltaapproach"] = (12, new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 }),
            ["05b_deltatowers"] = (12, new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 }),
            ["06a_sentinelwalls"] = (19, new[] { 0, 2, 3, 4, 5, 7, 9, 10, 11, 14, 15, 16, 17, 18, 19 }),
            ["06b_floodzone"] = (11, new[] { 1, 2, 3, 4, 6, 7, 8, 9, 10, 11 }),
            ["07a_highcharity"] = (27, new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 22, 23, 24, 25 }),
            ["07b_forerunnership"] = (17, new[] { 0, 1, 2, 4,/*jugg*/ 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 }),
            ["08a_deltacliffs"] = (11, new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 }),
            ["08b_deltacontrol"] = (19, new[] { 0, 1, 2, 3, 4, 5, 6, 8, 10, 11, 14, 15, 17, 18, 19 }),
        };

        // These have a separate max, because the max in our allowed may not be the same, and there
        // needs to be enough in the check array at runtime for all possible options
        private Dictionary<string, (int max, int[])> Weapons = new()
        {
            ["01a_tutorial"] = (0, new[] { 0 }),
            ["01b_spacestation"] = (10, new[] { 0, 1, 2, 3, 4, 5, 8, 9 }),
            ["03a_oldmombasa"] = (16, new[] { 0, 1, 2, 3, 4, 5, 6, 8, 9, 10, 14 }),
            ["03b_newmombasa"] = (14, new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 13 }),
            ["04a_gasgiant"] = (10, new[] { 0, 1, 2, 3, 4, 5, 6, 7 }),
            ["04b_floodlab"] = (6, new[] { 0, 1, 2, 3, 4, 5 }),
            ["05a_deltaapproach"] = (14, new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }),
            ["05b_deltatowers"] = (15, new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 15 }),
            ["06a_sentinelwalls"] = (14, new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 11, 13 }),
            ["06b_floodzone"] = (12, new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 11 }),
            ["07a_highcharity"] = (12, new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }),
            ["07b_forerunnership"] = (16, new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 14, 15 }),
            ["08a_deltacliffs"] = (11, new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }),
            ["08b_deltacontrol"] = (12, new[] { 0, 1, 2, 3, 5, 6, 7, 8, 9 })
        };

        private Dictionary<string, int[]> BannedSquadIndexes = new()
        {
            ["01a_tutorial"] = new int[] { },
            ["01b_spacestation"] = new int[] { },
            ["03a_oldmombasa"] = new int[] { },
            ["03b_newmombasa"] = new int[] { },
            ["04a_gasgiant"] = new int[] { },
            ["04b_floodlab"] = new int[] { },
            ["05a_deltaapproach"] = new int[] { },
            ["05b_deltatowers"] = new int[] { },
            ["06a_sentinelwalls"] = new int[] { 83, 84, 85, 86, 87, 88, 90, 91, 92, 93, 94, 95, 96, 97, 98, 107 }, // end fight squads
            ["06b_floodzone"] = new int[] { },
            ["07a_highcharity"] = new int[] { },
            ["07b_forerunnership"] = new int[] { },
            ["08a_deltacliffs"] = new int[] { },
            ["08b_deltacontrol"] = new int[] { 97, 102 }, // tartar sauce
        };

        public MainWindow()
        {
            InitializeComponent();
            this.context = new MainContext();
            DataContext = this.context;

            this.context.SeedText = Preferences.Current.Seed;
            this.context.ShouldRandomizeWeapons = Preferences.Current.RandomizeAiWeapons;
            this.logbox = this.Get<TextBox>(nameof(Logs));


            this.Process = new RpcRemoteProcess();

            this.Process.ProcessAttached += Attach;
            this.Process.ProcessDetached += Exit;
            this.Process.AttachException += Fail;

            try
            {
                this.Process.Attach(Guard, "MCC-Win64-Shipping");
            }
            catch (Win32Exception ex)
            {
                if (ex.NativeErrorCode == 0x5/*ACCESS_DENIED*/)
                {
                    //App.RestartAsAdmin();
                }
            }

        }

        public bool Guard(Process p)
        {
            var h2dll = p.Modules.Cast<ProcessModule>().FirstOrDefault(m => m.ModuleName == "halo2.dll");

            if (h2dll == null)
                return false;

            if(!Offsets.TryGetOffsets(h2dll, out offsets))
            {
                this.AppendLog($"Tried to attach, but version {h2dll.FileVersionInfo.FileVersion} is not supported");
                return false;
            }

            return true;
        }


        public void Attach(object? sender, ProcessAttachArgs p)
        {
            this.context.Process = $"{p.Process.Process.ProcessName} ({p.ProcessId})";

            var size = 4096 * 16;
            var alloc = this.Process.Allocate(size, MemoryProtection.ExecuteReadWrite);
            this.mem = new MemoryBlock(alloc, size);

            // TODO: need to unhook things for this not to crash the game
            //AppDomain.CurrentDomain.ProcessExit += (sender, e) => 
            //    this.Process.Free(this.mem.Address, this.mem.Size);

            this.h2 = this.Process.GetCommandSink("halo2.dll");
            _ = this.PollLevelName();
        }

        public void Exit(object? sender, EventArgs _)
        {
            this.context.Process = "";
            this.context.Level = "No game";
            this.SavePreferences();
        }

        public void Fail(object? sender, AttachExceptionArgs a)
        {
            throw a.Exception;
        }

        private string lastLevel = null;

        private async Task PollLevelName()
        {
            while (true)
            {
                if (this.Process.Process?.HasExited == true)
                {
                    await Task.Delay(500);
                    continue;
                }
                
                // When going to MCC menu, halo2.dll is deallocated, so this blows up
                try
                {
                    await this.h2.PollMemory(offsets.LevelName, 500, 32, s =>
                    {
                        var zero = s.IndexOf((byte)0x0);
                        var level = Encoding.UTF8.GetString(s.Slice(0, zero));

                        if (this.lastLevel != level)
                        {
                            this.LevelChange(level);
                        }
                    });
                }
                catch
                {
                }

                await Task.Delay(500);
            }
        }

        public void LevelChange(string level)
        {
            this.lastLevel = level;

            if (string.IsNullOrEmpty(level))
            {
                this.context.Level = "Unknown level";
                return;
            }

            if (level == "mainmenu")
            {
                this.context.Level = "Loading";
            }
            else
            {
                this.context.Level = level;
                this.HookEngine();
            }
        }

        public void HookEngine()
        {
            this.AppendLog($"Hooking on {this.context.Level}");
            var (hookChars, hookWeaps) = this.SetupData();

            if(hookChars == false && hookWeaps == false)
            {
                this.context.Logs += "Hook failed";
                this.context.Level = "Unknown level";
                return;
            }

            if(hookChars)
            {
                this.WriteCharIndexRng();
                this.WriteCharIndexCall();
                this.AppendLog($"Randomizing AI placement on {this.context.Level}");
            }
            
            if(hookWeaps && this.context.ShouldRandomizeWeapons)
            {
                this.WriteWeapIndexRng();
                this.WriteWeapIndexCall();
                this.AppendLog($"Randomizing AI weapons on {this.context.Level}");
            }

            this.SavePreferences();
        }

        public (bool hookChars, bool hookWeaps) SetupData()
        {
            this.charCount = 0;
            this.allowedChars = 0;
            this.charIndexes = 0;
            this.bannedSquadCount = 0;
            this.bannedSquads = 0;
            this.weapCount = 0;
            this.weapIndexes = 0;
            this.allowedWeaps = 0;

            (bool hookChars, bool hookWeaps) result = (false, false);

            int seed = 0;

            if(!string.IsNullOrWhiteSpace(this.context.SeedText))
            {
                if(!int.TryParse(this.context.SeedText, out seed))
                {
                    var hash = MD5.HashData(Encoding.UTF8.GetBytes(this.context.SeedText));
                    seed = MemoryMarshal.Read<int>(hash);

                    this.AppendLog($"Using seed {this.context.SeedText} as {seed:x}");
                }
            }

            if(seed == 0)
            {
                seed = Random.Shared.Next(int.MaxValue);
                this.AppendLog($"No seed, using {seed:x}");
            }

            mem.Allocate(4, out this.seedAddress, alignment: 1);
            this.h2.WriteAt(this.seedAddress, seed);

            // squadIndex and squadSpawnIndex is to store the spawn info during char roll for use in weap roll
            mem.Allocate(4, out this.squadIndex, alignment: 1);
            mem.Allocate(4, out this.squadSpawnIndex, alignment: 1);

            if (this.AllowedCharacters.TryGetValue(this.context.Level, out var chars))
            {
                var (maxChar, charIndexes) = chars;

                this.charCount = charIndexes.Length;

                var allowLookup = new int[maxChar + 1];
                for (int i = 0; i <= maxChar; i++)
                    allowLookup[i] = charIndexes.Contains(i) ? 1 : 0;

                mem.Allocate(sizeof(int) * allowLookup.Length, out this.allowedChars, alignment: 1);
                mem.Allocate(sizeof(int) * charIndexes.Length, out this.charIndexes, alignment: 1);

                this.h2.WriteAt(this.allowedChars, MemoryMarshal.AsBytes<int>(allowLookup));
                this.h2.WriteAt(this.charIndexes, MemoryMarshal.AsBytes<int>(charIndexes));
                result = (true, false);
            }

            if(this.BannedSquadIndexes.TryGetValue(this.context.Level, out var bannedSquads))
            {
                this.bannedSquadCount = bannedSquads.Length;
                mem.Allocate(sizeof(int) * bannedSquads.Length, out this.bannedSquads, alignment: 1);
                this.h2.WriteAt(this.bannedSquads, MemoryMarshal.AsBytes<int>(bannedSquads));
            }

            if(this.Weapons.TryGetValue(this.context.Level, out var weaps))
            {
                var (maxWeapon, weapons) = weaps;

                this.weapCount = weapons.Length;

                var allowLookup = new int[maxWeapon + 1];
                for (int i = 0; i <= maxWeapon; i++)
                    allowLookup[i] = weapons.Contains(i) ? 1 : 0;

                mem.Allocate(sizeof(int) * allowLookup.Length, out this.allowedWeaps, alignment: 1);
                mem.Allocate(sizeof(int) * weapons.Length, out this.weapIndexes, alignment: 1);

                this.h2.WriteAt(this.allowedWeaps, MemoryMarshal.AsBytes<int>(allowLookup));
                this.h2.WriteAt(this.weapIndexes, MemoryMarshal.AsBytes<int>(weapons));
                result = (result.hookChars, true);
            }

            return result;
        } 

        public void WriteCharIndexRng()
        {
            var asm = RngCodeGen.GenerateCharRng(this.seedAddress,
                this.squadIndex,
                this.squadSpawnIndex,
                this.charCount, 
                this.allowedChars, 
                this.charIndexes,
                this.bannedSquadCount,
                this.bannedSquads);

            if (this.charRng == default)
            {
                mem.Allocate(asm.Length, out this.charRng);
            }

            this.h2.WriteAt(this.charRng, asm);
        }

        public void WriteCharIndexCall()
        {
            var rip = this.h2.GetBaseOffset() + offsets.CharIndexPatch;
            var bytes = RngCodeGen.GenerateCharHook(this.charRng, rip);
            Debug.Assert(bytes.Length == 25);

            this.Process.SetProtection(rip, bytes.Length, MemoryProtection.ExecuteReadWrite);
            this.h2.Write(offsets.CharIndexPatch, bytes.AsSpan());
        }

        public void WriteWeapIndexRng()
        {
            var bytes = RngCodeGen.GenerateWeapRng(this.seedAddress,
                this.squadIndex,
                this.squadSpawnIndex, 
                this.weapCount, 
                this.allowedWeaps, 
                this.weapIndexes);

            if(this.weapRng == default)
            {
                mem.Allocate(bytes.Length, out this.weapRng);
            }

            this.h2.WriteAt(this.weapRng, bytes);
        }

        public void WriteWeapIndexCall()
        {
            var rip = this.h2.GetBaseOffset() + offsets.WeaponIndexPatch;

            var bytes = RngCodeGen.GenerateWeapHook(this.weapRng, rip);

            Debug.Assert(bytes.Length == 18);

            this.Process.SetProtection(rip, bytes.Length, MemoryProtection.ExecuteReadWrite);
            this.h2.Write(offsets.WeaponIndexPatch, bytes.AsSpan());
        }

        private void AppendLog(string log)
        {
            this.context.Logs += $"\r\n{log}";

            Dispatcher.UIThread.InvokeAsync(() =>
            {
                this.logbox.CaretIndex = this.context.Logs.Length;
            });
        }

        private void SavePreferences()
        {
            Preferences.Current.Seed = this.context.SeedText;
            Preferences.Current.RandomizeAiWeapons = this.context.ShouldRandomizeWeapons;
            Preferences.Persist();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            this.SavePreferences();
            base.OnClosing(e);
        }
    }
}
