using System;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;

namespace RocketUI
{
    [DataContract]
    public class Transform3D
    {
        public event EventHandler Changed;

        private Vector3     _localScale          = Vector3.One;
        private Vector3     _localScaleOrigin    = Vector3.Zero;
        private Vector3     _localPosition       = Vector3.Zero;
        private Vector3     _localRotationOrigin = Vector3.Zero;
        private Vector3     _localRotation       = Vector3.Zero;
        private Matrix      _world               = Matrix.Identity;
        private Transform3D _parentTransform;
        private bool        _dirty;

        public bool IsDirty
        {
            get => _dirty;
        }

        public virtual Transform3D ParentTransform
        {
            get { return _parentTransform; }
            set
            {
                if (_parentTransform != null)
                    _parentTransform.Changed -= OnParentChanged;

                _parentTransform = value;
                OnChanged();

                if (_parentTransform != null)
                    _parentTransform.Changed += OnParentChanged;
            }
        }

        
        [DataMember]
        public virtual Vector3 LocalScale
        {
            get => _localScale;
            set
            {
                if (_localScale == value)
                    return;
                _localScale = value;
                OnChanged();
            }
        }
        
        [DataMember]
        public virtual Vector3 LocalScaleOrigin
        {
            get => _localScaleOrigin;
            set
            {
                if (_localScaleOrigin == value)
                    return;
                _localScaleOrigin = value;
                OnChanged();
            }
        }

        [DataMember]
        public virtual Vector3 LocalPosition
        {
            get => _localPosition;
            set
            {
                if (_localPosition == value)
                    return;
                _localPosition = value;
                OnChanged();
            }
        }

        [DataMember]
        public virtual Vector3 LocalRotationOrigin 
        {
            get => _localRotationOrigin;
            set
            {
                if (_localRotationOrigin == value)
                    return;
                _localRotationOrigin = value;
                OnChanged();
            }
        }
       
        [DataMember]
        public virtual Vector3 LocalRotation
        {
            get => _localRotation;
            set
            {
                if (_localRotation == value)
                    return;
                _localRotation = new Vector3(value.X % 360.0f, value.Y % 360.0f, value.Z % 360.0f);
                OnChanged();
            }
        }


        public virtual Vector3 Scale
        {
            get
            {
                if (_dirty)
                    UpdateAbsolutes();
                var result = _localScale;
                if (_parentTransform != null)
                    result *= _parentTransform.Scale;
                return result;
            }
            set
            {
                if (ParentTransform == null)
                {
                    LocalScale = value;
                }
                else
                {
                    LocalScale = value / ParentTransform.Scale;
                }
            }
        }

        public virtual Vector3 Position
        {
            get
            {
                if (_dirty)
                    UpdateAbsolutes();
                return World.Translation;
            }
            set
            {
                if (ParentTransform == null)
                {
                    LocalPosition = value;
                }
                else
                {
                    LocalPosition = Vector3.Transform(value, Matrix.Invert(ParentTransform.World));
                }
            }
        }

        public virtual Vector3 Rotation
        {
            get
            {
                if (_dirty)
                    UpdateAbsolutes();
                if(_parentTransform != null)
                    return _parentTransform.Rotation + _localRotation;
                return _localRotation;
            }
            set
            {
                if (ParentTransform == null)
                {
                    LocalRotation = value;
                }
                else
                {
                    var result = value;
                    if (_parentTransform != null)
                    {
                        result = value - _parentTransform.Rotation;
                    }
                    LocalRotation = result;
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
            UpdateAbsolutes();
            Changed?.Invoke(this, EventArgs.Empty);
        }

        private void OnParentChanged(object sender, EventArgs args)
        {
            _dirty = true;
            UpdateAbsolutes();
        }

        private void UpdateAbsolutes()
        {
            _dirty = false;
            var world = Matrix.Identity
                     * Matrix.CreateTranslation(-_localScaleOrigin)
                     * Matrix.CreateScale(_localScale)
                     * Matrix.CreateTranslation(_localScaleOrigin)

                     * Matrix.CreateTranslation(-_localRotationOrigin)
                     * Matrix.CreateRotationX(MathHelper.ToRadians(_localRotation.Y))
                     * Matrix.CreateRotationY(MathHelper.ToRadians(_localRotation.X))
                     //* Matrix.CreateFromAxisAngle(Vector3.UnitX, MathHelper.ToRadians(_localRotation.X))
                     //* Matrix.CreateFromAxisAngle(Vector3.UnitY, MathHelper.ToRadians(_localRotation.Y))
                     //* Matrix.CreateFromAxisAngle(Vector3.UnitZ, MathHelper.ToRadians(_localRotation.Z))
                     * Matrix.CreateTranslation(_localRotationOrigin)
                     
                     * Matrix.CreateTranslation(_localPosition);

            if (ParentTransform != null)
                world *= ParentTransform.World;

            _world = world;
        }
    }
}