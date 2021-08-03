using System;

namespace Game
{
    [Serializable]
    public class Transform
    {
        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; }
        public Vector3 Scale { get; set; } = new Vector3(1.0f);

        public override string ToString()
        {
            return string.Format("position:{{0}},rotation:{{1}},scale:{{2}}", Position, Rotation, Scale);
        }
    }
}
