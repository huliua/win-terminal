using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace WinTerminal.Controls
{
    public partial class TerminalControl : UserControl
    {
        private Process _shellProcess;
        private string _inputBuffer = "";

        public TerminalControl()
        {
            InitializeComponent();
            Loaded += TerminalControl_Loaded;
        }

        private void TerminalControl_Loaded(object sender, RoutedEventArgs e)
        {
            AppendOutput("\x1b[32mWelcome to WinTerminal\x1b[0m\n");
            AppendOutput("\x1b[36mWindows 10 Compatible Terminal with AI Integration\x1b[0m\n\n");
            AppendOutput("Type \x1b[33mhelp\x1b[0m for available commands\n");
            FocusInput();
        }

        private void InputBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string command = InputBox.Text;
                InputBox.Text = "";
                ProcessCommand(command);
                e.Handled = true;
            }
        }

        private void ProcessCommand(string command)
        {
            AppendOutput("$ " + command + "\n");

            string cmd = command.Trim().ToLower();

            switch (cmd)
            {
                case "help":
                    AppendOutput("\n\x1b[33mAvailable commands:\x1b[0m\n");
                    AppendOutput("  \x1b[36mhelp\x1b[0m     - Show this help\n");
                    AppendOutput("  \x1b[36mclear\x1b[0m    - Clear terminal\n");
                    AppendOutput("  \x1b[36mclaude\x1b[0m   - Switch to Claude Code\n");
                    AppendOutput("  \x1b[36mcodex\x1b[0m    - Switch to Codex GUI\n");
                    AppendOutput("  \x1b[36mstatus\x1b[0m   - Show integration status\n");
                    AppendOutput("  \x1b[36mexit\x1b[0m     - Close terminal\n");
                    AppendOutput("\n\x1b[33mKeyboard shortcuts:\x1b[0m\n");
                    AppendOutput("  \x1b[36mCtrl+1\x1b[0m   - Terminal view\n");
                    AppendOutput("  \x1b[36mCtrl+2\x1b[0m   - Claude Code view\n");
                    AppendOutput("  \x1b[36mCtrl+3\x1b[0m   - Codex GUI view\n");
                    AppendOutput("  \x1b[36mCtrl+L\x1b[0m   - Clear terminal\n");
                    break;

                case "clear":
                    OutputText.Text = "";
                    break;

                case "claude":
                    SwitchToClaude();
                    break;

                case "codex":
                    SwitchToCodex();
                    break;

                case "status":
                    AppendOutput("\n\x1b[33mIntegration Status:\x1b[0m\n");
                    AppendOutput("  Claude Code: \x1b[32mAvailable\x1b[0m\n");
                    AppendOutput("  Codex GUI: \x1b[32mAvailable\x1b[0m\n\n");
                    break;

                case "exit":
                    AppendOutput("Closing terminal...\n");
                    Application.Current.Shutdown();
                    break;

                default:
                    if (!string.IsNullOrEmpty(cmd))
                    {
                        // Try to execute as system command
                        ExecuteSystemCommand(command);
                    }
                    break;
            }

            ScrollToBottom();
        }

        private void ExecuteSystemCommand(string command)
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = "/c " + command,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (var process = Process.Start(psi))
                {
                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();
                    process.WaitForExit();

                    if (!string.IsNullOrEmpty(output))
                        AppendOutput(output);
                    if (!string.IsNullOrEmpty(error))
                        AppendOutput("\x1b[31m" + error + "\x1b[0m");
                }
            }
            catch (Exception ex)
            {
                AppendOutput("\x1b[31mError: " + ex.Message + "\x1b[0m\n");
            }
        }

        private void SwitchToClaude()
        {
            var mainWindow = Window.GetWindow(this) as MainWindow;
            if (mainWindow != null)
            {
                // Find the Claude button and click it
                var btn = mainWindow.FindName("BtnClaude") as Button;
                if (btn != null)
                    btn.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            }
        }

        private void SwitchToCodex()
        {
            var mainWindow = Window.GetWindow(this) as MainWindow;
            if (mainWindow != null)
            {
                var btn = mainWindow.FindName("BtnCodex") as Button;
                if (btn != null)
                    btn.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            }
        }

        private void AppendOutput(string text)
        {
            // Simple ANSI color code parser
            text = ParseAnsiColors(text);
            OutputText.Text += text;
        }

        private string ParseAnsiColors(string text)
        {
            // Remove ANSI escape codes for now (WPF TextBlock doesn't support them natively)
            // In a full implementation, you'd parse these and apply TextBlock formatting
            return System.Text.RegularExpressions.Regex.Replace(text, @"\x1b\[[0-9;]*m", "");
        }

        private void ScrollToBottom()
        {
            OutputScroll.ScrollToEnd();
        }

        private void FocusInput()
        {
            InputBox.Focus();
        }
    }
}
