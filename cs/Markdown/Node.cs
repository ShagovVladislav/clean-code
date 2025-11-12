namespace Markdown;
    public abstract class Node
    {
        public List<Node> Children { get; init; } = [];

        protected abstract string Tag { get; }

        public virtual string ConvertToHtml()
        {
            if (Children.Count == 0)
                return string.Empty;

            var innerHtml = string.Concat(Children.Select(c => c.ConvertToHtml()));
            return $"<{Tag}>{innerHtml}</{Tag}>";
        }
    }

    public class DocumentNode() : Node
    {
        protected override string Tag => string.Empty;

        public override string ConvertToHtml()
        {
            return string.Join("\n", Children.Select(c => c.ConvertToHtml()));
        }
    }
    public class HeadingNode(int level) : Node
    {
        private int Level { get; } = Math.Clamp(level, MinLevel, MaxLevel);
        protected override string Tag => $"h{Level}";

        public const int MaxLevel = 6;
        public const int MinLevel = 1;
    }

    internal class ParagraphNode : Node
    {
        protected override string Tag => "p";
    }

    public class ItalicNode : Node
    {
        protected override string Tag => "em";
    }

 
    public class BoldNode : Node
    {
        protected override string Tag => "strong";
    }


    public class TextNode(string content) : Node
    {
        private string Content { get; } = content;
        
        protected override string Tag => string.Empty;
        
        public override string ConvertToHtml() => Content;
    }
