using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HelperUtilities
{
    //validation bool checking if string empty
    public static bool ValidateCheckEmptyString(Object thisObject, string fieldName, string stringToCheck)
    {
        //a
        if (stringToCheck == "")
        {
            Debug.Log(fieldName + " is empty and mush contain a value in object "+ thisObject.name.ToString());
            return true;
        }

        return false;
    }

    // checking the list if it has empty items or null value - return true if there is an error
    public static bool ValidateCheckEnumerableValues(Object thisObject, string fieldName,
        IEnumerable enumerableObjectTocheck)
    {
        bool error = false;
        int count = 0;

        foreach (var item in enumerableObjectTocheck)
        {
            if (item ==null)
            {
                Debug.Log(fieldName + " has null values in object "+thisObject.name.ToString());
            }
            else
            {
                count++;
            }
        }

        if (count==0)
        {
            Debug.Log(fieldName + " has no values in object "+thisObject.name.ToString());
            error = true;
        }

        return error;
    }
}
