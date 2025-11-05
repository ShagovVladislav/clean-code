using NUnit.Framework;
using System;  

namespace Markdown;
[TestFixture]

public class TestHeadingAndParagraphs()
{
    [TestCase("#_Heading_\n##Heading2", ExpectedResult = "<h1><em>Heading</em></h1>\n<h2>Heading2</h2>")]
    [TestCase(@"\#NotHeading", ExpectedResult = "<p>#NotHeading</p>")]
    [TestCase("# _Italic Heading_", ExpectedResult = "<h1><em>Italic Heading</em></h1>")]
    [TestCase("#__Bold Heading", ExpectedResult = "<h1><strong>Bold Heading</strong></h1>")]
    [TestCase("_Italic Paragraph_", ExpectedResult = "<p><em>Italic Paragraph</em></p>")]
    [TestCase("__Bold Paragraph__", ExpectedResult = "<p><strong>Bold Paragraph</strong></p>")]
    [TestCase(@"\_Not Italic Paragraph\_", ExpectedResult = "<p>_Not Italic Paragraph_</p>")]
    [TestCase(@"Not \Scree\ning", ExpectedResult = @"<p>Not \Scree\ning</p>")]
    [TestCase("___Italic Bold Paragraph___", ExpectedResult = "<p><em><strong>Italic Bold Paragraph</strong></em></p>")]
    public string HeadingAndParagraphs(string content)
    {
        
        content = Markdown.ToHtml(content);
        return content;
    }
}