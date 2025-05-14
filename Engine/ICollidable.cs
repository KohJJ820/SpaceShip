using Microsoft.Xna.Framework;

namespace GAlgoT2430.Engine
{
    public interface ICollidable
    {
        public virtual void OnCollision(CollisionInfo collisionInfo) { }

        public string GetGroupName();

        public Rectangle GetBound();
    }
}
