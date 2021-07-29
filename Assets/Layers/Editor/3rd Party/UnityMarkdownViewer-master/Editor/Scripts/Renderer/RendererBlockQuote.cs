////////////////////////////////////////////////////////////////////////////////

using Markdig.Renderers;
using Markdig.Syntax;

namespace ABXY.Layers.Editor.ThirdParty.Editor.Scripts.Renderer
{
    ////////////////////////////////////////////////////////////////////////////////
    // <blockquote>...</blockquote>
    /// <see cref="Markdig.Renderers.Html.QuoteBlockRenderer"/>

    public class RendererBlockQuote : MarkdownObjectRenderer<RendererMarkdown, QuoteBlock>
    {
        protected override void Write( RendererMarkdown renderer, QuoteBlock block )
        {
            var prevImplicit = renderer.ConsumeSpace;
            renderer.ConsumeSpace = false;

            renderer.Layout.StartBlock( true );
            renderer.WriteChildren( block );
            renderer.Layout.EndBlock();

            renderer.ConsumeSpace = prevImplicit;

            renderer.FinishBlock( true );
        }
    }
}
