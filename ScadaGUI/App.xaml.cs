using DataConcentrator;
using System.Data.Entity;
using System.Windows;

namespace ScadaGUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            try
            {
                ContextClass.Instance.Database.Initialize(false);
                Logger.Log("Aplikacija pokrenuta. Baza inicijalizovana.");
            }
            catch (System.Exception ex)
            {
                Logger.Log($"Greška pri inicijalizaciji baze: {ex.Message}");
            }
        }
    }
}
