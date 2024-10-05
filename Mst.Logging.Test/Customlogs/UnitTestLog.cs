using Mst.Logging.CustomLogs;
using Mst.Logging.Enums;


namespace Mst.Logging.Test.Customlogs;

public class UnitTestLog
{
  
    [Fact]
    public void Log_ShouldSetAndGetProperties()
    {
        // Arrange  
        var log = new Log
        {
            Level = LogLevelEnum.Information,
            Namespace = "Mst.Logging",
            ClassName = "Log",
            MethodName = "Log_ShouldSetAndGetProperties",
            RemoteIP = "192.168.1.1",
            Username = "test_user",
            RequestPath = "/api/logs",
            HttpReferrer = "http://example.com",
            Message = "Test log message",
            Parameters = "param1=value1;param2=value2",
            Exceptions = "NullReferenceException"
        };

        // Act & Assert  
        Assert.Equal(LogLevelEnum.Information, log.Level);
        Assert.Equal("Mst.Logging", log.Namespace);
        Assert.Equal("Log", log.ClassName);
        Assert.Equal("Log_ShouldSetAndGetProperties", log.MethodName);
        Assert.Equal("192.168.1.1", log.RemoteIP);
        Assert.Equal("test_user", log.Username);
        Assert.Equal("/api/logs", log.RequestPath);
        Assert.Equal("http://example.com", log.HttpReferrer);
        Assert.Equal("Test log message", log.Message);
        Assert.Equal("param1=value1;param2=value2", log.Parameters);
        Assert.Equal("NullReferenceException", log.Exceptions);
    }

    [Fact]
    public void Log_ToString_ShouldReturnFormattedString()
    {
        // Arrange  
        var log = new Log
        {
            Level = LogLevelEnum.Error,
            Namespace = "Mst.Logging",
            ClassName = "Log",
            MethodName = "Log_ToString_ShouldReturnFormattedString",
            RemoteIP = "192.168.1.1",
            Username = "test_user",
            RequestPath = "/api/logs",
            HttpReferrer = "http://example.com",
            Message = "Test log message",
            Parameters = null, // Test case for null  
            Exceptions = "Some exception occurred"
        };

        // Act  
        string result = log.ToString();

        // Assert  
        Assert.Contains("<Level>Error</Level>", result);
        Assert.Contains("<Namespace>Mst.Logging</Namespace>", result);
        Assert.Contains("<ClassName>Log</ClassName>", result);
        Assert.Contains("<MethodName>Log_ToString_ShouldReturnFormattedString</MethodName>", result);
        Assert.Contains("<RemoteIP>192.168.1.1</RemoteIP>", result);
        Assert.Contains("<Username>test_user</Username>", result);
        Assert.Contains("<RequestPath>/api/logs</RequestPath>", result);
        Assert.Contains("<HttpReferrer>http://example.com</HttpReferrer>", result);
        Assert.Contains("<Message>Test log message</Message>", result);
        Assert.Contains("<Parameters>NULL</Parameters>", result); // Check for null case  
        Assert.Contains("<Exceptions>Some exception occurred</Exceptions>", result);
    }
}