namespace EasyPeasy_Login.Infrastructure.Network.Configuration;

public interface ICommandExecutor
{
    Task<Models.ExecutionResult> ExecuteCommandAsync(string command, bool ignoreErrors = false);
    Task<Models.ExecutionResult> ExecuteCommandWithOutput(string command);
}