using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataConcentrator
{
    public enum EAlarmDirection
    {
        Above,
        Below
    }

    public enum EAlarmState
    {
        Active,
        Acknowledged,
        Inactive
    }

    public class Alarm
    {
        [Key]
        public int Id { get; set; }

        public double LimitValue { get; set; }

        public EAlarmDirection Direction { get; set; }

        public string Message { get; set; }

        public EAlarmState State { get; set; }

        // Veza ka tagu (analognom ulazu) nad kojim je alarm definisan
        [ForeignKey("Tag")]
        public string TagName { get; set; }
        public virtual Tag Tag { get; set; }
    }
}