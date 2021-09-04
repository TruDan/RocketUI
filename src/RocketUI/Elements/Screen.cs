using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Drawing;
using System.Numerics;
using Newtonsoft.Json;
using RocketUI.Utilities.Extensions;

namespace RocketUI
{
    public class Screen : RocketElement, IGuiScreen
    {
        public bool IsLayoutInProgress { get; protected set; } = false;

        [JsonIgnore]
        public override IGuiFocusContext FocusContext
        {
            get { return this; }
        }

        [JsonIgnore]
        public override IGuiScreen RootScreen
        {
            get => ParentElement?.RootScreen ?? this;
        }

        public IGuiControl FocusedControl { get; private set; }


        public Screen()
        {
            // AutoSizeMode = AutoSizeMode.None;
            // Anchor = Alignment.Fixed;
            ClipToBounds = true;
        }

        public void UpdateSize(int width, int height)
        {
            SetFixedSize(width, height);
            InvalidateLayout(true);
        }

        private object     _updateLock = new object();
        public  GuiManager GuiManager     { get; internal set; }
        public  bool       IsSelfUpdating { get; set; }
        public  bool       IsSelfDrawing  { get; set; }

        public bool IsSelfManaged
        {
            get => IsSelfDrawing && IsSelfUpdating;
            set
            {
                IsSelfUpdating = true;
                IsSelfDrawing = true;
            }
        }

        public bool IsAutomaticallyScaled { get; set; } = true;
        public bool SizeToWindow          { get; set; } = true;

        public void UpdateLayout()
        {
            if (!IsLayoutDirty || IsLayoutInProgress) return;
            IsLayoutInProgress = true;

            // ThreadPool.QueueUserWorkItem(o =>
            {
                // Pass 1 - Update the Preferred size for all elements with
                //          fixed sizes
                DoLayoutSizing();

                // Pass 2 - Update the actual sizes for all children based upon their
                //          parent sizes.
                BeginLayoutMeasure();
                Measure(new Size(Width, Height));

                // Pass 3 - Arrange all child elements based on the LayoutManager for
                //          the current element.
                BeginLayoutArrange();
                Arrange(new Rectangle(Point.Zero, new Size(Width, Height)));

                OnUpdateLayout();

                IsLayoutDirty = false;
                IsLayoutInProgress = false;
            } //);
        }

        protected override void OnUpdate()
        {
            if (IsLayoutDirty && !IsLayoutInProgress)
            {
                UpdateLayout();
            }

            base.OnUpdate();
        }

        public bool Focus(IGuiControl control)
        {
            FocusedControl?.InvokeFocusDeactivate();
            FocusedControl = control;
            FocusedControl?.InvokeFocusActivate();
            return true;
        }

        public void ClearFocus(IGuiControl control)
        {
            if (FocusedControl == control)
            {
                FocusedControl?.InvokeFocusDeactivate();
                FocusedControl = null;
            }
        }

        public void HandleContextActive()
        {
        }

        public void HandleContextInactive()
        {
        }


        public Vector2? Unproject(Ray ray)
        {
            Transform3D transform = new Transform3D();
            if (Tag is ITransformable transformable)
            {
                transform = transformable.Transform;
            }

            var normal = Vector3.Normalize(Vector3.Transform(Vector3.UnitZ, transform.Rotation));
            var plane = new Plane(transform.LocalPosition, -(
                transform.LocalPosition.X * normal.X +
                transform.LocalPosition.Y * normal.Y +
                transform.LocalPosition.Z * normal.Z
            ));
            var intersection = ray.Intersects(plane);
            if (intersection.HasValue)
            {
                // find intersectionpoint
                var intersectionPoint = ray.Position + (ray.Direction * intersection.Value);

                // unproject
                var cursorPos = Vector3.Transform(intersectionPoint, transform.World.Invert());
                return new Vector2(cursorPos.X, cursorPos.Y);
            }

            return null;
        }
    }
}