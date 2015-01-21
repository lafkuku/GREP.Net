using System;
using System.ComponentModel;
using Caliburn.Micro;
using Grep.Net.Entities;
using Grep.Net.WPF.Client.ViewModels.Entities;
using Grep.Net.WPF.Client.Interfaces;

namespace Grep.Net.WPF.Client.ViewModels.Entities
{
    public class MatchInfoViewModel : PropertyChangedBase, IViewModel<MatchInfo>
    {
        private MatchInfo _matchInfo;
        public  MatchInfo Entity{
            get { return _matchInfo; }
            set {
                _matchInfo = value;
                NotifyOfPropertyChange(()=> Entity); 
            }
        }
             
             
        public MatchInfoViewModel()
        {
        }
           
        public MatchInfoViewModel(MatchInfo matchInfo)
        {
            _matchInfo = matchInfo;
        }
          
      
        public string Line
        {
            get { return _matchInfo.Line; }
            set {
                _matchInfo.Line = value;
                NotifyOfPropertyChange(()=> Line); 
            }
        }                               
        public string Context
        {
            get { return _matchInfo.Context; }
            set {
                _matchInfo.Context = value;
                NotifyOfPropertyChange(()=> Context); 
            }
        }                               
        public int LineNumber
        {
            get { return _matchInfo.LineNumber; }
            set {
                _matchInfo.LineNumber = value;
                NotifyOfPropertyChange(()=> LineNumber); 
            }
        }                               
        public bool Reviewed
        {
            get { return _matchInfo.Reviewed; }
            set {
                _matchInfo.Reviewed = value;
                NotifyOfPropertyChange(()=> Reviewed); 
            }
        }                               
        public string Match
        {
            get { return _matchInfo.Match; }
            set {
                _matchInfo.Match = value;
                NotifyOfPropertyChange(()=> Match); 
            }
        }    
     
        public Guid GrepResultId
        {
            get { return _matchInfo.GrepResultId; }
            set
            {
                _matchInfo.GrepResultId = value;
                NotifyOfPropertyChange(() => GrepResultId);
            }
        }
             
        public Grep.Net.Entities.FileInfo FileInfo
        {
            get { return _matchInfo.FileInfo; }
            set {
                _matchInfo.FileInfo = value;
                NotifyOfPropertyChange(()=> FileInfo); 
            }
        }                               
        public Grep.Net.Entities.Pattern Pattern
        {
            get { return _matchInfo.Pattern; }
            set {
                _matchInfo.Pattern = value;
                NotifyOfPropertyChange(()=> Pattern); 
            }
        }                               
 }
}



