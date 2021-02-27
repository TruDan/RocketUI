using System;
using Microsoft.Xna.Framework;

namespace RocketUI
{
    public class Transform3D
    {
        public event EventHandler PositionChanged;
        
        private Vector3    _scale    = Vector3.One;
        private Vector3    _position = Vector3.Zero;
        private Quaternion _rotation = Quaternion.Identity;
        private Matrix     _world = Matrix.Identity;
        private bool       _dirty;
        
        public bool IsDirty
        {
            get => _dirty;
        }
        
        public virtual Vector3 Scale
        {
            get => _scale;
            set
            {
                if(_scale == value) 
                    return;
                _scale = value;
                OnPositionChanged();
            }
        }
        
        public virtual Vector3 Position
        {
            get => _position;
            set
            {
                if(_position == value) 
                    return;
                _position = value;
                OnPositionChanged();
            }
        }
        
        public virtual Quaternion Rotation
        {
            get => _rotation;
            set
            {
                if (_rotation == value)
                    return;
                _rotation = value;
                OnPositionChanged();
            }
        }

        public virtual Matrix World
        {
            get
            {
                if (_dirty)
                {
                    _dirty = false;
                    _world = Matrix.Identity
                            * Matrix.CreateScale(_scale)
                            * Matrix.CreateFromQuaternion(_rotation)
                            * Matrix.CreateTranslation(_position);
                }

                return _world;
            }
        }
        
        protected virtual void OnPositionChanged()
        {
            _dirty = true;
            PositionChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public interface ITransformable
    {
        Transform3D Transform { get; }

        Vector3 Scale
        {
            get => Transform.Scale;
            set => Transform.Scale = value;
        }
        
        Vector3 Position
        {
            get => Transform.Position;
            set => Transform.Position = value;
        }

        Quaternion Rotation
        {
            get => Transform.Rotation;
            set => Transform.Rotation = value;
        }
        
        Matrix World
        {
            get => Transform.World;
        }
    }
}