using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Anhinga.ViewModel;
using System.ComponentModel;

namespace Anhinga.View
{
    /// <summary>
    /// Interaction logic for AnhingaView.xaml
    /// </summary>
    public partial class AnhingaView : UserControl
    {
        private AnhingaMainViewModel _viewModel;

        public AnhingaMainViewModel ViewModel
        {
            get { return _viewModel; }
            set { _viewModel = value; }
        }

        public static void EnsureApplicationResources()
        {
            if (Application.Current == null)
            {
                // create the Application object
                new Application();
            }

            try
            {
                // merge in your application resources
                Application.Current.Resources.MergedDictionaries.Add(
                    Application.LoadComponent(
                        new Uri("AnhingaSkins;Component/Styles.xaml",
                        UriKind.Relative)) as ResourceDictionary);
                Application.Current.Resources.MergedDictionaries.Add(
                    Application.LoadComponent(
                        new Uri("AnhingaSkins;Component/GlassButton.xaml",
                        UriKind.Relative)) as ResourceDictionary);
            }
            catch
            {
            }
        }

        public AnhingaView()
        {
            _viewModel = new AnhingaMainViewModel(this);
            EnsureApplicationResources();
            InitializeComponent();
        }

        //TODO It's a dirty solution. You must add List of Commands to the TweetTextBlock instead
        private void NameClickedInTweet(object sender, RoutedEventArgs reArgs)
        {
            try
            {
                if (reArgs.OriginalSource is System.Windows.Documents.Hyperlink)
                {
                    Hyperlink h = reArgs.OriginalSource as Hyperlink;

                    string userScreenName = h.Tag.ToString();
                    _viewModel.HandleNameClick(userScreenName);
                }
            }
            catch (System.Exception err)
            {
                //HandleError(err);
            }
            finally
            {
                reArgs.Handled = true;
            }
        }

        private void HashtagClickedInTweet(object sender, RoutedEventArgs reArgs)
        {
            try
            {
                if (reArgs.OriginalSource is System.Windows.Documents.Hyperlink)
                {
                    Hyperlink h = reArgs.OriginalSource as Hyperlink;

                    string hashtag = h.Tag.ToString();
                    _viewModel.HandleHashtagClick(hashtag);
                }
            }
            catch (System.Exception err)
            {
                //HandleError(err);
            }
            finally
            {
                reArgs.Handled = true;
            }
        }

        //TODO it is just to show. you should implement it using triggers
        #region Visual effects
        private void Border_MouseEnter(object sender, MouseEventArgs e)
        {
            var border = (sender as Border);
            //border.Background = _linbrush;
            var panel = border.FindName("ActionButtonsSP") as StackPanel;
            panel.Visibility = Visibility.Visible;
        }

        private void Border_MouseLeave(object sender, MouseEventArgs e)
        {
            var border = (sender as Border);
            //border.Background = Brushes.White;
            var panel = border.FindName("ActionButtonsSP") as StackPanel;
            panel.Visibility = Visibility.Collapsed;
        }
        #endregion
    }
}
