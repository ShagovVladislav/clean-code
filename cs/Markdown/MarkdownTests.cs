using FluentAssertions;
using NUnit.Framework;

namespace Markdown;

[TestFixture]
public class TestHeadingAndParagraphs()
{
    [Test]
    public void ToHtml_ShouldParseHeading_WhenStartWithOctothorpe()
    {
        var content = Md.ToHtml("#Heading");
        
        content.Should().Be("<h1>Heading</h1>");
    }
    
    [Test]
    [TestCase("#Heading", "<h1>Heading</h1>")]
    [TestCase("##Heading", "<h2>Heading</h2>")]
    [TestCase("###Heading", "<h3>Heading</h3>")]
    [TestCase("####Heading", "<h4>Heading</h4>")]
    public void ToHtml_ShouldParseDifferentLevelHeading_WhenStartWithOctothorpe(string text, string expected)
    {
        var content = Md.ToHtml(text);
        
        content.Should().Be(expected);
    }
    
    [Test]
    public void ToHtml_ShouldParseItalic_WhenTextWrappedWithUnderscores()
    {
    var content = Md.ToHtml("_italic text_");
    
    content.Should().Be("<p><em>italic text</em></p>");
    }

    [Test]
    public void ToHtml_ShouldParseBold_WhenTextWrappedWithDoubleUnderscores()
    {
        var content = Md.ToHtml("__bold text__");
        
        content.Should().Be("<p><strong>bold text</strong></p>");
    }



    [Test]
    [TestCase("#_Italic Heading_", "<h1><em>Italic Heading</em></h1>")]
    [TestCase("#__Bold Heading__", "<h1><strong>Bold Heading</strong></h1>")]
    [TestCase("##_Italic Heading_", "<h2><em>Italic Heading</em></h2>")]
    [TestCase("###__Bold Heading__", "<h3><strong>Bold Heading</strong></h3>")]
    public void ToHtml_ShouldParseHeadingWithFormatting_WhenCombinedWithOctothorpe(string text, string expected)
    {
        var content = Md.ToHtml(text);
        
        content.Should().Be(expected);
    }

    [Test]
    [TestCase("__bold__ and _italic_", "<p><strong>bold</strong> and <em>italic</em></p>")]
    [TestCase("_italic_ and __bold__", "<p><em>italic</em> and <strong>bold</strong></p>")]
    [TestCase("Text with __bold__ in middle", "<p>Text with <strong>bold</strong> in middle</p>")]
    [TestCase("Start with _italic_ end", "<p>Start with <em>italic</em> end</p>")]
    public void ToHtml_ShouldParseMixedFormattings_WhenInParagraph(string text, string expected)
    {
        var content = Md.ToHtml(text);
        
        content.Should().Be(expected);
    }

    [Test]
    [TestCase("___bold italic___", "<p><strong><em>bold italic</em></strong></p>")] 
    [TestCase("__bold _with italic_ inside__", "<p><strong>bold <em>with italic</em> inside</strong></p>")]
    [TestCase("_italic __with bold__ inside_", "<p><em>italic <strong>with bold</strong> inside</em></p>")]
    public void ToHtml_ShouldParseNestedFormattings_WhenCombined(string text, string expected)
    {
        var content = Md.ToHtml(text);
        
        content.Should().Be(expected);
    }

    [Test]
    [TestCase("Multiple __bold__ and _italic_ words", "<p>Multiple <strong>bold</strong> and <em>italic</em> words</p>")]
    [TestCase("__bold__ and __bold__ with _italic_", "<p><strong>bold</strong> and <strong>bold</strong> with <em>italic</em></p>")]
    [TestCase("_italic_ _italic_ __bold__", "<p><em>italic</em> <em>italic</em> <strong>bold</strong></p>")]
    public void ToHtml_ShouldParseMultipleFormattings_WhenInSameText(string text, string expected)
    {
        var content = Md.ToHtml(text);
    
        content.Should().Be(expected);
    }   

