using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using DataConcentrator.Model;
using MaterialDesignThemes.Wpf;
using ScadaGUI.Services;

namespace ScadaGUI
{
    public partial class MainWindow : Window
    {
        private User _currentUser;
        private DispatcherTimer _inactivityTimer;

        // Izmijenjen konstruktor da prima prijavljenog korisnika
        public MainWindow(User currentUser)
        {
            InitializeComponent();
            _currentUser = currentUser;

            ApplyRoleBasedAccess();

            // Ako je admin, inicijalizuj tajmer za neaktivnost (5 minuta)
            if (_currentUser.UserRole == Role.Admin)
            {
                SetupInactivityTimer();
            }
        }

        private void ApplyRoleBasedAccess()
        {
            // Prema zahtjevu: Admin ima Read/Write access a ostalima samo Read.
            // Onemogućavamo akcije upisivanja/dodavanja za ostale uloge.
            if (_currentUser.UserRole != Role.Admin)
            {
                BtnAddTag.IsEnabled = false; // "Samo Read" pristup
                // Ovdje onemogućite i sve ostale kontrole koje vrše pisanje
            }
        }

        private void SetupInactivityTimer()
        {
            _inactivityTimer = new DispatcherTimer();
            _inactivityTimer.Interval = TimeSpan.FromMinutes(5);
            _inactivityTimer.Tick += InactivityTimer_Tick;
            _inactivityTimer.Start();

            // Prati samo namerne akcije (klik/kucanje) - MouseMove je namjerno izostavljen
            // jer se okida i pri običnom prelasku kursora preko (nefokusiranog) prozora,
            // što je tiho resetovalo tajmer i sprečavalo auto-logout da se ikad desi.
            this.KeyDown += ResetTimerOnActivity;
            this.MouseDown += ResetTimerOnActivity;
        }

        private void ResetTimerOnActivity(object sender, EventArgs e)
        {
            if (_inactivityTimer != null && _inactivityTimer.IsEnabled)
            {
                _inactivityTimer.Stop();
                _inactivityTimer.Start();
            }
        }

        private void InactivityTimer_Tick(object sender, EventArgs e)
        {
            _inactivityTimer.Stop();
            Logger.Log($"Admin '{_currentUser.Username}' je izlogovan zbog neaktivnosti od 5 minuta.");
            MessageBox.Show("Izlogovani ste zbog neaktivnosti.", "Auto-logout", MessageBoxButton.OK, MessageBoxImage.Information);

            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Logger.Log($"Korisnik '{_currentUser?.Username}' je zatvorio aplikaciju.");
        }

        private void BtnToggleTheme_Click(object sender, RoutedEventArgs e)
        {
            var paletteHelper = new PaletteHelper();
            Theme theme = paletteHelper.GetTheme();

            bool isDark = theme.GetBaseTheme() == BaseTheme.Dark;
            theme.SetBaseTheme(isDark ? BaseTheme.Light : BaseTheme.Dark);

            paletteHelper.SetTheme(theme);
        }

        // Ostale postojeće metode ostaju netaknute...
        private void AddButton_Click(object sender, RoutedEventArgs e) { /* ... */ }
    }
}