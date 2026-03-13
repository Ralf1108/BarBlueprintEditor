using BCnEncoder.Decoder;
using BCnEncoder.ImageSharp;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace BarFileConverter.Converters;

public class DdsToImageConverter(Dictionary<string, WebUnitDefinition> unitInfos) : IFileConverter
{
    public string GetSearchFilenamePattern()
    {
        return "*.dds";
    }

    public string GetTargetFilenameExtension()
    {
        return ".png";
    }

    public void Convert(string sourceFilePath, string targetFilePath)
    {
        var sourceFilename = Path.GetFileNameWithoutExtension(sourceFilePath);
        if (!unitInfos.ContainsKey(sourceFilename))
            return;

        using var fs = File.OpenRead(sourceFilePath);

        var decoder = new BcDecoder();
        using var image = decoder.DecodeToImageRgba32(fs);

        var resizeOptions = new ResizeOptions
        {
            Size = new Size(64, 64),
            Mode = ResizeMode.Crop,
            Position = AnchorPositionMode.Center,
            Sampler = KnownResamplers.Lanczos3
        };


        image.Mutate(x => x.Resize(resizeOptions));

        FileWalker.CreateDirectories(targetFilePath);
        image.Save(targetFilePath);
    }
}