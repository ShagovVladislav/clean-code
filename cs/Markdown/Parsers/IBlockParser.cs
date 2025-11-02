namespace Markdown.Parsers;

public interface IBlockParser
{
    bool CanParse(string block);
    string Parse(string block);
}