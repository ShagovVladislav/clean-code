namespace Markdown;

public static class Markdown
{
    public static string ToHtml(string markdownText)
    {
        var tokenizer = new Tokenizer(markdownText);
        var tokens = tokenizer.Tokenize();

        var parser = new TextParser(tokens);
        var nodes = parser.Parse();

        var renderer = new HtmlRenderer();
        return renderer.Render(nodes).Replace("\r", "");
    }
}
