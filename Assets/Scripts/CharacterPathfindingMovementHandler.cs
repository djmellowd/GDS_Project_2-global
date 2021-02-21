using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using V_AnimationSystem;
using CodeMonkey.Utils;

public class CharacterPathfindingMovementHandler : MonoBehaviour
{
    private const float speed = 40f;

    private V_UnitSkeleton unitSkeleton;
    private V_UnitAnimation unitAnimation;
    private AnimatedWalker animatedWalker;
    private int currentPathIndex;
    private List<Vector3> pathVectorList;

    private void Start()
    {
        Transform.bodyTransform = transform.Find("Body");

        unitSkeleton = new V_UnitSkeleton(1f, bodyTransform.TransformPoint, (Mesh mesh) => bodyTransform.GetComponent<MeshFilter>;
        unitAnimation = new V_UnitAnimation(unitSkeleton);
        animatedWalker = new AnimatedWalker(unitAnimation, UnitAnimType.GetUnitAnimType("dMarine_Idle"))
    }

    private void StopMoving()
    {
        pathVectorList = null;
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public void SetTargetPosition(Vector3 targetPosition)
    {
        currentPathIndex = 0;

        if (pathVectorList != null && pathVectorList.Count > 1)
        {
            pathVectorList.RemoveAt(0);
        }
    }
}
