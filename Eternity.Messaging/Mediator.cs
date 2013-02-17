using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;

namespace Eternity.Messaging
{
    public static class Mediator
    {
        private class WeakAction
        {
            private WeakReference Reference { get; set; }
            private string Method { get; set; }
            private bool UseParams { get; set; }
            private bool IsStatic { get; set; }
            private Type ObjectType { get; set; }

            public bool IsAlive { get { return IsStatic || Reference.IsAlive; } }

            public WeakAction(object o, string method, bool useParams)
            {
                Method = method;
                Reference = new WeakReference(o);
                ObjectType = o.GetType();
                UseParams = useParams;
                IsStatic = false;
            }

            public WeakAction(Type t, string method, bool useparams)
            {
                Method = method;
                Reference = null;
                ObjectType = t;
                UseParams = useparams;
                IsStatic = true;
            }

            public void Call(params object[] parameters)
            {
                if (!IsAlive) return;
                var m = ObjectType.GetMethod(Method);
                var target = IsStatic ? null : Reference.Target;
                m.Invoke(target, UseParams ? new object[] { parameters } : new object[0]);
            }
        }
        private static readonly Dictionary<Messages, List<WeakAction>> Subscribers;

        static Mediator()
        {
            Subscribers = new Dictionary<Messages, List<WeakAction>>();
            foreach (var t in AppDomain.CurrentDomain.GetAssemblies().Where(a => a.FullName.StartsWith("Eternity")).SelectMany(a => a.GetTypes()))
            {
                foreach (var method in t.GetMethods(BindingFlags.Static))
                {
                    foreach (var attr in method.GetCustomAttributes(typeof(SubscribeAttribute), true).Select(a => (SubscribeAttribute)a))
                    {
                        if (!method.GetParameters().Any()) AddSubscription(attr.Message, t, method.Name, false);
                        else if (method.GetParameters().Count() == 1 && method.GetParameters().Count(p => Attribute.IsDefined(p, typeof(ParamArrayAttribute))) == 1) AddSubscription(attr.Message, t, method.Name, true);
                        else throw new Exception("Message subscribers must have zero parameters or only one params parameter defined.");
                    }
                }
            }
        }

        private static void AddSubscription(Messages message, Type t, string action, bool useparams)
        {
            if (!Subscribers.ContainsKey(message))
            {
                Subscribers.Add(message, new List<WeakAction>());
            }
            var lst = Subscribers[message];
            lst.Add(new WeakAction(t, action, useparams));
        }

        private static void AddSubscription(Messages message, object o, string action, bool useparams)
        {
            if (!Subscribers.ContainsKey(message))
            {
                Subscribers.Add(message, new List<WeakAction>());
            }
            var lst = Subscribers[message];
            lst.Add(new WeakAction(o, action, useparams));
        }

        public static void Subscribe(object sub)
        {
            foreach (var method in sub.GetType().GetMethods())
            {
                foreach (var attr in method.GetCustomAttributes(typeof(SubscribeAttribute), true).Select(a => (SubscribeAttribute)a))
                {
                    if (!method.GetParameters().Any()) AddSubscription(attr.Message, sub, method.Name, false);
                    else if (method.GetParameters().Count() == 1 && method.GetParameters().Count(p => Attribute.IsDefined(p, typeof(ParamArrayAttribute))) == 1) AddSubscription(attr.Message, sub, method.Name, true);
                    else throw new Exception("Message subscribers must have zero parameters or only one params parameter defined.");
                }
            }
        }

        public static void Message(Messages message, params object[] parameters)
        {
            if (!Subscribers.ContainsKey(message)) return;
            var lst = Subscribers[message];
            lst.RemoveAll(a => !a.IsAlive);
            // Spool messages in new threads
            lst.ForEach(a => Task.Factory.StartNew(() => a.Call(parameters)));
        }
    }
}
