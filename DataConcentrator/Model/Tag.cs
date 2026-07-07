using DataConcentrator.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataConcentrator
{
    // napraviti AnalogInput, AnalogOuput, DigitalInput i
    // DigitalOutput klase koje nasledjuju Tag klasu
    public class Tag : INotifyPropertyChanged
    {
        private string name;

        private string description;

        private string address;

        private string alarmStatus = "Normal";

        // Null je za tagove koje cemo tek da inicijalizujemo, taman malo Mitrovic da vezba explicitnu konverziju :skull:
        public virtual ETagType Type { get; } = ETagType.NULL;

        // Runtime status alarma za bojenje reda u tabeli (Normal / Active / Acknowledged).
        // Ne perzistira se u bazi.
        [NotMapped]
        public string AlarmStatus
        {
            get { return alarmStatus; }
            set { alarmStatus = value; OnPropertyChanged("AlarmStatus"); }
        }


        #region Properties

        [Key]
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged("Name");
            }
        }

        public string Description
        {
            get { return description; }
            set
            {
                description = value;
                OnPropertyChanged("Description");
            }
        }
        
        public string Address
        {
            get { return address; }
            set
            {
                address = value;
                OnPropertyChanged("Address");
            }
        }
        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        #endregion
    }
}
