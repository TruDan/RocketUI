using Microsoft.Xna.Framework;

namespace RocketUI
{
    public interface ITransformable
    {
        Transform3D Transform { get; }
        
        Matrix World
        {
            get => Transform.World;
        }
    }
}