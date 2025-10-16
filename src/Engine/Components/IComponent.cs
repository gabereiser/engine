using System;

namespace Red.Components
{
    public interface IComponent
    {
        void Init();
        void Activate();
        void Deactivate();

        void Update(double time);

    }
}