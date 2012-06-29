using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using TweetSharp.Twitter.Model;
using TweetSharp.Twitter.Fluent;
using TweetSharp.Twitter.Extensions;
using Anhinga.Properties;
using System.Windows.Input;
using Anhinga.Command;
using Anhinga.Model;
using System.Windows.Controls;

namespace Anhinga.ViewModel
{
    public class DirectMsgsViewModel : PageBase
    {
        SendDM _sendAction;
        #region PageBase Members

        public override void Load()
        {
            _directMsgs.Clear();
            LoadReceivedMsgs();
            LoadSentMsgs();
        }

        public override string Name
        {
            get { return "Direct"; }
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

        public void SendMsgClick(string userScName)
        {
            try
            {
                _sendAction = new SendDM();
                _sendAction.AddresseeUser = userScName;
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
                if (_sendAction.CanExecute(textBox.Text))
                    _sendAction.Execute(textBox.Text, (object)_directMsgs);
                //TODO else get user to know what's wrong
                textBox.Text = "";
            }
            catch
            {
                //HandleError(err);
            }
        }
        #endregion

        ObservableCollection<TwitterDirectMessage> _directMsgs = new ObservableCollection<TwitterDirectMessage>();
        public ObservableCollection<TwitterDirectMessage> DirectMsgs
        {
            get { return _directMsgs; }
        }

        public void LoadReceivedMsgs()
        {
            var twitter = FluentTwitter.CreateRequest();
            twitter.AuthenticateWith(
                                Settings.Default.ConsumerKey,
                                Settings.Default.ConsumerSecret,
                                Model.OAuthHandler.Token,
                                Model.OAuthHandler.TokenSecret);
            twitter.DirectMessages().Received().AsJson();

            var response = twitter.Request();
            TweetSharpUtilities.ValidateResponse(response);

            var directmsgs = response.AsDirectMessages();
            foreach (TwitterDirectMessage directmsg in directmsgs)
            {
                _directMsgs.Add(directmsg);
            }
        }

        public void LoadSentMsgs()
        {
            var twitter = FluentTwitter.CreateRequest();
            twitter.AuthenticateWith(
                                Settings.Default.ConsumerKey,
                                Settings.Default.ConsumerSecret,
                                Model.OAuthHandler.Token,
                                Model.OAuthHandler.TokenSecret);
            twitter.DirectMessages().Sent().AsJson();

            var response = twitter.Request();
            TweetSharpUtilities.ValidateResponse(response);

            var directmsgs = response.AsDirectMessages();
            foreach (TwitterDirectMessage directmsg in directmsgs)
            {
                _directMsgs.Add(directmsg);
            }
        }

    }
}
