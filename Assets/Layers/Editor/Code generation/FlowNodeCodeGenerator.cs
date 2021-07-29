using ABXY.Layers.Editor.Code_generation.Core;
using ABXY.Layers.Runtime.Nodes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FlowNodeCodeGenerator
{
    private static Dictionary<System.Type, FlowNodeCodeGenerator> generators = new Dictionary<System.Type, FlowNodeCodeGenerator>();
    protected abstract System.Type handlesType { get; }

    public virtual void InsertPlayerCode(ClassBuilder parentClass, FlowNode targetNode) { }

    public virtual void InsertNonPlayerRuntimeCode(FileBuilder parentClass, FlowNode targetNode) { }


    public virtual void InsertEditorCode(FileBuilder parentClass, FlowNode targetNode) { }

    public static FlowNodeCodeGenerator GetGenerator(System.Type nodeType)
    {
        LoadGenerators();
        FlowNodeCodeGenerator generator = null;
        generators.TryGetValue(nodeType, out generator);
        return generator;
    }

    private static void LoadGenerators()
    {
        if (generators.Count != 0)
            return;

        foreach(System.Reflection.Assembly assem in System.AppDomain.CurrentDomain.GetAssemblies())
        {
            foreach(System.Type type in assem.GetTypes())
            {
                if (type.BaseType == typeof(FlowNodeCodeGenerator) && !generators.ContainsKey(type)) {
                    FlowNodeCodeGenerator newGenerator = System.Activator.CreateInstance(type) as FlowNodeCodeGenerator;
                    if (newGenerator != null)
                        generators.Add(newGenerator.handlesType, newGenerator);

                }
            }
        }
    }
}
