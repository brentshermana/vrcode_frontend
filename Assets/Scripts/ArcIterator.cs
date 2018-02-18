using UnityEngine;
using System;
using System.Collections.Generic;

// iterates through the coordinates of a circle
public class ArcIterator
{
    public Vector3 Axis;
    public Vector3 AxisPoint;
    public float Radius;
    
    public ArcIterator(Vector3 axis, Vector3 axisPoint, float radius)
    {
        this.Axis = axis;
        this.AxisPoint = axisPoint;
        this.Radius = radius;
    }

    public IEnumerable<Vector3> Iterator(float start_radians, float radians_inc, int max_iters = Int32.MaxValue)
    {
        Quaternion rotation = Quaternion.FromToRotation(Vector3.up, Axis);
        
        float radians = start_radians;
        int iteration = 0;
        
        while (iteration < max_iters)
        {
            // point is constructed using the y axis
            Vector3 point = new Vector3(
                Mathf.Sin(radians)*Radius,
                0f,
                Mathf.Cos(radians)*Radius
            );
            
            // rotate to the appropriate axis:
            point = rotation * point;
            // place it at the appropriate point along the axis
            point = point + AxisPoint;
            
            // update support variables
            radians += radians_inc;
            iteration++;

            yield return point;
        }
    }
}