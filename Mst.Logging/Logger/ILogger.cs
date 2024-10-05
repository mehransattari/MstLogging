using System.Collections;

namespace Mst.Logging.Logger;

public interface ILogger<T>
{
    void LogTrace(string message, Hashtable? parameters = null);

    void LogDebug(string message, Hashtable? parameters = null);

    void LogInformation(string message, Hashtable? parameters = null);

    void LogWarning(string message, Hashtable? parameters = null);

    void LogError(Exception? exception, string? message=null, Hashtable? parameters = null);

    void LogCritical(Exception? exception, string? message = null, Hashtable? parameters = null);
}

