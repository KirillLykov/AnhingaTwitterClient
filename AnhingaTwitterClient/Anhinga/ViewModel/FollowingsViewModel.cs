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
    public class FollowingsViewModel : UsersBaseViewModel
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
             .Users().GetFriends().AsJson().Request();

            var followings = response.AsUsers();
            foreach (TwitterUser user in followings)
            {
                base._users.Add(user);
            }
        }

        public override string Name
        {
            get { return "Followings"; }
        }
    }
}
