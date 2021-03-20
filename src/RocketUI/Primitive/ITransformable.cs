using Microsoft.Xna.Framework;

namespace RocketUI
{
    public interface ITransformable
    {
        Transform3D Transform { get; }

        Vector3 Scale
        {
            get => Transform.RelativeScale;
            set => Transform.RelativeScale = value;
        }
        
        Vector3 Position
        {
            get => Transform.RelativePosition;
            set => Transform.RelativePosition = value;
        }

        Quaternion Rotation
        {
            get => Transform.RelativeRotation;
            set => Transform.RelativeRotation = value;
        }
        
        Matrix World
        {
            get => Transform.World;
        }
    }
}