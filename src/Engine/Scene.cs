using System.Collections.Generic;
using Red.Entities;

namespace Red
{
    public static class Scene
    {
        static HashSet<SceneNode> nodes = new HashSet<SceneNode>();

        public static SceneNode Root = new SceneNode();

        public static void Register(Entity entity)
        {
            Root.AddChild(entity);
            nodes.Add(entity);
        }

        public static void Reset()
        {
            Root.Dispose();
            Root = new SceneNode();
        }

    }
}