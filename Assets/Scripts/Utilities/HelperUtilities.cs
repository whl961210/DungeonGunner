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
    /// <summary>
    /// null value debug check
    /// </summary>
    public static bool ValidateCheckNullValue(Object thisObject, string fieldName, UnityEngine.Object objectToCheck)
    {
        if (objectToCheck == null)
        {
            Debug.Log(fieldName + " is null and must contain a value in object " + thisObject.name.ToString());
            return true;
        }
        return false;
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
    /// <summary>
    /// positive value debug check- if zero is allowed set isZeroAllowed to true. Returns true if there is an error
    /// </summary>
    public static bool ValidateCheckPositiveValue(Object thisObject, string fieldName, int valueToCheck, bool isZeroAllowed)
    {
        bool error = false;

        if (isZeroAllowed)
        {
            if (valueToCheck < 0)
            {
                Debug.Log(fieldName + " must contain a positive value or zero in object " + thisObject.name.ToString());
                error = true;
            }
        }
        else
        {
            if (valueToCheck <= 0)
            {
                Debug.Log(fieldName + " must contain a positive value in object " + thisObject.name.ToString());
                error = true;
            }
        }

        return error;
    }
}
