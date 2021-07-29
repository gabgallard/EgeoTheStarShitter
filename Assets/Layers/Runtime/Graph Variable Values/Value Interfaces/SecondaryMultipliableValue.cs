using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface SecondaryMultipliableValue 
{

    object Multiply(object a, string secondType, object b);

    string[] GetSecondaryMultiplyTypes();
}