    [Test]
    public void ToHtml_ShouldParseComplexCombination_WhenAllElementsPresent()
    {
        var content = Md.ToHtml("#__Bold Heading__ with _italic_ and plain text");
    
        content.Should().Be("<h1><strong>Bold Heading</strong> with <em>italic</em> and plain text</h1>");
    }
    
    [Test]
    public void ToHtml_ShouldNotParseDigits_WhenItalic()
    {
        var text = "12_3_ _123_";
        
        var content = Md.ToHtml(text);
    
        content.Should().Be("<p>12_3_ _123_</p>");
    }
    
    [Test]
    public void ToHtml_ShouldParse_WhenUnderscoreInTheStartAndTheMiddle()
    {
        var text = "_St_art";
        
        var content = Md.ToHtml(text);
    
        content.Should().Be("<p><em>St</em>art</p>");
    }
    [Test]
    public void ToHtml_ShouldParse_WhenUnderscoreInDifferentWords()
    {
        var text = "On_e tw_o";
        
        var content = Md.ToHtml(text);
    
        content.Should().Be("<p>On_e tw_o</p>");
    }
    [Test]
    public void ToHtml_ShouldParse_WhenUnderscoreInTheMiddleAndTheEnd()
    {
        var text = "St_art_";
        
        var content = Md.ToHtml(text);
    
        content.Should().Be("<p>St<em>art</em></p>");
    }
    
    [Test]
    public void ToHtml_ShouldParse_WhenUnderscoreInTheMiddle()
    {
        var text = "S_tar_t";
        
        var content = Md.ToHtml(text);
    
        content.Should().Be("<p>S<em>tar</em>t</p>");
    }
    [Test]
    public void ToHtml_ShouldNorParse_WhenDifferentUnderscores()
    {
        var text = @"__Start_";
        
        var content = Md.ToHtml(text);
    
        content.Should().Be("<p>__Start_</p>");
    }
    
    [Test]
    public void ToHtml_ShouldNorParse_WhenCrossDifferentUnderscores()
    {
        var text = @"__Start _something__ now_";
        
        var content = Md.ToHtml(text);
    
        content.Should().Be("<p>__Start _something__ now_</p>");
    }
    [Test]
    public void ToHtml_ShouldNorParse_WhenOnlyUnderscores()
    {
        var text = @"___";
        
        var content = Md.ToHtml(text);
    
        content.Should().Be("<p>___</p>");
    }
    [Test]
    public void ToHtml_ShouldNorParse_WhenUnderscoresWithWhitespaceAfterStart()
    {
        var text = @"Start_ something_ now";
        
        var content = Md.ToHtml(text);
    
        content.Should().Be("<p>Start_ something_ now</p>");
    }
    [Test]
    public void ToHtml_ShouldNorParse_WhenUnderscoresWithWhitespaceBeforeEnd()
    {
        var text = @"Start _something _now";
        
        var content = Md.ToHtml(text);
    
        content.Should().Be("<p>Start _something _now</p>");
    }
    [Test]
    public void ToHtml_ShouldParse_WhenScreeningUnderscore()
    {
        var text = @"S\_tar\_t";
        
        var content = Md.ToHtml(text);
    
        content.Should().Be("<p>S_tar_t</p>");
    }
    [Test]
    public void ToHtml_ShouldParse_WhenScreeningTwoUnderscore()
    {
        var text = @"S\__tar\__t";
        
        var content = Md.ToHtml(text);
    
        content.Should().Be("<p>S_<em>tar_</em>t</p>");
    }
    
    [Test]
    public void ToHtml_ShouldParse_WhenScreeningOctothorpe()
    {
        var text = @"\#Start";
        
        var content = Md.ToHtml(text);
    
        content.Should().Be("<p>#Start</p>");
    }
    
    [Test]
    public void ToHtml_ShouldParse_WhenScreeningSlash()
    {
        var text = @"\\Start";
        
        var content = Md.ToHtml(text);
    
        content.Should().Be(@"<p>\Start</p>");
    }
}