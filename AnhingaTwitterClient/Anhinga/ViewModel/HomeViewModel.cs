using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TweetSharp.Twitter.Model;
using System.Collections.ObjectModel;
using TweetSharp.Twitter.Fluent;
using TweetSharp.Twitter.Extensions;
using Anhinga.Properties;

namespace Anhinga.ViewModel
{
    class HomeViewModel : TweetsBaseViewModel
    {
        override public ObservableCollection<TwitterStatus> Tweets
        {
            get; set;
        }

        string _name = "Home";

        override public string Name
        {
            get { return _name; }
        }

        public HomeViewModel()
        {
            Tweets = new ObservableCollection<TwitterStatus>();
        }

        override public void LoadTweets()
        {
            Tweets.Clear();
            TwitterResult response = FluentTwitter
             .CreateRequest()
             .AuthenticateWith(
                             Settings.Default.ConsumerKey,
                             Settings.Default.ConsumerSecret,
                             Model.OAuthHandler.Token,
                             Model.OAuthHandler.TokenSecret)
             .Statuses()
             .OnHomeTimeline().AsJson().Request();

            var statuses = response.AsStatuses();
            foreach (TwitterStatus status in statuses)
            {
                Tweets.Add(status);
            }
        }
    }
}
