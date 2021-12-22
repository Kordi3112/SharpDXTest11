using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpDX_Basic11.Source.Drawing
{
    class RenderTexture : IDisposable
    {
        // Properties
        public Texture2D RenderTargetTexture { get; set; }
        public RenderTargetView RenderTargetView { get; set; }
        public ShaderResourceView ShaderResourceView { get; private set; }


        // Puvlix Methods
        public bool Initialize(SharpDX.Direct3D11.Device device, Point size)
        {
            try
            {
                // Initialize and set up the render target description.
                Texture2DDescription textureDesc = new Texture2DDescription()
                {
                    Width = size.X,
                    Height = size.Y,
                    MipLevels = 1,
                    ArraySize = 1,
                    Format = Format.R32G32B32A32_Float,
                    SampleDescription = new SampleDescription(1, 0),
                    Usage = ResourceUsage.Default,
                    BindFlags = BindFlags.RenderTarget | BindFlags.ShaderResource,
                    CpuAccessFlags = CpuAccessFlags.None,
                    OptionFlags = ResourceOptionFlags.None
                };

                // Create the render target texture.
                RenderTargetTexture = new Texture2D(device, textureDesc);
                
                // Initialize and setup the render target view 
                RenderTargetViewDescription renderTargetViewDesc = new RenderTargetViewDescription()
                {
                    Format = textureDesc.Format,
                    Dimension = RenderTargetViewDimension.Texture2D,
                };
                renderTargetViewDesc.Texture2D.MipSlice = 0;

                // Create the render target view.
                RenderTargetView = new RenderTargetView(device, RenderTargetTexture, renderTargetViewDesc);

                // Initialize and setup the shader resource view 
                ShaderResourceViewDescription shaderResourceViewDesc = new ShaderResourceViewDescription()
                {
                    Format = textureDesc.Format,
                    Dimension = ShaderResourceViewDimension.Texture2D,
                };
                shaderResourceViewDesc.Texture2D.MipLevels = 1;
                shaderResourceViewDesc.Texture2D.MostDetailedMip = 0;

                // Create the render target view.
                ShaderResourceView = new ShaderResourceView(device, RenderTargetTexture, shaderResourceViewDesc);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void SetRenderTarget(DeviceContext context, DepthStencilView depthStencilView)
        {
            // Bind the render target view and depth stencil buffer to the output pipeline.
            context.OutputMerger.SetTargets(depthStencilView, RenderTargetView);
        }
        public void ClearRenderTarget(DeviceContext context, DepthStencilView depthStencilView, float red, float green, float blue, float alpha)
        {
            // Setup the color the buffer to.
            var color = new Color4(red, green, blue, alpha);

            // Clear the back buffer.
            context.ClearRenderTargetView(RenderTargetView, color);

            // Clear the depth buffer.
            context.ClearDepthStencilView(depthStencilView, DepthStencilClearFlags.Depth, 1.0f, 0);
        }

        public void Dispose()
        {
            ShaderResourceView?.Dispose();
            ShaderResourceView = null;
            RenderTargetView?.Dispose();
            RenderTargetView = null;
            RenderTargetTexture?.Dispose();
            RenderTargetTexture = null;
        }
    }
}
