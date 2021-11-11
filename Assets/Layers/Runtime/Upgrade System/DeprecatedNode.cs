using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[AttributeUsage(AttributeTargets.Class)]
public class DeprecatedNode : System.Attribute
{
    public string DeprecationNotice = "";

    public DeprecatedNode(string deprecationNotice)
    {
        DeprecationNotice = deprecationNotice ?? throw new ArgumentNullException(nameof(deprecationNotice));
    }
}
