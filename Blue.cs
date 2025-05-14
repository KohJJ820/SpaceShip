// Descriptions
// Blue will be created when the player press Key Q (merge_2) or Key R (merge_1) on keyboard or right clicked on mouse (Turning)
// When merge_1 happened, Blue will move vertically to merge with Red
// When merge_2 happened, Blue will move towards spaceship's orientation with decreasing speed, stop after some time
// When Turning happened, Blue will move vertically oppose to Red and start to move around spaceship once it reached the radius of spaceship
// Blue will stay in the screen for some time before getting destroyed if no collision with red happened
// Blue will be destroyed when collide with Red
// Blue will not destroy the asteroid when collided. Instead, it attract the asteroids within radius to move towards it
// Contains animation that updates 4 frames per second
// The Blue will be destroyed once get out from the screen

using GAlgoT2430.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Lab06
{
    public class Blue : SpriteGameObject, ICollidable
    {
        private Rectangle _rectangle;
        private Vector2 _velocity;
        private float speed = 200f;
        private float lastRealTime, stayTime; // stayTime is used to handle the stay time of Blue
        private int count = 0;
        private bool isMerge_1; 
        private String[] textures;
        public Blue(bool isMerge_1) : base("Blue", "blue_1")
        {
            this.isMerge_1 = isMerge_1;
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

            lastRealTime = ScalableGameTime.RealTime;
            stayTime = ScalableGameTime.RealTime;

            // Collision
            // If merge_1 happened then OneInsideAnother, for better visual
            if (isMerge_1)
            {
                _game.CollisionEngine.Listen(typeof(Blue), typeof(Red), CollisionEngine.OneInsideAnother);
            }
            _game.CollisionEngine.Listen(typeof(Blue), typeof(Background), CollisionEngine.NotAABB);

            // Textures
            textures = ["blue_1", "blue_2", "blue_3"];
        }

        public override void Update()
        {
            float elapsedSeconds = ScalableGameTime.DeltaTime;
            float realTime = ScalableGameTime.RealTime;

            // 4 frame per second
            if (realTime - lastRealTime > 0.25)
            {
                lastRealTime = realTime;
                count += 1;
                if (count == 3)
                {
                    count = 0;
                }
                _textureName = textures[count];
                LoadContent();
            }

            Orientation += 1 * elapsedSeconds;
            Position.X += _velocity.X * speed * elapsedSeconds;
            Position.Y += _velocity.Y * speed * elapsedSeconds;

            // Slowly stop the blue
            speed -= 100 * elapsedSeconds;
            if (speed <= 0)
            {
                speed = 0;
            }

            // Destroy after 10 seconds
            if (realTime - stayTime >= 10)
            {
                GameObjectCollection.DeInstantiate(this);
            }
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
        
        public void setVelocity(Vector2 _velocity)
        {
            this._velocity = _velocity;
        }

        public void setSpeed(float speed)
        {
            this.speed = speed;
        }

        public void attract()
        {
            
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
            if (collisionInfo.Other is Red || collisionInfo.Other is Background)
            {
                // Destruct the missile
                GameObjectCollection.DeInstantiate(this);
            }
        }
    }
}