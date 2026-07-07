using DataConcentrator;
using DataConcentrator.Model;
using MaterialDesignThemes.Wpf;
using ScadaGUI.Services;
using ScadaGUI.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace ScadaGUI
{
    public partial class MainWindow : Window
    {
        private User _currentUser;
        private DispatcherTimer _inactivityTimer;
        private DispatcherTimer _clockTimer;

        // Tagovi za koje je već prikazan alarm popup (da ne iskače više puta dok se ne potvrdi)
        private readonly HashSet<string> _shownAlarmPopups = new HashSet<string>();

        public MainWindow(User currentUser)
        {
            InitializeComponent();
            _currentUser = currentUser;

            ApplyRoleBasedAccess();
            InitAlarmSoundControls();
            InitLocalizationControls();
            ApplyLanguage();
            RefreshTagsGrid();
            StartClock();

            // Pokreni skeniranje za sve ulazne tagove i poveži alarm događaj
            StartScanningAllTags();

            if (_currentUser.UserRole == Role.Admin)
            {
                SetupInactivityTimer();
            }
        }

        #region Scan wiring

        private void StartScanningAllTags()
        {
            foreach (var tag in ContextClass.Instance.Tags.ToList())
            {
                HookAndStart(tag);
            }
        }

        private void HookAndStart(Tag tag)
        {
            if (tag is AnalogInput ai)
            {
                ai.AlarmActivated -= OnAlarmActivated; // izbegni dvostruko vezivanje
                ai.AlarmActivated += OnAlarmActivated;
                ai.StartScan();
            }
            else if (tag is DigitalInput di)
            {
                di.StartScan();
            }
        }

        // Poziva se sa scan niti kada vrednost uđe u alarmnu zonu
        private void OnAlarmActivated(Alarm alarm, double currentValue)
        {
            Dispatcher.BeginInvoke((Action)(() =>
            {
                AlarmSoundManager.PlayLoop();
                Logger.Log($"Alarm aktiviran: tag '{alarm.TagName}', poruka '{alarm.Message}', vrednost {currentValue:F2}");

                // DataGrid se sam prebojava (INotifyPropertyChanged na AlarmStatus),
                // ali forsiramo osvežavanje da red odmah postane crven.
                TagsDataGrid.Items.Refresh();

                // Popup samo jednom po tagu dok se ne potvrdi
                if (!_shownAlarmPopups.Contains(alarm.TagName))
                {
                    _shownAlarmPopups.Add(alarm.TagName);
                    MessageBox.Show(
                        $"🚨 ALARM: {alarm.TagName}\n\nPoruka: {alarm.Message}\nVrednost: {currentValue:F2}\n\nPotvrdite alarm dugmetom POTVRDI (ACK).",
                        "SCADA Alarm", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }));
        }

        #endregion

        private void InitLocalizationControls()
        {
            var zones = new List<string>();
            for (int i = -12; i <= 14; i++)
                zones.Add((i >= 0 ? "+" : "") + i);
            CbTimezone.ItemsSource = zones;
            CbTimezone.SelectedItem = (LocalizationManager.TimeZoneOffsetHours >= 0 ? "+" : "")
                + (int)LocalizationManager.TimeZoneOffsetHours;

            CbDateFormat.SelectedIndex = 0;
            CbLanguage.SelectedIndex = LocalizationManager.CurrentLanguage == "en" ? 1 : 0;
        }

        private void StartClock()
        {
            _clockTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _clockTimer.Tick += (s, e) => TxtClock.Text = LocalizationManager.NowFormatted();
            _clockTimer.Start();
            TxtClock.Text = LocalizationManager.NowFormatted();
        }

        private void ApplyLanguage()
        {
            BtnAddTag.Content = LocalizationManager.Get("Btn_AddTag");
            BtnDeleteTag.Content = LocalizationManager.Get("Btn_DeleteTag");
            BtnShowAlarms.Content = LocalizationManager.Get("Btn_Alarms");
            BtnToggleTheme.Content = LocalizationManager.Get("Btn_ToggleTheme");
            BtnDetails.Content = LocalizationManager.Get("Btn_Details");
            BtnScanToggle.Content = LocalizationManager.Get("Btn_ScanToggle");
            BtnAck.Content = LocalizationManager.Get("Btn_Ack");
            BtnReport.Content = LocalizationManager.Get("Btn_Report");
            BtnWrite.Content = LocalizationManager.Get("Btn_Write");
            LblWrite.Text = LocalizationManager.Get("Lbl_Write");

            LblSound.Text = LocalizationManager.Get("Lbl_Sound");
            LblVolume.Text = LocalizationManager.Get("Lbl_Volume");
            LblLanguage.Text = LocalizationManager.Get("Lbl_Language");
            LblTimezone.Text = LocalizationManager.Get("Lbl_Timezone");
            LblDateFormat.Text = LocalizationManager.Get("Lbl_DateFormat");
            LblClock.Text = LocalizationManager.Get("Lbl_Clock");

            BtnAddTag.ToolTip = LocalizationManager.Get("Tt_AddTag");
            BtnDeleteTag.ToolTip = LocalizationManager.Get("Tt_DeleteTag");
            BtnShowAlarms.ToolTip = LocalizationManager.Get("Tt_Alarms");
            BtnToggleTheme.ToolTip = LocalizationManager.Get("Tt_ToggleTheme");
            CbAlarmSound.ToolTip = LocalizationManager.Get("Tt_Sound");
            SliderVolume.ToolTip = LocalizationManager.Get("Tt_Volume");
            CbLanguage.ToolTip = LocalizationManager.Get("Tt_Language");
            CbTimezone.ToolTip = LocalizationManager.Get("Tt_Timezone");
            CbDateFormat.ToolTip = LocalizationManager.Get("Tt_DateFormat");
            BtnDetails.ToolTip = LocalizationManager.Get("Tt_Details");
            BtnScanToggle.ToolTip = LocalizationManager.Get("Tt_ScanToggle");
            BtnAck.ToolTip = LocalizationManager.Get("Tt_Ack");
            BtnReport.ToolTip = LocalizationManager.Get("Tt_Report");
            BtnWrite.ToolTip = LocalizationManager.Get("Tt_Write");

            ColName.Header = LocalizationManager.Get("Col_Name");
            ColType.Header = LocalizationManager.Get("Col_Type");
            ColAddress.Header = LocalizationManager.Get("Col_Address");
            ColValue.Header = LocalizationManager.Get("Col_Value");
            ColStatus.Header = LocalizationManager.Get("Col_Status");
            ColDescription.Header = LocalizationManager.Get("Col_Description");
        }

        private void CbLanguage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CbLanguage.SelectedItem is ComboBoxItem item && item.Tag is string code)
            {
                LocalizationManager.SetLanguage(code);
                ApplyLanguage();
                if (TxtClock != null) TxtClock.Text = LocalizationManager.NowFormatted();
            }
        }

        private void CbTimezone_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CbTimezone.SelectedItem is string offsetText && double.TryParse(offsetText, out double offset))
            {
                LocalizationManager.TimeZoneOffsetHours = offset;
                if (TxtClock != null) TxtClock.Text = LocalizationManager.NowFormatted();
            }
        }

        private void CbDateFormat_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CbDateFormat.SelectedItem is ComboBoxItem item)
            {
                LocalizationManager.DateFormat = item.Content.ToString();
                if (TxtClock != null) TxtClock.Text = LocalizationManager.NowFormatted();
            }
        }

        private void InitAlarmSoundControls()
        {
            CbAlarmSound.ItemsSource = AlarmSoundManager.AvailableSounds.Keys.ToList();
            CbAlarmSound.SelectedItem = AlarmSoundManager.SelectedSoundName;
            SliderVolume.Value = AlarmSoundManager.Volume * 100;
        }

        private void CbAlarmSound_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CbAlarmSound.SelectedItem is string soundName)
                AlarmSoundManager.SelectedSoundName = soundName;
        }

        private void SliderVolume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            AlarmSoundManager.Volume = e.NewValue / 100.0;
        }

        private void ApplyRoleBasedAccess()
        {
            // Samo admin ima Write pristup; ostali samo čitaju (monitoring + report)
            if (_currentUser.UserRole != Role.Admin)
            {
                BtnAddTag.IsEnabled = false;
                BtnDeleteTag.IsEnabled = false;
                BtnWrite.IsEnabled = false;
                BtnScanToggle.IsEnabled = false;
                TxtWriteValue.IsEnabled = false;
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
            StopAllScans();
            Logger.Log($"Admin '{_currentUser.Username}' je izlogovan zbog neaktivnosti od 5 minuta.");
            MessageBox.Show("Izlogovani ste zbog neaktivnosti.", "Auto-logout", MessageBoxButton.OK, MessageBoxImage.Information);

            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _clockTimer?.Stop();
            AlarmSoundManager.Stop();
            StopAllScans();

            // Zaustavi simulator ako je pokrenut
            if (PLC.instance != null)
                PLC.instance.Abort();

            Logger.Log($"Korisnik '{_currentUser?.Username}' je zatvorio aplikaciju.");
        }

        private void StopAllScans()
        {
            foreach (var ai in ContextClass.Instance.Tags.OfType<AnalogInput>())
                ai.StopScan();
            foreach (var di in ContextClass.Instance.Tags.OfType<DigitalInput>())
                di.StopScan();
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
                    if (addTagWindow.CreatedTag != null)
                    {
                        lock (ContextClass.Instance)
                        {
                            ContextClass.Instance.Tags.Add(addTagWindow.CreatedTag);
                            ContextClass.Instance.SaveChanges();
                        }
                        Logger.Log($"Tag '{addTagWindow.CreatedTag.Name}' je dodan.");

                        // Odmah pokreni skeniranje za novi ulazni tag
                        HookAndStart(addTagWindow.CreatedTag);
                    }
                    else if (addTagWindow.CreatedAlarm != null)
                    {
                        lock (ContextClass.Instance)
                        {
                            ContextClass.Instance.Alarms.Add(addTagWindow.CreatedAlarm);
                            ContextClass.Instance.SaveChanges();
                        }
                        Logger.Log($"Alarm za tag '{addTagWindow.CreatedAlarm.TagName}' je dodan.");
                    }

                    RefreshTagsGrid();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Greška pri čuvanju: {ex.Message}");
                }
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(TagsDataGrid.SelectedItem is Tag selectedTag))
            {
                MessageBox.Show("Molimo izaberite tag koji želite obrisati.", "Brisanje taga", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var confirm = MessageBox.Show(
                $"Da li ste sigurni da želite obrisati tag '{selectedTag.Name}'?\nSvi alarmi vezani za ovaj tag će takođe biti obrisani.",
                "Potvrda brisanja", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (confirm != MessageBoxResult.Yes)
                return;

            try
            {
                // Prvo zaustavi nit skeniranja pre brisanja iz baze
                if (selectedTag is AnalogInput ai) ai.StopScan();
                else if (selectedTag is DigitalInput di) di.StopScan();

                lock (ContextClass.Instance)
                {
                    var vezaniAlarmi = ContextClass.Instance.Alarms.Where(a => a.TagName == selectedTag.Name).ToList();
                    foreach (var alarm in vezaniAlarmi)
                    {
                        var aktivirani = ContextClass.Instance.ActivatedAlarms.Where(aa => aa.AlarmId == alarm.Id).ToList();
                        ContextClass.Instance.ActivatedAlarms.RemoveRange(aktivirani);
                        ContextClass.Instance.Alarms.Remove(alarm);
                    }

                    ContextClass.Instance.Tags.Remove(selectedTag);
                    ContextClass.Instance.SaveChanges();
                }

                _shownAlarmPopups.Remove(selectedTag.Name);
                Logger.Log($"Tag '{selectedTag.Name}' je obrisan.");
                RefreshTagsGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška pri brisanju: {ex.Message}");
            }
        }

        // Ručni upis vrednosti u izlazni tag (AO/DO)
        private void WriteValueButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(TagsDataGrid.SelectedItem is Tag selectedTag))
            {
                MessageBox.Show("Izaberite izlazni tag (AO ili DO) iz tabele.", "Upis vrednosti", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!double.TryParse(TxtWriteValue.Text.Trim(), out double newValue))
            {
                MessageBox.Show("Unesite validan broj.", "Upis vrednosti", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (selectedTag is AnalogOutput ao)
            {
                ao.WriteValue(newValue);
            }
            else if (selectedTag is DigitalOutput doTag)
            {
                if (newValue != 0 && newValue != 1)
                {
                    MessageBox.Show("Digitalni izlaz može imati samo vrednost 0 ili 1.", "Upis vrednosti", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                doTag.WriteValue(newValue);
            }
            else
            {
                MessageBox.Show("Vrednosti se mogu upisati samo u izlazne tagove (AO/DO). Ulazi se čitaju sa senzora.", "Nije dozvoljeno", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Logger.Log($"Upisana vrednost {newValue} u tag '{selectedTag.Name}'.");
            TxtWriteValue.Clear();
            TagsDataGrid.Items.Refresh();
            MessageBox.Show("Vrednost je uspešno upisana.", "Upis vrednosti", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // Potvrda (acknowledge) alarma za izabrani AI tag
        private void AcknowledgeButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(TagsDataGrid.SelectedItem is AnalogInput ai))
            {
                MessageBox.Show("Izaberite analogni ulaz (AI) koji je u alarmu.", "Potvrda alarma", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (ai.AlarmStatus != "Active")
            {
                MessageBox.Show("Ovaj tag nema aktivnih (nepotvrđenih) alarma.", "Potvrda alarma", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            ai.AcknowledgeAlarms();
            _shownAlarmPopups.Remove(ai.Name);
            Logger.Log($"Alarm potvrđen (ACK) za tag '{ai.Name}'.");

            // Ako više nijedan tag nema aktivan alarm, ugasi zvuk
            if (!ContextClass.Instance.Tags.OfType<AnalogInput>().Any(t => t.HasActiveAlarm))
                AlarmSoundManager.Stop();

            TagsDataGrid.Items.Refresh();
        }

        // Detalji: prikaz svih alarma vezanih za izabrani AI tag
        private void DetailsButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(TagsDataGrid.SelectedItem is AnalogInput ai))
            {
                MessageBox.Show("Izaberite analogni ulaz (AI) za prikaz detalja o alarmima.", "Detalji", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            List<Alarm> alarms;
            lock (ContextClass.Instance)
            {
                alarms = ContextClass.Instance.Alarms.Where(a => a.TagName == ai.Name).ToList();
            }

            if (alarms.Count == 0)
            {
                MessageBox.Show($"Tag '{ai.Name}' nema definisanih alarma.", "Detalji", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            string details = $"Alarmi za tag '{ai.Name}':\n---------------------------------\n";
            foreach (var a in alarms)
                details += $"• {a.Message}  |  {a.Direction} {a.LimitValue}  |  {a.State}\n";

            MessageBox.Show(details, "Detalji o alarmima", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // Uključivanje/isključivanje skeniranja za izabrani ulazni tag
        private void ScanToggleButton_Click(object sender, RoutedEventArgs e)
        {
            if (TagsDataGrid.SelectedItem is AnalogInput ai)
            {
                ai.IsOnScan = !ai.IsOnScan;
                if (ai.IsOnScan) ai.StartScan(); else ai.StopScan();
                Logger.Log($"Scan za '{ai.Name}' je {(ai.IsOnScan ? "UKLJUČEN" : "ISKLJUČEN")}.");
            }
            else if (TagsDataGrid.SelectedItem is DigitalInput di)
            {
                di.IsOnScan = !di.IsOnScan;
                if (di.IsOnScan) di.StartScan(); else di.StopScan();
                Logger.Log($"Scan za '{di.Name}' je {(di.IsOnScan ? "UKLJUČEN" : "ISKLJUČEN")}.");
            }
            else
            {
                MessageBox.Show("On/Off scan je moguć samo za ulazne tagove (AI/DI).", "Scan", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        // Report: .txt sa AI vrednostima koje su bile u opsegu (High+Low)/2 ± 5
        private void ReportButton_Click(object sender, RoutedEventArgs e)
        {
            var lines = new List<string>
            {
                "=== SCADA IZVEŠTAJ ===",
                "Kriterijum: vrednost analognog ulaza u opsegu (HighLimit + LowLimit) / 2 ± 5",
                $"Generisano: {LocalizationManager.NowFormatted()}",
                "TagName\tVreme\t\tVrednost",
                "----------------------------------------------"
            };

            int count = 0;
            foreach (var ai in ContextClass.Instance.Tags.OfType<AnalogInput>())
            {
                double mid = (ai.HighLimit + ai.LowLimit) / 2.0;
                List<KeyValuePair<DateTime, double>> snapshot;
                lock (ai.History)
                {
                    snapshot = ai.History.Where(h => h.Value >= mid - 5 && h.Value <= mid + 5).ToList();
                }

                foreach (var h in snapshot)
                {
                    lines.Add($"{ai.Name}\t{h.Key:yyyy-MM-dd HH:mm:ss}\t{h.Value:F2}");
                    count++;
                }
            }

            string fileName = $"Izvestaj_AI_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
            string path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);
            File.WriteAllLines(path, lines);

            Logger.Log($"Generisan izveštaj '{fileName}' sa {count} zapisa.");
            MessageBox.Show($"Izveštaj sačuvan ({count} zapisa):\n{path}", "Report", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ShowAlarmsButton_Click(object sender, RoutedEventArgs e)
        {
            AlarmsWindow alarmsWindow = new AlarmsWindow();
            alarmsWindow.Owner = this;
            alarmsWindow.ShowDialog();
        }

        private void RefreshTagsGrid()
        {
            try
            {
                lock (ContextClass.Instance)
                {
                    TagsDataGrid.ItemsSource = ContextClass.Instance.Tags.ToList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška pri učitavanju: {ex.Message}");
            }
        }
    }
}
