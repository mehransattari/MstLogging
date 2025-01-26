namespace Mst.Logging.CustomLogs;

using System.Collections;
using System.Text.Json;

public class ExceptionsCustom
{
    public string ExceptionType { get; set; }
    public string Message { get; set; }
    public string StackTrace { get; set; }
    public ExceptionsCustom? InnerException { get; set; }
    public IDictionary Data { get; set; }

    public string ToJson()
    {
        return JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
    }
}