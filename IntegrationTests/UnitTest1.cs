namespace IntegrationTests; 
using Xunit;

public class GeneratedRunnerTests
{
    [Theory]
    [InlineData("the 10 quick brown #@%$! foxes jumped over 1.5 lazy dogs")]
    public void GeneratedString(string value)
    {
        Assert.True(TestSource.CompareResults(TestSource.CalcStringRunner(value), TestSource.Test1));
    }
    [Theory]
    [InlineData("the 10 quick brown #@%$! foxes jumped over 1.5 lazy dogs")]
    public void GeneratedStringTable(string value)
    {
        Assert.True(TestSource.CompareResults(TestSource.CalcStringTableRunner(value), TestSource.Test1));
    }
    [Theory]
    [InlineData("the 10 quick brown #@%$! foxes jumped over 1.5 lazy dogs")]
    public void GeneratedTextReader(string value)
    {
        Assert.True(TestSource.CompareResults(TestSource.CalcTextReaderRunner(new StringReader(value)), TestSource.Test1));
    }
    [Theory]
    [InlineData("the 10 quick brown #@%$! foxes jumped over 1.5 lazy dogs")]
    public void GeneratedTextReaderTable(string value)
    {
        Assert.True(TestSource.CompareResults(TestSource.CalcTextReaderTableRunner(new StringReader(value)), TestSource.Test1));
    }
    [Theory]
    [InlineData("the 10 quick brown #@%$! foxes jumped over 1.5 lazy dogs")]
    public void GeneratedClassTextReaderTable(string value)
    {
        var fooLexer = new FooLexer();
        fooLexer.Set(value);
        Assert.True(TestSource.CompareResults(fooLexer, TestSource.Test1));
    }
}