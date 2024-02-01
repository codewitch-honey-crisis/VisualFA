using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using VerifyXunit;
using VisualFA;

namespace NetEscapades.EnumGenerators.Tests;

public static class TestHelper
{
    public static Task Verify(string source, bool refVfa = false)
    {
        SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(source);
        var references = new List<PortableExecutableReference>()  
        {
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location)
        };
        if(refVfa)
        {
            references.Add(MetadataReference.CreateFromFile(typeof(VisualFA.FA).Assembly.Location));
        }

        CSharpCompilation compilation = CSharpCompilation.Create(
            assemblyName: "Tests",
            syntaxTrees: new[] { syntaxTree },
            references: references);

        var generator = new FASourceGenerator();

        GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);

        driver = driver.RunGenerators(compilation);

        return Verifier
            .Verify(driver)
            .UseDirectory("Snapshots");
    }
}