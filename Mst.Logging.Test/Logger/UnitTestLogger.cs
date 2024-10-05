using Microsoft.AspNetCore.Http;
using Moq;
using Mst.Logging.CustomLogs;
using Mst.Logging.Enums;
using Mst.Logging.Logger;
using System.Collections;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace Mst.Logging.Test.Logger;

public class MockLogger : Logger<object>
{
    public MockLogger(IHttpContextAccessor httpContextAccessor = null) : base(httpContextAccessor)
    {
    }

    protected override void LogByFavoriteLibrary(Log log, Exception exception)
    {
        // Mock implementation for testing
    }
}

public class UnitTestLogger
{
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private readonly MockLogger _mockLogger;

    public UnitTestLogger()
    {
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        _mockLogger = new MockLogger(_httpContextAccessorMock.Object);
    }

    #region SetCurrentCulture

    [Fact]
    public void SetCurrentCulture_Should_Change_ThreadCulture_To_enUS_And_Return_Previous_Culture()
    {
        // Arrange
        var previousCulture = Thread.CurrentThread.CurrentCulture; // فرهنگ فعلی قبل از تغییر
        var logger = new MockLogger();

        // Act
        var returnedCulture = logger.SetCurrentCulture();

        // Asserts
        Assert.NotNull(returnedCulture);
        Assert.Equal(previousCulture.Name, returnedCulture.Name);
        Assert.Equal("en-US", Thread.CurrentThread.CurrentCulture.Name);
        Thread.CurrentThread.CurrentCulture = previousCulture;
    }


    #endregion

    #region AppendExceptionMessage

    [Fact]
    public void AppendExceptionMessage_Should_Use_Exception_Tag_For_First_Exception()
    {
        //Arrange
        var logger = new MockLogger();
        var exception = new Exception("Test exception message");
        var result = new StringBuilder();
        int index = 0;

        //Act
        logger.AppendExceptionMessage(result, exception, index);

        //Asserts
        string expectedOutput = "<Exception>Test exception message</Exception>";
        Assert.Equal(expectedOutput, result.ToString());
    }


    #endregion

    #region GetParameters

    [Fact]
    public void GetParameters_Should_Parameters_IsNull_Return_Empty()
    {
        //Arrange
        var logger = new MockLogger();
        Hashtable? parameters = null;

        //Act
        var result = logger.GetParameters(parameters);

        //Asserts
        Assert.Equal(result, string.Empty);
    }

    [Fact]
    public void GetParameters_Should_Count_Parameters_IsZero_Return_Empty()
    {
        //Arrange
        var logger = new MockLogger();
        Hashtable? parameters = new Hashtable();

        //Act
        var result = logger.GetParameters(parameters);

        //Asserts
        Assert.Equal(result, string.Empty);
    }

    [Fact]
    public void GetParameters_Should_Have_One_Parameter_Return_FormattedString()
    {
        //Arrange  
        var logger = new MockLogger();
        Hashtable parameters = new Hashtable
        {
            {"param1", "value1"}
        };

        //Act  
        var result = logger.GetParameters(parameters);

        //Asserts  
        var expected = "<parameter><key>param1</key><value>value1</value></parameter>";
        Assert.Equal(expected, result);
    }

    [Fact]
    public void GetParameters_Should_Have_Null_Value_Parameter_Return_FormattedString()
    {
        //Arrange  
        var logger = new MockLogger();
        Hashtable parameters = new Hashtable
        {
            {"param2", null}
        };

        //Act  
        var result = logger.GetParameters(parameters);

        //Asserts  
        var expected = "<parameter><key>param2</key><value>NULL</value></parameter>";
        Assert.Equal(expected, result);
    }

    #endregion

    #region AppendParameter

    [Fact]
    public void AppendParameter_Should_Return_FilledParameter()
    {
        //Arrange
        var logger = new MockLogger();
        var stringBuilder = new StringBuilder();
        object key = "key1";
        object? value = "value1";

        //Act
        logger.AppendParameter(stringBuilder, key, value);

        //Asserts
        var expected = "<parameter><key>key1</key><value>value1</value></parameter>";
        Assert.Equal(expected, stringBuilder.ToString());
    }

    [Fact]
    public void AppendParameter_Should_Return_NullValueParameter()
    {
        // Arrange
        var logger = new MockLogger();
        var stringBuilder = new StringBuilder();
        object key = "key2";
        object? value = null;

        // Act
        logger.AppendParameter(stringBuilder, key, value);

        // Assert
        var expected = "<parameter><key>key2</key><value>NULL</value></parameter>";
        Assert.Equal(expected, stringBuilder.ToString());
    }
    #endregion

