using Mst.Logging.Enums;


namespace Mst.Logging.Test.Enums;

public class UnitTestLogLevelEnum
{
    [Theory]
    [InlineData(LogLevelEnum.Trace, 0)]
    [InlineData(LogLevelEnum.Debug, 1)]
    [InlineData(LogLevelEnum.Information, 2)]
    [InlineData(LogLevelEnum.Warning, 3)]
    [InlineData(LogLevelEnum.Error, 4)]
    [InlineData(LogLevelEnum.Critical, 5)]
    public void EnumValues_ShouldHaveExpectedNamesAndValues(LogLevelEnum logLevel, int expectedValue)
    {
        // Act & Assert  
        Assert.Equal(expectedValue, (int)logLevel);
        Assert.Equal(logLevel.ToString(), Enum.GetName(typeof(LogLevelEnum), logLevel));
    }

    [Fact]
    public void EnumCount_ShouldBeCorrect()
    {
        // Arrange  
        int expectedCount = 6;

        // Act  
        var actualCount = Enum.GetNames(typeof(LogLevelEnum)).Length;

        // Assert  
        Assert.Equal(expectedCount, actualCount);
    }
}
