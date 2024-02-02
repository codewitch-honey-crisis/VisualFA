namespace IntegrationTests; 
using Xunit;

public class EnumExtensionsTests
{
    [Theory]
    [InlineData("the 10 quick brown #@%$! foxes jumped over 1.5 lazy dogs")]
    public void GeneratedString(string value)
    {
        var matches = new List<FAMatch>();
        matches.AddRange(TestSource.CalcStringRunner(value));
        Assert.True(TestSource.EqualsMatches(matches,TestSource.Test1.Value));
    }
    [Theory]
    [InlineData("the 10 quick brown #@%$! foxes jumped over 1.5 lazy dogs")]
    public void GeneratedStringTable(string value)
    {
        var matches = new List<FAMatch>();
        matches.AddRange(TestSource.CalcStringTableRunner(value));
        Assert.True(TestSource.EqualsMatches(matches, TestSource.Test1.Value));
    }
    [Theory]
    [InlineData("the 10 quick brown #@%$! foxes jumped over 1.5 lazy dogs")]
    public void GeneratedTextReader(string value)
    {
        var matches = new List<FAMatch>();
        matches.AddRange(TestSource.CalcTextReaderRunner(new StringReader(value)));
        Assert.True(TestSource.EqualsMatches(matches, TestSource.Test1.Value));
    }
    [Theory]
    [InlineData("the 10 quick brown #@%$! foxes jumped over 1.5 lazy dogs")]
    public void GeneratedTextReaderTable(string value)
    {
        var matches = new List<FAMatch>();
        matches.AddRange(TestSource.CalcTextReaderTableRunner(new StringReader(value)));
        Assert.True(TestSource.EqualsMatches(matches, TestSource.Test1.Value));
    }
}