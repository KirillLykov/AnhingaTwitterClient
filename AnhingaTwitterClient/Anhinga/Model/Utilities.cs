using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TweetSharp.Twitter.Fluent;
using Anhinga.Properties;
using TweetSharp.Twitter.Model;
using TweetSharp.Twitter.Extensions;
using System.Collections.ObjectModel;
using TweetSharp.Twitter.Service;

namespace Anhinga.Model
{
    public class AdvancedSearchParam
    {
        public string AnyWords { get; set; }
        public string WithoutWords { get; set; }
        public string Tag { get; set; }
        public string Language { get; set; }
        public string Place { get; set; }
        public double Distance { get; set; }

        public TwitterGeoLocation GeoLocation { get; set; }

        public AdvancedSearchParam()
        {
            AnyWords = WithoutWords = Tag = Language = Place = "";
            GeoLocation = new TwitterGeoLocation();
        }

        public override string ToString()
        {
            //well, the right solution is to create wrappers around string and implement ToString but I decided to do it simple
            StringBuilder res = new StringBuilder();
            res.Append(AnyWords);
            if (WithoutWords.Length != 0)
                res.Append( " -" + WithoutWords.Replace(" ", " -") );
            if (Tag.Length != 0)
                res.Append(" #" + Tag);
            //if (Language)
            if (Place.Length != 0)
                res.Append(" near:\"" + Place + "\"");
            if (Distance != 0)
                res.Append(" within:" + Distance + "mi");
            return res.ToString();
        }
    }

    static class TweetSharpUtilities
    {
        public static TwitterStatus PostTweet(string message)
        {
            var twitter = FluentTwitter.CreateRequest();
            twitter.AuthenticateWith(
                                Settings.Default.ConsumerKey,
                                Settings.Default.ConsumerSecret,
                                OAuthHandler.Token,
                                OAuthHandler.TokenSecret);
            twitter.Statuses().Update(message);

            var response = twitter.Request();
            ValidateResponse(response);
            return response.AsStatus();
        }

        public static TwitterStatus PostReply(long tweetId, string text)
        {
            var twitter = FluentTwitter.CreateRequest();
            twitter.AuthenticateWith(
                                Settings.Default.ConsumerKey,
                                Settings.Default.ConsumerSecret,
                                OAuthHandler.Token,
                                OAuthHandler.TokenSecret);
            twitter.Statuses().Update(text).InReplyToStatus(tweetId);

            var response = twitter.Request();
            ValidateResponse(response);
            return response.AsStatus();
        }

        public static TwitterStatus Retweet(long tweetID)
        {
            var twitter = FluentTwitter.CreateRequest();
            twitter.AuthenticateWith(
                                Settings.Default.ConsumerKey,
                                Settings.Default.ConsumerSecret,
                                OAuthHandler.Token,
                                OAuthHandler.TokenSecret);
            twitter.Statuses().Retweet(tweetID).AsJson();
            var response = twitter.Request();
            ValidateResponse(response);
            return response.AsStatus();
        }

        public static TwitterDirectMessage SendDirectMsg(string screenName, string message)
        {
            var twitter = FluentTwitter.CreateRequest();
            twitter.AuthenticateWith(
                                Settings.Default.ConsumerKey,
                                Settings.Default.ConsumerSecret,
                                OAuthHandler.Token,
                                OAuthHandler.TokenSecret);
            twitter.DirectMessages().Send(screenName, message).AsJson();

            var response = twitter.Request();
            ValidateResponse(response);
            return response.AsDirectMessage();
        }

        public static void LookUpUser(string username, ref ObservableCollection<TwitterUser> users)
        {
            users.Clear();
            if (username.Length == 0)
                return;

            var lookup = FluentTwitter.CreateRequest()
                .AuthenticateWith(Settings.Default.ConsumerKey,
                      Settings.Default.ConsumerSecret,
                      Model.OAuthHandler.Token,
                      Model.OAuthHandler.TokenSecret)
                .Users().Lookup(username);

            var result = lookup.Request();
            ValidateResponse(result);

            var resUsers = result.AsUsers();
            foreach (var user in resUsers)
            {
                users.Add(user);
            }
        }

        public static void GetUsersTweets(TwitterUser user, ref ObservableCollection<TwitterStatus> tweets)
        {
            tweets.Clear();

            TwitterResult response;
            if (user.Id  != 0)
                response = FluentTwitter
                    .CreateRequest()
                    .AuthenticateWith(
                        Settings.Default.ConsumerKey,
                             Settings.Default.ConsumerSecret,
                             Model.OAuthHandler.Token,
                             Model.OAuthHandler.TokenSecret)
                    .Statuses().OnUserTimeline().For(user.Id).AsJson().Request();
            else
                response = FluentTwitter
                   .CreateRequest()
                   .AuthenticateWith(
                       Settings.Default.ConsumerKey,
                            Settings.Default.ConsumerSecret,
                            Model.OAuthHandler.Token,
                            Model.OAuthHandler.TokenSecret)
                   .Statuses().OnUserTimeline().For(user.ScreenName).AsJson().Request();

            TweetSharpUtilities.ValidateResponse(response);

            var statuses = response.AsStatuses();
            foreach (TwitterStatus status in statuses)
            {
                tweets.Add(status);
            }
        }

