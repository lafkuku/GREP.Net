using System;
using System.Linq;

namespace Grep.Net.WPF.Client.Interfaces
{
    internal interface IReloadable : IFileEntity
    {
        void Reload(); 
    }
}