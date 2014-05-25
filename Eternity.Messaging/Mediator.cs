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
                var m = ObjectType.GetMethod(Method, Flags);
                var target = IsStatic ? null : Reference.Target;
                m.Invoke(target, UseParams ? new object[] { parameters } : new object[0]);
            }
        }
        private static readonly Dictionary<string, List<WeakAction>> Subscribers;

        private const BindingFlags Flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

        static Mediator()
        {
            Subscribers = new Dictionary<string, List<WeakAction>>();
            foreach (var t in AppDomain.CurrentDomain.GetAssemblies().Where(a => a.FullName.StartsWith("Eternity")).SelectMany(a => a.GetTypes()))
            {
                foreach (var method in t.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
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

        private static void AddSubscription(object message, Type t, string action, bool useparams)
        {
            var str = message.ToString();
            if (!Subscribers.ContainsKey(str))
            {
                Subscribers.Add(str, new List<WeakAction>());
            }
            var lst = Subscribers[str];
            lst.Add(new WeakAction(t, action, useparams));
        }

        private static void AddSubscription(object message, object o, string action, bool useparams)
        {
            var str = message.ToString();
            if (!Subscribers.ContainsKey(str))
            {
                Subscribers.Add(str, new List<WeakAction>());
            }
            var lst = Subscribers[str];
            lst.Add(new WeakAction(o, action, useparams));
        }

        public static void Subscribe(object sub)
        {
            foreach (var method in sub.GetType().GetMethods(Flags))
            {
                foreach (var attr in method.GetCustomAttributes(typeof(SubscribeAttribute), true).Select(a => (SubscribeAttribute)a))
                {
                    if (!method.GetParameters().Any()) AddSubscription(attr.Message, sub, method.Name, false);
                    else if (method.GetParameters().Count() == 1 && method.GetParameters().Count(p => Attribute.IsDefined(p, typeof(ParamArrayAttribute))) == 1) AddSubscription(attr.Message, sub, method.Name, true);
                    else throw new Exception("Message subscribers must have zero parameters or only one params parameter defined.");
                }
            }
        }

        public static void Message(object message, params object[] parameters)
        {
            var str = message.ToString();
            if (!Subscribers.ContainsKey(str)) return;
            var lst = Subscribers[str];
            lst.RemoveAll(a => !a.IsAlive);
            // Spool messages in new threads
            //lst.ForEach(a => Task.Factory.StartNew(() => a.Call(parameters)));
            // Spooling into threads caused marshalling issues with OpenGL, keep it single threaded for now.
            lst.ForEach(a => a.Call(parameters));
        }
    }
}
