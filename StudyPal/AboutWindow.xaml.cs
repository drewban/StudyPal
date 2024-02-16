using System;
using System.Windows;

namespace StudyPal
{
    public partial class AboutWindow : Window
    {
        public AboutWindow()
        {
            InitializeComponent();
        }

        private void aboutWindowOK_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}