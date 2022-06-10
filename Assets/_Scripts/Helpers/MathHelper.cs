using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class MathHelper
{
    public static void ModifyValue(ModificationType modificationType, ref float value, float amount)
    {
        switch (modificationType)
        {
            case ModificationType.Add:
                value += amount;
                break;
            case ModificationType.Subtract:
                value -= amount;
                break;
            case ModificationType.Set:
                value = amount;
                break;
            case ModificationType.Divide:
                value /= amount;
                break;
            case ModificationType.Multiply:
                value *= amount;
                break;
            default:
                break;
        }
    }

    public static float BetweenFloats(float a = 0f, float b = 100f)
    {
        return UnityEngine.Random.Range(a, b);
    }

    public static int BetweenInts(int a = 0, int b = 100)
    {
        return UnityEngine.Random.Range(a, b);
    }

    public static float FromFloatZeroTo(float value)
    {
        return UnityEngine.Random.Range(0f, value);
    }

    public static int FromIntZeroTo(int value)
    {
        return UnityEngine.Random.Range(0, value);
    }

    public static T RandomFromList<T>(T[] list)
    {
        int index = FromIntZeroTo(list.Length);
        return list[index];
    }

    public static T FromList<T>(T[] list, out int index)
    {
        index = FromIntZeroTo(list.Length);
        return list[index];
    }

    public static Vector3 InArea(Vector3 spawnArea)
    {
        float xPos = UnityEngine.Random.Range(-spawnArea.x / 2f, spawnArea.x / 2f);
        float yPos = UnityEngine.Random.Range(-spawnArea.y / 2f, spawnArea.y / 2f);
        float zPos = UnityEngine.Random.Range(-spawnArea.z / 2f, spawnArea.z / 2f);
        return new Vector3(xPos, yPos, zPos);
    }

    public static Vector3 OfVectorDirectionAroundY()
    {
        int index = FromIntZeroTo(4);
        if (index == 0)
        {
            return Vector3.forward;
        }
        else if (index == 1)
        {
            return Vector3.back;
        }
        else if (index == 3)
        {
            return Vector3.left;
        }
        else
        {
            return Vector3.right;
        }
    }

    public static Vector3 PointAtCircumferenceXZ(Vector3 center, float radius)
    {
        float theta = FromFloatZeroTo(360);
        float opposite = radius * Mathf.Sin(theta);
        float adjacent = radius * Mathf.Cos(theta);
        return center + new Vector3(adjacent, 0f, opposite);
    }

    public static Vector3 OfVectorDirectionAny()
    {
        int index = FromIntZeroTo(6);
        if (index == 0)
        {
            return Vector3.forward;
        }
        else if (index == 1)
        {
            return Vector3.back;
        }
        else if (index == 3)
        {
            return Vector3.left;
        }
        else if (index == 4)
        {
            return Vector3.right;
        }
        else if (index == 5)
        {
            return Vector3.up;
        }
        else
        {
            return Vector3.down;
        }
    }

    //My glorious tier chance, number line, random index generator
    public static int RandomIndex<T>(T[] list, float[] chances)
    {
        float[] tierChances = new float[list.Length];
        float prevChance = 0f;
        //makes tierChances look like a number line
        //0--[chance 1]--30--[chance 2]--70--[chance 3]--100
        for (int i = 0; i < list.Length; i++)
        {
            tierChances[i] = prevChance + chances[i];
            prevChance = tierChances[i];
        }

        //simple randomizes a number and then check the ranges
        int randomTier = UnityEngine.Random.Range(0, 100);
        for (int i = 0; i < tierChances.Length; i++)
        {
            float highNum = i == tierChances.Length - 1 ? 100 : tierChances[i];
            float lowNum = i == 0 ? 0 : tierChances[i - 1];
            if (randomTier > lowNum && randomTier < highNum)
            {
                return i;
            }
        }

        return 0;
    }
}