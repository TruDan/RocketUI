using System.Collections.ObjectModel;

namespace RocketUI
{
    public class ItemsControl<TItem> : ValuedControl<TItem>
    {
        public ObservableCollection<TItem> Items { get; set; }

        public ItemsControl()
        {
            
        } 
    }
}