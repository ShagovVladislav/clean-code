namespace Markdown;

public class Tokenizer(string text)
{
    private int position;

    private char Current => position < text.Length ? text[position] : '\0';
    private char Next => position + 1 < text.Length ? text[position + 1] : '\0';
    private bool Eof => position >= text.Length;

    private void Advance(int count = 1) => position += count;

    public List<Token> Tokenize()
    {
        var tokens = new List<Token>();
        var italicOpen = false;
        var boldOpen = false;

        while (!Eof)
        {
            switch (Current)
            {
                case '#':
                {
                    if (position > 0 && text[position - 1] == '\\')
                    {
                        tokens.Add(Token.Text("#"));
                        Advance();
                        continue;
                    }
                    var start = position;
                    while (Current == '#')
                        Advance();
                    tokens.Add(Token.Heading(text[start..position]));
                    continue;
                }

                case '\\':
                {
                    Advance(); 
                    if (!Eof)
                    {
                        tokens.Add(Token.Text(Current.ToString()));
                        Advance();
                    }

                    continue;
                }

                case '\n':
                case '\r':
                {
                    while (Current == '\n' || Current == '\r')
                        Advance();
                    tokens.Add(Token.NewLine());
                    continue;
                }

                case '_':
                {
                    var firstEscaped = position > 0 && text[position - 1] == '\\';

                    var start = position;
                    var count = 1;

                    while (Next == '_')
                    {
                        count++;
                        Advance();
                    }

                    if (Next == ' ' && !italicOpen && count == 1 || 
                        Next == ' ' && !boldOpen && count == 2)
                    {
                        tokens.Add(Token.Text("_"));
                        Advance();
                        continue;
                    }
                    if (firstEscaped)
                    {
                        tokens.Add(Token.Text("_"));
                        count--;
                        start++;
                    }

                    if (count <= 0)
                    {
                        Advance();
                        continue;
                    }

                    var kind = AnalyzeUnderscore(text, start, count, ref italicOpen, ref boldOpen);
                    switch (kind)
                    {
                        case EmphasisType.ItalicStart:
                            tokens.Add(Token.ItalicStart());
                            break;
                        case EmphasisType.BoldStart:
                            tokens.Add(Token.BoldStart());
                            break;
                        case EmphasisType.ItalicEnd:
                            tokens.Add(Token.ItalicEnd());
                            break;
                        case EmphasisType.BoldEnd:
                            tokens.Add(Token.BoldEnd());
                            break;
                        case EmphasisType.BoldItalicStart:
                            tokens.Add(Token.BoldStart());
                            tokens.Add(Token.ItalicStart());
                            break;
                        case EmphasisType.BoldItalicEnd:
                            tokens.Add(Token.ItalicEnd());
                            tokens.Add(Token.BoldEnd());
                            break;
                        default:
                            tokens.Add(Token.Text(new string('_', count)));
                            break;
                    }

                    Advance();
                    continue;
                }
            }

            var textStart = position;
            while (!Eof && Current != '_' && Current != '\n' && Current != '\r' && Current != '#' && Current != '\\')
                Advance();

            if (position > textStart)
                tokens.Add(Token.Text(text[textStart..position]));
        }

        tokens.Add(Token.EndOfFile());
        return ConvertUnmatchedTokensToText(tokens);
    }