    #region GetExceptions
    [Fact]
    public void GetExceptions_Should_Is_Null_Exception_Return_Empty()
    {
        //Arrange
        var logger = new MockLogger();

        //Act
        var result = logger.GetExceptions(null);

        //Assert
        Assert.Empty(result);
    }

    [Fact]
    public void GetExceptions_Should_Return_Correctly_Formatted_Exception_When_Exception_Is_Not_Null()
    {
        // Arrange  
        var logger = new MockLogger();
        var exception = new Exception("The parameter cannot be null.");
     
        // Act  
        var result = logger.GetExceptions(exception);

        // Assert  
        Assert.Contains("<Exception>The parameter cannot be null.</Exception>", result);
    }



    #endregion

    #region SetLog
    [Fact]
    public void SetLog_Should_Set_Properties_Correctly()
    {
        // Arrange  
        var methodBase = MethodBase.GetCurrentMethod();
        var message = "Test Log Message";
        var exception = new Exception("Test Exception");
        var parameters = new Hashtable { { "Key", "Value" } };

        var httpContext = new DefaultHttpContext();
        httpContext.Connection.RemoteIpAddress = System.Net.IPAddress.Parse("127.0.0.1");
        httpContext.User = new System.Security.Claims.ClaimsPrincipal(new System.Security.Claims.ClaimsIdentity(new[]
        {
            new System.Security.Claims.Claim("name", "TestUser")
        }));
        httpContext.Request.Path = "/test/path";
        httpContext.Request.Headers["Referer"] = "http://localhost";

        _httpContextAccessorMock.Setup(_ => _.HttpContext).Returns(httpContext);

        var logger = new MockLogger(_httpContextAccessorMock.Object);

        // Act  
        var log = logger.SetLog(LogLevelEnum.Information, methodBase, message, exception, parameters);

        // Assert  
        Assert.Equal(LogLevelEnum.Information, log.Level);
        Assert.Equal("Object", log.ClassName);
        Assert.Equal(methodBase.Name, log.MethodName);
        Assert.Equal(message, log.Message);
        Assert.Equal("<Exception>Test Exception</Exception>", log.Exceptions);
        Assert.Equal("<parameter><key>Key</key><value>Value</value></parameter>", log.Parameters);
        Assert.Equal("127.0.0.1", log.RemoteIP);
        Assert.Equal("Unknown Username", log.Username);
        Assert.Equal("/test/path", log.RequestPath);
        Assert.Equal("http://localhost", log.HttpReferrer);
    }

    [Fact]
    public void SetLog_Should_Honor_Null_Exception()
    {
        // Arrange  
        var methodBase = MethodBase.GetCurrentMethod();
        var message = "Test Log Message";

        var httpContext = new DefaultHttpContext();
        _httpContextAccessorMock.Setup(_ => _.HttpContext).Returns(httpContext);

        var logger = new MockLogger(_httpContextAccessorMock.Object);

        // Act  
        var log = logger.SetLog(LogLevelEnum.Information, methodBase, message, null, null);

        // Assert  
        Assert.Equal(string.Empty, log.Exceptions);
    }
    #endregion

    #region Log  

    [Fact]
    public void Log_Should_CurrentCulture_SetEnglish()
    {
        // Arrange  
        var methodBase = MethodBase.GetCurrentMethod();
        var message = "Test Log Message";
        var previousCulture = Thread.CurrentThread.CurrentCulture;
        var httpContext = new DefaultHttpContext();
        _httpContextAccessorMock.Setup(_ => _.HttpContext).Returns(httpContext);

        var logger = new MockLogger(_httpContextAccessorMock.Object);

        // Act  
        logger.Log(LogLevelEnum.Information, methodBase, message, null, null);

        //Asserts
        var expected = new CultureInfo(name: "en-US");
        Assert.Equal(expected, Thread.CurrentThread.CurrentCulture);
    }
    #endregion

    #region GetMethodBase
    [Fact]
    public void GetMethodBase_Should_Return_MethodBase_For_Calling_Method()
    {
        // Arrange  
        var stackTrace = new StackTrace();

        // Act  
        var methodBase = _mockLogger.GetMethodBase(); 

        // Assert  
        Assert.NotNull(methodBase);  
        Assert.Equal("GetMethodBase_Should_Return_MethodBase_For_Calling_Method", methodBase.Name); 
    }
    #endregion

    #region LogCritical

   
    #endregion
}
