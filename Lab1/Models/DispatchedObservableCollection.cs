using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Lab1.Models
{
    public class DispatchedObservableCollection<T> : ObservableCollection<T>
    {
        public DispatchedObservableCollection() : base()
        {
        }

        public DispatchedObservableCollection(IOrderedEnumerable<T> collection) : base(collection)
        {
        }

        public override event NotifyCollectionChangedEventHandler? CollectionChanged;

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            var collectionChanged = CollectionChanged;

            if (collectionChanged == null) return;

            Dispatcher? dispatcher = (
                from NotifyCollectionChangedEventHandler handler
                in collectionChanged.GetInvocationList()
                let dispatcherOb = handler.Target as DispatcherObject
                where dispatcherOb != null
                select dispatcherOb.Dispatcher).FirstOrDefault();

            if (dispatcher != null && dispatcher.CheckAccess() == false)
            {
                dispatcher.Invoke(DispatcherPriority.DataBind, () => OnCollectionChanged(e));
            }
            else
            {
                foreach (NotifyCollectionChangedEventHandler handler in collectionChanged.GetInvocationList())
                {
                    handler.Invoke(this, e);
                }
            }
        }
    }
}
