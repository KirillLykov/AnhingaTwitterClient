﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using TweetSharp.Twitter.Model;
using TweetSharp.Twitter.Fluent;
using TweetSharp.Twitter.Extensions;
using Anhinga.Properties;

namespace Anhinga.ViewModel
{
    public class RepliesViewModel : TweetsBaseViewModel
    {
        override public ObservableCollection<TwitterStatus> Tweets
        {
            get; set;
        }

        public RepliesViewModel()
        {
            Tweets = new ObservableCollection<TwitterStatus>();
        }

        string _name = "Replies";

        override public string Name
        {
            get { return _name; }
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
                .Statuses().Mentions().AsJson().Request();

            var statuses = response.AsStatuses();
            foreach (TwitterStatus status in statuses)
            {
                Tweets.Add(status);
            }
        }
    }
}
