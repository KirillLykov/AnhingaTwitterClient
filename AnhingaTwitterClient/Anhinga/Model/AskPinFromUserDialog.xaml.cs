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
using System.Windows.Shapes;

namespace Anhinga.Model
{
    public partial class AskPinFromUserDialog : Window
    {
        public string ObtainedInfo { get; set; }

        public AskPinFromUserDialog(string title)
        {
            this.Title = title;
            InitializeComponent();
        }

        private void OK_button_Click(object sender, RoutedEventArgs e)
        {
            ObtainedInfo = this.PinTextBox.Text;
            DialogResult = true;
        }
    }
}
