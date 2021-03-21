using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using V_AnimationSystem;
using V_ObjectSystem;

public class V_UnitSkeleton_Composite_Walker {

    private V_Object parentObject;

    private V_UnitSkeleton unitSkeleton;
    private UnitAnimType walkAnimType;
    private UnitAnimType idleAnimType;
    private string[] replaceBodyPartArray;

    public V_UnitSkeleton_Composite_Walker(V_Object parentObject, V_UnitSkeleton unitSkeleton, UnitAnimType walkAnimType, UnitAnimType idleAnimType, string[] replaceBodyPartArray) {
        this.parentObject = parentObject;
        this.unitSkeleton = unitSkeleton;
        this.walkAnimType = walkAnimType;
        this.idleAnimType = idleAnimType;
        this.replaceBodyPartArray = replaceBodyPartArray;
    }

    public void UpdateBodyParts(bool isMoving, Vector3 dir) {
        if (isMoving) {
            unitSkeleton.ReplaceBodyPartSkeletonAnim(walkAnimType.GetUnitAnim(dir), replaceBodyPartArray);
        } else {
            unitSkeleton.ReplaceBodyPartSkeletonAnim(idleAnimType.GetUnitAnim(dir), replaceBodyPartArray);
        }
    }

}
