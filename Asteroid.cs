// Description
// All the attraction logics written here

using GAlgoT2430.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Lab06
{
    public class Asteroid : SpriteGameObject, ICollidable
    {
        private Rectangle _rectangle;
        private Vector2 _velocity;
        private float speed = 60f;
        private float rotateDirection;
        private Vector2 _windowCentrePosition;
        private Random _rand;
        private SoundEffect _soundEffect;
        public Asteroid() : base("spaceMeteors_002_small")
        {
            _rand = new Random();
        }

        private Vector2 ComputeRandomPosition()
        {
            float width = _game.Graphics.PreferredBackBufferWidth;
            float height = _game.Graphics.PreferredBackBufferHeight;
            float x, y;
            
            while (true)
            {
                x = _rand.NextSingle() * (width + 100);
                y = _rand.NextSingle() * (height + 100);
                if (x < 50 || x > width + 50 || y < 50 || y > height + 50)
                {
                    x -= 50;
                    y -= 50;
                    break;
                }
            }
                
            Vector2 position = new Vector2(x, y);

            return position;
        }

        public override void Initialize()
        {
            // Position
            Position = ComputeRandomPosition();
            
            // Target Position
            _windowCentrePosition = new Vector2(_game.Graphics.PreferredBackBufferWidth / 2f, _game.Graphics.PreferredBackBufferHeight / 2f);
            _velocity = Vector2.Normalize(_windowCentrePosition - Position);

            // Sound Effect
            _soundEffect = _game.Content.Load<SoundEffect>("enemyExplosion");

            // Rotate Direction
            rotateDirection = _rand.NextSingle() < 0.5 ? 1f : -1f;

            // Origin
            Origin = new Vector2(Texture.Width / 2, Texture.Height / 2);
            
            // Bound
            _rectangle.X = (int)(Position.X - Origin.X);
            _rectangle.Y = (int)(Position.Y - Origin.Y);
            _rectangle.Width = Texture.Width;
            _rectangle.Height = Texture.Height;

            // Collision
            _game.CollisionEngine.Listen(typeof(SpaceShip), typeof(Asteroid), CollisionEngine.AABB);
            _game.CollisionEngine.Listen(typeof(Missile), typeof(Asteroid), CollisionEngine.AABB);
            _game.CollisionEngine.Listen(typeof(Red), typeof(Asteroid), CollisionEngine.AABB);
            _game.CollisionEngine.Listen(typeof(Purple), typeof(Asteroid), CollisionEngine.AABB);
            _game.CollisionEngine.Listen(typeof(StickyMissile), typeof(Asteroid), CollisionEngine.AABB);
        }

        public override void Update()
        {
            float elapsedSeconds = ScalableGameTime.DeltaTime;

            Orientation += rotateDirection * elapsedSeconds;
            
            Position += _velocity * speed * elapsedSeconds;

            if (GameObjectCollection.FindByName("Blue") != null)
            {
                var blue = GameObjectCollection.FindByName("Blue"); 
                float radius = 150f;
                float distance = (blue.Position - Position).Length();

                if (distance < radius)
                {
                    Vector2 attraction = Vector2.Normalize(blue.Position - Position);
                    Position += attraction * 50 * elapsedSeconds;
                }
            }

            if (GameObjectCollection.FindByName("Purple") != null)
            {
                var purple = GameObjectCollection.FindByName("Purple"); 
                float radius = 300f * purple.Scale.X;
                float distance = (purple.Position - Position).Length();

                if (distance < radius)
                {
                    Vector2 attraction = Vector2.Normalize(purple.Position - Position);
                    Position += attraction * 200 * elapsedSeconds;
                }
            }
        }

        public Vector2 GetVelocity()
        {
            return this._velocity;
        }

        public override void Draw()
        {
            _game.SpriteBatch.Begin();
            _game.SpriteBatch.Draw(Texture, Position, null, Color.White, Orientation, Origin, Scale, SpriteEffects.None, 0f);
            _game.SpriteBatch.End();
        }

        public string GetGroupName()
        {
            return GetType().Name;
        }

        public Rectangle GetBound()
        {
            _rectangle.Location = (Position - Origin).ToPoint();
            return _rectangle;
        }

        public void OnCollision(CollisionInfo collisionInfo)
        {
            if (collisionInfo.Other is Missile || collisionInfo.Other is SpaceShip)
            {
                // Destruct the missile
                GameObjectCollection.DeInstantiate(this);
                _soundEffect.Play();
            }
        }
    }
}