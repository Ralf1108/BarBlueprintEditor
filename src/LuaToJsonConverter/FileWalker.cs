using System.Text.RegularExpressions;
using LuaToJsonConverter.Converters;

namespace LuaToJsonConverter;

public class FileWalker
{
    public static void CreateDirectories(string filePath)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
    }

    public static void Walk(
        string sourceFolder,
        string targetFolder,
        IFileConverter converter,
        Regex skip)
    {
        var fullSourceFolderPath = Path.GetFullPath(sourceFolder);
        if (skip.IsMatch(fullSourceFolderPath))
            return;

        var fullTargetFolderPath = Path.GetFullPath(targetFolder);

        var filenamePattern = converter.GetSearchFilenamePattern();
        var fileExtension= converter.GetTargetFilenameExtension();
        foreach (var filePath in Directory.EnumerateFiles(sourceFolder, filenamePattern, SearchOption.AllDirectories))
        {
            var sourceFullFilePath = Path.GetFullPath(filePath);
            if (skip.IsMatch(sourceFullFilePath))
                continue;

            var subPath = Path.GetDirectoryName(sourceFullFilePath)!.Replace(fullSourceFolderPath, "").Trim("\\").ToString();
            var targetFile = Path.ChangeExtension(Path.GetFileName(sourceFullFilePath), fileExtension);
            var targetFilePath = Path.Combine(fullTargetFolderPath, subPath, targetFile);

            try
            {
                converter.Convert(sourceFullFilePath, targetFilePath);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error converting file '{sourceFullFilePath}': " + ex.Message, ex);
            }
        }
    }
}