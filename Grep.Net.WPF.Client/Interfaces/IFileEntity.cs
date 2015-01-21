using System;
using System.Linq;

namespace Grep.Net.WPF.Client.Interfaces
{
    public interface IFileEntity
    {
        void Save(); 

        void Delete();
    }
}