    private static EmphasisType AnalyzeUnderscore(string text, int pos, int count, ref bool italicOpen, ref bool boldOpen)
{
    var beforeIdx = pos - 1;
    var afterIdx = pos + count;

    var prev = beforeIdx >= 0 ? text[beforeIdx] : '\0';
    var next = afterIdx < text.Length ? text[afterIdx] : '\0';

    var prevIsLetter = char.IsLetter(prev);
    var nextIsLetter = char.IsLetter(next);
    var prevIsDigit = char.IsDigit(prev);
    var nextIsDigit = char.IsDigit(next);
    var prevIsSpace = beforeIdx < 0 || char.IsWhiteSpace(prev);
    var nextIsSpace = afterIdx >= text.Length || char.IsWhiteSpace(next);
    var prevIsPunct = char.IsPunctuation(prev);
    var nextIsPunct = char.IsPunctuation(next);

    if (prevIsLetter && nextIsLetter)
    {
        var searchPos = afterIdx;
        var whitespaceBetween = false;
        while (searchPos < text.Length)
        {
            if(text[searchPos] == ' ') whitespaceBetween = true;
            if (text[searchPos] == '_' && whitespaceBetween)
            {
                var nextUnderscoreBefore = searchPos - 1;
                var nextUnderscoreAfter = searchPos + 1;
                
                var nextUnderscorePrev = nextUnderscoreBefore >= 0 ? text[nextUnderscoreBefore] : '\0';
                var nextUnderscoreNext = nextUnderscoreAfter < text.Length ? text[nextUnderscoreAfter] : '\0';
                
                if (char.IsLetter(nextUnderscorePrev) && char.IsLetter(nextUnderscoreNext))
                {
                    return EmphasisType.None;
                }
                break;
            }
            searchPos++;
        }
    }

    if (prevIsDigit || nextIsDigit)
        return EmphasisType.None;

    switch (count)
    {
        case 1 when prevIsLetter || prevIsPunct && nextIsLetter:
            italicOpen = !italicOpen;
            return italicOpen ? EmphasisType.ItalicStart : EmphasisType.ItalicEnd;
        case 1 when (prevIsSpace || prevIsPunct || prev == '\0') && nextIsLetter:
            italicOpen = true;
            return EmphasisType.ItalicStart;
        case 1 when !prevIsLetter || (!nextIsSpace && !nextIsPunct && next != '\0'):
            return EmphasisType.None;
        case 1:
            italicOpen = false;
            return EmphasisType.ItalicEnd;
        case 2 when (prevIsSpace || prevIsPunct || prev == '\0') && nextIsLetter && !boldOpen:
            boldOpen = true;
            return EmphasisType.BoldStart;
        case 2 when !boldOpen || (nextIsLetter && !nextIsSpace && !nextIsPunct):
            return EmphasisType.None;
        case 2:
            boldOpen = false;
            return EmphasisType.BoldEnd;
    }

    var canOpen = !nextIsSpace && next != '\0';
    var canClose = !prevIsSpace && !prevIsPunct;

    return count switch
    {
        3 when canOpen && !canClose => EmphasisType.BoldItalicStart,
        3 when !canOpen && canClose => EmphasisType.BoldItalicEnd,
        _ => EmphasisType.None
    };
}
    
    
   private List<Token> ConvertUnmatchedTokensToText(List<Token> tokens)
{
    var tokensToConvert = new HashSet<int>();

    var boldRanges = FindValidTagPairs(tokens, TokenType.BoldStart, TokenType.BoldEnd);
    var italicRanges = FindValidTagPairs(tokens, TokenType.ItalicStart, TokenType.ItalicEnd);

    foreach (var boldRange in boldRanges)
    {
        foreach (var italicRange in italicRanges.Where(italicRange => IsCrossing(boldRange.start, boldRange.end, italicRange.start, italicRange.end)))
        {
            tokensToConvert.Add(boldRange.start);
            tokensToConvert.Add(boldRange.end);
            tokensToConvert.Add(italicRange.start);
            tokensToConvert.Add(italicRange.end);
        }
    }

    var openStack = new Stack<(Token token, int index)>();
    for (var i = 0; i < tokens.Count; i++)
    {
        var token = tokens[i];

        if (tokensToConvert.Contains(i)) continue;

        switch (token.Type)
        {
            case TokenType.BoldStart:
            case TokenType.ItalicStart:
                openStack.Push((token, i));
                break;
            case TokenType.BoldEnd:
                if (openStack.Count > 0 && openStack.Peek().token.Type == TokenType.BoldStart)
                {
                    openStack.Pop();
                }
                else
                {
                    tokensToConvert.Add(i);
                }
                break;
            case TokenType.ItalicEnd:
                if (openStack.Count > 0 && openStack.Peek().token.Type == TokenType.ItalicStart)
                {
                    openStack.Pop();
                }
                else
                {
                    tokensToConvert.Add(i);
                }
                break;
        }
    }

    tokensToConvert.UnionWith(openStack.Select(x => x.index));

    return tokens.Select((t, i) => tokensToConvert.Contains(i) ? Token.Text(t.Value) : t).ToList();
}

private List<(int start, int end)> FindValidTagPairs(List<Token> tokens, TokenType startType, TokenType endType)
{
    var pairs = new List<(int start, int end)>();
    var stack = new Stack<int>();

    for (var i = 0; i < tokens.Count; i++)
    {
        var token = tokens[i];
        
        if (token.Type == startType)
        {
            stack.Push(i);
        }
        else if (token.Type == endType && stack.Count > 0)
        {
            var startIndex = stack.Pop();
            pairs.Add((startIndex, i));
        }
    }

    return pairs;
}

private bool IsCrossing(int start1, int end1, int start2, int end2)
{
    return (start2 > start1 && start2 < end1 && end2 > end1) ||
           (start1 > start2 && start1 < end2 && end1 > end2);
}

}
