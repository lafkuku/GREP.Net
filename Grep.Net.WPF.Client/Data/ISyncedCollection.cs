using System;
using System.Linq;

namespace Grep.Net.WPF.Client.Data
{
    public interface ISyncedCollection
    {
        Object GetSyncedItemFromSource(object sourceItem); 
    }
}