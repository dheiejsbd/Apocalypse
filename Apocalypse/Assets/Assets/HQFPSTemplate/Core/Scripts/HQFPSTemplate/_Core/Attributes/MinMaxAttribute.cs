using UnityEngine;

public class MinMaxAttribute : PropertyAttribute
{
    public float MinLimit = 0;
    public float MaxLimit = 1;
    public bool DrawRangeValue = false;

    public MinMaxAttribute(float min, float max, bool drawRangeValue = true)
    {
        MinLimit = min;
        MaxLimit = max;
        DrawRangeValue = drawRangeValue;
    }   
}