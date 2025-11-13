using FluentAssertions;

namespace Markdown.Tests;

[TestFixture]
public class ConvertToHtmlTests()
{
    [Test]
    public void ConvertToHtml_ShouldParseBulletList_WhenStartsWithDash()
    {
        var text = "- Item 1\n- Item 2\n- Item 3";
        
        var content = Md.ConvertToHtml(text);
    
        content.Should().Be("<ul><li>Item 1</li><li>Item 2</li><li>Item 3</li></ul>");
    }
    
    [Test]
    public void ConvertToHtml_ShouldParseBulletList_WhenStartsWithAsterisk()
    {
        var text = "* Item 1\n* Item 2\n* Item 3";
        
        var content = Md.ConvertToHtml(text);
    
        content.Should().Be("<ul><li>Item 1</li><li>Item 2</li><li>Item 3</li></ul>");
    }
    
    [Test]
    public void ConvertToHtml_ShouldParseBulletList_WhenStartsWithPlus()
    {
        var text = "+ Item 1\n+ Item 2\n+ Item 3";
        
        var content = Md.ConvertToHtml(text);
    
        content.Should().Be("<ul><li>Item 1</li><li>Item 2</li><li>Item 3</li></ul>");
    }
    
    [Test]
    public void ConvertToHtml_ShouldParseSingleListItem_WhenOnlyOneItem()
    {
        var text = "- Single item";
        
        var content = Md.ConvertToHtml(text);
    
        content.Should().Be("<ul><li>Single item</li></ul>");
    }
    
    [Test]
    public void ConvertToHtml_ShouldParseListWithFormatting_WhenItemsHaveEmphasis()
    {
        var text = "- _italic item_\n- __bold item__\n- ___bold italic___";
        
        var content = Md.ConvertToHtml(text);
    
        content.Should().Be("<ul><li><em>italic item</em></li><li><strong>bold item</strong></li><li><strong><em>bold italic</em></strong></li></ul>");
    }
    
    [Test]
    public void ConvertToHtml_ShouldParseMixedContent_WhenListAndParagraphs()
    {
        var text = "Paragraph before\n- List item 1\n- List item 2\nParagraph after";
        
        var content = Md.ConvertToHtml(text);
    
        content.Should().Be("<p>Paragraph before</p>\n<ul><li>List item 1</li><li>List item 2</li></ul>\n<p>Paragraph after</p>");
    }
    
    [Test]
    public void ConvertToHtml_ShouldNotParseAsList_WhenNoSpaceAfterMarker()
    {
        var text = "-no space item";
        
        var content = Md.ConvertToHtml(text);
    
        content.Should().Be("<p>-no space item</p>");
    }
    
    [Test]
    public void ConvertToHtml_ShouldNotParseAsList_WhenMarkerInMiddleOfLine()
    {
        var text = "Text with - dash in middle";
        
        var content = Md.ConvertToHtml(text);
    
        content.Should().Be("<p>Text with - dash in middle</p>");
    }
    
    [Test]
    public void ConvertToHtml_ShouldNotParseEmptyListItem_WhenNoContentAfterMarker()
    {
        var text = "-\n- Item 2";
    
        var content = Md.ConvertToHtml(text);

        content.Should().Be("<p>-</p>\n<ul><li>Item 2</li></ul>");
    }
    
    [Test]
    public void ConvertToHtml_ShouldParseListWithMultipleParagraphs_WhenMixedContent()
    {
        var text = "# Heading\n- List item\nParagraph text\n- Another item";
        
        var content = Md.ConvertToHtml(text);
    
        content.Should().Be("<h1> Heading</h1>\n<ul><li>List item</li></ul>\n<p>Paragraph text</p>\n<ul><li>Another item</li></ul>");
    }
    
    [Test]
    public void ConvertToHtml_ShouldParseComplexListItem_WhenMultipleFormattings()
    {
        var text = "- Start with __bold__ and _italic_ and end";
        
        var content = Md.ConvertToHtml(text);
    
        content.Should().Be("<ul><li>Start with <strong>bold</strong> and <em>italic</em> and end</li></ul>");
    }
    
    [Test]
    public void ConvertToHtml_ShouldParseListAfterHeading_WhenCombined()
    {
        var text = "# My List\n- First item\n- Second item";
        
        var content = Md.ConvertToHtml(text);
    
        content.Should().Be("<h1> My List</h1>\n<ul><li>First item</li><li>Second item</li></ul>");
    }
    
