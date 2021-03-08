using System;
using System.Collections.Generic;

namespace RocketUI
{
    public class ScrollableMultiStackContainer : ScrollableStackContainer
    {
        private List<StackContainer> _rows = new List<StackContainer>();

        private readonly Action<StackContainer> _defaultRowBuilder;

        public ScrollableMultiStackContainer(Action<StackContainer> defaultRowBuilder = null)
        {
            _defaultRowBuilder = defaultRowBuilder;
        }

        public ScrollableMultiStackContainer()
        {
            
        }

        public void AddChild(int row, IGuiElement element)
        {
            EnsureRows(row + 1);

            _rows[row].AddChild(element);
        }

        private void EnsureRows(int count)
        {
            if (_rows.Count < count)
            {
                for (int i = 0; i < (count - _rows.Count); i++)
                {
                    AddRow();
                }
            }
        }

        public StackContainer AddRow(Action<StackContainer> rowBuilder = null)
        {
            var stack = new StackContainer()
            {
                Orientation = Orientation == Orientation.Horizontal ? Orientation.Vertical : Orientation.Horizontal,
                ChildAnchor = ChildAnchor.SwapXy()
            };

            _defaultRowBuilder?.Invoke(stack);
            rowBuilder?.Invoke(stack);

            _rows.Add(stack);
            AddChild(stack);
            return stack;
		}

	    public StackContainer AddRow(params RocketElement[] elements)
	    {
		    return AddRow(row =>
		    {
			    foreach (var element in elements)
			    {
				    row.AddChild(element);
			    }
		    });
	    }
	}

    public class MultiStackContainer : StackContainer
    {
        private List<StackContainer> _rows = new List<StackContainer>();

        private readonly Action<StackContainer> _defaultRowBuilder;

        public MultiStackContainer(Action<StackContainer> defaultRowBuilder = null)
        {
            _defaultRowBuilder = defaultRowBuilder;
        }

        public MultiStackContainer()
        {
            
        }

        public void AddChild(int row, IGuiElement element)
        {
            EnsureRows(row + 1);

            _rows[row].AddChild(element);
        }

        private void EnsureRows(int count)
        {
            if (_rows.Count < count)
            {
                for (int i = 0; i < (count - _rows.Count); i++)
                {
                    AddRow();
                }
            }
        }

        public StackContainer AddRow(Action<StackContainer> rowBuilder = null)
        {
            var stack = new StackContainer()
            {
                Orientation = Orientation == Orientation.Horizontal ? Orientation.Vertical : Orientation.Horizontal,
                Anchor = ChildAnchor,
                ChildAnchor = ChildAnchor.SwapXy()
            };

            _defaultRowBuilder?.Invoke(stack);
            rowBuilder?.Invoke(stack);

            _rows.Add(stack);
            AddChild(stack);
            return stack;
        }

        public StackContainer AddRow(params RocketElement[] elements)
        {
            return AddRow(row =>
            {
                foreach (var element in elements)
                {
                    row.AddChild(element);
                }
            });
        }
    }
}
