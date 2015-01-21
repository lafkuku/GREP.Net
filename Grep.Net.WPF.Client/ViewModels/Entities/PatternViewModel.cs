using System;
using System.Linq;
using Caliburn.Micro;
using Grep.Net.Entities;
using Grep.Net.WPF.Client.Interfaces;

namespace Grep.Net.WPF.Client.ViewModels.Entities
{
    public class PatternViewModel : PropertyChangedBase, ISelectable
    {
        public Pattern _pattern;

        public Pattern Pattern
        { 
            get
            {
                return _pattern;
            }
            set 
            { 
                _pattern = value;
                NotifyOfPropertyChange(() => Pattern);
                NotifyOfPropertyChange(() => PatternStr);
                NotifyOfPropertyChange(() => ReferenceUrl);
                NotifyOfPropertyChange(() => Recommendation);
                NotifyOfPropertyChange(() => PatternInfo);
            }
        }

        public String PatternStr
        {
            get
            {
                return Pattern.PatternStr;
            }
            set
            {
                Pattern.PatternStr = value;
                NotifyOfPropertyChange(() => this.PatternStr);
            }
        }

        public String ReferenceUrl
        {
            get
            {
                return Pattern.ReferenceUrl;
            }
            set
            {
                Pattern.ReferenceUrl = value;
                NotifyOfPropertyChange(() => this.ReferenceUrl);
            }
        }

        public String Recommendation
        {
            get
            {
                return Pattern.Recommendation;
            }
            set
            {
                Pattern.Recommendation = value;
                NotifyOfPropertyChange(() => this.Recommendation);
            }
        }

        public String PatternInfo
        {
            get
            {
                return Pattern.PatternInfo;
            }
            set
            {
                Pattern.PatternInfo = value;
                NotifyOfPropertyChange(() => this.PatternInfo);
            }
        }

        public bool IsEnabled
        {
            get
            {
                return Pattern.IsEnabled;
            }
            set
            {
                Pattern.IsEnabled = value;
                NotifyOfPropertyChange(() => this.IsEnabled);
            }
        }

        private bool _isSelected;

        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                _isSelected = value;
                NotifyOfPropertyChange(() => IsSelected);
            }
        }
  
        public PatternViewModel()
        {
            Pattern = new Pattern();
        }
    }
}