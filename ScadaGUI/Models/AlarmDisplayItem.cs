using System;

namespace ScadaGUI.Models
{
    public class AlarmDisplayItem
    {
        public int Id { get; set; }
        public int AlarmId { get; set; }
        public string TagName { get; set; }
        public string Message { get; set; }
        public DateTime Timestamp { get; set; }
        public bool IsAcknowledged { get; set; }
    }
}
