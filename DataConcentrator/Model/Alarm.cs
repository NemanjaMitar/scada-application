using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataConcentrator.Model
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

        [ForeignKey("TagNavigation")]
        public string TagName { get; set; }

        public virtual Tag TagNavigation { get; set; }
    }
}