    [Test]
    public void ConvertToHtml_ShouldNotParseEscapedMarker_WhenBackslashBefore()
    {
        var text = "\\- Not a list item";
        
        var content = Md.ConvertToHtml(text);
    
        content.Should().Be("<p>- Not a list item</p>");
    }
    
    [Test]
    public void ConvertToHtml_ShouldParseMultipleLists_WhenSeparatedByContent()
    {
        var text = "- List A1\n- List A2\nParagraph\n- List B1\n- List B2";
        
        var content = Md.ConvertToHtml(text);
    
        content.Should().Be("<ul><li>List A1</li><li>List A2</li></ul>\n<p>Paragraph</p>\n<ul><li>List B1</li><li>List B2</li></ul>");
    }
    [Test]
    public void ConvertToHtml_ShouldParseHeading_WhenStartWithOctothorpe()
    {
        var text = "#Heading";
        
        var content = Md.ConvertToHtml(text);
        
        content.Should().Be("<h1>Heading</h1>");
    }
    
    [Test]
    [TestCase("#Heading", "<h1>Heading</h1>")]
    [TestCase("##Heading", "<h2>Heading</h2>")]
    [TestCase("###Heading", "<h3>Heading</h3>")]
    [TestCase("####Heading", "<h4>Heading</h4>")]
    public void ConvertToHtml_ShouldParseDifferentLevelHeading_WhenStartWithOctothorpe(string text, string expected)
    {
        var content = Md.ConvertToHtml(text);
        
        content.Should().Be(expected);
    }
    
    [Test]
    public void ConvertToHtml_ShouldParseItalic_WhenTextWrappedWithUnderscores()
    {
        var text = "_italic text_";
        
        var content = Md.ConvertToHtml(text);
    
        content.Should().Be("<p><em>italic text</em></p>");
    }

    [Test]
    public void ConvertToHtml_ShouldParseBold_WhenTextWrappedWithDoubleUnderscores()
    {
        var text = "__bold text__";
        
        var content = Md.ConvertToHtml(text);
        
        content.Should().Be("<p><strong>bold text</strong></p>");
    }



    [Test]
    [TestCase("#_Italic Heading_", "<h1><em>Italic Heading</em></h1>")]
    [TestCase("#__Bold Heading__", "<h1><strong>Bold Heading</strong></h1>")]
    [TestCase("##_Italic Heading_", "<h2><em>Italic Heading</em></h2>")]
    [TestCase("###__Bold Heading__", "<h3><strong>Bold Heading</strong></h3>")]
    public void ConvertToHtml_ShouldParseHeadingWithFormatting_WhenCombinedWithOctothorpe(string text, string expected)
    {
        var content = Md.ConvertToHtml(text);
        
        content.Should().Be(expected);
    }

    [Test]
    [TestCase("__bold__ and _italic_", "<p><strong>bold</strong> and <em>italic</em></p>")]
    [TestCase("_italic_ and __bold__", "<p><em>italic</em> and <strong>bold</strong></p>")]
    [TestCase("Text with __bold__ in middle", "<p>Text with <strong>bold</strong> in middle</p>")]
    [TestCase("Start with _italic_ end", "<p>Start with <em>italic</em> end</p>")]
    public void ConvertToHtml_ShouldParseMixedFormattings_WhenInParagraph(string text, string expected)
    {
        var content = Md.ConvertToHtml(text);
        
        content.Should().Be(expected);
    }

    [Test]
    [TestCase("___bold italic___", "<p><strong><em>bold italic</em></strong></p>")] 
    [TestCase("__bold _with italic_ inside__", "<p><strong>bold <em>with italic</em> inside</strong></p>")]
    [TestCase("_italic __with bold__ inside_", "<p><em>italic <strong>with bold</strong> inside</em></p>")]
    public void ConvertToHtml_ShouldParseNestedFormattings_WhenCombined(string text, string expected)
    {
        var content = Md.ConvertToHtml(text);
        
        content.Should().Be(expected);
    }

    [Test]
    [TestCase("Multiple __bold__ and _italic_ words", "<p>Multiple <strong>bold</strong> and <em>italic</em> words</p>")]
    [TestCase("__bold__ and __bold__ with _italic_", "<p><strong>bold</strong> and <strong>bold</strong> with <em>italic</em></p>")]
    [TestCase("_italic_ _italic_ __bold__", "<p><em>italic</em> <em>italic</em> <strong>bold</strong></p>")]
    public void ConvertToHtml_ShouldParseMultipleFormattings_WhenInSameText(string text, string expected)
    {
        var content = Md.ConvertToHtml(text);
    
        content.Should().Be(expected);
    }   

