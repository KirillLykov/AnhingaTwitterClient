using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using TweetSharp.Twitter.Model;
using TweetSharp.Twitter.Fluent;
using TweetSharp.Twitter.Extensions;
using Anhinga.Properties;
using Anhinga.Model;
using Anhinga.Command;
using System.Windows.Input;
using System.Windows.Controls;

namespace Anhinga.ViewModel
{
    public class SearchTweetsViewModel : TweetsBaseViewModel
    {
        ObservableCollection<TwitterStatus> _tweets;
        override public ObservableCollection<TwitterStatus> Tweets
        {
            get { return _tweets; }
        }

        //TODO do you realy need this property?
        public TwitterUser User 
        {
            get; set;
        }

        public string HashTag
        {
            get; set;
        }

        private string _searchPhrase = "";

        override public string Name
        {
            get { return "ST"; }
        }

        public SearchTweetsViewModel()
        {
            _tweets = new ObservableCollection<TwitterStatus>();
            HashTag = "";
        }

        //TODO refactoring extract subclasses or introduce strategy
        override public void LoadTweets()
        {
            _tweets.Clear();
            if (User != null)
            {
                TweetSharpUtilities.GetUsersTweets(User, ref _tweets);
                User = null;
            }
            else if (_searchPhrase.Length != 0)
            {
                TweetSharpUtilities.SearchTweets(_searchPhrase, ref _tweets);
                _searchPhrase = "";
            }
            else if (HashTag.Length != 0)
            {
                TweetSharpUtilities.SearchByTag(HashTag, ref _tweets);
                HashTag = "";
            }
        }

        #region Search Phrase command
        RelayCommand<TextBox> _searchPhraseCommand;
        public ICommand SearchPhraseCommand
        {
            get
            {
                if (_searchPhraseCommand == null)
                {
                    _searchPhraseCommand = new RelayCommand<TextBox>(param => this.SearchPhrase(param));
                }
                return _searchPhraseCommand;
            }
        }

        public void SearchPhrase(TextBox tb)
        {
            try
            {
                _searchPhrase = tb.Text;
                Load();
                tb.Text = "";
            }
            catch
            {
                //HandleError(err);
            }
        }
        #endregion
    }
}
