using System.Net;
using BarBlueprintEditor.Shared.Dtos;
using HtmlAgilityPack;

namespace LuaToJsonConverter;

public static class UnitInfosWebScraper
{
    public static async Task<List<WebUnitDefinition>> GetUnitDefinitions()
    {
        var unitTypes = new List<string>
        {
            //"aircraft",
            //"bots",
            "buildings",
            "defense-buildings",
            "factories",
            //"hovercraft",
            //"ships",
            //"vehicles"
        };

        var fractions = new List<string>
        {
            "armada",
            "cortex"
        };

        var allUnits = new List<WebUnitDefinition>();
        foreach (var fraction in fractions)
        foreach (var unitType in unitTypes)
        {
            var url = $"https://www.beyondallreason.info/units/{fraction}-{unitType}";
            var units = await DownloadImagesBeyondAllReasonFromUrl(url);
            allUnits.AddRange(units);
        }

        return allUnits;
    }

    private static async Task<List<WebUnitDefinition>> DownloadImagesBeyondAllReasonFromUrl(string url)
    {
        var html = await GetHtml(url);

        var doc = new HtmlDocument();
        doc.LoadHtml(html);
        var xPathItems = "/html/body/div[3]/div[4]/div/div[4]/div[2]/div[2]/div[2]/div";
        var itemsNode = doc.DocumentNode.SelectSingleNode(xPathItems);

        var units = itemsNode.ChildNodes
            .Select(childNode =>
            {
                var nameNode = childNode.SelectSingleNode("a");
                var name = nameNode.Attributes["href"].Value.Split('/').Last();

                var imageNode = childNode.SelectSingleNode("a/img[1]");
                var imageUrl = imageNode.Attributes["src"].Value;
                var extension = Path.GetExtension(imageUrl);

                var titleNode = childNode.SelectSingleNode("a/div[4]/div[1]");
                var title = titleNode.InnerText.Replace("/", "-");

                var descriptionNode = childNode.SelectSingleNode("a/div[4]/div[2]");
                var description = descriptionNode.InnerText.Replace("/", "-");

                var techTopNode =
                    childNode.SelectSingleNode("a/div[not(contains(@class, 'w-condition-invisible'))]");
                var techNode = techTopNode.SelectSingleNode("div");
                var tech = techNode.InnerText;

                var imageName = $"{tech} - {title} - {description}{extension}";
                return new WebUnitDefinition(name, imageUrl, title, description, tech);
            })
            .ToList();
        return units;
    }

    private static async Task<string> GetHtml(string url)
    {
        // hosting is on cloudflare, so we need to set the user agent and accept headers to avoid getting a 403 forbidden error
        var handler = new HttpClientHandler
        {
            AutomaticDecompression =
                DecompressionMethods.GZip |
                DecompressionMethods.Deflate |
                DecompressionMethods.Brotli
        };

        using var client = new HttpClient(handler);
        var request = new HttpRequestMessage(HttpMethod.Get, url);

        // Force HTTP/2 like curl
        request.Version = HttpVersion.Version20;
        request.VersionPolicy = HttpVersionPolicy.RequestVersionOrHigher;
        request.Headers.UserAgent.ParseAdd("curl/8.5.0");
        request.Headers.Accept.ParseAdd("*/*");

        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        var html = await response.Content.ReadAsStringAsync();
        return html;
    }
}