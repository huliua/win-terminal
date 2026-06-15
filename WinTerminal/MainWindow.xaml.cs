using System.Windows;
using System.Windows.Input;

namespace WinTerminal
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void SetActiveTab(string tab)
        {
            BtnTerminal.Style = (Style)FindResource("TabButton");
            BtnClaude.Style = (Style)FindResource("TabButton");
            BtnCodex.Style = (Style)FindResource("TabButton");

            TerminalView.Visibility = Visibility.Collapsed;
            ClaudeView.Visibility = Visibility.Collapsed;
            CodexView.Visibility = Visibility.Collapsed;

            switch (tab)
            {
                case "terminal":
                    BtnTerminal.Style = (Style)FindResource("TabButtonActive");
                    TerminalView.Visibility = Visibility.Visible;
                    break;
                case "claude":
                    BtnClaude.Style = (Style)FindResource("TabButtonActive");
                    ClaudeView.Visibility = Visibility.Visible;
                    break;
                case "codex":
                    BtnCodex.Style = (Style)FindResource("TabButtonActive");
                    CodexView.Visibility = Visibility.Visible;
                    break;
            }
        }

        private void BtnTerminal_Click(object sender, RoutedEventArgs e) => SetActiveTab("terminal");
        private void BtnClaude_Click(object sender, RoutedEventArgs e) => SetActiveTab("claude");
        private void BtnCodex_Click(object sender, RoutedEventArgs e) => SetActiveTab("codex");

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                switch (e.Key)
                {
                    case Key.D1:
                        SetActiveTab("terminal");
                        e.Handled = true;
                        break;
                    case Key.D2:
                        SetActiveTab("claude");
                        e.Handled = true;
                        break;
                    case Key.D3:
                        SetActiveTab("codex");
                        e.Handled = true;
                        break;
                }
            }
        }
    }
}
