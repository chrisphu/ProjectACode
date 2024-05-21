using Godot;
using System;

namespace ProjectA;

public static class FloatExtensionMethods
{
    /// <summary>
    /// Calculates damped factor independent of frame rate for lerping.
    /// </summary>
    /// <param name="smoothness">Arbitrary number from 0.0f to 1.0f for lerp smoothness. Greater values are
    /// more smooth.</param>
    /// <param name="delta">Amount of time elapsed since previous frame.</param>
    /// <remarks>
    /// <see href="https://www.rorydriscoll.com/2016/03/07/frame-rate-independent-damping-using-lerp/" />
    /// </remarks>
    public static float DampFactorForLerp(float smoothness, double delta)  
    {  
        return (1.0f - Mathf.Pow(1.0f - smoothness, (float)delta));  
    }

    /// <summary>
    /// Provides linearly interpolated float value.
    /// </summary>
    /// <param name="start">Start value.</param>
    /// <param name="to">End value.</param>
    /// <param name="weight">Percentage of the way towards to from start, typically between 0.0f and 1.0f.</param>
    /// <remarks>
    /// <list>
    ///     <item>
    ///         <see href="https://docs.godotengine.org/en/stable/tutorials/math/interpolation.html" />
    ///     </item>
    ///     <item>
    ///         Using the terms "to" and "weight" to match existing terms used by Vector3.Lerp.
    ///     </item>
    /// </list>
    /// </remarks>
    public static float Lerp(this float start, float to, float weight)
    {
        return start + (to - start) * weight;
    }
}