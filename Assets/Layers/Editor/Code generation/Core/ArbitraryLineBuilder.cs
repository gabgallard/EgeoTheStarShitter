using System.Collections.Generic;

namespace ABXY.Layers.Editor.Code_generation.Core
{
    public class ArbitraryLineBuilder : ArbitraryBuilder
    {
        public ArbitraryLineBuilder(string content) : base(new List<string>(new string[] { content }))
        {
        }
    }
}
