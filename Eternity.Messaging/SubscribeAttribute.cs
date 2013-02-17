using System;

namespace Eternity.Messaging
{
    public class SubscribeAttribute : Attribute
    {
        public Messages Message { get; set; }

        public SubscribeAttribute(Messages message)
        {
            Message = message;
        }
    }
}
