using Reactor.Components;
using Reactor.Entities;
using Reactor.Math3D;

namespace Reactor.Types
{
    public class Camera : Entity
    {
        public float Near { get; set; }
        public float Far { get; set; }
        public float Zoom { get; set; }
        
        public Matrix Projection { get; set; }
        public Matrix View { get; set; }

        public Camera()
        {
            var fbs = Application.Instance.Window.ClientSize;
            var aspectRatio = fbs.Width / (float)fbs.Height;
            Projection = Matrix.CreatePerspectiveFieldOfView(80.0f, aspectRatio, Near, Far);

            var transform = new Transform();
            AddComponent(transform);
            View = Matrix.CreateLookAt(transform.Position, -Vector3.UnitZ, Vector3.UnitY);
        }
    }
}