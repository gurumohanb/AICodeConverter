// See https://aka.ms/new-console-template for more information

using ICSharpCode.CodeConverter;
using ICSharpCode.CodeConverter.CSharp;
using Microsoft.CodeAnalysis;
internal class Program
{
    private static async Task Main(string[] args)
    {
        Console.WriteLine("Hello, World!");

        var vbCodeOptions = new CodeWithOptions("Imports System.Text.RegularExpressions\r\nImports Exact.BackOffice.Services\r\nImports Exact.Contracts.Core\r\nImports Exact.Core\r\nImports Exact.Data\r\nImports Exact.Enums\r\nImports Exact.Repository\r\n\r\nNamespace Exact.Campaign\r\n\t''' <summary>\r\n\t''' Tools for communication campaigns\r\n\t''' </summary>\r\n\t''' <remarks></remarks>\r\n\tPublic Class BannerService\r\n\t\tPrivate _env As Environment\r\n\r\n\t\tPublic Sub New(env As Environment)\r\n\t\t\tMe._env = env\r\n\t\tEnd Sub\r\n\tEnd Class\r\n\r\n\r\nEnd Namespace");
        vbCodeOptions.SetFromLanguage(LanguageNames.VisualBasic);
        vbCodeOptions.SetToLanguage(LanguageNames.CSharp);
        var test = await CodeConverter.ConvertAsync(vbCodeOptions);
        string result = test.ConvertedCode;
       
    }
}