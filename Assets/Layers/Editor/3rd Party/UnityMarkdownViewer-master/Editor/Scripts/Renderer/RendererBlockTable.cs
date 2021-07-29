////////////////////////////////////////////////////////////////////////////////

using Markdig.Extensions.Tables;
using Markdig.Renderers;

namespace ABXY.Layers.Editor.ThirdParty.Editor.Scripts.Renderer
{
    ////////////////////////////////////////////////////////////////////////////////
    // <h1>...</h1>
    /// <see cref="Markdig.Renderers.Html.HeadingRenderer"/>

    public class RendererBlockTable : MarkdownObjectRenderer<RendererMarkdown, Markdig.Extensions.Tables.Table>
    {
        protected override void Write(RendererMarkdown renderer, Table obj)
        {
            renderer.Table(obj);
        }
    }
}
