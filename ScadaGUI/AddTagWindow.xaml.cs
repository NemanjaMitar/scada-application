using DataConcentrator;
using DataConcentrator.Model;
using ScadaGUI.Utils;
using System;
using System.Linq;
using System.Windows;
using MessageBox = System.Windows.Forms.MessageBox;

namespace ScadaGUI
{
    public partial class AddTagWindow : Window
    {
        public Tag CreatedTag { get; private set; }
        public Alarm CreatedAlarm { get; private set; }

        public AddTagWindow()
        {
            InitializeComponent();
            ApplyLanguage();

            // Popuni listu postojećim AI tagovima (alarm se vezuje za analogni ulaz/senzor)
            CbTagList.ItemsSource = ContextClass.Instance.Tags.OfType<AnalogInput>().ToList();
        }

        private void ApplyLanguage()
        {
            LblTitle.Text = LocalizationManager.Get("Add_Title");
            Title = LocalizationManager.Get("Add_Title");
            LblTagType.Text = LocalizationManager.Get("Add_TagType");

            RbAI.Content = LocalizationManager.Get("Add_RbAI");
            RbAO.Content = LocalizationManager.Get("Add_RbAO");
            RbDI.Content = LocalizationManager.Get("Add_RbDI");
            RbDO.Content = LocalizationManager.Get("Add_RbDO");
            RbAlarm.Content = LocalizationManager.Get("Add_RbAlarm");

            LblName.Text = LocalizationManager.Get("Add_Name");
            LblAddress.Text = LocalizationManager.Get("Add_Address");
            LblDescription.Text = LocalizationManager.Get("Add_Description");

            LblAiLow.Text = LocalizationManager.Get("Add_AiLow");
            LblAiHigh.Text = LocalizationManager.Get("Add_AiHigh");
            LblAiUnits.Text = LocalizationManager.Get("Add_AiUnits");
            LblAiDeadband.Text = LocalizationManager.Get("Add_AiDeadband");
            LblAiHysteresis.Text = LocalizationManager.Get("Add_AiHysteresis");
            LblAiScan.Text = LocalizationManager.Get("Add_AiScan");
            ChkAiOnScan.Content = LocalizationManager.Get("Add_OnScan");
            ChkDiOnScan.Content = LocalizationManager.Get("Add_OnScan");
            LblAoInit.Text = LocalizationManager.Get("Add_AoInit");
            LblDiScan.Text = LocalizationManager.Get("Add_DiScan");
            LblDoInit.Text = LocalizationManager.Get("Add_DoInit");

            LblAlarmSensor.Text = LocalizationManager.Get("Add_AlarmSensor");
            LblAlarmLimit.Text = LocalizationManager.Get("Add_AlarmLimit");
            LblAlarmDirection.Text = LocalizationManager.Get("Add_AlarmDirection");
            LblAlarmMessage.Text = LocalizationManager.Get("Add_AlarmMessage");

            BtnSave.Content = LocalizationManager.Get("Add_Save");
            BtnCancel.Content = LocalizationManager.Get("Add_Cancel");
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (PanelAI == null || PanelAO == null || PanelDI == null || PanelDO == null || PanelAlarm == null || PanelCommon == null)
                return;

            PanelAI.Visibility = Visibility.Collapsed;
            PanelAO.Visibility = Visibility.Collapsed;
            PanelDI.Visibility = Visibility.Collapsed;
            PanelDO.Visibility = Visibility.Collapsed;
            PanelAlarm.Visibility = Visibility.Collapsed;

            // Zajednička polja (Ime/Adresa/Opis) su svojstva TAGA - za alarm ih sakrivamo,
            // jer se alarm samo vezuje za postojeći senzor (AI tag).
            PanelCommon.Visibility = RbAlarm.IsChecked == true ? Visibility.Collapsed : Visibility.Visible;

            if (RbAI.IsChecked == true) PanelAI.Visibility = Visibility.Visible;
            else if (RbAO.IsChecked == true) PanelAO.Visibility = Visibility.Visible;
            else if (RbDI.IsChecked == true) PanelDI.Visibility = Visibility.Visible;
            else if (RbDO.IsChecked == true) PanelDO.Visibility = Visibility.Visible;
            else if (RbAlarm.IsChecked == true) PanelAlarm.Visibility = Visibility.Visible;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Alarm se obrađuje posebno - ne zahteva Ime/Adresu (to su svojstva taga)
            if (RbAlarm.IsChecked == true)
            {
                SaveAlarm();
                return;
            }

            if (string.IsNullOrWhiteSpace(TxtName.Text) || string.IsNullOrWhiteSpace(TxtAddress.Text))
            {
                MessageBox.Show("Ime i Adresa su obavezna polja!");
                return;
            }

            if (RbAI.IsChecked == true)
            {
                if (!double.TryParse(TxtAiLowLimit.Text, out double lowLimit) ||
                    !double.TryParse(TxtAiHighLimit.Text, out double highLimit))
                {
                    MessageBox.Show("Donja i gornja granica moraju biti brojevi!");
                    return;
                }
                if (highLimit <= lowLimit)
                {
                    MessageBox.Show("Gornja granica mora biti veća od donje!");
                    return;
                }
                if (!double.TryParse(TxtAiScanTime.Text, out double aiScan) || aiScan <= 0)
                {
                    MessageBox.Show("Period osvežavanja mora biti pozitivan broj!");
                    return;
                }

                double.TryParse(TxtAiDeadband.Text, out double deadband);
                double.TryParse(TxtAiHysteresis.Text, out double hysteresis);

                CreatedTag = new AnalogInput
                {
                    Name = TxtName.Text,
                    Address = TxtAddress.Text,
                    Description = TxtDescription.Text,
                    LowLimit = lowLimit,
                    HighLimit = highLimit,
                    Units = TxtAiUnits.Text,
                    Deadband = deadband,
                    Hysteresis = hysteresis,
                    ScanTime = TimeSpan.FromSeconds(aiScan),
                    IsOnScan = ChkAiOnScan.IsChecked == true
                };
            }
            else if (RbAO.IsChecked == true)
            {
                CreatedTag = new AnalogOutput
                {
                    Name = TxtName.Text,
                    Address = TxtAddress.Text,
                    Description = TxtDescription.Text,
                    InitValue = double.TryParse(TxtAoInitValue.Text, out double val) ? val : 0
                };
            }
            else if (RbDI.IsChecked == true)
            {
                if (!double.TryParse(TxtDiScanTime.Text, out double diScan) || diScan <= 0)
                {
                    MessageBox.Show("Period skeniranja mora biti pozitivan broj!");
                    return;
                }

                CreatedTag = new DigitalInput
                {
                    Name = TxtName.Text,
                    Address = TxtAddress.Text,
                    Description = TxtDescription.Text,
                    ScanTime = TimeSpan.FromSeconds(diScan),
                    IsOnScan = ChkDiOnScan.IsChecked == true
                };
            }
            else if (RbDO.IsChecked == true)
            {
                CreatedTag = new DigitalOutput
                {
                    Name = TxtName.Text,
                    Address = TxtAddress.Text,
                    Description = TxtDescription.Text,
                    InitValue = double.TryParse(TxtDoInitValue.Text, out double val) ? val : 0
                };
            }

            this.DialogResult = true;
            this.Close();
        }

        private void SaveAlarm()
        {
            // Alarm mora biti vezan za postojeći AI senzor
            if (CbTagList.SelectedValue == null)
            {
                MessageBox.Show("Alarm se mora vezati za postojeći AI tag (senzor). Prvo dodajte AI tag.");
                return;
            }

            if (!double.TryParse(TxtAlarmLimit.Text, out double limit))
            {
                MessageBox.Show("Granica alarma mora biti broj!");
                return;
            }

            if (string.IsNullOrWhiteSpace(TxtAlarmMessage.Text))
            {
                MessageBox.Show("Poruka alarma je obavezna!");
                return;
            }

            CreatedAlarm = new Alarm
            {
                LimitValue = limit,
                Direction = CbDirection.SelectedIndex == 0 ? EAlarmDirection.Above : EAlarmDirection.Below,
                Message = TxtAlarmMessage.Text,
                State = EAlarmState.Inactive,
                TagName = CbTagList.SelectedValue.ToString()
            };

            this.DialogResult = true;
            this.Close();
        }
    }
}
