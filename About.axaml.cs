using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using System.Diagnostics;

namespace H2Randomizer
{
    [PropertyChanged.DoNotNotify]
    public partial class About : Window
    {
        public About()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public void GotoGithub(object sender, PointerReleasedEventArgs args)
        {
            Process.Start(new ProcessStartInfo("https://github.com/ronbrogan/H2Randomizer")
            {
                UseShellExecute = true,
            });
        }
    }
}
