using System.Collections;
using System.Runtime.CompilerServices;

namespace Mst;

public interface ILogger<T>
{
    void LogTrace(string message, Hashtable? parameters = null,
        [CallerMemberName] string methodName = "", object? DataObject = null);

    void LogDebug(string message, Hashtable? parameters = null,
        [CallerMemberName] string methodName = "", object? DataObject = null);

    void LogInformation(string message, Hashtable? parameters = null,
        [CallerMemberName] string methodName = "",object? DataObject = null);

    void LogWarning(string message, Hashtable? parameters = null, 
        [CallerMemberName] string methodName = "", object? DataObject = null);

    void LogError(Exception? exception, string? message=null, Hashtable? parameters = null,
        [CallerMemberName] string methodName = "", object? DataObject = null);

    void LogCritical(Exception? exception, string? message = null, Hashtable? parameters = null, 
        [CallerMemberName] string methodName = "", object? DataObject = null);
}

