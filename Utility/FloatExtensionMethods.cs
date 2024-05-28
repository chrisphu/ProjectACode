using Godot;
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
}