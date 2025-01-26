using Mst.Logging.Enums;
using System.Text.Json;

namespace Mst.Logging.CustomLogs;

public class Log : ILog
{
    public LogLevelEnum Level { get; set; }

    public string? Namespace { get; set; }
    public string? ClassName { get; set; }
    public string? MethodName { get; set; }
    public string? MethodNameReceive { get; set; }

    public string? RemoteIP { get; set; }
    public string? Username { get; set; }
    public string? RequestPath { get; set; }
    public string? HttpReferrer { get; set; }

    public string? Message { get; set; }
    public string? Parameters { get; set; }
    public ExceptionsCustom? Exceptions { get; set; }

    public Object Data { get; set; }

    public string ToJson()
    {
        return JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
    }

    public override string ToString()
    {
        var logObject = new
        {
            Level = Enum.GetName(Level) ?? "UNKNOWN",
            Namespace = GetValueOrNull(Namespace),
            ClassName = GetValueOrNull(ClassName),
            MethodName = GetValueOrNull(MethodName),
            MethodNameReceive = GetValueOrNull(MethodNameReceive),
            RemoteIP = GetValueOrNull(RemoteIP),
            RequestPath = GetValueOrNull(RequestPath),
            HttpReferrer = GetValueOrNull(HttpReferrer),
            Username = GetValueOrNull(Username),
            Message = GetValueOrNull(Message),
            Exceptions = Exceptions, 
            Parameters = GetValueOrNull(Parameters),
            Data = Data,
        };

        var result = logObject.ToJson();

        return result;
    }

    private string GetValueOrNull(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? "NULL" : value;
    }
}

public static class ObjectExtensions
{
    public static string ToJson(this object obj)
    {
        if (obj == null)
            return "{}"; // بازگرداندن یک JSON خالی اگر شیء null باشد

        return JsonSerializer.Serialize(obj, new JsonSerializerOptions
        {
            WriteIndented = true // برای خوانایی بهتر JSON
        });
    }
}