    [Test]
    public void ConvertToHtml_ShouldParseComplexCombination_WhenAllElementsPresent()
    {
        var text = "#__Bold Heading__ with _italic_ and plain text";
        
        var content = Md.ConvertToHtml(text);
    
        content.Should().Be("<h1><strong>Bold Heading</strong> with <em>italic</em> and plain text</h1>");
    }
    
    [Test]
    public void ConvertToHtml_ShouldNotParseDigits_WhenItalic()
    {
        var text = "12_3_ _123_";
        
        var content = Md.ConvertToHtml(text);
    
        content.Should().Be("<p>12_3_ _123_</p>");
    }
    
    [Test]
    public void ConvertToHtml_ShouldParse_WhenUnderscoreInTheStartAndTheMiddle()
    {
        var text = "_St_art";
        
        var content = Md.ConvertToHtml(text);
    
        content.Should().Be("<p><em>St</em>art</p>");
    }
    [Test]
    public void ConvertToHtml_ShouldParse_WhenUnderscoreInDifferentWords()
    {
        var text = "On_e tw_o";
        
        var content = Md.ConvertToHtml(text);
    
        content.Should().Be("<p>On_e tw_o</p>");
    }
    [Test]
    public void ConvertToHtml_ShouldParse_WhenUnderscoreInTheMiddleAndTheEnd()
    {
        var text = "St_art_";
        
        var content = Md.ConvertToHtml(text);
    
        content.Should().Be("<p>St<em>art</em></p>");
    }
    
    [Test]
    public void ConvertToHtml_ShouldParse_WhenUnderscoreInTheMiddle()
    {
        var text = "S_tar_t";
        
        var content = Md.ConvertToHtml(text);
    
        content.Should().Be("<p>S<em>tar</em>t</p>");
    }
    [Test]
    public void ConvertToHtml_ShouldNorParse_WhenDifferentUnderscores()
    {
        var text = @"__Start_";
        
        var content = Md.ConvertToHtml(text);
    
        content.Should().Be("<p>__Start_</p>");
    }
    
    [Test]
    public void ConvertToHtml_ShouldNorParse_WhenCrossDifferentUnderscores()
    {
        var text = @"__Start _something__ now_";
        
        var content = Md.ConvertToHtml(text);
    
        content.Should().Be("<p>__Start _something__ now_</p>");
    }
    [Test]
    public void ConvertToHtml_ShouldNorParse_WhenOnlyUnderscores()
    {
        var text = @"___";
        
        var content = Md.ConvertToHtml(text);
    
        content.Should().Be("<p>___</p>");
    }
    [Test]
    public void ConvertToHtml_ShouldNorParse_WhenUnderscoresWithWhitespaceAfterStart()
    {
        var text = @"Start_ something_ now";
        
        var content = Md.ConvertToHtml(text);
    
        content.Should().Be("<p>Start_ something_ now</p>");
    }
    [Test]
    public void ConvertToHtml_ShouldNorParse_WhenUnderscoresWithWhitespaceBeforeEnd()
    {
        var text = @"Start _something _now";
        
        var content = Md.ConvertToHtml(text);
    
        content.Should().Be("<p>Start _something _now</p>");
    }
    [Test]
    public void ConvertToHtml_ShouldParse_WhenScreeningUnderscore()
    {
        var text = @"S\_tar\_t";
        
        var content = Md.ConvertToHtml(text);
    
        content.Should().Be("<p>S_tar_t</p>");
    }
    [Test]
    public void ConvertToHtml_ShouldParse_WhenScreeningTwoUnderscore()
    {
        var text = @"S\__tar\__t";
        
        var content = Md.ConvertToHtml(text);
    
        content.Should().Be("<p>S_<em>tar_</em>t</p>");
    }
    
    [Test]
    public void ConvertToHtml_ShouldParse_WhenScreeningOctothorpe()
    {
        var text = @"\#Start";
        
        var content = Md.ConvertToHtml(text);
    
        content.Should().Be("<p>#Start</p>");
    }
    
    [Test]
    public void ConvertToHtml_ShouldParse_WhenScreeningSlash()
    {
        var text = @"\\Start";
        
        var content = Md.ConvertToHtml(text);
    
        content.Should().Be(@"<p>\Start</p>");
    }
}