using System.Collections.Generic;
using Reactor.Common;
using Reactor.Components;

namespace Reactor.Systems
{
    public class BaseSystem<T> where T : Component
    {
        protected static List<T> components = new List<T>();

        public static void Register(T component)
        {
            components.Add(component);
        }

        public static void Update(double time)
        {
            foreach (T component in components)
            {
                component.Update(time);
            }
        }
    }
}