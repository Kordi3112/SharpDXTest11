using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpDX_Basic11.Source.Shaders
{
    struct VertexPositionColorTexture
    {
        public Vector4 Position;
        public Vector4 Color;
        public Vector2 TextCoords;

        public VertexPositionColorTexture(Vector4 pos, Vector4 col, Vector2 tex)
        {
            Position = pos;
            Color = col;
            TextCoords = tex;
        }
        public VertexPositionColorTexture(Vector2 pos, Vector4 col, Vector2 tex)
        {
            Position = new Vector4(pos.X, pos.Y, 0, 1f);
            Color = col;
            TextCoords = tex;
        }
    }
}
