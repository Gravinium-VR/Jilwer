using System;

namespace Gravinium.Jilwer.Event
{
    [AttributeUsage(AttributeTargets.Method)]
    public class JilwerEvent : Attribute
    {
        public string EventId;

        public JilwerEvent(string id)
        {
            EventId = id;
        }
    }
}