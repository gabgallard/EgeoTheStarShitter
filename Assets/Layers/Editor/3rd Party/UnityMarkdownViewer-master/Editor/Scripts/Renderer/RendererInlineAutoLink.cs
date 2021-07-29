////////////////////////////////////////////////////////////////////////////////

using Markdig.Renderers;
using Markdig.Syntax.Inlines;

namespace ABXY.Layers.Editor.ThirdParty.Editor.Scripts.Renderer
{
    ////////////////////////////////////////////////////////////////////////////////
    // <a href="...">...</a>
    /// <see cref="Markdig.Renderers.Html.Inlines.AutolinkInlineRenderer"/>

    public class RendererInlineAutoLink : MarkdownObjectRenderer<RendererMarkdown, AutolinkInline>
    {
        protected override void Write( RendererMarkdown renderer, AutolinkInline node )
        {
            renderer.Link = node.Url;
            renderer.Text( node.Url );
            renderer.Link = null;
        }
    }
}
