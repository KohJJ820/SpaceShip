using GAlgoT2430.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Lab06
{
    public class Background : SpriteGameObject, ICollidable
    {
        private Rectangle _rectangle;

        public Background() : base("purple")
        {

        }

        public override void Initialize()
        {
            _rectangle = new Rectangle(0, 0, _game.Graphics.PreferredBackBufferWidth, _game.Graphics.PreferredBackBufferHeight);
            _rectangle.Width = _game.Graphics.PreferredBackBufferWidth;
            _rectangle.Height = _game.Graphics.PreferredBackBufferHeight;

            // // Collision
            // _game.CollisionEngine.Listen(typeof(Background), typeof(Missile), CollisionEngine.NotAABB);
            // _game.CollisionEngine.Listen(typeof(Asteroid), typeof(Background), CollisionEngine.NotAABB);
        }

        public override void Update()
        {
            // Instead of 
            // float elapsedSeconds = (float)ScalableGameTime.GameTime.ElapsedGameTime;
            // We do this
            float elapsedSeconds = ScalableGameTime.DeltaTime;
        }

        public override void Draw()
        {
            _game.SpriteBatch.Begin(samplerState: SamplerState.LinearWrap);
            _game.SpriteBatch.Draw(Texture, Vector2.Zero, _rectangle, Color.White);
            _game.SpriteBatch.End();
        }

        public string GetGroupName()
        {
            return GetType().Name;
        }

        public Rectangle GetBound()
        {
            return _rectangle;
        }
    }
}
