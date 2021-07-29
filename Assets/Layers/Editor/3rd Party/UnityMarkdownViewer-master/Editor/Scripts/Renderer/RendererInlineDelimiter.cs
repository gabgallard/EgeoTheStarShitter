////////////////////////////////////////////////////////////////////////////////

using Markdig.Renderers;
using Markdig.Syntax.Inlines;

namespace ABXY.Layers.Editor.ThirdParty.Editor.Scripts.Renderer
{
    /// <see cref="Markdig.Renderers.Html.Inlines.DelimiterInlineRenderer"/>

    public class RendererInlineDelimiter : MarkdownObjectRenderer<RendererMarkdown, DelimiterInline>
    {
        protected override void Write( RendererMarkdown renderer, DelimiterInline node )
        {
            renderer.Text( node.ToLiteral() );
            renderer.WriteChildren( node );
        }
    }
}
