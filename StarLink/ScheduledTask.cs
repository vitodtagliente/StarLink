using System;
using System.Reflection;

namespace StarLink
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ScheduledTaskSettings : Attribute
    {
        public int Tick = 60000; // 60s = 60000ms
    }

    [ScheduledTaskSettings]
    public abstract class ScheduledTask
    {
        public int Tick { get { return Settings.Tick; } }

        public ScheduledTaskSettings Settings
        {
            get
            {
                return GetType().GetCustomAttribute<ScheduledTaskSettings>();
            }
        }

        public abstract void Execute();
    }
}
