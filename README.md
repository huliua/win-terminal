# WinTerminal

A Windows terminal application built with WPF (.NET Framework 4.6) that supports Claude Code and Codex GUI integration. **Fully compatible with Windows 10 version 10240 (1507) and above.**

## Features

- **Terminal** — Built-in terminal with command execution, ANSI color support, and keyboard shortcuts
- **Claude Code** — Chat interface with Claude CLI and Anthropic API integration
- **Codex GUI** — Embedded WebBrowser for Codex with navigation controls
- **Windows 10 10240 Compatible** — Built with .NET Framework 4.6, no extra runtime needed

## Prerequisites

- Windows 10 version 10240 or later (64-bit)
- Visual Studio 2015+ or MSBuild 14.0+
- .NET Framework 4.6 (pre-installed on Windows 10)

## Build

### Using Visual Studio
1. Open `WinTerminal.sln` in Visual Studio
2. Set configuration to `Release`
3. Build → Build Solution (Ctrl+Shift+B)
4. Output: `WinTerminal\bin\Release\WinTerminal.exe`

### Using MSBuild (command line)
```cmd
"C:\Windows\Microsoft.NET\Framework64\v4.0.30319\MSBuild.exe" WinTerminal.sln /p:Configuration=Release
```

### Using the build script
```cmd
build.bat
```

## Usage

### Terminal
- Type `help` for available commands
- Any unrecognized command is executed via `cmd.exe`
- Keyboard shortcuts:
  - `Ctrl+1` — Terminal view
  - `Ctrl+2` — Claude Code view
  - `Ctrl+3` — Codex GUI view

### Claude Code
Two modes supported (auto-detected):
1. **CLI mode** — Install Claude CLI: `npm install -g @anthropic-ai/claude-code`
2. **API mode** — Set environment variable: `set ANTHROPIC_API_KEY=your-key`

### Codex GUI
- Embedded browser with back/forward/reload controls
- URL bar for direct navigation
- Default: https://chat.openai.com

## Project Structure

```
WinTerminal/
├── WinTerminal.sln          # Solution file
├── build.bat                # Build script
├── README.md
└── WinTerminal/
    ├── WinTerminal.csproj   # Project file (.NET 4.6)
    ├── App.config
    ├── App.xaml / App.xaml.cs
    ├── MainWindow.xaml / .cs
    ├── Properties/
    │   └── AssemblyInfo.cs
    ├── Controls/
    │   ├── TerminalControl.xaml / .cs
    │   ├── ClaudeControl.xaml / .cs
    │   └── CodexControl.xaml / .cs
    ├── Services/
    │   ├── ClaudeService.cs
    │   └── TerminalService.cs
    └── ViewModels/
        └── MainViewModel.cs
```

## Why WPF instead of Electron?

Electron requires Windows 10 1709+ (build 16299) because Chromium dropped support for older versions. WPF with .NET Framework 4.6 runs on Windows 10 10240 natively with zero extra dependencies.

## License

MIT
