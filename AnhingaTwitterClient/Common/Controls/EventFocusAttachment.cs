using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Anhinga.Controls
{
    public class EventFocusAttachment
    {
        public static Control GetElementToFocus(Button button)
        {
            return (Control)button.GetValue(ElementToFocusProperty);
        }

        public static void SetElementToFocus(Button button, Control value)
        {
            button.SetValue(ElementToFocusProperty, value);
        }

        public static readonly DependencyProperty ElementToFocusProperty =
            DependencyProperty.RegisterAttached("ElementToFocus", typeof(Control),
            typeof(EventFocusAttachment), new UIPropertyMetadata(null, ElementToFocusPropertyChanged));

        //public static readonly DependencyProperty RootUCProperty =
        //    DependencyProperty.Register("RootUserControl", typeof(IRootUserControl),
        //        typeof(UserShortInfo));
        //public IRootUserControl RootUserControl
        //{
        //    get { return GetValue(RootUCProperty) as IRootUserControl; }
        //    set { SetValue(RootUCProperty, value); }
        //}

        public static void ElementToFocusPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var button = sender as Button;
            if (button != null)
            {
                button.Click += (s, args) =>
                {
                    Control control = GetElementToFocus(button);
                    if (control != null)
                    {
                        control.Focus();
                    }
                    if (control is TextBox && button.Name.Equals("Reply"))
                    {
                        TextBox tb = (TextBox)control;
                        tb.Text = '@' + (string)button.Tag + ' ';
                        tb.CaretIndex = tb.Text.Length;
                    }
                };
            }
        }
    }
}
