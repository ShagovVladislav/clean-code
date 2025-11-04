using System.Collections.Generic;
using System.Text;

namespace Markdown;
public class TextParser
{
    private readonly List<Token> _tokens;
    private int _pos;

    public TextParser(List<Token> tokens)
    {
        _tokens = tokens;
        _pos = 0;
    }

    private Token Current => _pos < _tokens.Count ? _tokens[_pos] : new Token(TokenType.EndOfFile);
    private void Advance() => _pos++;

    public List<Node> Parse()
    {
    var nodes = new List<Node>();

    while (Current.Type != TokenType.EndOfFile)
    {
        if (Current.Type == TokenType.NewLine)
        {
            Advance();
            continue;
        }

        if (Current.Type == TokenType.Heading)
        {
            string hashes = Current.Value ?? "";
            int level = Math.Min(6, Math.Max(1, hashes.Length)); 
            Advance();
            
            var contentNodes = ParseInlineElements();
            var sb = new StringBuilder();
            foreach (var n in contentNodes)
                n.ToHtml(sb);
                
            nodes.Add(new Heading(level, sb.ToString()));
        }
        else
        {
            var contentNodes = ParseInlineElements();
            var sb = new StringBuilder();
            foreach (var n in contentNodes)
                n.ToHtml(sb);

            nodes.Add(new Paragraph(sb.ToString()));
        }
        
        if (Current.Type == TokenType.NewLine)
            Advance();
    }

    return nodes;
}

    private List<Node> ParseInlineElements()
    {
        var contentNodes = new List<Node>();

        while (Current.Type != TokenType.NewLine && Current.Type != TokenType.EndOfFile)
        {
            if (Current.Type == TokenType.BoldStart)
            {
                Advance(); 
                string inner = ReadInlineUntil(TokenType.BoldStart, TokenType.EndOfFile);
                contentNodes.Add(new Bold(inner));
                if (Current.Type == TokenType.BoldStart)
                    Advance();
            }
            else if (Current.Type == TokenType.ItalicStart)
            {
                Advance(); 
                string inner = ReadInlineUntil(TokenType.ItalicStart, TokenType.EndOfFile);
                contentNodes.Add(new Italic(inner));
                if (Current.Type == TokenType.ItalicStart)
                    Advance(); 
            }
            else if (Current.Type == TokenType.Text)
            {
                contentNodes.Add(new Text(Current.Value));
                Advance();
            }
            else
            {
                Advance(); 
            }
        }
        
        return contentNodes;
    }
    private string ReadInlineUntil(TokenType stop, TokenType end)
    {
        var sb = new StringBuilder();
        while (Current.Type != end && Current.Type != stop)
        {
            if (Current.Type == TokenType.Text)
                sb.Append(Current.Value);
            Advance();
        }
        return sb.ToString();
    }
}

