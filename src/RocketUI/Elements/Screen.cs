using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;

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

        public new GuiManager GuiManager { get; set; }

        public int ZIndex { get; set; } = 0;
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
        //public  GuiManager GuiManager            { get; internal set; }
        public  bool       IsSelfUpdating         { get; set; }
        public  bool       IsSelfDrawing         { get; set; }
        public  bool       IsSelfManaged
        {
            get => IsSelfDrawing && IsSelfUpdating;
            set
            {
                IsSelfUpdating = value;
                IsSelfDrawing = value;
            }
        }

        public  bool       IsAutomaticallyScaled { get; set; } = true;
        public  bool       SizeToWindow { get; set; } = true;

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
            }//);
        }

        protected override void OnUpdate(GameTime gameTime)
        {
            if (IsLayoutDirty && !IsLayoutInProgress)
            {
                UpdateLayout();
            }

            base.OnUpdate(gameTime);
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
        
    }
}
