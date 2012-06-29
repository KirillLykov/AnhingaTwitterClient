using System;
using System.Collections.ObjectModel;
using TweetSharp.Twitter.Model;
using Anhinga.Command;
using System.Windows.Input;

namespace Anhinga.ViewModel
{
    public class PageBase
    {
        public virtual void Load() { }
        public virtual string Name { get {return "";} }

        #region Refresh command
        RelayCommand _refreshCommand;
        public virtual ICommand RefreshCommand
        {
            get
            {
                if (_refreshCommand == null)
                {
                    _refreshCommand = new RelayCommand(() => this.Refresh());
                }
                return _refreshCommand;
            }
        }

        public virtual void Refresh()
        {
            try
            {
                Load();
            }
            catch
            {
                //HandleError(err);
            }
        }
        #endregion
    }
}
