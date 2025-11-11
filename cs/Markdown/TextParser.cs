namespace Markdown;

public class TextParser(List<Token> tokens)
{
    private int pos;

    private Token Current => pos < tokens.Count ? tokens[pos] : Token.EndOfFile();
    private void Advance() => pos++;

    public List<Node> Parse()
    {
        var nodes = new List<Node>();

        while (Current.Type != TokenType.EndOfFile)//пока не токен с концом
        {
            if (Current.Type == TokenType.NewLine)//если перенос - скип
            {
                Advance();
                continue;
            }

            var isHeading = Current.Type == TokenType.Heading;//это заголовок?
            var level = 0;//ур заголовка
            if (isHeading)
                level = GetHeadingLevel();//получаем уровень если это заголовок

            var contentNodes = ParseInlineElements();//получаем содержимое заголовка или параграфа

            Node blockNode = isHeading ? new HeadingNode(level) : new ParagraphNode();// создаём заголовок или параграф
            blockNode.Children.AddRange(contentNodes);// добавляем в дети узлы содержимого
            nodes.Add(blockNode);//добавляем в общий список узлов уже заполненный заголовок или параграф

            if (Current.Type == TokenType.NewLine)
                Advance();
        }

        return nodes;//вернули список узлов

        int GetHeadingLevel()
        {
            var hashes = Current.Value;
            var level = Math.Clamp(hashes.Length, HeadingNode.MinLevel, HeadingNode.MaxLevel);
            Advance();
            return level;
        }
    }

    private List<Node> ParseInlineElements()//парсинг всех bold, italic etc
    {
        var contentNodes = new List<Node>();//список внутренних нод

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
        
        TokenType endType;
        switch (startType)
        {
            case TokenType.ItalicStart:
                endType = TokenType.ItalicEnd;
                break;
            case TokenType.BoldStart:
                endType = TokenType.BoldEnd;
                break;
            default: 
                endType = startType;
                break;
        }

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
