using System.Numerics;

namespace RocketUI
{
    public interface ITransformable
    {
        Transform3D Transform { get; }
        
        Matrix4x4 World
        {
            get => Transform.World;
        }
    }
}