using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace WinTerminal.Services
{
    public class TerminalService
    {
        private Process _process;
        private StringBuilder _outputBuffer;

        public event Action<string> OutputReceived;
        public event Action<int> ProcessExited;

        public void Start(string workingDirectory = null)
        {
            _outputBuffer = new StringBuilder();

            var psi = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = "/k",
                WorkingDirectory = workingDirectory ?? Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding = Encoding.UTF8
            };

            _process = new Process { StartInfo = psi, EnableRaisingEvents = true };
            _process.OutputDataReceived += OnOutputDataReceived;
            _process.ErrorDataReceived += OnErrorDataReceived;
            _process.Exited += OnProcessExited;

            _process.Start();
            _process.BeginOutputReadLine();
            _process.BeginErrorReadLine();
        }

        public void WriteInput(string input)
        {
            if (_process != null && !_process.HasExited)
            {
                _process.StandardInput.WriteLine(input);
            }
        }

        private void OnOutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                OutputReceived?.Invoke(e.Data + "\n");
            }
        }

        private void OnErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                OutputReceived?.Invoke("\x1b[31m" + e.Data + "\x1b[0m\n");
            }
        }

        private void OnProcessExited(object sender, EventArgs e)
        {
            ProcessExited?.Invoke(_process.ExitCode);
        }

        public void Stop()
        {
            if (_process != null && !_process.HasExited)
            {
                _process.Kill();
            }
        }
    }
}
