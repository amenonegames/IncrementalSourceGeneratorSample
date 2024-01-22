using System;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.DotnetRuntime.Extensions;
using Microsoft.CodeAnalysis.Text;

namespace IncrementalSourceGeneratorSample
{
    [Generator(LanguageNames.CSharp)]
    public class SourceGenerator : IIncrementalGenerator
    {

        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            context.RegisterPostInitializationOutput( static x => SetAttribute(x));
            var provider = context.SyntaxProvider.ForAttributeWithMetadataName
            (
                context,
                "SourceGeneratorSample.SampleAttribute",
                static (node, cancellation) => node is ClassDeclarationSyntax,
                static (cont, cancellation) => cont
            )
                .Collect();


            context.RegisterSourceOutput(
                provider,
                static (sourceProductionContext, metaDataArray) =>
                {
                    var builder = new StringBuilder();
                    
                    foreach (var meta in metaDataArray)
                    {
                        var symbol = meta.TargetSymbol;
                        var name = symbol.Name;
                        var nameSpaceIsGlobal = symbol.ContainingNamespace.IsGlobalNamespace;
                        var nameSpaceStr = nameSpaceIsGlobal ? "" : $"namespace {symbol.ContainingNamespace.ToDisplayString()}";
                        var classAccessiblity = symbol.DeclaredAccessibility.ToString().ToLower();

                        if (!nameSpaceIsGlobal)
                        {
                            builder.AppendLine(nameSpaceStr);
                            builder.AppendLine("{");
                        }


                        builder.Append($@"
    {classAccessiblity} partial class {name}
    {{
        public void Hello()
        {{
            System.Console.WriteLine(""Hello, {name}!"");
        }}
    }}");
                        if (!nameSpaceIsGlobal)
                        {
                            builder.AppendLine("}");
                        }
                        
                        sourceProductionContext.AddSource($"{name}.g.cs", SourceText.From(builder.ToString(), Encoding.UTF8));
                        
                        builder.Clear();
                    }

                });
        }
        
        static void SetAttribute(IncrementalGeneratorPostInitializationContext context)
        {
            const string AttributeText = @"
using System;
namespace SourceGeneratorSample
{
    [AttributeUsage(AttributeTargets.Class,
                    Inherited = false, AllowMultiple = false)]
        sealed class SampleAttribute : Attribute
    {
    
        public SampleAttribute()
        {   
        }
        
    }
}
";                
            context.AddSource
            (
                "SampleAttribute.cs",
                SourceText.From(AttributeText,Encoding.UTF8)
            );
        }
    }
}