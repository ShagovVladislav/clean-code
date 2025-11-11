namespace Markdown;

public static class Md
{
    public static string ToHtml(string markdownText)
    {
        var tokenizer = new Tokenizer(markdownText);
        var tokens = tokenizer.Tokenize();
        
        var parser = new TextParser(tokens);
        var nodes = parser.Parse();

        return HtmlRenderer.Render(nodes).Replace("\r", "");
    }
}
