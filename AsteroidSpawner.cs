using GAlgoT2430.Engine;
using System.Collections.Generic;

namespace Lab06
{
    public class AsteroidSpawner : GameObject
    {
        private float lastRealTime;
        public AsteroidSpawner() : base("AsteroidSpawner") {}

        public override void Initialize()
        {
            lastRealTime = ScalableGameTime.RealTime;
        }
        
        public override void Update()
        {
            float realTime = ScalableGameTime.RealTime;

            if (realTime - lastRealTime >= 5)
            {
                for (int i = 0; i < 8; i++)
                {
                    Asteroid asteroid = new Asteroid();
                    asteroid.LoadContent();
                    asteroid.Initialize();
                }
                lastRealTime = realTime;
            }
        }
    }
}