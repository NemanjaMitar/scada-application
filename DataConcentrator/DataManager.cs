using DataConcentrator.Model;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace DataConcentrator
{
    public static class DataManager
    {
        public static void SaveChanges()
        {
            ContextClass.Instance.SaveChanges();
        }

        public static List<Tag> GetAllTags()
        {
            return ContextClass.Instance.Tags.ToList();
        }

        public static List<Alarm> GetAlarmsForTag(string tagName)
        {
            return ContextClass.Instance.Alarms.Where(a => a.TagName == tagName).ToList();
        }

        public static void AddTag(Tag tag)
        {
            ContextClass.Instance.Tags.Add(tag);
            ContextClass.Instance.SaveChanges();
        }

        public static void RemoveTag(Tag tag)
        {
            var alarms = ContextClass.Instance.Alarms.Where(a => a.TagName == tag.Name).ToList();
            ContextClass.Instance.Alarms.RemoveRange(alarms);
            ContextClass.Instance.Tags.Remove(tag);
            ContextClass.Instance.SaveChanges();
        }

        public static void AddAlarm(Alarm alarm)
        {
            ContextClass.Instance.Alarms.Add(alarm);
            ContextClass.Instance.SaveChanges();
        }

        public static void RemoveAlarm(Alarm alarm)
        {
            ContextClass.Instance.Alarms.Remove(alarm);
            ContextClass.Instance.SaveChanges();
        }

        public static void SaveActivatedAlarm(ActivatedAlarm activatedAlarm)
        {
            ContextClass.Instance.ActivatedAlarms.Add(activatedAlarm);
            ContextClass.Instance.SaveChanges();
        }
    }
}
