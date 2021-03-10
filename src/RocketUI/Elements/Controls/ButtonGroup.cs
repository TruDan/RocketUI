// /*
//  * Alex.API
//  *
//  * Copyright (c)  Dan Spiteri
//  *
//  * /

using System;

namespace RocketUI
{
	public class ButtonGroup : StackContainer
	{
		private IValuedControl<bool>                   _checkedControl;
		public event EventHandler<GuiElementEventArgs> CheckedControlChanged;

		public IValuedControl<bool> CheckedControl
		{
			get => _checkedControl;
			private set
			{
				_checkedControl = value;
				CheckedControlChanged?.Invoke(this, new GuiElementEventArgs(_checkedControl));
			}
		}

		protected override void OnChildAdded(IGuiElement element)
		{
			if (element is IValuedControl<bool> toggleButton)
			{
				if (toggleButton.Value)
				{

					if(CheckedControl != null && toggleButton != CheckedControl)
					{
						toggleButton.Value = false;
					}
					else if (CheckedControl == null)
					{
						CheckedControl = toggleButton;
					}
				}

				toggleButton.ValueChanged += ToggleButtonOnValueChanged;
			}

			base.OnChildAdded(element);
		}

		private void ToggleButtonOnValueChanged(object sender, bool value)
		{
			if (sender is IValuedControl<bool> senderControl)
			{
				if (value)
				{
					CheckedControl = senderControl;

					ForEachChild(e =>
					{
						if (e == sender) return;
						if (e is IValuedControl<bool> boolControl)
						{
							boolControl.Value = false;
						}
					});
				}
				else
				{
					if (CheckedControl == senderControl)
					{
						CheckedControl = null;
					}
				}
			}
		}

		protected override void OnChildRemoved(IGuiElement element)
		{
			if (element is IValuedControl<bool> toggleButton)
			{
				toggleButton.ValueChanged -= ToggleButtonOnValueChanged;
				if (toggleButton.Value && CheckedControl == toggleButton)
				{
					CheckedControl = null;
					toggleButton.Value = false;
				}
			}

			base.OnChildRemoved(element);
		}

		public  ButtonGroup()
		{
		}
	}
}