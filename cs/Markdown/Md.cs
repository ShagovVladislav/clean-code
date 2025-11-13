namespace Markdown;

public static class Md
{
    private static readonly Tokenizer Tokenizer = new();
    private static readonly TextParser Parser = new();
    public static string ToHtml(string markdownText)
    {
        var tokens = Tokenizer.Tokenize(markdownText);
        
        return Parser.Parse(tokens);
    }
}
