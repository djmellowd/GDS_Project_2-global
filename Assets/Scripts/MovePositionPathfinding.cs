﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GridPathfindingSystem;

public class MovePositionPathfinding : MonoBehaviour {

    private Action onReachedTargetPosition;
    private List<Vector3> pathVectorList;
    private int pathIndex = -1;

    public void SetMovePosition(Vector3 movePosition, Action onReachedTargetPosition) {
        this.onReachedTargetPosition = onReachedTargetPosition;
        pathVectorList = GridPathfinding.instance.GetPathRouteWithShortcuts(transform.position, movePosition).pathVectorList;
        if (pathVectorList.Count > 0) {
        }
        if (pathVectorList.Count > 0) {
            pathIndex = 0;
        } else {
            pathIndex = -1;
        }
    }

    private void Update() {
        if (pathIndex != -1) {
            Vector3 nextPathPosition = pathVectorList[pathIndex];
            Vector3 moveVelocity = (nextPathPosition - transform.position).normalized;
            GetComponent<IMoveVelocity>().SetVelocity(moveVelocity);

            float reachedPathPositionDistance = 1f;
            if (Vector3.Distance(transform.position, nextPathPosition) < reachedPathPositionDistance) {
                pathIndex++;
                if (pathIndex >= pathVectorList.Count) {
                    pathIndex = -1;
                    onReachedTargetPosition();
                }
            }
        } else {
            GetComponent<IMoveVelocity>().SetVelocity(Vector3.zero);
        }
    }

}
