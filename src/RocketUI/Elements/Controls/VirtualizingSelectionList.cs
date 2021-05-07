using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace RocketUI
{
    public class VirtualizingSelectionList : SelectionList
    {
        private ICollection<SelectionListItem> _items;
        private int                            _itemHeight = 50;

        public ICollection<SelectionListItem> Items
        {
            get => _items;
            set { _items = value; }
        }

        public int ItemHeight
        {
            get => _itemHeight;
            set => _itemHeight = value;
        }

        public VirtualizingSelectionList()
        {
        }

        protected override Size MeasureChildrenCore(Size availableSize, IReadOnlyCollection<RocketElement> children)
        {
            return new Size(availableSize.Width, Items.Count * ItemHeight);
        }

        protected override void ArrangeChildrenCore(Rectangle finalRect, IReadOnlyCollection<RocketElement> children)
        {
            //var contentRect = new Rectangle(finalRect.Location  - new Thickness((int) ScrollOffset.X, (int)ScrollOffset.Y, 0, 0), finalRect.Size + new Thickness((int)ScrollOffset.X, (int)ScrollOffset.Y, 0, 0));
            var contentRect = new Rectangle(finalRect.Location, Size.Max(finalRect.Size, ContentSize));
            var offset      = new Vector2(ScrollOffset.X, ScrollOffset.Y % ItemHeight);
            contentRect.Offset(-offset);

            base.ArrangeChildrenCore(contentRect, children);
			
            if (HorizontalScrollBar != null)
            {
                PositionChild(HorizontalScrollBar, Alignment.BottomFill, finalRect, Thickness.Zero,
                    Thickness.Zero,              true);
            }

            if (VerticalScrollBar != null)
            {
                PositionChild(VerticalScrollBar, Alignment.FillRight, finalRect, Thickness.Zero,
                    Thickness.Zero,            true);
            }

            //ForEachChild(c => ((GuiElement)c).RenderTransform = Matrix.CreateTranslation(-ScrollOffset.X, -ScrollOffset.Y, 0));
        }

        protected override void OnScrollOffsetChanged(Vector2 oldValue, Vector2 newValue) 
        {
            base.OnScrollOffsetChanged(oldValue, newValue);

            var minIndex = (int)Math.Floor(newValue.Y / ItemHeight);
            var nVisible =(int) Math.Ceiling(Size.Height / (double)ItemHeight);
            var maxIndex = minIndex + nVisible;

            if (maxIndex > _items.Count)
            {
                maxIndex = Items.Count;
                minIndex = maxIndex - nVisible;
            }
            
            Children.Clear();
            foreach (var item in _items.Skip(minIndex).Take(nVisible))
            {
                Children.Add(item);
            }
        }
    }
}