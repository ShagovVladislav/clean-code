using System.Text;

namespace Markdown;

public class HtmlRenderer
{
    public string Render(List<Node> nodes)
    {
        var sb = new StringBuilder();
        for (int i = 0; i < nodes.Count; i++)
        {
            nodes[i].ToHtml(sb);
            if (i < nodes.Count - 1)
                sb.AppendLine();
        }
        return sb.ToString();
    }

}