using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;

namespace Grep.Net.WPF.Client.ViewModels
{
    public class UniquifyViewModel : PropertyChangedBase
    {
        private string _text;

        public String Text
        {
            get
            {
                return _text;
            }
            set
            {
                _text = value;
                NotifyOfPropertyChange(() => Text);
            }
        }

        public UniquifyViewModel()
        {

        }
    }
}
