using System;
using System.Windows;

namespace StudyPal
{
    public partial class ErrorWindow : Window
    {
        public ErrorWindow()
        {
            InitializeComponent();
        }

        private void errorWindowOK_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}