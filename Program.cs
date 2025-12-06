// See https://aka.ms/new-console-template for more information

using ICSharpCode.CodeConverter;
using ICSharpCode.CodeConverter.CSharp;
using Microsoft.CodeAnalysis;
internal class Program
{
    private static async Task Main(string[] args)
    {
        Console.WriteLine("Hello, World!");

        var vbCodeOptions = new CodeWithOptions("");
        vbCodeOptions.SetFromLanguage(LanguageNames.VisualBasic);
        vbCodeOptions.SetToLanguage(LanguageNames.CSharp);
        var test = await CodeConverter.ConvertAsync(vbCodeOptions);
        string result = test.ConvertedCode;
       
    }
}
