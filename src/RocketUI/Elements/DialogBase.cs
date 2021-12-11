using RocketUI.Layout;

namespace RocketUI
{
    public abstract class DialogBase : Screen
    {
        /// <summary>
        ///     If true, a user won't be able to interact with any other UI components outside of this dialog until it has been closed.
        /// </summary>
        public bool AlwaysInFront { get; set; } = true;

        public bool ShowBackdrop
        {
            get => _showBackdrop;
            set
            {
                _showBackdrop = value;
            }
        }

        protected Container ContentContainer;
        private bool _showBackdrop;
        protected DialogBase()
        {
            AddChild(ContentContainer = new Container()
            {
                Anchor = Alignment.MiddleCenter
            });

            ZIndex = int.MaxValue;
        }

        /// <summary>
        ///     Opens the Dialog
        /// </summary>
        public void Show()
        {
            GuiManager?.ShowDialog(this);
        }
        
        /// <summary>
        ///     Attempts to close the dialog
        /// </summary>
        public void Close()
        {
            GuiManager?.HideDialog(this);
        }

        public virtual void OnShow()
        {
            
        }
        
        public virtual void OnClose()
        {
            
        }
    }
}
