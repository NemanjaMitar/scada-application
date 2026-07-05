using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataConcentrator
{
    public class ActivatedAlarm
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Alarm")]
        public int AlarmId { get; set; }
        public virtual Alarm Alarm { get; set; }

        public string TagName { get; set; }

        public string Message { get; set; }

        public DateTime Timestamp { get; set; }
    }
}