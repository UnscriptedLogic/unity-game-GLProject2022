using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
}