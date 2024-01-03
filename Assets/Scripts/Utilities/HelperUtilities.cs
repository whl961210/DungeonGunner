using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HelperUtilities
{
    public static bool ValidateCheckEmptyString(Object thisObject, string fileName, string stringToCheck)
    {
        if (stringToCheck == "")
        {
            Debug.LogError(fileName + " is empty and must contain a value in the object " + thisObject.name.ToString() + ".");
            return true;
        }
        else
        {
            return false;
        }
    }
    public static bool ValidateCheckEnumerableValues(Object thisObject, string fileName, IEnumerable enumerableToCheck)
    {
        bool error = false;
        int count = 0;

        if (enumerableToCheck == null)
        {
            Debug.LogError(fileName + " is empty and must contain a value in the object " + thisObject.name.ToString() + ".");
            return true;
        }
        foreach (var item in enumerableToCheck)
        {
            if (item == null)
            {
                Debug.LogError(fileName + " contains a null value in the object " + thisObject.name.ToString() + ".");
                error = true;
            }
            else
            {
                count++;
            }
        }
        if (count == 0)
        {
            Debug.LogError(fileName + " is empty and must contain a value in the object " + thisObject.name.ToString() + ".");
            error = true;
        }
        return error;
    }
}
