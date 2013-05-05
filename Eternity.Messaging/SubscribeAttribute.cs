using System;

namespace Eternity.Messaging
{
    public class SubscribeAttribute : Attribute
    {
        public string Message { get; set; }

        public SubscribeAttribute(object message)
        {
            Message = message.ToString();
        }
    }
}
