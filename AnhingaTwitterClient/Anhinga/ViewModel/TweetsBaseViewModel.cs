using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Anhinga.Command;
using System.Windows.Input;
using System.Collections.ObjectModel;
using TweetSharp.Twitter.Model;
using TweetSharp.Twitter.Fluent;
using Anhinga.Properties;
using Anhinga.Model;
using System.Windows.Controls;

namespace Anhinga.ViewModel
{
    public class TweetsBaseViewModel : PageBase
    {
        IAction _actionToProccess;

        public TweetsBaseViewModel()
        {
            _actionToProccess = new SendTweet();
        }

        #region Reply click command
        RelayCommand<long> _replyClickCommand;
        public ICommand ReplyClickCommand
        {
            get
            {
                if (_replyClickCommand == null)
                {
                    _replyClickCommand = new RelayCommand<long>(param => this.ReplyClick(param));
                }
                return _replyClickCommand;
            }
        }

        public void ReplyClick(long tweetId)
        {
            try
            {
                var action = new SendReply();
                action.AddresseeTweetID = tweetId;
                _actionToProccess = action;
            }
            catch
            {
                //HandleError(err);
            }
        }
        #endregion

        #region Retweet click command
        RelayCommand<long> _retweetClickCommand;
        public ICommand RetweetClickCommand
        {
            get
            {
                if (_retweetClickCommand == null)
                {
                    _retweetClickCommand = new RelayCommand<long>(param => 
                        {            
                            try
                            {
                                TweetSharpUtilities.Retweet(param);
                            }
                            catch
                            {
                                //HandleError(err);
                            }
                         }
                        );
                }
                return _retweetClickCommand;
            }
        }
        #endregion

        #region SendMsg click command
        RelayCommand<string> _sendMsgClickCommand;
        public ICommand SendMsgClickCommand
        {
            get
            {
                if (_sendMsgClickCommand == null)
                {
                    _sendMsgClickCommand = new RelayCommand<string>(param => this.SendMsgClick(param));
                }
                return _sendMsgClickCommand;
            }
        }

        public void SendMsgClick(string userScreenName)
        {
            try
            {
                var action = new SendDM();
                action.AddresseeUser = userScreenName;
                _actionToProccess = action;
            }
            catch
            {
                //HandleError(err);
            }
        }
        #endregion

        #region SendCommand
        RelayCommand<TextBox> _sendCommand;
        public ICommand SendCommand
        {
            get
            {
                if (_sendCommand == null)
                {
                    _sendCommand = new RelayCommand<TextBox>(param => this.SendTweet(param));
                }
                return _sendCommand;
            }
        }

        private void SendTweet(TextBox textBox)
        {
            try
            {
                if (_actionToProccess != null && _actionToProccess.CanExecute(textBox.Text))
                    _actionToProccess = _actionToProccess.Execute(textBox.Text, Tweets);
                //TODO else get user to know what's wrong
                textBox.Text = "";
            }
            catch
            {
                //HandleError(err);
            }
        }
        #endregion

        #region PageBase Members

        public override void Load()
        {
            LoadTweets();
        }

        public override string Name
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region TweetsbaseViewModel's methods

        public virtual void LoadTweets()
        {
            throw new NotImplementedException();
        }

        public virtual ObservableCollection<TwitterStatus> Tweets
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
        #endregion
    }
}
