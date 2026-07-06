using DataConcentrator;
using DataConcentrator.Model;
using MaterialDesignThemes.Wpf;
using ScadaGUI.Services;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Threading;

namespace ScadaGUI
{
    public partial class MainWindow : Window
    {
        private User _currentUser;
        private DispatcherTimer _inactivityTimer;

        public MainWindow(User currentUser)
        {
            InitializeComponent();
            _currentUser = currentUser;

            ApplyRoleBasedAccess();

            if (_currentUser.UserRole == Role.Admin)
            {
                SetupInactivityTimer();
            }
        }

        private void ApplyRoleBasedAccess()
        {
            if (_currentUser.UserRole != Role.Admin)
            {
                BtnAddTag.IsEnabled = false;
            }
        }

        private void SetupInactivityTimer()
        {
            _inactivityTimer = new DispatcherTimer();
            _inactivityTimer.Interval = TimeSpan.FromMinutes(5);
            _inactivityTimer.Tick += InactivityTimer_Tick;
            _inactivityTimer.Start();

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

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            AddTagWindow addTagWindow = new AddTagWindow();
            addTagWindow.Owner = this;

            if (addTagWindow.ShowDialog() == true)
            {
                try
                {
                    // 1. Ako je kreiran Tag
                    if (addTagWindow.CreatedTag != null)
                    {
                        ContextClass.Instance.Tags.Add(addTagWindow.CreatedTag);
                        Logger.Log($"Tag '{addTagWindow.CreatedTag.Name}' je dodan.");
                    }
                    // 2. Ako je kreiran Alarm
                    else if (addTagWindow.CreatedAlarm != null)
                    {
                        ContextClass.Instance.Alarms.Add(addTagWindow.CreatedAlarm);
                        Logger.Log($"Alarm za tag '{addTagWindow.CreatedAlarm.TagName}' je dodan.");
                    }

                    ContextClass.Instance.SaveChanges();
                    RefreshTagsGrid();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Greška pri čuvanju: {ex.Message}");
                }
            }
        }

        private void RefreshTagsGrid()
        {
            try
            {
                var sviTagovi = ContextClass.Instance.Tags.ToList();
                System.Diagnostics.Debug.WriteLine($"Osvježeno. Broj tagova: {sviTagovi.Count}");
                // Ako kasnije dodaš DataGrid, ovde stavi: TagsDataGrid.ItemsSource = sviTagovi;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška pri učitavanju: {ex.Message}");
            }
        }
    }
}