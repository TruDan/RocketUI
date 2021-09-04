using System;
using System.Diagnostics;
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
            _sw = Stopwatch.StartNew();
        }

        public AutoUpdatingTextElement()
        {
            
        }

        private Stopwatch _sw;
        protected override void OnUpdate()
        {
            if(_sw.Elapsed > Interval)
            {
                Text = _updateFunc();
                _sw.Restart();
            }
        }
    }
}