        public static void SearchTweets(string phrase, ref ObservableCollection<TwitterStatus> tweets)
        {
            tweets.Clear();

            var response = FluentTwitter.CreateRequest()
                .Search().Query()
                .Containing(phrase)
                .AsJson().Request();

            var results = response.AsSearchResult();
            foreach (var status in results.Statuses)
            {
                tweets.Add(status);
            }
        }

        public static void SearchByTag(string tag, ref ObservableCollection<TwitterStatus> tweets)
        {
            tweets.Clear();
            var search = FluentTwitter.CreateRequest()
                .Search().Query().ContainingHashTag(tag)
                .AsJson();

            var response = search.Request();
            ValidateResponse(response);

            TwitterSearchResult results = response.AsSearchResult();

            foreach (TwitterStatus status in results.Statuses)
            {
                tweets.Add(status);
            }
        }

        private static ITwitterSearchQuery AddNotContainingWords(this ITwitterSearchQuery query, string withoutWords)
        {
            //it is a hack because NotContaining works strange
            withoutWords = withoutWords.Replace(" ", " -");
            return query.NotContaining(withoutWords);
        }

        public static void AdvancedSearch(AdvancedSearchParam criterion, ref ObservableCollection<TwitterStatus> tweets)
        {
            tweets.Clear();
            //TwitterResult response;
            //if (criterion.Distance != 0 && criterion.GeoLocation != null)
            //    response = FluentTwitter.CreateRequest()
            //        .Search().Query().ContainingHashTag(criterion.Tag)
            //        .Containing(criterion.AnyWords).NotContaining(criterion.WithoutWords)
            //        .InLanguage(criterion.Language).Within(criterion.Distance).Of(criterion.GeoLocation)
            //        .AsJson().Request();
            //else
            //    response = FluentTwitter.CreateRequest()
            //        .Search().Query().ContainingHashTag(criterion.Tag)
            //        .AddNotContainingWords(criterion.WithoutWords).Containing(criterion.AnyWords)
            //        .InLanguage(criterion.Language)
            //        .AsJson().Request();

            var service = new TwitterService();
            service.AuthenticateWith(Settings.Default.ConsumerKey,
                            Settings.Default.ConsumerSecret,
                            Model.OAuthHandler.Token,
                            Model.OAuthHandler.TokenSecret);
            TwitterSearchResult results = service.SearchForTweets(criterion.ToString(), 1, 40);
            if (results == null)
                return;

            //ValidateResponse(response);
            //TwitterSearchResult results = response.AsSearchResult();

            foreach (TwitterStatus status in results.Statuses)
            {
                tweets.Add(status);
            }
        }
                   
        public static void ValidateResponse(TwitterResult response)
        {
            if (response.IsTwitterError)
                throw new System.ApplicationException(response.ResponseHttpStatusDescription);
        }
    }

    interface IAction
    {
        IAction Execute(string message, object obj);
        bool CanExecute(string message);
    }

    class SendTweet : IAction
    {
        public bool CanExecute(string message)
        {
            return message.Length != 0;
        }

        public IAction Execute(string message, object obj)
        {
            var tweet = TweetSharpUtilities.PostTweet(message);
            ObservableCollection<TwitterStatus> collection = obj as ObservableCollection<TwitterStatus>;
            if (collection != null)
                collection.Add(tweet);
            return this;
        }
    }

    class SendDM : IAction
    {
        public string AddresseeUser { get; set; }

        public bool CanExecute(string message)
        {
            return message.Length != 0 && AddresseeUser.Length != 0;
        }

        public IAction Execute(string message, object obj)
        {
            var dmessage = TweetSharpUtilities.SendDirectMsg(AddresseeUser, message);
            ObservableCollection<TwitterDirectMessage> collection = obj as ObservableCollection<TwitterDirectMessage>;
            if (collection != null)
                collection.Add(dmessage);
            return new SendTweet();
        }
    }

    class SendReply : IAction
    {
        public long AddresseeTweetID { get; set; }

        public bool CanExecute(string message)
        {
            return message.Length != 0 && AddresseeTweetID != 0;
        }

        public IAction Execute(string message, object obj)
        {
            var tweet = TweetSharpUtilities.PostReply(AddresseeTweetID, message);
            ObservableCollection<TwitterStatus> collection = obj as ObservableCollection<TwitterStatus>;
            if (collection != null)
                collection.Add(tweet);
            return new SendTweet();
        }
    }
}
