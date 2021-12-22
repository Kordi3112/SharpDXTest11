using SharpDX_Basic11.Source.App;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpDX_Basic11
{
    class Program
    {
        public static void Main()
        {
            using (var app = new App1())
            {
                app.Run("SharpDX test");
            }
        }

    }
}
