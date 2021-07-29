////////////////////////////////////////////////////////////////////////////////

using UnityEngine;

namespace ABXY.Layers.Editor.ThirdParty.Editor.Scripts.Layout
{
    public interface IActions
    {
        Texture FetchImage( string url );
        void    SelectPage( string url );
    }
}

