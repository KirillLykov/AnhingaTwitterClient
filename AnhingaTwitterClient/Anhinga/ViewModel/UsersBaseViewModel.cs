using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Anhinga.Command;
using System.Windows.Input;
using System.Collections.ObjectModel;
using TweetSharp.Twitter.Fluent;
using TweetSharp.Twitter.Extensions;
using Anhinga.Properties;
using TweetSharp.Twitter.Model;
using System.Windows.Controls;
using Anhinga.Model;

namespace Anhinga.ViewModel
{
    public abstract class UsersBaseViewModel : PageBase
    {
        protected ObservableCollection<TwitterUser> _users = new ObservableCollection<TwitterUser>();
        public virtual ObservableCollection<TwitterUser> Users
        {
            get
            {
                return _users;
            }
        }

        #region Search User click command
        RelayCommand<TextBox> _searchUserCommand;
        public ICommand SearchUserCommand
        {
            get
            {
                if (_searchUserCommand == null)
                {
                    _searchUserCommand = new RelayCommand<TextBox>(param => this.SearchUser(param));
                }
                return _searchUserCommand;
            }
        }

        public void SearchUser(TextBox searchTextBox)
        {
            try
            {
                TweetSharpUtilities.LookUpUser(searchTextBox.Text, ref _users);
                searchTextBox.Text = "";
            }
            catch
            {
                //HandleError(err);
            }
        }
        #endregion

        #region Follow / Unfollow command
        RelayCommand<object> _followUnfollowCommand;
        public ICommand FollowUnfollowCommand
        {
            get
            {
                if (_followUnfollowCommand == null)
                {
                    _followUnfollowCommand = new RelayCommand<object>(param => this.FollowUnfollow(param));
                }
                return _followUnfollowCommand;
            }
        }

        public void FollowUnfollow(object obj)
        {
            try
            {
                TwitterUser user = (TwitterUser)obj;
                bool isFollowing = (bool)user.IsFollowing;//IsFriend(user.Id);

                if (!isFollowing)
                {
                    Unfollow(user.Id);
                }
                else
                {
                    Follow(user.Id);
                }
            }
            catch
            {
                //HandleError(err);
            }
        }
        #endregion

        //todo iFollowing parameter is deprecated so i should use this method instead but i don't want to store userID
        //private bool IsFriend(int userid)
        //{
        //    var follow = FluentTwitter.CreateRequest()
        //        .AuthenticateWith(Settings.Default.ConsumerKey,
        //              Settings.Default.ConsumerSecret,
        //              Model.OAuthHandler.Token,
        //              Model.OAuthHandler.TokenSecret)
        //        .Friendships().Verify(current_user_id).IsFriendsWith(userid).AsJson();
        //    var response = follow.Request();
        //    return false;
        //}

        public void Follow(int userid)
        {
            var follow = FluentTwitter.CreateRequest()
                .AuthenticateWith(Settings.Default.ConsumerKey,
                                  Settings.Default.ConsumerSecret,
                                  Model.OAuthHandler.Token,
                                  Model.OAuthHandler.TokenSecret)
                .Friendships().Befriend(userid)
                .WithNotifications().AsJson();

            var response = follow.Request();
            var celebrity = response.AsUser();
            //Followings.Add(celebrity);
        }

        public void Unfollow(int userid)
        {
            var leave = FluentTwitter.CreateRequest()
                .AuthenticateWith(Settings.Default.ConsumerKey,
                                  Settings.Default.ConsumerSecret,
                                  Model.OAuthHandler.Token,
                                  Model.OAuthHandler.TokenSecret)
                .Friendships().Destroy(userid).AsJson();

            var response = leave.Request();
            //TODO Think how to do it better or it is ok
            //Followings.Remove(response.AsUser());
        }
    }
}
