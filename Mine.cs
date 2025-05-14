// Descriptions
// The Mine will be spreaded after a certain time of Sticky Missile being shooted or the Sticky Missile hit the Asteroid
// The total of 5 Mines will be spreaded by one Sticky Missile
// Each of the Mine will stick on the different closest Asteroid
// If the Mine does not have a target, it will spread to a random direction
// After certain time it will explode together with the Asteroid and both instance will be destroyed
// If the Asteroid with Mine get destroyed before the time reached, it will explode earlier and both instance will be destroyed as well
// The Mine will be destroyed once get out from the screen

using GAlgoT2430.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Lab06
{
    public class Mine : SpriteGameObject, ICollidable
    {
        private Rectangle _rectangle;
        private Vector2 _velocity;
        private float speed = 500f;
        public Mine() : base("mine")
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
            _game.CollisionEngine.Listen(typeof(Background), typeof(Mine), CollisionEngine.NotAABB);
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

        public Vector2 GetVelocity()
        {
            return this._velocity;
        }
        public void SetVelocity(Vector2 _velocity)
        {
            this._velocity = _velocity;
        }

        public float GetSpeed()
        {
            return this.speed;
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
            if (collisionInfo.Other is Background)
            {
                // Destruct the missile
                GameObjectCollection.DeInstantiate(this);
            }
        }
    }
}