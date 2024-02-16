using System;
using System.Windows;

namespace StudyPal
{
    public partial class UnavailableWindow : Window
    {
        public UnavailableWindow()
        {
            InitializeComponent();
            RickRoll.Position = TimeSpan.Zero;
            RickRoll.Play();
        }

        private void unavailableWindowOK_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void RickRoll_MediaEnded(object sender, RoutedEventArgs e)
        {
                RickRoll.Position = TimeSpan.Zero;
                RickRoll.Play();
        }
    }
}