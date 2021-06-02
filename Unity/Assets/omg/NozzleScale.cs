using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteAlways]
public class NozzleScale : MonoBehaviour
{
    // These define the base measurements that the modle of the nozzle represents without any scaling or adjustments
    // IE as modeled and imported into Unity
    private const float k_baseExitDiameter = 1f; // 1m
    private const float k_baseExitRadius = k_baseExitDiameter * 0.5f;
    private const float k_baseExitArea = 3.1415f * (k_baseExitRadius * k_baseExitRadius);
    private const float k_baseThroatDiameter = 0.365f; // 365cm
    private const float k_baseThroatRadius = k_baseThroatDiameter * 0.5f;
    private const float k_baseThroatArea = 3.1415f * (k_baseThroatRadius * k_baseThroatRadius);
    private const float k_baseAreaRatio = k_baseExitArea / k_baseThroatArea;

    private float areaRatio;
    public float exitDiameter = 1f;

    private float computedThroatRadius = 0.365f;
    private float computedLength;

    private Vector3 nozzleScale = Vector3.one;
    public Vector3 pivotOffset;
    // Update is called once per frame
    void Update()
    {
        nozzleScale = new Vector3(exitDiameter, exitDiameter, exitDiameter);
        ScaleAround(this.gameObject, pivotOffset, nozzleScale);
    }
    
    /// <summary>
    /// Scales the target around an arbitrary point by scaleFactor.
    /// This is relative scaling, meaning using  scale Factor of Vector3.one
    /// will not change anything and new Vector3(0.5f,0.5f,0.5f) will reduce
    /// the object size by half.
    /// The pivot is assumed to be the position in the space of the target.
    /// Scaling is applied to localScale of target.
    /// </summary>
    /// <param name="target">The object to scale.</param>
    /// <param name="pivot">The point to scale around in space of target.</param>
    /// <param name="scaleFactor">The factor with which the current localScale of the target will be multiplied with.</param>
    public static void ScaleAroundRelative(GameObject target, Vector3 pivot, Vector3 scaleFactor)
    {
        // pivot
        var pivotDelta = target.transform.localPosition - pivot;
        pivotDelta.Scale(scaleFactor);
        target.transform.localPosition = pivot + pivotDelta;
 
        // scale
        var finalScale = target.transform.localScale;
        finalScale.Scale(scaleFactor);
        target.transform.localScale = finalScale;
    }
    
    /// <summary>
    /// Scales the target around an arbitrary pivot.
    /// This is absolute scaling, meaning using for example a scale factor of
    /// Vector3.one will set the localScale of target to x=1, y=1 and z=1.
    /// The pivot is assumed to be the position in the space of the target.
    /// Scaling is applied to localScale of target.
    /// </summary>
    /// <param name="target">The object to scale.</param>
    /// <param name="pivot">The point to scale around in the space of target.</param>
    /// <param name="scaleFactor">The new localScale the target object will have after scaling.</param>
    public static void ScaleAround(GameObject target, Vector3 pivot, Vector3 newScale)
    {
        // pivot
        Vector3 pivotDelta = target.transform.localPosition - pivot; // diff from object pivot to desired pivot/origin
        Vector3 scaleFactor = new Vector3(
            newScale.x / target.transform.localScale.x,
            newScale.y / target.transform.localScale.y,
            newScale.z / target.transform.localScale.z );
        pivotDelta.Scale(scaleFactor);
        target.transform.localPosition = pivot + pivotDelta;
 
        //scale
        target.transform.localScale = newScale;
    }
    
}
