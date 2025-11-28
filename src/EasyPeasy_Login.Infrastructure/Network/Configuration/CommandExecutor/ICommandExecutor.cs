namespace EasyPeasy_Login.Infrastructure.Network.Configuration;

public interface ICommandExecutor
{
    Task<ExecutionResult> ExecuteCommandAsync(string command, bool ignoreErrors = false);
    Task<ExecutionResult> ExecuteCommandWithOutput(string command);
}