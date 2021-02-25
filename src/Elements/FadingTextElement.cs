using System;
using Microsoft.Xna.Framework;
using RocketUI.Utilities.Helpers;

namespace RocketUI
{
    public class FadingTextElement : TextElement
    {
        private TimeSpan _displayTime = TimeSpan.FromSeconds(5);

        public TimeSpan DisplayTime
        {
            get
            {
                return _displayTime;
            }
            set
            {
                _displayTime = value;
            }
        }

        public FadingTextElement()
        {
            
        }

        private DateTime _fadeEndTime = DateTime.MinValue;
        protected override void OnTextUpdated()
        {
            base.OnTextUpdated();

            _fadeEndTime = DateTime.UtcNow + _displayTime;
        }

        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);

            if (_fadeEndTime <= DateTime.UtcNow)
                return;
            
            var value = (float)((1f / _displayTime.TotalMilliseconds) * (_fadeEndTime - DateTime.UtcNow).TotalMilliseconds);
            TextOpacity = MathHelpers.Clamp(value, 0f, 1f);
        }
    }
}