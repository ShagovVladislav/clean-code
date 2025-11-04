using NUnit.Framework;
using System;  

namespace Markdown;
[TestFixture]

public class TestHeadingAndParagraphs()
{
    [TestCase("#_Heading_\n##Heading2", ExpectedResult = "<h1><em>Heading</em></h1>\n<h2>Heading2</h2>")]
    public string HeadingAndParagraphs(string content)
    {
        
        content = Markdown.ToHtml(content);
        return content;
    }
}