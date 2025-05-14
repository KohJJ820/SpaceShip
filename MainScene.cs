using GAlgoT2430.Engine;

namespace Lab06
{
    public class MainScene : GameScene
    {
        public override void CreateScene()
        {
            // This is where you will construct game objects
            Background background = new Background();
            SpaceShip spaceShip = new SpaceShip();
            // Missle missle = new Missle();
            // Asteroid asteroid = new Asteroid();
            AsteroidSpawner asteroidSpawner = new AsteroidSpawner();
        }
    }
}
