using GAlgoT2430.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Lab06
{
    public class Missile : SpriteGameObject, ICollidable
    {
        private Rectangle _rectangle;
        private Vector2 _velocity;
        private float speed = 300f;
        public Missile() : base("spaceMissiles_015_right")
        {

        }

        public override void Initialize()
        {
            // Position
            Position = new Vector2(_game.Graphics.PreferredBackBufferWidth / 2f, _game.Graphics.PreferredBackBufferHeight / 2f);
            
            // Origin
            Origin = new Vector2(Texture.Width / 2, Texture.Height / 2);
            
            // Bound
            _rectangle.X = (int)(Position.X - Origin.X);
            _rectangle.Y = (int)(Position.Y - Origin.Y);
            _rectangle.Width = Texture.Width;
            _rectangle.Height = Texture.Height;

            // Collision
            _game.CollisionEngine.Listen(typeof(Background), typeof(Missile), CollisionEngine.NotAABB);
            _game.CollisionEngine.Listen(typeof(Asteroid), typeof(Missile), CollisionEngine.AABB);
        }

        public override void Update()
        {
            float elapsedSeconds = ScalableGameTime.DeltaTime;
            
            Position.X += _velocity.X * speed * elapsedSeconds;
            Position.Y += _velocity.Y * speed * elapsedSeconds;
        }

        public override void Draw()
        {
            _game.SpriteBatch.Begin();
            _game.SpriteBatch.Draw(Texture, Position, null, Color.White, Orientation, Origin, Scale, SpriteEffects.None, 0f);
            _game.SpriteBatch.End();
        }

        public void SetVelocity(Vector2 _velocity)
        {
            this._velocity = _velocity;
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
            if (collisionInfo.Other is Background || collisionInfo.Other is Asteroid)
            {
                // Destruct the missile
                GameObjectCollection.DeInstantiate(this);
            }
        }
    }
}