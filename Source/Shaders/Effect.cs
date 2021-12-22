using SharpDX.D3DCompiler;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpDX_Basic11.Source.Shaders
{
    class Effect : IDisposable
    {
        public VertexShader VertexShader { get; set; }
        public PixelShader PixelShader { get; set; }
        public ShaderSignature ShaderSignature { get; set; }
        public InputLayout InputLayout { get; set; }

        public Effect()
        {

        }

        public void LoadFromFile(Device device, string fileName, string VSProfile, string PSProfile, string VSVersion, string PSVersion)
        {
            // Compile Vertex and Pixel shaders
            var vertexShaderByteCode = ShaderBytecode.CompileFromFile(fileName, VSProfile, VSVersion);
            VertexShader = new VertexShader(device, vertexShaderByteCode);

            var pixelShaderByteCode = ShaderBytecode.CompileFromFile(fileName, PSProfile, PSVersion);
            PixelShader = new PixelShader(device, pixelShaderByteCode);

            ShaderSignature = ShaderSignature.GetInputSignature(vertexShaderByteCode);
        }

        public void ApplyShader(DeviceContext context)
        {
            context.InputAssembler.InputLayout = InputLayout;
            context.VertexShader.Set(VertexShader);
            context.PixelShader.Set(PixelShader);
        }

        public void Dispose()
        {
            VertexShader.Dispose();
            PixelShader.Dispose();
            ShaderSignature.Dispose();
            InputLayout.Dispose();
        }
    }
}
