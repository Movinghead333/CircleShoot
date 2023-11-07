using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class Utility
{
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
}
