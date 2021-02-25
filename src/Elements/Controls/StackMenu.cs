using System;

namespace RocketUI
{
    public class StackMenu : ScrollableStackContainer
    {
        public StackMenu()
        {
        }

        public void AddMenuItem(string label, Action action, bool enabled = true, bool isTranslationKey = false)
        {
            AddChild(new StackMenuItem(label, action, isTranslationKey)
            {
				Enabled = enabled
			});
        }

	    public void AddSpacer()
	    {
		    AddChild(new StackMenuSpacer());
	    }
	}
}
