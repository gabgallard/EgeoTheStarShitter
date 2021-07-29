////////////////////////////////////////////////////////////////////////////////

using Markdig.Renderers;
using Markdig.Syntax.Inlines;

namespace ABXY.Layers.Editor.ThirdParty.Editor.Scripts.Renderer
{
    /// <see cref="Markdig.Renderers.Html.Inlines.LiteralInlineRenderer"/>
    /// <see cref="Markdig.Renderers.Normalize.Inlines.LiteralInlineRenderer"/>

    public class RendererInlineLiteral : MarkdownObjectRenderer<RendererMarkdown, LiteralInline>
    {
        protected override void Write( RendererMarkdown renderer, LiteralInline node )
        {
            renderer.Text( node.Content.ToString() );
        }
    }
}
