using SharpDX.Windows;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SharpDX_Basic11.Source.App
{
    class GameLoop : IDisposable
    {
        Stopwatch _stopwatch = new Stopwatch();
        Stopwatch _stopwatch2 = new Stopwatch();

        int _fpsCount = 0;
        int[] _averageLoopTicks;
        int _averageLoopTick = 0;


        public int FPS { get; private set; }

        public bool IsActive { get; set; }

        public delegate void _update();
        public event _update Update;

        public delegate void _fixedUpdate();
        public event _fixedUpdate FixedUpdate;

        public delegate void _render();
        public event _render Render;

        public GameLoop()
        {
            int size = 20;
            _averageLoopTicks = new int[size];

            _averageLoopTicks[0] = 10000000 / 60;

            for (int i = 1; i < size; i++)
            {
                _averageLoopTicks[i] = _averageLoopTicks[0];
            }

            IsActive = true;
        }

        public void Start(RenderForm renderForm)
        {
            _stopwatch.Restart();
            _stopwatch2.Restart();

            int targetFPS = 144;
            int targetTicksPerFrame = (int)TimeSpan.TicksPerSecond / targetFPS * 2;

            RenderLoop.Run(renderForm, () =>
            {

                int elapsedTicks = (int)_stopwatch2.ElapsedTicks;
                _stopwatch2.Restart();

                UpdateFPS();

                Update?.Invoke();
                FixedUpdate?.Invoke();

                Render?.Invoke();

                if (_averageLoopTick < targetTicksPerFrame)
                {
                    Thread.Sleep(new TimeSpan((targetTicksPerFrame - _averageLoopTick)));
                }

                UpdateAverage(elapsedTicks);
            });


        }

        private void UpdateFPS()
        {
            _fpsCount++;

            if (_stopwatch.Elapsed.TotalSeconds >= 1.0f)
            {
                FPS = _fpsCount;
                _fpsCount = 0;


                //Debug.WriteLine("FPS: " + FPS + " " + _stopwatch.ElapsedTicks.ToString());

                _stopwatch.Restart();
            }
        }

        void UpdateAverage(int newValue)
        {
            int sum = 0;

            for (int i = 0; i < _averageLoopTicks.Length - 1; i++)
            {
                int next = _averageLoopTicks[i + 1];

                _averageLoopTicks[i] = next;
                sum += next;

            }

            _averageLoopTicks[_averageLoopTicks.Length - 1] = newValue;
            sum += newValue;

            _averageLoopTick = sum / _averageLoopTicks.Length;
        }

        public void Dispose()
        {

        }
    }
}
