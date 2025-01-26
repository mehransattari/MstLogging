using Microsoft.AspNetCore.Http;
using System.Collections;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Text;
using Mst.Logging.Enums;
using Mst.Logging.CustomLogs;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;


namespace Mst.Logging.Logger;

public abstract class Logger<T> : ILogger<T>
{
    #region Constructor
    protected Logger(IHttpContextAccessor httpContextAccessor = null)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected IHttpContextAccessor _httpContextAccessor { get; }

    #endregion

    #region Abstract Methods
    protected abstract void LogByFavoriteLibrary(Log log, Exception exception);

    #endregion

    #region Main Methods

    public void LogCritical(Exception? exception, string? message = "", 
                            Hashtable? parameters = null,
                            [CallerMemberName] string methodName = "",
                            object? DataObject = null)
    {        
        var methodBase = GetMethodBase();

        if (methodBase is not null)
        {
            Log(LogLevelEnum.Critical, methodBase, methodName, message, exception, parameters, DataObject);
        }
    }

    public void LogDebug(string message, Hashtable? parameters = null,
                        [CallerMemberName] string methodName = "",
                        object? DataObject = null)
    {
        var methodBase = GetMethodBase();
        if (methodBase is not null)
        {
            Log(LogLevelEnum.Debug, methodBase, methodName, message, null, parameters, DataObject);
        }
    }

    public void LogError(Exception? exception, string? message = "",
                         Hashtable? parameters = null, [CallerMemberName] string methodName = "",
                         object? DataObject = null)
    {
        var methodBase = GetMethodBase();
        if (methodBase is not null)
        {
            Log(LogLevelEnum.Error, methodBase, methodName, message, exception, parameters, DataObject);
        }
    }

    public void LogInformation(string message, Hashtable? parameters = null,
                              [CallerMemberName] string methodName = "",
                              object? DataObject = null)
    {
        var methodBase = GetMethodBase();
        if (methodBase is not null)
        {
            Log(LogLevelEnum.Information, methodBase, methodName, message, null, parameters, DataObject);
        }
    }

    public void LogTrace(string message, Hashtable? parameters = null,
                        [CallerMemberName] string methodName = "",
                        object? DataObject = null)
    {
        var methodBase = GetMethodBase();
        if (methodBase is not null)
        {
            Log(LogLevelEnum.Trace, methodBase, methodName, message, null, parameters, DataObject);
        }
    }

    public void LogWarning(string message, Hashtable? parameters = null,
                          [CallerMemberName] string methodName = "",
                          object? DataObject = null)
    {
        var methodBase = GetMethodBase();
        if (methodBase is not null)
        {
            Log(LogLevelEnum.Warning, methodBase, methodName, message, null, parameters, DataObject);
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
                       string methodNameReceive,
                       string message,
                       Exception? exception,
                       Hashtable? parameters, 
                       object? DataObject = null)
    {
        if (exception == null && string.IsNullOrWhiteSpace(message))      
            return;      

        //Convert currentCulture to english
        var currentCultureInfo = SetCurrentCulture();

        var log = SetLog(logLevel, methodBase, methodNameReceive, message, exception, parameters, DataObject);

        //Log main
        LogByFavoriteLibrary(log: log, exception: exception);

        //Convert currentCulture to before CurrentCulture
        if (currentCultureInfo != null)
            Thread.CurrentThread.CurrentCulture = currentCultureInfo;
    }

    public Log SetLog(LogLevelEnum logLevel,
                       MethodBase methodBase,
                       string methodNameReceive,
                       string message,
                       Exception? exception,
                       Hashtable? parameters, 
                       object? DataObject = null)
    {
        var log = new Log
        {
            Level = logLevel,
            ClassName = typeof(T).Name,
            MethodName = methodBase.Name,
            MethodNameReceive = methodNameReceive,
            Namespace = typeof(T).Namespace ?? "Unknown Namespace",
            Message = message,
            Exceptions = GetExceptionsCustom(exception),
            Parameters = GetParameters(parameters),
            Data = DataObject is not null ? JsonSerializer.Serialize(DataObject) : null
        };

        if (_httpContextAccessor?.HttpContext != null)
        {
            log.RemoteIP = _httpContextAccessor.HttpContext.Connection?.RemoteIpAddress?.ToString() ?? "Unknown RemoteIP";
            log.Username = _httpContextAccessor.HttpContext.User?.Identity?.Name ?? "Unknown Username";
            log.RequestPath = _httpContextAccessor.HttpContext.Request?.Path ?? "Unknown RequestPath";
            log.HttpReferrer = _httpContextAccessor.HttpContext.Request?.Headers["Referer"].ToString() ?? "Unknown HttpReferrer";
        }

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

    public ExceptionsCustom? GetExceptionsCustom(Exception? exception)
    {
        if (exception is null)
            return null;

        var exceptionInfo = new ExceptionsCustom
        {
            ExceptionType = exception.GetType().Name,
            Message = exception.Message,
            StackTrace = exception?.StackTrace?? "UNKNOWN",
            InnerException = exception?.InnerException != null ? GetExceptionsCustom(exception.InnerException) : null,
            Data = exception.Data.Count > 0 ? exception.Data : null
        };

        return exceptionInfo;
    }

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
