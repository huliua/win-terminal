using System.Windows.Navigation;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WinTerminal.Controls
{
    public partial class CodexControl : UserControl
    {
        public CodexControl()
        {
            InitializeComponent();
            Loaded += CodexControl_Loaded;
        }

        private void CodexControl_Loaded(object sender, RoutedEventArgs e)
        {
            Browser.Navigate(new Uri("https://chat.openai.com"));
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            if (Browser.CanGoBack)
                Browser.GoBack();
        }

        private void BtnForward_Click(object sender, RoutedEventArgs e)
        {
            if (Browser.CanGoForward)
                Browser.GoForward();
        }

        private void BtnReload_Click(object sender, RoutedEventArgs e)
        {
            Browser.Refresh();
        }

        private void BtnGo_Click(object sender, RoutedEventArgs e)
        {
            NavigateToUrl();
        }

        private void UrlBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                NavigateToUrl();
                e.Handled = true;
            }
        }

        private void NavigateToUrl()
        {
            string url = UrlBox.Text.Trim();
            if (string.IsNullOrEmpty(url)) return;

            if (!url.StartsWith("http://") && !url.StartsWith("https://"))
            {
                url = "https://" + url;
                UrlBox.Text = url;
            }

            try
            {
                Browser.Navigate(new Uri(url));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Invalid URL: " + ex.Message, "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Browser_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            // Allow navigation
        }

        private void Browser_LoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            // Update URL bar after navigation
            if (Browser.Source != null)
            {
                UrlBox.Text = Browser.Source.ToString();
            }
        }
    }
}
