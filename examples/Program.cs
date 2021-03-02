using System;
using Atlas;
using Microsoft.Xna.Framework;

namespace Atlas.Examples
{
    class Program
    {
        static void Main(string[] args)
        {
            var engine = new EngineCore();
            engine.ScreenSize = new Point(1280, 720);
            engine.ContentDirectory = "content";
            engine.Font = "04b_03.ttf";
            engine.Start<Pong.MainScene>();
        }
    }
}
