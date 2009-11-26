using System;
using System.Collections.ObjectModel;
using System.Net;
using System.Collections;
using System.Collections.Generic;

namespace APD.Client.Widget.Admin.Utils
{
    public static class ListExtensionMethods
    { 
        public static ObservableCollection<T> ToObservableList<T>(this IEnumerable<T> list)
        {
            var retValue = new ObservableCollection<T>();
            foreach (var item in list)
                retValue.Add(item);

            return retValue;
        }
    }
}
