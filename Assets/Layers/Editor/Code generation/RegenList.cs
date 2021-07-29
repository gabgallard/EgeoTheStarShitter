using System.Collections.Generic;
using ABXY.Layers.Runtime;
using ABXY.Layers.Runtime.Nodes.Signal_Sources;
using ABXY.Layers.Runtime.Settings;
using UnityEditor;
using UnityEngine;
using static ABXY.Layers.Runtime.Settings.LayersSettings;

namespace ABXY.Layers.Editor.Code_generation
{
    public class RegenList
    {
        

        public static void Generate()
        {
            foreach (SoundGraphDBEntry soundGraph in LayersSettings.GetOrCreateSettings().GetGraphsNeedingCodeGen())
            {
                if (soundGraph != null)
                {
                    CodeGenerator.Generate(soundGraph);
                    LayersSettings.GetOrCreateSettings().MarkAsGenerated(soundGraph);
                }
            }
        }

        public static void GenerateAll()
        {
            foreach (SoundGraphDBEntry graph in LayersSettings.GetOrCreateSettings().GetAllDBItems())
            {
                if (graph != null)
                {
                    CodeGenerator.Generate(graph);
                    LayersSettings.GetOrCreateSettings().MarkAsGenerated(graph);
                }
            }
        }


    }
}
