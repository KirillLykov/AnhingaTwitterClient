using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Anhinga.Command;
using System.Windows.Controls;
using System.Windows.Input;
using System.Collections.ObjectModel;
using TweetSharp.Twitter.Model;
using Anhinga.Model;

namespace Anhinga.ViewModel
{
    public class AdvancedSearchViewModel : TweetsBaseViewModel
    {
        #region TweetsBaseViewModel interface
        ObservableCollection<TwitterStatus> _tweets;
        override public ObservableCollection<TwitterStatus> Tweets
        {
            get { return _tweets; }
        }

        public override string Name { get { return "Ad.S"; } }

        override public void LoadTweets()
        {
            if (!SearchParams.AnyWords.Equals(""))
                Model.TweetSharpUtilities.AdvancedSearch(SearchParams, ref _tweets);
        }
        #endregion

        public AdvancedSearchParam SearchParams
        { get; set; }

        public AdvancedSearchViewModel()
        {
            _tweets = new ObservableCollection<TwitterStatus>();
            SearchParams = new AdvancedSearchParam();
        }

        #region Search command
        RelayCommand _searchCommand;
        public ICommand SearchCommand
        {
            get
            {
                if (_searchCommand == null)
                {
                    _searchCommand = new RelayCommand(() => this.LoadTweets());
                }
                return _searchCommand;
            }
        }
        #endregion
    }
}
