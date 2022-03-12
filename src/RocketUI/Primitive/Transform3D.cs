﻿using System;
using System.Runtime.Serialization;
using System.Numerics;
using RocketUI.Utilities.Helpers;

namespace RocketUI
{
    [DataContract]
    public class Transform3D
    {
        public event EventHandler Changed;

        private Vector3     _scale          = Vector3.One;
        private Vector3     _scaleOrigin    = Vector3.Zero;
        private Vector3     _position       = Vector3.Zero;
        private Vector3     _rotationOrigin = Vector3.Zero;
        private Vector3     _rotation       = Vector3.Zero;
        private Matrix4x4      _world               = Matrix4x4.Identity;
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
                
                OnChanged();
            }
        }

        
        [DataMember]
        public virtual Vector3 Scale
        {
            get => _scale;
            set
            {
                if (_scale == value)
                    return;
                _scale = value;
                OnChanged();
            }
        }
        
        [DataMember]
        public virtual Vector3 ScaleOrigin
        {
            get => _scaleOrigin;
            set
            {
                if (_scaleOrigin == value)
                    return;
                _scaleOrigin = value;
                OnChanged();
            }
        }

        [DataMember]
        public virtual Vector3 Position
        {
            get => _position;
            set
            {
                if (_position == value)
                    return;
                _position = value;
                OnChanged();
            }
        }

        [DataMember]
        public virtual Vector3 RotationOrigin 
        {
            get => _rotationOrigin;
            set
            {
                if (_rotationOrigin == value)
                    return;
                _rotationOrigin = value;
                OnChanged();
            }
        }
       
        [DataMember]
        public virtual Vector3 Rotation
        {
            get => _rotation;
            set
            {
                if (_rotation == value)
                    return;
                _rotation = new Vector3(value.X % 360.0f, value.Y % 360.0f, value.Z % 360.0f);
                OnChanged();
            }
        }


        public virtual Vector3 WorldPosition => World.Translation;

        public virtual Vector3 WorldScale
        {
            get
            {
                Matrix4x4.Decompose(_world, out var scale, out _, out _);
                return scale;
            }
        }
        
        public virtual Quaternion WorldRotation
        {
            get
            {
                Matrix4x4.Decompose(_world, out _, out var rotation, out _);
                return rotation;
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
        }

        private void OnParentChanged(object sender, EventArgs args)
        {
            _dirty = true;
            UpdateAbsolutes();
        }

        private void UpdateAbsolutes()
        {
            _dirty = false;
            var world = Matrix4x4.Identity
                     * Matrix4x4.CreateTranslation(-_scaleOrigin)
                     * Matrix4x4.CreateScale(_scale)
                     * Matrix4x4.CreateTranslation(_scaleOrigin)

                     * Matrix4x4.CreateTranslation(-_rotationOrigin)
                     * Matrix4x4.CreateRotationX(_rotation.Y.ToRadians())
                     * Matrix4x4.CreateRotationY(_rotation.X.ToRadians())
                     //* Matrix4x4.CreateFromAxisAngle(Vector3.UnitX, MathHelper.ToRadians(_localRotation.X))
                     //* Matrix4x4.CreateFromAxisAngle(Vector3.UnitY, MathHelper.ToRadians(_localRotation.Y))
                     //* Matrix4x4.CreateFromAxisAngle(Vector3.UnitZ, MathHelper.ToRadians(_localRotation.Z))
                     * Matrix4x4.CreateTranslation(_rotationOrigin)
                     
                     * Matrix4x4.CreateTranslation(_position);

            if (ParentTransform != null)
            {
                _world = world * ParentTransform.World;
            }
            else
            {
                _world = world;
            }
            
            Changed?.Invoke(this, EventArgs.Empty);
        }
    }
}