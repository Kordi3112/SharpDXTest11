using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX_Basic11.Source.Drawing;
using SharpDX_Basic11.Source.Shaders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Buffer = SharpDX.Direct3D11.Buffer;

namespace SharpDX_Basic11.Source.App
{
    class App1 : App
    {
        SwapChainDescription SwapChainDesc { get; set; }
        SharpDX.Direct3D11.Device _device;
        SharpDX.Direct3D11.Device Device => _device;
        DeviceContext Context => Device.ImmediateContext;
        SwapChain _swapChain;
        SwapChain SwapChain => _swapChain;

        bool userResized = true;
        Factory Factory { get; set; }

        Effect Effect1 { get; set; }
        Effect Effect2 { get; set; }

        //RENDER
        Texture2D backBuffer = null;
        RenderTargetView renderView = null;
        Texture2D depthBuffer = null;
        DepthStencilView depthView = null;
        //

        Buffer ContantBuffer { get; set; }

        RenderTexture RenderTexture { get; set; }

        Buffer VertexBuffer { get; set; }
        Buffer VertexBuffer2 { get; set; }
        Buffer IndexBuffer { get; set; }

        public override void Initialize()
        {
            // SwapChain description
            SwapChainDesc = new SwapChainDescription()
            {
                BufferCount = 1,
                ModeDescription =
               new ModeDescription(Form.ClientSize.Width, Form.ClientSize.Height,
                                        new Rational(60, 1), Format.R8G8B8A8_UNorm),
                IsWindowed = true,
                OutputHandle = Form.Handle,
                SampleDescription = new SampleDescription(1, 0),
                SwapEffect = SwapEffect.Discard,
                Usage = Usage.RenderTargetOutput
            };

            // Used for debugging dispose object references
            // Configuration.EnableObjectTracking = true;

            // Disable throws on shader compilation errors
            //Configuration.ThrowOnShaderCompileError = false;

            // Create Device and SwapChain
            SharpDX.Direct3D11.Device.CreateWithSwapChain(DriverType.Hardware, DeviceCreationFlags.None, SwapChainDesc, out _device, out _swapChain);

            // Ignore all windows events
            Factory = SwapChain.GetParent<Factory>();
            Factory.MakeWindowAssociation(Form.Handle, WindowAssociationFlags.IgnoreAll);


            // Compile Vertex and Pixel shaders
            Effect1 = new Effect();
            Effect1.LoadFromFile(Device, "Source/Shaders/BasicPositionColor.fx", "VS", "PS", "vs_4_0", "ps_4_0");
            // Layout from VertexShader input signature
            Effect1.InputLayout = new InputLayout(Device, Effect1.ShaderSignature, new[]
                    {
                        new InputElement("POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
                        new InputElement("COLOR", 0, Format.R32G32B32A32_Float, 16, 0)
                    });

            Effect2 = new Effect();
            Effect2.LoadFromFile(Device, "Source/Shaders/BasicPositionColorTexture.fx", "VS", "PS", "vs_4_0", "ps_4_0");
            // Layout from VertexShader input signature
            Effect2.InputLayout = new InputLayout(Device, Effect2.ShaderSignature, new[]
                    {
                        new InputElement("POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
                        new InputElement("COLOR", 0, Format.R32G32B32A32_Float, 16, 0),
                        new InputElement("TEXCOORD", 0, Format.R32G32_Float, 32, 0),
                    });

            // Create Constant Buffer
            ContantBuffer = new Buffer(Device, Utilities.SizeOf<Matrix>(), ResourceUsage.Default, BindFlags.ConstantBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0);



            //Position Color
            VertexBuffer = Buffer.Create(Device, BindFlags.VertexBuffer, new[] {
                new VertexPositionColor(new Vector4(-1, -1, 0, 1), Color.Red.ToVector4()),
                new VertexPositionColor(new Vector4(-1, 1, 0, 1), Color.Green.ToVector4()),
                new VertexPositionColor(new Vector4(1, 1, 0, 1), Color.Blue.ToVector4()),
                new VertexPositionColor(new Vector4(1, -1, 0, 1), Color.Yellow.ToVector4())
            });

            //Position Color Texture
            VertexBuffer2 = Buffer.Create(Device, BindFlags.VertexBuffer, new[] {
                new VertexPositionColorTexture(new Vector4(-1, -1, 0, 1), Color.White.ToVector4(), new Vector2(0,1)),
                new VertexPositionColorTexture(new Vector4(-1, 1, 0, 1), Color.White.ToVector4(),new Vector2(0,0)),
                new VertexPositionColorTexture(new Vector4(1, 1, 0, 1), Color.White.ToVector4(),new Vector2(1,0)),
                new VertexPositionColorTexture(new Vector4(1, -1, 0, 1), Color.White.ToVector4(),new Vector2(1,1))
            });

            IndexBuffer = Buffer.Create(Device, BindFlags.IndexBuffer, new[] {
                0,1,2,
                0,2,3
            });

            //Set RenderTexture
            RenderTexture = new RenderTexture();

            if (!RenderTexture.Initialize(Device, new Point(Form.Bounds.Width, Form.Bounds.Height)))
                Console.WriteLine("Cannot init RenderTexture");

            //Create Sampler
            var samplerState = new SamplerState(Device, new SamplerStateDescription()
            {
                //Filter = Filter.MinimumMinMagMipLinear,
                //Filter = Filter.MaximumMinMagPointMipLinear,
                Filter = Filter.MinimumMinLinearMagPointMipLinear,
                AddressU = TextureAddressMode.Wrap,
                AddressV = TextureAddressMode.Wrap,
                AddressW = TextureAddressMode.Wrap,
                MipLodBias = 0,
                MaximumAnisotropy = 1,
                ComparisonFunction = Comparison.Always,
                BorderColor = new SharpDX.Mathematics.Interop.RawColor4(0, 0, 0, 0),
                MinimumLod = 0,
                MaximumLod = float.MaxValue,
            });

            //Set Shader
            Effect1.ApplyShader(Context);

            //Set Buffers
            Context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            Context.VertexShader.SetConstantBuffer(0, ContantBuffer);


            //Set Sampler
            Context.PixelShader.SetSampler(0, samplerState);

            // Setup handler on resize form
            Form.UserResized += (sender, args) => userResized = true;

            // Setup full screen mode change F5 (Full) F4 (Window)
            Form.KeyUp += (sender, args) =>
            {
                if (args.KeyCode == Keys.F5)
                    SwapChain.SetFullscreenState(true, null);
                else if (args.KeyCode == Keys.F4)
                    SwapChain.SetFullscreenState(false, null);
                else if (args.KeyCode == Keys.Escape)
                    Form.Close();
                else if (args.KeyCode == Keys.Space)
                {

                }
            };
        }



        public override void Update()
        {
            CheckIfResized();
        }



        public override void Render()
        {
            //Camera
            var proj = Matrix.OrthoLH(3 * Form.Bounds.Width / Form.Bounds.Height, 3, 0.01f, 100f);
            var view = Matrix.LookAtLH(new Vector3(0, 0, -10), new Vector3(0, 0, 20), Vector3.UnitY);
            var viewProj = Matrix.Multiply(view, proj);

            var world = Matrix.Identity;

            var worldViewProj = world * viewProj;
            worldViewProj.Transpose();

            //Update wvp matrix
            Context.UpdateSubresource(ref worldViewProj, ContantBuffer);

            DrawOnTexture();

            //Set BackBuffer as render target
            Context.OutputMerger.SetTargets(depthView, renderView);

            // Clear views
            Context.ClearDepthStencilView(depthView, DepthStencilClearFlags.Depth, 1.0f, 0);
            Context.ClearRenderTargetView(renderView, Color.Pink);

            //Set TextureColor Shader
            Effect2.ApplyShader(Context);

            //Set Buffers
            Context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(VertexBuffer2, Utilities.SizeOf<VertexPositionColorTexture>(), 0));
            Context.InputAssembler.SetIndexBuffer(IndexBuffer, Format.R32_UInt, 0);

            //Set Texture to Shader
            Context.PixelShader.SetShaderResource(0, RenderTexture.ShaderResourceView);

            //Draw 
            Context.DrawIndexed(6, 0, 0);

            // Present!
            SwapChain.Present(0, PresentFlags.None);
        }

        private void DrawOnTexture()
        {
            //Set Color Shader
            Effect1.ApplyShader(Context);

            //Set Buffers
            Context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(VertexBuffer, Utilities.SizeOf<VertexPositionColor>(), 0));
            Context.InputAssembler.SetIndexBuffer(IndexBuffer, Format.R32_UInt, 0);

            //Set Target
            RenderTexture.SetRenderTarget(Context, depthView);

            //Clear Targets - Green Bgound
            RenderTexture.ClearRenderTarget(Context, depthView, 0, 1, 0, 1);

            //Draw on RenderTarget
            Context.DrawIndexed(6, 0, 0);

        }

