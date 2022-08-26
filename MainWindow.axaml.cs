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
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static H2Randomizer.RngCodeGen;

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

        public string Version { get; set; } = "v" + FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion;
    }

    [PropertyChanged.DoNotNotify]
    public partial class MainWindow : Window
    {
        private TextBox logbox;
        private Randomizer randomizer;

        public RpcRemoteProcess Process { get; private set; }

        public MainContext context;
        private Offsets offsets;
        private MemoryBlock mem;

        private ICommandSink h2;

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
        private LevelDataAllocation levelData;

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
                this.randomizer = new Randomizer(this.offsets, this.Process, this.context, this.h2, this.mem, this.AppendLog);

                if (this.randomizer.TryHook())
                {
                    this.PollLogs();
                }

                this.SavePreferences();
            }
        }

        private void PollLogs()
        {
            _ = this.h2.PollMemoryAt(this.randomizer.Alloc.LogIndex, 500, 4100, ProcessLogs);
        }

        private void ProcessLogs(ReadOnlySpan<byte> logBytes)
        {
            this.h2.WriteAt(this.randomizer.Alloc.LogIndex, 0);

            var count = MemoryMarshal.Read<int>(logBytes.Slice(0, 4));
            var records = MemoryMarshal.Cast<byte, LogRecord>(logBytes.Slice(4));

            for(var i = 0; i < count; i++)
            {
                var log = records[i];

                Func<ILevelData, int, string> choiceName = log.LogType == LogType.WeaponChoice 
                    ? LevelData.GetWeapName 
                    : LevelData.GetCharName;

#if DEBUG
                this.AppendLog($"{log.LogType} / {log.SquadIndex} / {log.SpawnIndex} => {choiceName(this.randomizer.Level, log.ChosenValue)}");
#endif
            }
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
