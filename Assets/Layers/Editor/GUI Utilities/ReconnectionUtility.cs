using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReconnectionUtility
{
    private Dictionary<string, List<NodePort>> connections = new Dictionary<string, List<NodePort>>();

    private Node node;

    public ReconnectionUtility(Node node)
    {
        this.node = node;
        foreach(NodePort port in node.DynamicPorts)
        {
            if (port.IsConnected)
            {
                List<NodePort> connectedPorts = port.GetConnections();

                if (connections.ContainsKey(port.fieldName))
                    connections[port.fieldName] = connectedPorts;
                else
                    connections.Add(port.fieldName, connectedPorts);
            }
        }
    }

    public void Reload()
    {
        foreach (NodePort port in node.DynamicPorts)
        {
            List<NodePort> previousPorts = new List<NodePort>();
            if (connections.TryGetValue(port.fieldName, out previousPorts))
            {

                foreach (NodePort previousPort in previousPorts)
                {
                    if (previousPort != null && previousPort.direction != port.direction)
                    {
                        port.Connect(previousPort);
                    }
                }
            }
        }
    }
}
