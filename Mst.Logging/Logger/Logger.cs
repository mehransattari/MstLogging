using Microsoft.AspNetCore.Http;
using System.Collections;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Text;
using Mst.Logging.Enums;
using Mst.Logging.CustomLogs;


namespace Mst.Logging.Logger;

public abstract class Logger<T> : ILogger<T>
{
    #region Constructor
    protected Logger(IHttpContextAccessor httpContextAccessor = null)
    {
        HttpContextAccessor = httpContextAccessor;
    }

    protected IHttpContextAccessor HttpContextAccessor { get; }

    #endregion

    #region Abstract Methods
    protected abstract void LogByFavoriteLibrary(Log log, Exception exception);

    #endregion

    #region Main Methods

    public void LogCritical(Exception? exception, string? message = null, 
                            Hashtable? parameters = null)
    {        
        var methodBase = GetMethodBase();

        if (methodBase is not null)
        {
            Log(LogLevelEnum.Critical, methodBase, message, null, parameters);
        }
    }

    public void LogDebug(string message, Hashtable? parameters = null)
    {
        var methodBase = GetMethodBase();
        if (methodBase is not null)
        {
            Log(LogLevelEnum.Debug, methodBase, message, null, parameters);
        }
    }

    public void LogError(Exception? exception, string? message = null, Hashtable? parameters = null)
    {
        var methodBase = GetMethodBase();
        if (methodBase is not null)
        {
            Log(LogLevelEnum.Error, methodBase, message, null, parameters);
        }
    }

    public void LogInformation(string message, Hashtable? parameters = null)
    {
        var methodBase = GetMethodBase();
        if (methodBase is not null)
        {
            Log(LogLevelEnum.Information, methodBase, message, null, parameters);
        }
    }

    public void LogTrace(string message, Hashtable? parameters = null)
    {
        var methodBase = GetMethodBase();
        if (methodBase is not null)
        {
            Log(LogLevelEnum.Trace, methodBase, message, null, parameters);
        }
    }

    public void LogWarning(string message, Hashtable? parameters = null)
    {
        var methodBase = GetMethodBase();
        if (methodBase is not null)
        {
            Log(LogLevelEnum.Warning, methodBase, message, null, parameters);
        }
    }

    #endregion

    #region Private Methods

    public MethodBase? GetMethodBase()
    {
        var stackTrace = new StackTrace();
        var frame = stackTrace.GetFrame(1); // فریم دوم  

        // بررسی اینکه آیا فریم موجود است  
        if (frame != null)
        {
            return frame.GetMethod();
        }

        return null; // در صورتی که فریم موجود نباشد 
    }

    public void Log(LogLevelEnum logLevel,
                       MethodBase methodBase,
                       string message,
                       Exception? exception,
                       Hashtable? parameters)
    {
        if (exception == null && string.IsNullOrWhiteSpace(message))
        {
            return;
        }

        //Convert currentCulture to english
        var currentCultureInfo = SetCurrentCulture();

        var log = SetLog(logLevel, methodBase, message, exception, parameters);

        //Log main
        LogByFavoriteLibrary(log: log, exception: exception);

        //Convert currentCulture to before CurrentCulture
        if (currentCultureInfo != null)
            Thread.CurrentThread.CurrentCulture = currentCultureInfo;
    }

    public Log SetLog(LogLevelEnum logLevel,
                       MethodBase methodBase,
                       string message,
                       Exception? exception,
                       Hashtable? parameters)
    {
        Log log = new Log();

        log.Level = logLevel;
        log.ClassName = typeof(T).Name;
        log.MethodName = methodBase.Name;
        log.Namespace = typeof(T).Namespace ?? "Unknown Namespace";
        log.Message = message;
        log.Exceptions = GetExceptions(exception: exception);
        log.Parameters = GetParameters(parameters: parameters);

        log.RemoteIP = HttpContextAccessor?.HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? "Unknown RemoteIP";
        log.Username = HttpContextAccessor?.HttpContext?.User?.Identity?.Name ?? "Unknown Username";
        log.RequestPath = HttpContextAccessor?.HttpContext?.Request?.Path ?? "Unknown RequestPath";
        log.HttpReferrer = HttpContextAccessor?.HttpContext?.Request?.Headers["Referer"] ?? "Unknown HttpReferrer";

        return log;
    }

    public CultureInfo? SetCurrentCulture()
    {
        var currentCultureName = Thread.CurrentThread.CurrentCulture.Name;

        var newCultureInfo = new CultureInfo(name: "en-US");

        var currentCultureInfo = new CultureInfo(currentCultureName);

        Thread.CurrentThread.CurrentCulture = newCultureInfo;

        return currentCultureInfo;
    }

    #endregion

    #region virtual Methods

    public virtual string GetExceptions(Exception? exception)
    {
        if (exception == null)
        {
            return string.Empty;
        }

        var result = new StringBuilder();
        var currentException = exception;
        int index = 0;

        while (currentException != null)
        {
            AppendExceptionMessage(result, currentException, index);
            currentException = currentException.InnerException;
            index++;
        }

        return result.ToString();
    }

    public void AppendExceptionMessage(StringBuilder result, Exception exception, int index)
    {
        string exceptionTag = index == 0 ? nameof(Exception) : nameof(Exception.InnerException);
        result.Append($"<{exceptionTag}>{exception.Message}</{exceptionTag}>");
    }

    public virtual string GetParameters(Hashtable? parameters)
    {
        if (parameters == null || parameters.Count == 0)
        {
            return string.Empty;
        }

        var stringBuilder = new StringBuilder();

        foreach (DictionaryEntry item in parameters)
        {
            if (item.Key != null)
            {
                AppendParameter(stringBuilder, item.Key, item.Value);
            }
        }

        return stringBuilder.ToString();
    }

    public void AppendParameter(StringBuilder stringBuilder, object key, object? value)
    {
        stringBuilder.Append("<parameter>");
        stringBuilder.Append($"<key>{key}</key>");
        stringBuilder.Append($"<value>{(value == null ? "NULL" : value.ToString())}</value>");
        stringBuilder.Append("</parameter>");
    }

    #endregion
}
