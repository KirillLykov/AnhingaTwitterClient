using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.ComponentModel;

namespace Anhinga.Controls
{
    /// <summary>
    /// I got it from Witty (New BSD License)
    /// Custom TextBlock to allow parsing hyperlinks and @names
    /// </summary>
    public class TweetTextBlock : TextBlock
    {
        #region Private fields

        private TextPointer startpointer;
        private TextPointer endpointer;
        private bool selectingText = false;
        private object previousbackgroundcolor;

        #endregion

        #region Contructors

        public TweetTextBlock()
        {
            previousbackgroundcolor = this.GetValue(TextElement.BackgroundProperty);

            this.MouseLeftButtonDown += new MouseButtonEventHandler(TweetTextBlock_MouseLeftButtonDown);
            this.MouseLeftButtonUp += new MouseButtonEventHandler(TweetTextBlock_MouseLeftButtonUp);
            this.MouseMove += new MouseEventHandler(TweetTextBlock_MouseMove);
            this.MouseLeave += new MouseEventHandler(TweetTextBlock_MouseLeave);
        }

        #endregion

        #region Dependency properties

        public string TweetText
        {
            get { return (string)GetValue(TweetTextProperty); }
            set { SetValue(TweetTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TweetText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TweetTextProperty =
            DependencyProperty.Register("TweetText", typeof(string), typeof(TweetTextBlock),
            new FrameworkPropertyMetadata(string.Empty, new PropertyChangedCallback(OnTweetTextChanged)));

        #endregion

        void TweetTextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var mousePos = Mouse.GetPosition(this);
                startpointer = this.GetPositionFromPoint(mousePos, true);

                TextRange highlighttext = new TextRange(startpointer, startpointer);
                selectingText = true;
            }
            catch (Exception err)
            {
                //App.log.Error(err.ToString());
            }
        }

        void TweetTextBlock_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            endpointer = this.GetPositionFromPoint(Mouse.GetPosition(this), true);
            selectingText = false;

            try
            {
                TextRange clipboardtext = new TextRange(startpointer, endpointer);
                Clipboard.SetText(clipboardtext.Text);
            }
            catch (Exception err)
            {
                //App.log.Error(err.ToString());
                MessageBox.Show("Unable to select text across tweets", "Text Select", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            TextRange documenttext = new TextRange(this.ContentStart, this.ContentEnd);
            documenttext.ApplyPropertyValue(TextElement.BackgroundProperty, previousbackgroundcolor);
        }

        void TweetTextBlock_MouseMove(object sender, MouseEventArgs e)
        {
            if (selectingText)
            {
                TextPointer currentpointer = this.GetPositionFromPoint(Mouse.GetPosition(this), true);

                try
                {
                    TextRange highlighttext = new TextRange(startpointer, currentpointer);
                    highlighttext.ApplyPropertyValue(TextElement.BackgroundProperty, new SolidColorBrush(Colors.LightGoldenrodYellow));
                }
                catch (Exception err)
                {
                    //App.log.Error(err.ToString());
                    MessageBox.Show(err.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        void TweetTextBlock_MouseLeave(object sender, MouseEventArgs e)
        {
            try
            {
	            TextRange documenttext = new TextRange(this.ContentStart, this.ContentEnd);
	            documenttext.ApplyPropertyValue(TextElement.BackgroundProperty, previousbackgroundcolor);
	            selectingText = false;
            }
            catch (System.Exception ex)
            {
                //App.log.Error(ex.ToString());
            }
        }

        private static void OnTweetTextChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            string text = args.NewValue as string;
            if (!string.IsNullOrEmpty(text))
            {
                TweetTextBlock textblock = (TweetTextBlock)obj;
                textblock.Inlines.Clear();
                textblock.Inlines.Add(" ");

                string[] words = Regex.Split(text, @"([ \(\)\{\}\[\]])");

                string possibleUserName = words[0].ToString();

                if ((possibleUserName.Length > 1) && (possibleUserName.Substring(1, 1) == "@"))
                {
                    textblock = FormatName(textblock, possibleUserName);
                    words.SetValue("", 0);
                }

                foreach (string word in words)
                {
                    // clickable hyperlinks
                    if (IsUrl(word))
                    {
                        try
                        {
                            Hyperlink link = new Hyperlink();
                            link.NavigateUri = new Uri(word);
                            link.Inlines.Add(word);
                            link.Click += new RoutedEventHandler(link_Click);
                            link.ToolTip = "Open link in the default browser";
                            textblock.Inlines.Add(link);
                        }
                        catch(Exception err)
                        {
                            //App.log.Error(err.ToString());
                            textblock.Inlines.Add(word);
                        }
                    }
                    // clickable @name
                    else if (word.StartsWith("@"))
                    {
                        textblock = FormatName(textblock, word);

                    }

                    // clickable #hashtag
                    else if (word.StartsWith("#"))
                    {
                        string hashtag = String.Empty;
                        Match foundHashtag = Regex.Match(word, @"#(\w+)(?<suffix>.*)");
                        if (foundHashtag.Success)
                        {
                            hashtag = foundHashtag.Groups[1].Captures[0].Value;
                            Hyperlink tag = new Hyperlink();
                            tag.Inlines.Add(hashtag);

                            string hashtagUrl = "http://search.twitter.com/search?q=%23{0}";

                            // The main application has access to the Settings class, where a 
                            // user-defined hashtagUrl can be stored.  This hardcoded one that
                            // is used to set the NavigateUri is just a default behavior that
                            // will be used if the click event is not handled for some reason.

                            tag.NavigateUri = new Uri(String.Format(hashtagUrl, hashtag));
                            tag.ToolTip = "Show statuses that include this hashtag";
                            tag.Tag = hashtag;

                            tag.Click += new RoutedEventHandler(hashtag_Click);

                            textblock.Inlines.Add("#");
                            textblock.Inlines.Add(tag);
                            textblock.Inlines.Add(foundHashtag.Groups["suffix"].Captures[0].Value);
                        }
                    }
                    else
                    {
                        textblock.Inlines.Add(word);
                    }
                }

                textblock.Inlines.Add(" ");
            }
        }

        public static TweetTextBlock FormatName(TweetTextBlock textblock, string word)
        {
            string userName = String.Empty;
            string firstLetter = word.Substring(0, 1);

            Match foundUsername = Regex.Match(word, @"@(\w+)(?<suffix>.*)");

            if (foundUsername.Success)
            {
                userName = foundUsername.Groups[1].Captures[0].Value;
                Hyperlink name = new Hyperlink();
                name.Inlines.Add(userName);
                name.NavigateUri = new Uri("http://twitter.com/" + userName);
                name.ToolTip = "View @" + userName + "'s recent tweets";
                name.Tag = userName;

                name.Click += new RoutedEventHandler(name_Click);

                if (firstLetter != "@")
                    textblock.Inlines.Add(firstLetter);

                textblock.Inlines.Add("@");
                textblock.Inlines.Add(name);
                textblock.Inlines.Add(foundUsername.Groups["suffix"].Captures[0].Value);
            }
            return textblock;
        }

        #region Clickable #hashtag

        public static readonly RoutedEvent HashtagClickEvent = EventManager.RegisterRoutedEvent(
            "HashtagClick", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(TweetTextBlock));

        public event RoutedEventHandler HashtagClick
        {
            add { AddHandler(HashtagClickEvent, value); }
            remove { RemoveHandler(HashtagClickEvent, value); }
        }

        static void hashtag_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (e.OriginalSource is Hyperlink)
                {
                    Hyperlink h = e.OriginalSource as Hyperlink;
                    if (h.Parent is TweetTextBlock)
                    {
                        TweetTextBlock p = h.Parent as TweetTextBlock;
                        p.RaiseEvent(new RoutedEventArgs(HashtagClickEvent, h));
                        return;
                    }
                }

                // As a fallback (i.e., if the event is not handled), we launch the hyperlink's
                // URL in a web browser

                System.Diagnostics.Process.Start(((Hyperlink)sender).NavigateUri.ToString());

            }
            catch (Exception err)
            {
                //App.log.Error(err.ToString());
                MessageBox.Show("There was a problem launching the specified URL.", "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }
        #endregion

        #region Clickable @name

        public static readonly RoutedEvent NameClickEvent = EventManager.RegisterRoutedEvent(
            "NameClick", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(TweetTextBlock));

        public event RoutedEventHandler NameClick
        {
            add { AddHandler(NameClickEvent, value); }
            remove { RemoveHandler(NameClickEvent, value); }
        }

        static void name_Click(object sender, RoutedEventArgs e)
        {
            // The event handler in the main application can handle the click event in a custom
            // fashion (i.e., show the tweets in Witty or launch a URL, etc).  That behavior is
            // not implemented here.
            try
            {
                if (e.OriginalSource is Hyperlink)
                {
                    Hyperlink h = e.OriginalSource as Hyperlink;
                    if (h.Parent is TweetTextBlock)
                    {
                        TweetTextBlock p = h.Parent as TweetTextBlock;
                        p.RaiseEvent(new RoutedEventArgs(NameClickEvent, h));
                        return;
                    }
                }

                // As a fallback (i.e., if the event is not handled), we launch the hyperlink's
                // URL in a web browser

                System.Diagnostics.Process.Start(((Hyperlink)sender).NavigateUri.ToString());

            }
            catch (Exception err)
            {
                //App.log.Error(err.ToString());
                MessageBox.Show("There was a problem launching the specified URL.", "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        #endregion

        #region Regular Hyperlink Click

        static void link_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(((Hyperlink)sender).NavigateUri.ToString());
            }
            catch (Exception err)
            {
                //App.log.Error(err.ToString());
                MessageBox.Show("There was a problem launching the specified URL.", "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        #endregion

        public static bool IsUrl(string word)
        {
            if (!Uri.IsWellFormedUriString(word, UriKind.Absolute))
                return false;

            Uri uri = new Uri(word);
            foreach (string acceptedScheme in new string[] { "http", "https", "ftp" })
                if (uri.Scheme == acceptedScheme)
                    return true;

            return false;
        }
    }
}
