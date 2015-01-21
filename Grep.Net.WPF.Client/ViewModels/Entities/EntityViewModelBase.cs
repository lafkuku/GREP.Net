using System;
using System.Linq;
using Caliburn.Micro;
using Grep.Net.Entities;

namespace Grep.Net.WPF.Client.ViewModels.Entities
{
    public class EntityViewModelBase<T> : PropertyChangedBase where T : IEntity
    {
        private T Model { get; set; }
    }
}