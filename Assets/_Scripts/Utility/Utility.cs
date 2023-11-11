using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class Utility
{
    public static bool IsInFOVConeAndLineOfSightOptimized(
        Vector3 sourcePosition,
        Vector3 sourceDirection,
        Vector3 targetPosition,
        float detectionRange,
        float fov
    )
    {
        return IsInRange(sourcePosition, targetPosition, detectionRange) &&
               IsInFOVCone(sourcePosition, sourceDirection, fov, targetPosition) &&
               IsInLineOfSight(sourcePosition, targetPosition, LayerMask.GetMask("Environment"));
       
    }

    public static bool IsInRange(Vector3 sourcePosition, Vector3 targetPosition, float range)
    {
        return Vector3.Distance(sourcePosition, targetPosition) < range;
    }

    public static bool IsInFOVCone(Vector3 eyePosition, Vector3 eyeDirection, float fov, Vector3 targetPosition)
    {
        Vector3 targetVector = targetPosition - eyePosition;
        float angle = Vector3.Angle(targetVector, eyeDirection);

        return angle < (fov / 2f);
    }

    public static bool IsInLineOfSight(Vector3 sourcePosition, Vector3 targetPosition, LayerMask layerMask)
    {
        Vector3 direction = targetPosition - sourcePosition;

        RaycastHit2D raycastHit2D = Physics2D.Raycast(sourcePosition, direction, direction.magnitude, layerMask);

        if (raycastHit2D.collider != null)
        {
            Debug.DrawLine(sourcePosition, raycastHit2D.point, Color.red);
        }

        return raycastHit2D.collider == null;
    }

    public static Quaternion OrientationFromVector3(Vector3 directionVector)
    {
        float angle = Mathf.Atan2(-directionVector.x, directionVector.y) * Mathf.Rad2Deg;
        return Quaternion.AngleAxis(angle, Vector3.forward);
    }

    public static Quaternion OrientationFromAngle(float angle)
    {
        return Quaternion.AngleAxis(angle, Vector3.forward);
    }
}
