using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpDX_Basic11.Source.App
{
    class App : IDisposable
    {
        public RenderForm Form { get; set; }

        /// <summary>Basic render Init - calls after creating RenderForm </summary>
        public virtual void Initialize()
        {

        }

        public void Run(string formTitle)
        {
            Form = new RenderForm(formTitle);

            Initialize();

            using (GameLoop loop = new GameLoop())
            {
                loop.Update += Update;
                loop.Render += Render;

                loop.Start(Form);
            }
        }


        public virtual void Update()
        {

        }


        public virtual void Render()
        {

        }

        public virtual void Dispose()
        {

        }
    }
}
