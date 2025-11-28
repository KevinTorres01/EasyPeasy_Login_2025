using System.Diagnostics;

namespace EasyPeasy_Login.Infrastructure.Network.Configuration;

public class CommandExecutor : ICommandExecutor
{
    public async Task<ExecutionResult> ExecuteCommandAsync(string command, bool ignoreErrors = false)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "/bin/bash",
                Arguments = $"-c \"{command}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        process.Start();

        string output = await process.StandardOutput.ReadToEndAsync();
        string error = await process.StandardError.ReadToEndAsync();
        await process.WaitForExitAsync();
        var exit = process.ExitCode;
        var result = new ExecutionResult(exit, output, error);

        if (!ignoreErrors && !result.Success)
        {
            throw new InvalidOperationException($"Command failed: {command}\n{error}");
        }

        return result;
    }

    public async Task<ExecutionResult> ExecuteCommandWithOutput(string command)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "/bin/bash",
                Arguments = $"-c \"{command}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        process.Start();
        string output = await process.StandardOutput.ReadToEndAsync();
        string error = await process.StandardError.ReadToEndAsync();
        await process.WaitForExitAsync();

        return new ExecutionResult(process.ExitCode, output, error);
    }
}