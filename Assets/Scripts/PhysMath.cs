using UnityEngine;

public class PhysMath
{

    public static GameObject FindBone(GameObject armature, string name)
    {
        GameObject obj = FindBoneRecursive(armature, name);
        if (obj == null)
        {
            Debug.Log("Couldn't find bone of name " + name);
        }
        return obj;
    }
    static GameObject FindBoneRecursive(GameObject armature, string name)
    {
        for (int i = 0; i < armature.transform.childCount; i++)
        {
            if (armature.transform.GetChild(i).gameObject.name.ToLower().Trim() == name.ToLower().Trim())
            {
                return armature.transform.GetChild(i).gameObject;
            }
            else
            {
                GameObject obj = FindBoneRecursive(armature.transform.GetChild(i).gameObject, name);
                if (obj != null)
                    return obj;
            }
        }
        return null;
    }
    public static float gramsToKiloJoules(float grams)
    {
        return grams * 9.091f * Mathf.Pow(10, 10);
    }

    public static float kiloJoulesToGrams(float kiloJoules)
    {
        return kiloJoules / (9.091f * Mathf.Pow(10, 10));
    }

    public static float gramsToKiloWattHours(float grams)
    {
        return grams * 2.5182f * Mathf.Pow(10, 10);
    }

    public static float kiloWattHoursToGrams(float kiloWattHours)
    {
        return kiloWattHours / (2.5182f * Mathf.Pow(10, 10));
    }

    public static float hoursToSeconds(float hours)
    {
        return hours * 3600;
    }
    public static float secondsToHours(float seconds)
    {
        return seconds / 3600;
    }

}
