using System.Diagnostics;

public class Logger : ILogger
{
    private readonly string _logPath;

    public Logger(string logFilePath)
    {
        _logPath = logFilePath;

        if (!Directory.Exists("Logs"))
        {
            Directory.CreateDirectory("Logs");
        }

        if (!File.Exists(_logPath))
        {
            File.Create(_logPath).Dispose();
        }
    }

    public void Log(string message, LogLevel level = LogLevel.Info)
    {
        var stackFrame = new StackFrame(1, true);
        var method = stackFrame.GetMethod();
        var className = method.DeclaringType?.FullName;
        var methodName = method.Name;
        var lineNumber = stackFrame.GetFileLineNumber();

        var logMessage = $"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} [{level}] {className}.{methodName} (Line: {lineNumber}) - {message}";
        WriteToFile(logMessage);
    }

    private void WriteToFile(string message)
    {
        using (StreamWriter writer = new StreamWriter(_logPath, true))
        {
            writer.WriteLine(message);
        }
    }
}

public enum LogLevel
{
    Info,
    Warning,
    Error,
    Critical
}
