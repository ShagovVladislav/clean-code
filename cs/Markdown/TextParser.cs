namespace Markdown;

public class TextParser()
{
    private int pos;
    private List<Token> tokens = null!;
    private Token Current => pos < tokens.Count ? tokens[pos] : Token.EndOfFile();
    private void Advance() => pos++;

    public string Parse(List<Token> tokensList)
    {
        tokens = tokensList;
        pos = 0;
        var document = new DocumentNode();

        while (Current.Type != TokenType.EndOfFile)
        {
            if (Current.Type == TokenType.NewLine)
            {
                continue;
            }

            var isHeading = Current.Type == TokenType.Heading;
            var level = 0;
            if (isHeading)
                level = GetHeadingLevel();

            var contentNodes = ParseInlineElements();

            Node blockNode = isHeading ? new HeadingNode(level) : new ParagraphNode();
            blockNode.Children.AddRange(contentNodes);
            document.Children.Add(blockNode);

            if (Current.Type == TokenType.NewLine)
                Advance();
        }
        
        return document.ConvertToHtml();

        int GetHeadingLevel()
        {
            var hashes = Current.Value;
            var level = Math.Clamp(hashes.Length, HeadingNode.MinLevel, HeadingNode.MaxLevel);
            Advance();
            return level;
        }
    }

    private List<Node> ParseInlineElements()
    {
        var contentNodes = new List<Node>();

        while (HasContent())
        {
            switch (Current.Type)
            {
                case TokenType.BoldStart:
                    contentNodes.Add(ParseStyledNode(TokenType.BoldStart, 
                        innerNodes => new BoldNode { Children = innerNodes }));
                    break;
                case TokenType.ItalicStart:
                    contentNodes.Add(ParseStyledNode(TokenType.ItalicStart, 
                        innerNodes => new ItalicNode { Children = innerNodes }));
                    break;
                case TokenType.Text:
                    contentNodes.Add(new TextNode(Current.Value));
                    Advance();
                    break;
                default:
                    Advance();
                    break;
            }
        }

        return contentNodes;
    }

    private Node ParseStyledNode(TokenType startType, Func<List<Node>, Node> factory)
    {
        var endType = startType switch
        {
            TokenType.ItalicStart => TokenType.ItalicEnd,
            TokenType.BoldStart => TokenType.BoldEnd,
            _ => startType
        };

        Advance();
        var inner = new List<Node>();

        while (Current.Type != TokenType.EndOfFile && Current.Type != endType && Current.Type != TokenType.NewLine)
        {
            switch (Current.Type)
            {
                case TokenType.Text:
                    inner.Add(new TextNode(Current.Value));
                    Advance();
                    break;
                case TokenType.ItalicStart:
                    inner.Add(ParseStyledNode(TokenType.ItalicStart, c => new ItalicNode { Children = c }));
                    break;
                case TokenType.BoldStart:
                    inner.Add(ParseStyledNode(TokenType.BoldStart, c => new BoldNode { Children = c }));
                    break;
                default:
                    Advance();
                    break;
            }
        }

        if (Current.Type == endType)
            Advance(); 

        return factory(inner);
    }

    private bool HasContent()
    {
        return Current.Type != TokenType.NewLine && Current.Type != TokenType.EndOfFile;
    }
}
