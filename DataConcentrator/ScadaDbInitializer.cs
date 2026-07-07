using System;
using System.Data.Entity;
using DataConcentrator.Model;

namespace DataConcentrator
{
    // Kreira bazu iz trenutnog modela (sve tabele: Tags, Alarms, ActivatedAlarms, Users).
    // Ako se model promeni, baza se automatski rekreira. Pri kreiranju ubacuje admin nalog
    // i skup demo tagova/alarma za testiranje funkcionalnosti.
    public class ScadaDbInitializer : DropCreateDatabaseIfModelChanges<ContextClass>
    {
        protected override void Seed(ContextClass context)
        {
            // --- Admin nalog ---
            context.Users.Add(new User
            {
                Username = "Aljosa",
                Password = "Aljosa11111111!",
                UserRole = Role.Admin
            });

            // --- Analogni ulazi (mapirani na žive adrese PLC simulatora) ---
            context.Tags.Add(new AnalogInput
            {
                Name = "AI_Sinus", Address = "ADDR001", Description = "Sinusni signal (senzor pritiska)",
                LowLimit = -100, HighLimit = 100, Units = "bar", Deadband = 0, Hysteresis = 2,
                ScanTime = TimeSpan.FromSeconds(1), IsOnScan = true
            });
            context.Tags.Add(new AnalogInput
            {
                Name = "AI_Rampa", Address = "ADDR002", Description = "Rampa 0-100 (nivo rezervoara)",
                LowLimit = 0, HighLimit = 100, Units = "%", Deadband = 0, Hysteresis = 2,
                ScanTime = TimeSpan.FromSeconds(1), IsOnScan = true
            });
            context.Tags.Add(new AnalogInput
            {
                Name = "AI_Kosinus", Address = "ADDR003", Description = "Kosinusni signal (temperatura)",
                LowLimit = -50, HighLimit = 50, Units = "C", Deadband = 0, Hysteresis = 1,
                ScanTime = TimeSpan.FromSeconds(1), IsOnScan = true
            });
            context.Tags.Add(new AnalogInput
            {
                Name = "AI_Random", Address = "ADDR004", Description = "Slučajna vrednost (protok)",
                LowLimit = 0, HighLimit = 50, Units = "l/s", Deadband = 0, Hysteresis = 2,
                ScanTime = TimeSpan.FromSeconds(1), IsOnScan = true
            });

            // --- Analogni izlaz ---
            context.Tags.Add(new AnalogOutput
            {
                Name = "AO_Ventil", Address = "ADDR005", Description = "Otvorenost ventila", InitValue = 0
            });

            // --- Digitalni ulazi ---
            context.Tags.Add(new DigitalInput
            {
                Name = "DI_Motor", Address = "ADDR009", Description = "Status glavnog motora",
                ScanTime = TimeSpan.FromSeconds(1), IsOnScan = true
            });
            context.Tags.Add(new DigitalInput
            {
                Name = "DI_Senzor1", Address = "ADDR011", Description = "Granični prekidač 1",
                ScanTime = TimeSpan.FromSeconds(1), IsOnScan = true
            });
            context.Tags.Add(new DigitalInput
            {
                Name = "DI_Senzor2", Address = "ADDR012", Description = "Granični prekidač 2",
                ScanTime = TimeSpan.FromSeconds(1), IsOnScan = true
            });
            context.Tags.Add(new DigitalInput
            {
                Name = "DI_Senzor3", Address = "ADDR013", Description = "Granični prekidač 3",
                ScanTime = TimeSpan.FromSeconds(1), IsOnScan = true
            });

            // --- Digitalni izlaz ---
            context.Tags.Add(new DigitalOutput
            {
                Name = "DO_Pumpa", Address = "ADDR010", Description = "Komanda pumpe", InitValue = 0
            });

            // --- Demo alarmi (vezani za AI senzore) ---
            context.Alarms.Add(new Alarm
            {
                TagName = "AI_Rampa", LimitValue = 50, Direction = EAlarmDirection.Above,
                Message = "Nivo rezervoara previsok (>50%)", State = EAlarmState.Inactive
            });
            context.Alarms.Add(new Alarm
            {
                TagName = "AI_Random", LimitValue = 25, Direction = EAlarmDirection.Above,
                Message = "Protok iznad 25 l/s", State = EAlarmState.Inactive
            });
            context.Alarms.Add(new Alarm
            {
                TagName = "AI_Sinus", LimitValue = -50, Direction = EAlarmDirection.Below,
                Message = "Pritisak ispod -50 bar", State = EAlarmState.Inactive
            });

            base.Seed(context);
        }
    }
}
