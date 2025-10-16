using Red.Common;
using Red.Components;

namespace Red.Systems
{
    public class ComponentSystem<T> : BaseSystem<T> where T : Component, IComponent
    {

    }
}