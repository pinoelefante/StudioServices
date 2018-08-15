using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

public class MyObservableCollection<T> : ObservableCollection<T>
{
    public void AddRange(IEnumerable<T> list, bool clearBefore = false)
    {
        if (clearBefore)
            this.Clear();

        if (list != null && list.Any())
        {
            foreach (var item in list)
                this.Add(item);
        }
    }
    public new void Remove(T o)
    {
        Device.BeginInvokeOnMainThread(() =>
        {
            base.Remove(o);
        });
        
    }
}

