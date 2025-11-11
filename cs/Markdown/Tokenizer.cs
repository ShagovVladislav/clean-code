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

        while (!Eof)//пока не конец файла
        {
            switch (Current)//текущий
            {
                case '#':
                {
                    var start = position;//старт - текущая позиция
                    while (Current == '#')
                        Advance();//двигаемся пока решётка
                    tokens.Add(Token.Heading(text[start..position]));//добавляем в токены заголовок с уровнем равным кол-ву решёток
                    continue;
                }
                case '\n':
                case '\r':
                {
                    while (Current == '\n' || Current == '\r')//если переносы - добавляем новую строку
                        Advance();
                    tokens.Add(Token.NewLine());
                    continue;
                }
                case '_':
                {
                    var start = position;
                    var count = 1;
                    while (Next == '_')//если дальше _ 
                    {
                        count++;//считаем
                        Advance();//идём дальше
                    }
//получили кол-во подчерков
                    var kind = AnalyzeUnderscore(text, start, count);
                    switch (kind)
                    {
                        //получили инфу какой токен ставить
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
            //установили нужный токен
//если не специальный указатель
            var textStart = position;
            while (!Eof && Current != '_' && Current != '\n' && Current != '\r' && Current != '#')
                Advance();//читаем до следующего спец символа

            if (position > textStart)
                tokens.Add(Token.Text(text[textStart..position]));//добавляем токен текста
        }

        tokens.Add(Token.EndOfFile());
        return tokens;
    }
    
    private static EmphasisType AnalyzeUnderscore(string text, int pos, int count)
    {
        var before = pos - 1;//индекс до подчерков
        var after = pos + count;//индекс после подчерков
//проверяем что нормально считается
        var prev = before >= 0 ? text[before] : '\0';
        var next = after < text.Length ? text[after] : '\0';
//проверяем соседние с подчерками слева от старта и справа от конца
        var prevIsSpaceOrPunct = before < 0 || char.IsWhiteSpace(prev) || char.IsPunctuation(prev);
        var nextIsSpaceOrEnd = after >= text.Length || char.IsWhiteSpace(next) || next == '\0';

        var prevIsDigit = before >= 0 && char.IsDigit(prev);//если рядом число
        var nextIsDigit = after < text.Length && char.IsDigit(next);

        if (prevIsDigit && nextIsDigit || (count == 1 && prev == '_') || (count == 2 && next == '_'))
            return EmphasisType.None;//если рядом числа, или только 1, или только 2 - вернуть ничего
//проверяем что можем начать и ничего не мешает
        var canStart = !nextIsSpaceOrEnd && !nextIsDigit;
        var canEnd = !prevIsSpaceOrPunct && !prevIsDigit;

        return count switch
        {
            3 when canStart && !canEnd => EmphasisType.BoldItalicStart,
            3 when !canStart && canEnd => EmphasisType.BoldItalicEnd,
            2 when canStart && !canEnd => EmphasisType.BoldStart,
            2 when !canStart && canEnd => EmphasisType.BoldEnd,
            1 when canStart && !canEnd => EmphasisType.ItalicStart,
            1 when !canStart && canEnd => EmphasisType.ItalicEnd,
            _ => EmphasisType.None
        };
    }
}
