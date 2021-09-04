using System;
using System.Numerics;
using System.Runtime.Serialization;
using RocketUI.Utilities.Extensions;

namespace RocketUI
{
    [DataContract]
    public class Transform3D
    {
        public event EventHandler Changed;

        private Vector3     _localScale    = Vector3.One;
        private Vector3     _localPosition = Vector3.Zero;
        private Quaternion  _localRotation = Quaternion.Identity;
        private Matrix4x4   _world         = Matrix4x4.Identity;
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
        public virtual Quaternion LocalRotation
        {
            get => _localRotation;
            set
            {
                if (_localRotation == value)
                    return;
                _localRotation = value;
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
                    LocalPosition = Vector3.Transform(value,ParentTransform.World.Invert());
                }
            }
        }

        public virtual Quaternion Rotation
        {
            get
            {
                if (_dirty)
                    UpdateAbsolutes();
                if (_parentTransform != null)
                    return Quaternion.Multiply(_localRotation, _parentTransform.Rotation);
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
                        result *= Quaternion.Inverse(_parentTransform.Rotation);
                    }

                    LocalRotation = result;
                }
            }
        }

        public virtual Matrix4x4 World
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
            _world = Matrix4x4.Identity
                     * Matrix4x4.CreateScale(_localScale)
                     * Matrix4x4.CreateFromQuaternion(_localRotation)
                     * Matrix4x4.CreateTranslation(_localPosition);

            if (ParentTransform != null)
                _world *= ParentTransform.World;
        }
    }
}