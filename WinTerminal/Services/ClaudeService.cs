using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace WinTerminal.Services
{
    public class ClaudeService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiEndpoint = "https://api.anthropic.com/v1/messages";
        private readonly string _apiKey;
        private readonly JavaScriptSerializer _json;

        public ClaudeService()
        {
            _httpClient = new HttpClient();
            _apiKey = Environment.GetEnvironmentVariable("ANTHROPIC_API_KEY");
            _json = new JavaScriptSerializer();
        }

        public async Task<ClaudeStatus> CheckAvailabilityAsync()
        {
            var status = new ClaudeStatus();

            // Check CLI
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "claude",
                    Arguments = "--version",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (var process = Process.Start(psi))
                {
                    string output = await process.StandardOutput.ReadToEndAsync();
                    await process.WaitForExitAsync();

                    if (process.ExitCode == 0)
                    {
                        status.CliAvailable = true;
                        status.Version = output.Trim();
                        status.IsAvailable = true;
                    }
                }
            }
            catch
            {
                // CLI not available
            }

            // Check API
            if (!string.IsNullOrEmpty(_apiKey))
            {
                status.ApiAvailable = true;
                status.IsAvailable = true;
            }

            return status;
        }

        public async Task<ClaudeResponse> SendPromptAsync(string prompt)
        {
            // Try CLI first
            if (await IsCliAvailableAsync())
            {
                return await ExecuteCliAsync(prompt);
            }

            // Fall back to API
            if (!string.IsNullOrEmpty(_apiKey))
            {
                return await CallApiAsync(prompt);
            }

            return new ClaudeResponse
            {
                Success = false,
                Error = "No Claude integration available. Please install Claude CLI or set ANTHROPIC_API_KEY"
            };
        }

        private async Task<bool> IsCliAvailableAsync()
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "claude",
                    Arguments = "--version",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (var process = Process.Start(psi))
                {
                    await process.WaitForExitAsync();
                    return process.ExitCode == 0;
                }
            }
            catch
            {
                return false;
            }
        }

        private async Task<ClaudeResponse> ExecuteCliAsync(string prompt)
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "claude",
                    Arguments = "--prompt \"" + prompt.Replace("\"", "\\\"") + "\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (var process = Process.Start(psi))
                {
                    string output = await process.StandardOutput.ReadToEndAsync();
                    string error = await process.StandardError.ReadToEndAsync();
                    await process.WaitForExitAsync();

                    if (process.ExitCode == 0)
                    {
                        return new ClaudeResponse
                        {
                            Success = true,
                            Content = output.Trim()
                        };
                    }
                    else
                    {
                        return new ClaudeResponse
                        {
                            Success = false,
                            Error = "CLI exited with code " + process.ExitCode + ": " + error
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                return new ClaudeResponse
                {
                    Success = false,
                    Error = ex.Message
                };
            }
        }

        private async Task<ClaudeResponse> CallApiAsync(string prompt)
        {
            try
            {
                // Build JSON manually to avoid Newtonsoft.Json dependency
                string escapedPrompt = EscapeJson(prompt);
                string jsonBody = "{\"model\":\"claude-sonnet-4-20250514\",\"max_tokens\":4096,\"messages\":[{\"role\":\"user\",\"content\":\"" + escapedPrompt + "\"}]}";

                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("x-api-key", _apiKey);
                _httpClient.DefaultRequestHeaders.Add("anthropic-version", "2023-06-01");

                var response = await _httpClient.PostAsync(_apiEndpoint, content);
                var responseJson = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    // Parse response manually
                    string text = ExtractContentText(responseJson);
                    return new ClaudeResponse
                    {
                        Success = true,
                        Content = text
                    };
                }
                else
                {
                    return new ClaudeResponse
                    {
                        Success = false,
                        Error = "API error " + (int)response.StatusCode + ": " + responseJson
                    };
                }
            }
            catch (Exception ex)
            {
                return new ClaudeResponse
                {
                    Success = false,
                    Error = ex.Message
                };
            }
        }

        private string ExtractContentText(string json)
        {
            // Simple extraction of "text":"..." from the response
            // Looking for the pattern in content array
            int textIdx = json.IndexOf("\"text\"");
            if (textIdx < 0) return json;

            int colonIdx = json.IndexOf(':', textIdx);
            if (colonIdx < 0) return json;

            int startQuote = json.IndexOf('"', colonIdx + 1);
            if (startQuote < 0) return json;

            int endQuote = json.IndexOf('"', startQuote + 1);
            if (endQuote < 0) return json;

            string raw = json.Substring(startQuote + 1, endQuote - startQuote - 1);
            return raw.Replace("\\n", "\n").Replace("\\\"", "\"").Replace("\\\\", "\\");
        }

        private string EscapeJson(string text)
        {
            return text
                .Replace("\\", "\\\\")
                .Replace("\"", "\\\"")
                .Replace("\n", "\\n")
                .Replace("\r", "\\r")
                .Replace("\t", "\\t");
        }
    }

    public class ClaudeStatus
    {
        public bool IsAvailable { get; set; }
        public bool CliAvailable { get; set; }
        public bool ApiAvailable { get; set; }
        public string Version { get; set; }
    }

    public class ClaudeResponse
    {
        public bool Success { get; set; }
        public string Content { get; set; }
        public string Error { get; set; }
    }

    public static class ProcessExtensions
    {
        public static Task WaitForExitAsync(this Process process)
        {
            var tcs = new TaskCompletionSource<bool>();
            process.EnableRaisingEvents = true;
            process.Exited += (sender, args) => tcs.TrySetResult(true);
            return tcs.Task;
        }
    }
}
