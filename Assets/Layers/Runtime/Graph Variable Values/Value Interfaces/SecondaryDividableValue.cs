using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface SecondaryDividableValue
{
    object Divide(object a, string secondType, object b);

    string[] GetSecondaryDivideTypes();
}
