using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TweetSharp.Twitter.Fluent;
using TweetSharp.Twitter.Model;
using TweetSharp.Twitter.Extensions;
using Anhinga.Properties;

namespace Anhinga.ViewModel
{
    class FollowersViewModel : UsersBaseViewModel
    {
        public override void Load()
        {
            _users.Clear();
            TwitterResult response = FluentTwitter
               .CreateRequest()
               .AuthenticateWith(
                            Settings.Default.ConsumerKey,
                                Settings.Default.ConsumerSecret,
                                Model.OAuthHandler.Token,
                                Model.OAuthHandler.TokenSecret)
             .Users().GetFollowers().AsJson().Request();

            var followers = response.AsUsers();
            foreach (TwitterUser user in followers)
            {
                base._users.Add(user);
            }
        }

        public override string Name
        {
            get { return "Followers"; }
        }
    }
}
