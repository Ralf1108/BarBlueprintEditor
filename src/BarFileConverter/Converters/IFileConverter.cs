namespace BarFileConverter.Converters;

public interface IFileConverter
{
    string GetSearchFilenamePattern();
    string GetTargetFilenameExtension();
    void Convert(string sourceFilePath, string targetFilePath);
}