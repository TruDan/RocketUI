using System;

namespace RocketUI
{
    public class StackMenuItem : Button
    {
        public StackMenuItem(string text, Action action, bool isTranslationKey = false) : base(text, action, isTranslationKey)
        {
        }
        
        public StackMenuItem(){}
    }
}
