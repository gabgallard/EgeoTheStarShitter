using System;
using System.Collections;
using System.Collections.Generic;
using ABXY.Layers.Editor.Code_generation;
using ABXY.Layers.Editor.Code_generation.Core;
using ABXY.Layers.Runtime;
using ABXY.Layers.Runtime.Nodes;
using ABXY.Layers.Runtime.Nodes.Playback;
using UnityEngine;

public class StateMachineCodeGenerator : FlowNodeCodeGenerator
{
    protected override Type handlesType => typeof(StateMachineNode);

    public override void InsertPlayerCode(ClassBuilder parentClass, FlowNode targetNode)
    {
        StateMachineNode castNode = targetNode as StateMachineNode;
        EnumBuilder enumbuilder = new EnumBuilder(ReflectionUtils.RemoveSpecialCharacters( castNode.stateMachineName+"States"),false);
        foreach(State state in castNode.States)
        {
            enumbuilder.AddValue(ReflectionUtils.RemoveSpecialCharacters(state.stateName));
        }
        enumbuilder.AddValue("NotActive");
        parentClass.AddField(enumbuilder);
    }

    public override void InsertEditorCode(FileBuilder parentClass, FlowNode targetNode)
    {
        parentClass.AddPreambleField(new ArbitraryLineBuilder("using System;"));
        parentClass.AddPreambleField(new ArbitraryLineBuilder("using ABXY.Layers.Runtime;"));
        parentClass.AddPreambleField(new ArbitraryLineBuilder("using ABXY.Layers.Editor;"));
        parentClass.AddPreambleField(new ArbitraryLineBuilder("using ABXY.Layers.Editor.Graph_Variable_Editors;"));
        parentClass.AddPreambleField(new ArbitraryLineBuilder("using UnityEditor;"));
        parentClass.AddPreambleField(new ArbitraryLineBuilder("using UnityEngine;"));

        
        StateMachineNode castNode = targetNode as StateMachineNode;

        dynamic model = new {
            classname = ReflectionUtils.RemoveSpecialCharacters(castNode.stateMachineName + "States"),
            parentclassname = ReflectionUtils.RemoveSpecialCharacters(targetNode.soundGraph.name),
            prettyname = castNode.stateMachineName + " States",
            parentgraphname = castNode.soundGraph.name
        };
        TemplateBuilder template = new TemplateBuilder(Resources.Load<TextAsset>("Layers/StateMachineEnum/StateMachineEnumVarInspector").text, model);
        parentClass.AddField(template);
    }

    public override void InsertNonPlayerRuntimeCode(FileBuilder parentClass, FlowNode targetNode)
    {
        parentClass.AddPreambleField(new ArbitraryLineBuilder("using System;"));
        parentClass.AddPreambleField(new ArbitraryLineBuilder("using ABXY.Layers.Runtime.Nodes.Logic;"));
        parentClass.AddPreambleField(new ArbitraryLineBuilder("using UnityEngine;"));

        StateMachineNode castNode = targetNode as StateMachineNode;

        dynamic model = new
        {
            classname = ReflectionUtils.RemoveSpecialCharacters(castNode.stateMachineName + "States"),
            parentclassname = ReflectionUtils.RemoveSpecialCharacters(targetNode.soundGraph.name),
            prettyname = castNode.stateMachineName + " States",
            parentgraphname = castNode.soundGraph.name
        };
        TemplateBuilder template = new TemplateBuilder(Resources.Load<TextAsset>("Layers/StateMachineEnum/StateMachineEnumVarValue").text, model);
        parentClass.AddField(template);

    }
}
