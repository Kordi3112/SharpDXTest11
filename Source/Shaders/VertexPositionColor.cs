using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpDX_Basic11.Source.Shaders
{
    struct VertexPositionColor
    {
        public Vector4 Position;
        public Vector4 Color;

        public VertexPositionColor(Vector4 pos, Vector4 col)
        {
            Position = pos;
            Color = col;
        }
        public VertexPositionColor(Vector2 pos, Vector4 col)
        {
            Position = new Vector4(pos.X, pos.Y, 0, 1f);
            Color = col;
        }

        public VertexPositionColor(Vector3 pos, Vector4 col)
        {
            Position = new Vector4(pos.X, pos.Y, pos.Z, 1f);
            Color = col;
        }
    }
}
