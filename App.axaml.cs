using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using PropertyChanged;
using System.ComponentModel;
using System.Diagnostics;

namespace H2Randomizer
{
    [DoNotNotify]
    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow();
            }

            base.OnFrameworkInitializationCompleted();
        }

        public static void RestartAsAdmin()
        {
            var proc = Process.GetCurrentProcess().MainModule.FileName;

            var info = new ProcessStartInfo(proc);
            info.UseShellExecute = true;
            info.Verb = "runas";

            try
            {
                var asAdmin = Process.Start(info);
                Process.GetCurrentProcess().Kill();
            }
            catch (Win32Exception wex)
            {
                if (wex.NativeErrorCode == 0x4C7/*ERROR_CANCELLED*/)
                {
                    //MessageBox.Show("Cannot attach to the game, it's likely running as Admin and this tool is not.", "H2Randomizer Error");
                    Process.GetCurrentProcess().Kill();
                }
                else
                {
                    throw;
                }
            }
        }
    }
}
