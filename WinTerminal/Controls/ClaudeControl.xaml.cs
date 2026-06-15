using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WinTerminal.Services;

namespace WinTerminal.Controls
{
    public partial class ClaudeControl : UserControl
    {
        private readonly ClaudeService _claudeService;
        private readonly ObservableCollection<ChatMessage> _messages;
        private bool _isSending;

        public ClaudeControl()
        {
            InitializeComponent();
            _claudeService = new ClaudeService();
            _messages = new ObservableCollection<ChatMessage>();
            MessagesList.ItemsSource = _messages;
            Loaded += ClaudeControl_Loaded;
        }

        private async void ClaudeControl_Loaded(object sender, RoutedEventArgs e)
        {
            var status = await _claudeService.CheckAvailabilityAsync();
            if (status.IsAvailable)
            {
                StatusText.Text = status.CliAvailable ? "CLI: " + status.Version : "API Available";
                StatusText.Foreground = System.Windows.Media.Brushes.LimeGreen;
            }
            else
            {
                StatusText.Text = "Not configured - set ANTHROPIC_API_KEY or install Claude CLI";
                StatusText.Foreground = System.Windows.Media.Brushes.IndianRed;
            }
        }

        private async void SendBtn_Click(object sender, RoutedEventArgs e)
        {
            await SendMessageAsync();
        }

        private async void InputBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                await SendMessageAsync();
                e.Handled = true;
            }
        }

        private async System.Threading.Tasks.Task SendMessageAsync()
        {
            string message = InputBox.Text.Trim();
            if (string.IsNullOrEmpty(message) || _isSending) return;

            var status = await _claudeService.CheckAvailabilityAsync();
            if (!status.IsAvailable)
            {
                AddMessage("Claude Code is not configured. Please set ANTHROPIC_API_KEY environment variable or install Claude CLI.", false, true);
                return;
            }

            AddMessage(message, true);
            InputBox.Text = "";
            _isSending = true;
            SendBtn.IsEnabled = false;
            SendBtn.Content = "Sending...";

            try
            {
                var response = await _claudeService.SendPromptAsync(message);
                if (response.Success)
                {
                    AddMessage(response.Content, false);
                }
                else
                {
                    AddMessage("Error: " + response.Error, false, true);
                }
            }
            catch (Exception ex)
            {
                AddMessage("Error: " + ex.Message, false, true);
            }
            finally
            {
                _isSending = false;
                SendBtn.IsEnabled = true;
                SendBtn.Content = "Send";
            }
        }

        private void AddMessage(string text, bool isUser, bool isError = false)
        {
            _messages.Add(new ChatMessage
            {
                Text = text,
                IsUser = isUser,
                IsError = isError,
                Alignment = isUser ? HorizontalAlignment.Right : HorizontalAlignment.Left
            });
        }
    }

    public class ChatMessage : INotifyPropertyChanged
    {
        public string Text { get; set; }
        public bool IsUser { get; set; }
        public bool IsError { get; set; }
        public HorizontalAlignment Alignment { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
