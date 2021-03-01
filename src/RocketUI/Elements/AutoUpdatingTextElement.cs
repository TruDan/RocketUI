using System;
using Microsoft.Xna.Framework;
using Portable.Xaml.Markup;

namespace RocketUI
{
    [ContentProperty(nameof(Text))]
    public class AutoUpdatingTextElement : TextElement
    {
        private readonly Func<string> _updateFunc;
        public           TimeSpan     Interval { get; set; } = TimeSpan.Zero;
        
        public AutoUpdatingTextElement(Func<string> updateFunc, bool hasBackground = false) : base(hasBackground)
        {
            _updateFunc = updateFunc;
            Text        = _updateFunc();
            _nextUpdate = TimeSpan.Zero;
        }

        public AutoUpdatingTextElement()
        {
            
        }

        private TimeSpan _nextUpdate;
        protected override void OnUpdate(GameTime gameTime)
        {
            if (gameTime.TotalGameTime > _nextUpdate)
            {
                Text = _updateFunc();
                _nextUpdate = gameTime.TotalGameTime + Interval;
            }
        }
    }
}