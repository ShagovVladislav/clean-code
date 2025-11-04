namespace Markdown;

public class Tokenizer
{
    private readonly string _text;
    private int _position;

    public Tokenizer(string text)
    {
        _text = text.Replace("\r", "");
        _position = 0;
    }
    
    private char Current => _position < _text.Length ? _text[_position] : '\0';
    private void Advance(int count = 1) => _position += count;

    private char Peek(int offset = 1)
    {
        int pos = _position + offset;
        return pos < _text.Length ? _text[pos] : '\0';
    }

    public List<Token> Tokenize()
    {
        var tokens = new List<Token>();

        while (_position < _text.Length)
        {
            if (Current == '#')
            {
                int start = _position;
                while (Current == '#')
                    Advance();

                string hashes = _text.Substring(start, _position - start);
                tokens.Add(new Token(TokenType.Heading, hashes));
            }
            else if (Current == '_' && Peek() == '_')
            {
                Advance(2);
                tokens.Add(new Token(TokenType.BoldStart, "__"));
            }
            else if (Current == '_')
            {
                Advance();
                tokens.Add(new Token(TokenType.ItalicStart, "_"));
            }
            else if (Current == '\n')
            {
                Advance();
                tokens.Add(new Token(TokenType.NewLine, "\\n"));
            }
            else
            {
                string text = ReadText();
                tokens.Add(new Token(TokenType.Text, text));
            }
        }
        tokens.Add(new Token(TokenType.EndOfFile));
        return tokens;
    }

    private string ReadText()
    {
        int start = _position;
        while (_position < _text.Length && !"#_\n".Contains(Current))
            Advance();

        return _text.Substring(start, _position - start);
    }
    
}