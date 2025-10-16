using Red.Components;
using System.Collections.Generic;

namespace Red.Entities
{
    public class Entity : SceneNode
    {
        public List<Component> Components { get; internal set; } = new List<Component>();

        public Entity()
        {
            Scene.Register(this);
        }
        public void AddComponent(Component component)
        {
            Components.Add(component);
            component.entity = this;
        }

        public T GetComponent<T>() where T : Component
        {
            foreach (Component component in Components)
            {
                if (component.GetType().Equals(typeof(T)))
                {
                    return (T)component;
                }
            }
            return null;
        }
    }
}