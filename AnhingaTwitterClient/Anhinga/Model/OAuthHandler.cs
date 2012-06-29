using System;
using System.Xml.Linq;
using TweetSharp;
using TweetSharp.Model;
using TweetSharp.Twitter.Fluent;
using TweetSharp.Twitter.Extensions;
using Anhinga.Properties;

namespace Anhinga.Model
{
    internal sealed class OAuthHandler
    {
        public static OAuthToken AccessToken
        {
            get
            {
                return Instance._AccessToken;
            }
        }

        public static string Token
        {
            get
            {
                return Instance._AccessToken.Token;
            }
        }

        public static string TokenSecret
        {
            get
            {
                return Instance._AccessToken.TokenSecret;
            }
        }

        #region Singletons methods
        private static volatile OAuthHandler instance;
        private static object syncRoot = new Object();

        private static OAuthHandler Instance
        {
            get 
            {
                if (instance == null) 
                {
                    lock (syncRoot) 
                    {
                        if (instance == null)
                            instance = new OAuthHandler();
                    }
                }

                return instance;
            }
        }
        #endregion

        #region Non static methods
        private OAuthToken _AccessToken
        {
            get
            {
                if (Settings.Default.AccessTokenElement.Length == 0 ||
                    Settings.Default.AccessTokenSecretElement.Length == 0)
                    return null;

                return new OAuthToken
                {
                    Token = Settings.Default.AccessTokenElement,
                    TokenSecret = Settings.Default.AccessTokenSecretElement
                };
            }

            set
            {
                if (value == null)
                    return;
                Settings.Default.AccessTokenElement = value.Token;
                Settings.Default.AccessTokenSecretElement = value.TokenSecret;
                Settings.Default.Save();
            }
        }

        private OAuthHandler()
        {
            if (_AccessToken == null)
            {
                _AccessToken = PerformAuthorisation();
            } 
        }
        #endregion

        #region Private static methods

        private static string getPinFromUser()
        {
            AskPinFromUserDialog propertyWindow = new AskPinFromUserDialog("Enter PIN");
            var retval = propertyWindow.ShowDialog();
            if (retval == false)
            {
                throw new ApplicationException("pin is empty");
            }
            return propertyWindow.ObtainedInfo;
        }

        private static OAuthToken PerformAuthorisation()
        {
            FluentTwitter.SetClientInfo(
                new TwitterClientInfo
                {
                    ConsumerKey = Settings.Default.ConsumerKey,
                    ConsumerSecret = Settings.Default.ConsumerSecret
                });

            var twit = FluentTwitter.CreateRequest().Authentication.GetRequestToken();

            var response = twit.Request();

            var RequestToken = response.AsToken();
            twit = twit.Authentication.AuthorizeDesktop(RequestToken.Token);

            string verifier = getPinFromUser();

            twit.Authentication.GetAccessToken(RequestToken.Token, verifier);

            var response2 = twit.Request();

            return response2.AsToken();
        }

        #endregion
    }

}
