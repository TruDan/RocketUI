#if STRIDE
using Stride.Core.Mathematics;
#else
using Microsoft.Xna.Framework;
#endif

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