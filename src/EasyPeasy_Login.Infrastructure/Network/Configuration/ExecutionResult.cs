namespace EasyPeasy_Login.Infrastructure.Network.Configuration;

public record ExecutionResult(int ExitCode, string Output, string Error)
{
    public bool Success => ExitCode == 0;
    public string Combined => Output + "\n" + Error;
}