using System.Text;

namespace Markdown;

public abstract class Node
{
    public abstract void ToHtml(StringBuilder sb);
}

class Heading : Node
{
    public int Level { get; }
    public string Content { get; }

    public Heading(int level, string content)
    {
        Level = level;
        Content = content;
    }
    override public void ToHtml(StringBuilder sb)
    {
        sb.Append($"<h{Level}>{Content}</h{Level}>");
    }
}

class Paragraph : Node
{
    private string Content { get; }

    public Paragraph(string content)
    {
        Content = content;
    }
    override public void ToHtml(StringBuilder sb)
    {
        sb.Append($"<p>{Content}</p>");
    }
}

class Italic : Node
{
    private string Content { get; }

    public Italic(string content)
    {
        Content = content;
    }
    override public void ToHtml(StringBuilder sb)
    {
        sb.Append($"<em>{Content}</em>");
    }
}

class Bold : Node
{
    private string Content { get; }

    public Bold(string content)
    {
        Content = content;
    }
    override public void ToHtml(StringBuilder sb)
    {
        sb.Append($"<strong>{Content}</strong>");
    }
}

class Text : Node
{
    public string Content { get; }

    public Text(string content)
    {
        Content = content;
    }

    public override void ToHtml(StringBuilder sb)
    {
        sb.Append(Content);
    }
}
