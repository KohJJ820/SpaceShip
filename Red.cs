// Descriptions
// Red will be created when the player press Key E (merge_2) or Key R (merge_1) on keyboard or right clicked on mouse (Turning)
// When merge_1 happened, Red will move vertically to merge with Blue
// When merge_2 happened, Red will move towards the spaceship's orientation
// When Turning happened, Red will move vertically oppose to Blue and start to move around spaceship once it reached the radius of spaceship
// Red will be destroyed when collide with Blue
// Red will destroy the asteroids when collided with it
// Merging detection happens in this class
// No animation because I cant find any of it
// The Red will be destroyed once get out from the screen


using GAlgoT2430.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Lab06
{
    public class Red : SpriteGameObject, ICollidable
    {
        private Rectangle _rectangle;
        private Vector2 _velocity;
        private float speed = 200f;
        private bool isMerge_1; // The variable to check if merge_1 or merge_2 happened
        public Red(bool isMerge_1) : base("Red", "red")
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

            // Collision
            // If merge_1 happened then OneInsideAnother, for better visual
            if (isMerge_1)
            {
                _game.CollisionEngine.Listen(typeof(Blue), typeof(Red), CollisionEngine.OneInsideAnother);
            }
            _game.CollisionEngine.Listen(typeof(Background), typeof(Red), CollisionEngine.NotAABB);
        }

        public override void Update()
        {
            float elapsedSeconds = ScalableGameTime.DeltaTime;
            
            Orientation += 1 * elapsedSeconds;
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
        
        public void setVelocity(Vector2 _velocity)
        {
            this._velocity = _velocity;
        }

        public void setSpeed(float speed)
        {
            this.speed = speed;
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

            if (collisionInfo.Other is Blue)
            {
                GameObjectCollection.DeInstantiate(this);
            }
        }
    }
}