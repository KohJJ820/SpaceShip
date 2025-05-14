// Descriptions
// The Sticky Missile will be fired automatically for every 5 seconds
// The Sticky Missile will mimic the sin graph movement
// The Sticky Missile will destroy and spread 5 Mines after a certain time
// The Sticky Missile will destroy and spread earlier if it hits the Asteroid
// The Sticky Missile is not able destroy the Asteroid since it only transport the Minesdd
// The Sticky Missile will be destroyed once get out from screen

using System;
using GAlgoT2430.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Lab06
{
    public class StickyMissile : SpriteGameObject, ICollidable
    {
        private Rectangle _rectangle;
        private Vector2 _velocity;
        private float time;
        private float speed = 150f;
        public StickyMissile() : base("StickyMissile", "stickyMissile")
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
            _game.CollisionEngine.Listen(typeof(Background), typeof(StickyMissile), CollisionEngine.NotAABB);
        }

        public override void Update()
        {
            float elapsedSeconds = ScalableGameTime.DeltaTime;
            time += elapsedSeconds;

            // The motion of the missle to mimic sin graph
            float swing = 150 * (float)Math.Sin(time * 5);
            Vector2 yaxis = Vector2.Normalize(new Vector2(-_velocity.Y, _velocity.X));

            Vector2 temp = (_velocity * speed) + (yaxis * swing);

            Orientation = (float)Math.Atan2(temp.Y, temp.X);
            
            Position.X += temp.X * elapsedSeconds;
            Position.Y += temp.Y * elapsedSeconds;
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