        private void CheckIfResized()
        {
            if (userResized)
            {
                // Dispose all previous allocated resources
                Utilities.Dispose(ref backBuffer);
                Utilities.Dispose(ref renderView);
                Utilities.Dispose(ref depthBuffer);
                Utilities.Dispose(ref depthView);

                // Resize the backbuffer
                SwapChain.ResizeBuffers(SwapChainDesc.BufferCount, Form.ClientSize.Width, Form.ClientSize.Height, Format.Unknown, SwapChainFlags.None);

                // Get the backbuffer from the swapchain
                backBuffer = Texture2D.FromSwapChain<Texture2D>(SwapChain, 0);

                // Renderview on the backbuffer
                renderView = new RenderTargetView(Device, backBuffer);

                // Create the depth buffer
                depthBuffer = new Texture2D(Device, new Texture2DDescription()
                {
                    Format = Format.D32_Float_S8X24_UInt,
                    ArraySize = 1,
                    MipLevels = 1,
                    Width = Form.ClientSize.Width,
                    Height = Form.ClientSize.Height,
                    SampleDescription = new SampleDescription(1, 0),
                    Usage = ResourceUsage.Default,
                    BindFlags = BindFlags.DepthStencil,
                    CpuAccessFlags = CpuAccessFlags.None,
                    OptionFlags = ResourceOptionFlags.None
                });

                // Create the depth buffer view
                depthView = new DepthStencilView(Device, depthBuffer);

                // Setup targets and viewport for rendering
                Context.Rasterizer.SetViewport(new Viewport(0, 0, Form.ClientSize.Width, Form.ClientSize.Height, 0.0f, 1.0f));

                Context.OutputMerger.SetTargets(depthView, renderView);

                // We are done resizing
                userResized = false;
            }
        }

        public override void Dispose()
        {
            // Release all resources
            Effect1?.Dispose();
            Effect2?.Dispose();

            VertexBuffer.Dispose();
            VertexBuffer2.Dispose();
            IndexBuffer.Dispose();

            depthBuffer.Dispose();
            depthView.Dispose();
            renderView.Dispose();
            backBuffer.Dispose();
            Context.ClearState();
            Context.Flush();

            Context.Dispose();
            Device.Dispose();
            SwapChain.Dispose();
            Factory.Dispose();
        }

    }
}
