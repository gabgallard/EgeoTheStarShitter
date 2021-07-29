////////////////////////////////////////////////////////////////////////////////

using Markdig.Renderers;
using Markdig.Syntax;

namespace ABXY.Layers.Editor.ThirdParty.Editor.Scripts.Renderer
{
    // <p>...</p>

    /// <see cref="Markdig.Renderers.Html.ParagraphRenderer"/>

    public class RendererBlockParagraph : MarkdownObjectRenderer<RendererMarkdown, ParagraphBlock>
    {
        protected override void Write( RendererMarkdown renderer, ParagraphBlock block )
        {
            renderer.WriteLeafBlockInline( block );
            renderer.FinishBlock( true );
        }
    }
}
