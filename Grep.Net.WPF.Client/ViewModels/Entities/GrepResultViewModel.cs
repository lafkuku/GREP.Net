using System;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using Caliburn.Micro;
using Grep.Net.Entities;
using Grep.Net.WPF.Client.Interfaces;

namespace Grep.Net.WPF.Client.ViewModels.Entities
{
    public class GrepResultViewModel : PropertyChangedBase, IViewModel<GrepResult>
    {
        private GrepResult _grepResult { get; set; }

        public GrepResult Entity
        {
            get
            {
                return _grepResult;
            }
            set
            {
                _grepResult = value;
                NotifyOfPropertyChange(() => Entity);
            }
        }
   
        public MatchInfo SelectedMatchInfo { get; set; }

        public ListCollectionView MatchInfos
        {
            get
            {
                var lcv = new ListCollectionView(_grepResult.MatchInfos as System.Collections.IList);
                return lcv;
            }
        }

        public void OnItemClick(object dataContext)
        {
        }

        public GrepResultViewModel()
        {
            
        }

        public void SortClick(ActionExecutionContext context)
        {
            Button b = context.Source as Button;

            ListCollectionView lcv = (context.View as ListView).ItemsSource as ListCollectionView;
            if (b == null || lcv == null)
            {
                //TODO: Log + Error
                return;
            }

            switch (b.Content.ToString())
            {
                case "File":
                    lcv.SortDescriptions.Clear();
                    lcv.SortDescriptions.Add(new System.ComponentModel.SortDescription("FileInfo.Name", System.ComponentModel.ListSortDirection.Ascending));
                    break;
                case "Line#":
                    lcv.SortDescriptions.Clear();
                    lcv.SortDescriptions.Add(new System.ComponentModel.SortDescription("LineNumber", System.ComponentModel.ListSortDirection.Ascending));
                    break;
                case "Pattern":
                    lcv.SortDescriptions.Clear();
                    lcv.SortDescriptions.Add(new System.ComponentModel.SortDescription("Pattern.PatternStr", System.ComponentModel.ListSortDirection.Ascending));
                    break;
                case "Context":

                    lcv.SortDescriptions.Clear();
                    lcv.SortDescriptions.Add(new System.ComponentModel.SortDescription("Line", System.ComponentModel.ListSortDirection.Ascending));
                    break;
                case "Fulle Path":

                    lcv.SortDescriptions.Clear();
                    lcv.SortDescriptions.Add(new System.ComponentModel.SortDescription("FileInfo.FullName", System.ComponentModel.ListSortDirection.Ascending));
                    break;
                default:
                    break;
            }
            return;
        }
    }
}