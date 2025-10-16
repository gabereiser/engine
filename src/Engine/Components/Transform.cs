using Red.Math3D;

namespace Red.Components
{
    public class Transform : Component
    {
        public Vector3 Position { get; set; } = new Vector3();
        public Vector3 Scale { get; set; } = new Vector3();
        public Quaternion Rotation { get; set; } = new Quaternion();

    }
}