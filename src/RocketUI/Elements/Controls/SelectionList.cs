using System;

namespace RocketUI
{
    public class SelectionList : ScrollableStackContainer
    {
        public event EventHandler<SelectionListItem> SelectedItemChanged;

        private SelectionListItem _selectedItem;

        public SelectionList()
        {
            
        }
        
        public SelectionListItem SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (_selectedItem == value) return;
                _selectedItem = value;

                SelectedItemChanged?.Invoke(this, _selectedItem);
            }
        }

        public void UnsetSelectedItem(SelectionListItem selected)
        {
            if (SelectedItem == selected)
            {
                SelectedItem = null;
            }
        }

        public void SetSelectedItem(SelectionListItem selected)
        {
            SelectedItem = selected;
        }

        protected override void OnChildAdded(IGuiElement element)
        {
            base.OnChildAdded(element);
            if (element is SelectionListItem listItem)
            {
                listItem.List = this;
            }
        }

        protected override void OnChildRemoved(IGuiElement element)
        {
            base.OnChildRemoved(element);
            if (element is SelectionListItem listItem)
            {
                listItem.List = null;

                if (SelectedItem == listItem)
                {
                    SelectedItem = null;
                }
            }
        }
    }
}
