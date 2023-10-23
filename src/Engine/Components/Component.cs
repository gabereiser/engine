using Reactor.Entities;

namespace Reactor.Components
{
    public abstract class Component : IComponent
    {
        public Entity entity;
        
        public virtual void Init()
        {
        }

        public virtual void Activate()
        {
        }

        public virtual void Deactivate()
        {
        }

        public virtual void Update(double time)
        {
        }
    }
}