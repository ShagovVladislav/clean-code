namespace Markdown;

public static class HtmlRenderer
{
    public static string Render(List<Node> nodes)
    {
        return string.Join("\n", nodes.Select(n => n.ConvertToHtml()));
    }
}