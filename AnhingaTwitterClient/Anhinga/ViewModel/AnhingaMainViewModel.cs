using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using TweetSharp.Twitter.Fluent;
using Anhinga.Command;
using System.Windows.Input;
using Anhinga.Properties;
using Anhinga.Model;
using System.ComponentModel;
using System.Windows;
using Anhinga.View;
using System.Windows.Controls;
using TweetSharp.Twitter.Model;

namespace Anhinga.ViewModel
{
    public class AnhingaMainViewModel
    {
        #region Is design mode
        private static bool? _isInDesignMode;
        /// <summary>
        /// Gets a value indicating whether the control is in design mode
        /// (running in Blend or Visual Studio).
        /// </summary>
        public static bool IsInDesignMode
        {
            get
            {
                if (!_isInDesignMode.HasValue)
                {
                    var prop = DesignerProperties.IsInDesignModeProperty;
                    _isInDesignMode = (bool)DependencyPropertyDescriptor.FromProperty(prop, typeof(FrameworkElement)).
                        Metadata.DefaultValue;
                }

                return _isInDesignMode.Value;
            }
        }
        #endregion

        #region Avatar click command
        RelayCommand<TwitterUser> _avatarClickCommand;
        public ICommand AvatarClickCommand
        {
            get
            {
                if (_avatarClickCommand == null)
                {
                    _avatarClickCommand = new RelayCommand<TwitterUser>(param => this.AvatarClick(param));
                }
                return _avatarClickCommand;
            }
        }

        public void AvatarClick(TwitterUser user)
        {
            try
            {
                _searchTweetsPage.User = user;
                _searchTweetsPage.Load();

                var oo = _mainView.Tabs.Items.IndexOf(_searchTweetsPage);
                _mainView.Tabs.SelectedIndex = oo;
            }
            catch
            {
                //HandleError(err);
            }
        }
        #endregion

        AnhingaView _mainView;
        SearchTweetsViewModel _searchTweetsPage;

        ObservableCollection<PageBase> _pages;
        public ObservableCollection<PageBase> Pages
        {
            get { return _pages; }
            set { _pages = value; }
        }

        public AnhingaMainViewModel(AnhingaView mainView)
        {
            try
            {
                if (!IsInDesignMode)
                {
                    _mainView = mainView;

                    _pages = new ObservableCollection<PageBase>();
                    _pages.Add(new HomeViewModel());
                    _pages.Add(new RepliesViewModel());
                    _pages.Add(new DirectMsgsViewModel());

                    _searchTweetsPage = new SearchTweetsViewModel();
                    _pages.Add(_searchTweetsPage);

                    _pages.Add(new RetweetsViewModel());
                    _pages.Add(new FollowersViewModel());
                    _pages.Add(new FollowingsViewModel());
                    _pages.Add(new AdvancedSearchViewModel());

                    //foreach (var page in _pages)
                    //{
                    //    page.Load();
                    //}
                }
            }
            catch
            {
                //HandleException(err);
            }
        }

        #region Handle name and tag clicks
        //TODO public -> private
        public void HandleHashtagClick(string hashTag)
        {
            try
            {
                _searchTweetsPage.HashTag = hashTag;
                SetSearchItem();
            }
            catch
            {
                //HandleError(err);
            }
        }

        //TODO get rid of it when you implement Commands in TwitterTweetBox
        //temporary solution
        public void HandleNameClick(string userScreenName)
        {
            try
            {
                _searchTweetsPage.User = new TwitterUser { ScreenName = userScreenName };
                SetSearchItem();
            }
            catch
            {
                //HandleError(err);
            }
        }
        #endregion

        void SetSearchItem()
        {
            _searchTweetsPage.Load();
            var oo = _mainView.Tabs.Items.IndexOf(_searchTweetsPage);
            _mainView.Tabs.SelectedIndex = oo;
        }
    }
}
