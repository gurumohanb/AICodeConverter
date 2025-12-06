// See https://aka.ms/new-console-template for more information

using ICSharpCode.CodeConverter;
using ICSharpCode.CodeConverter.CSharp;
using Microsoft.CodeAnalysis;
using System.Diagnostics;
using System.Text;

internal class Program
{
    private static async Task Main(string[] args)
    {
        Console.WriteLine("Enter folder path of the vbproj:");
        var sourceRoot = Console.ReadLine();
        // string sourceRoot = args[0];
        if (!Directory.Exists(sourceRoot))
        {
            Console.WriteLine($"Directory not found: {sourceRoot}");
            return;
        }

        string targetRoot = Path.Combine(sourceRoot, "Migrated");
        Directory.CreateDirectory(targetRoot);

        Console.WriteLine($"Starting migration from: {sourceRoot}");
        await ProcessDirectoryAsync(sourceRoot, targetRoot);

        Console.WriteLine("Migration complete!");

    }

    private static async Task ProcessDirectoryAsync(string sourceDir, string targetDir)
    {
        List<string> excludedDirNames = new List<string>()
    { "Migrated", "bin", "obj" };
        string[] allDirectories = [.. Directory
            .GetDirectories(sourceDir,"*",SearchOption.AllDirectories)
            .Where(dirPath => !excludedDirNames.Any(excludeName =>
        Path.GetFileName(dirPath).Equals(excludeName, StringComparison.OrdinalIgnoreCase)))];
        // Include the source directory itself
        allDirectories = allDirectories.Prepend(sourceDir).ToArray();
        foreach (var dir in allDirectories)
        {
            string directoryNDepth = GetDirectoryNDepth(sourceDir, dir);
            string nestedTarget = Path.Combine(targetDir, directoryNDepth);
            Directory.CreateDirectory(nestedTarget);
            foreach (var file in Directory.GetFiles(dir))
            {
                if (file.EndsWith(".vb"))
                {
                    string targetFile = Path.Combine(nestedTarget, Path.GetFileNameWithoutExtension(file) + ".cs");
                    await ConvertFileAsync(file, targetFile);
                }
                else if (file.EndsWith(".vbproj"))
                {
                    string targetFile = Path.Combine(targetDir, Path.GetFileNameWithoutExtension(file) + ".csproj");
                    ConvertProjectFile(file, targetFile);
                }
                else
                {
                    // Copy other files as-is
                    File.Copy(file, Path.Combine(nestedTarget, Path.GetFileName(file)), true);
                }
            }
        }


    }

    public static string GetDirectoryNDepth(string root, string target)
    {
        string[] splittedRoot = root.Split(Path.DirectorySeparatorChar);
        string[] splittedTarget = target.Split(Path.DirectorySeparatorChar);

        StringBuilder sb = new StringBuilder();

        for (int i = splittedRoot.Length; i < splittedTarget.Length; i++)
        {

            sb.Append(String.Format("{0}{1}", splittedTarget[i], Path.DirectorySeparatorChar));
        }

        return sb.ToString();
    }

    private static void ConvertProjectFile(string sourceFile, string targetFile)
    {
        try
        {
            string vbProjContent = File.ReadAllText(sourceFile);
            string csProjContent = vbProjContent.Replace("<Project Sdk=\"Microsoft.VisualBasic\"", "<Project Sdk=\"Microsoft.NET.Sdk\"");
            csProjContent = csProjContent.Replace(".vb\"", ".cs\"");

            File.WriteAllText(targetFile, csProjContent);
            Console.WriteLine($"Converted project file: {sourceFile} -> {targetFile}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error converting project file {sourceFile}: {ex.Message}");
        }
    }

    private static async Task ConvertFileAsync(string sourceFile, string targetFile)
    {
        string vbCode = await File.ReadAllTextAsync(sourceFile);

        var conversionResult = await CodeConverter.ConvertAsync(VbToCSharp(vbCode));

        if (!conversionResult.Success)
        {
            Console.WriteLine($"Failed to convert {sourceFile}: {string.Join(", ", conversionResult.Exceptions)}");
            return;
        }

        await File.WriteAllTextAsync(targetFile, conversionResult.ConvertedCode);
        Console.WriteLine($"Converted: {sourceFile} -> {targetFile}");

    }
    private static CodeWithOptions VbToCSharp(string vbCode)
    {
        var vbCodeOptions = new CodeWithOptions(vbCode);
        vbCodeOptions.SetFromLanguage(LanguageNames.VisualBasic);
        vbCodeOptions.SetToLanguage(LanguageNames.CSharp);
        return vbCodeOptions;
    }
}
