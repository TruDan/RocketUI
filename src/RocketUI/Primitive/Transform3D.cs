using System;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;

namespace RocketUI
{
    [DataContract]
    public class Transform3D
    {
        public event EventHandler Changed;

        private Vector3     _relativeScale    = Vector3.One;
        private Vector3     _relativePosition = Vector3.Zero;
        private Quaternion  _relativeRotation = Quaternion.Identity;
        private Vector3     _scale            = Vector3.One;
        private Vector3     _position         = Vector3.Zero;
        private Quaternion  _rotation         = Quaternion.Identity;
        private Matrix      _world            = Matrix.Identity;
        private Transform3D _parentTransform;
        private bool        _dirty;

        public bool IsDirty
        {
            get => _dirty;
        }

        public virtual Transform3D ParentTransform
        {
            get
            {
                return _parentTransform;
            }
            set
            {
                if (_parentTransform != null)
                    _parentTransform.Changed -= OnParentChanged;
                
                _parentTransform = value;
                
                if (_parentTransform != null)
                    _parentTransform.Changed += OnParentChanged;
            }
        }

        [DataMember]
        public virtual Vector3 RelativeScale
        {
            get => _relativeScale;
            set
            {
                if (_relativeScale == value)
                    return;
                _relativeScale = value;
                OnChanged();
            }
        }

        [DataMember]
        public virtual Vector3 RelativePosition
        {
            get => _relativePosition;
            set
            {
                if (_relativePosition == value)
                    return;
                _relativePosition = value;
                OnChanged();
            }
        }

        [DataMember]
        public virtual Quaternion RelativeRotation
        {
            get => _relativeRotation;
            set
            {
                if (_relativeRotation == value)
                    return;
                _relativeRotation = value;
                OnChanged();
            }
        }


        public virtual Vector3 Scale
        {
            get
            {                
                if (_dirty)
                    UpdateAbsolutes();
                return _position;
            }
            set
            {
                if (ParentTransform == null)
                {
                    RelativeScale = value;
                }
                else
                {
                    RelativeScale = value / ParentTransform.Scale;
                }
            }
        }

        public virtual Vector3 Position
        {
            get
            {
                
                if (_dirty)
                    UpdateAbsolutes();
                return _position;
            }
            set
            {
                if (ParentTransform == null)
                {
                    RelativePosition = value;
                }
                else
                {
                    RelativePosition = value - ParentTransform.Position;
                }
            }
        }

        public virtual Quaternion Rotation
        {
            get
            {                
                if (_dirty)
                    UpdateAbsolutes();
                return _rotation;
            }
            set
            {
                if (ParentTransform == null)
                {
                    RelativeRotation = value;
                }
                else
                {
                    RelativeRotation = value / ParentTransform.Rotation;
                }
            }
        }
        
        public virtual Matrix World
        {
            get
            {
                if (_dirty)
                    UpdateAbsolutes();

                return _world;
            }
        }

        protected virtual void OnChanged()
        {
            _dirty = true;
            Changed?.Invoke(this, EventArgs.Empty);
        }

        private void OnParentChanged(object sender, EventArgs args)
        {
            _dirty = true;
        }
        
        private void UpdateAbsolutes()
        {
            _dirty = false;
            _world = Matrix.Identity
                     * Matrix.CreateScale(_relativeScale)
                     * Matrix.CreateFromQuaternion(_relativeRotation)
                     * Matrix.CreateTranslation(_relativePosition);
                    
            if (ParentTransform != null)
                _world *= ParentTransform.World;
            try
            {
                if (_world.Decompose(out _scale, out _rotation, out _position))
                {

                }
            }
            catch (Exception ex)
            {                
                _dirty = true;
            }
        }
    }
}