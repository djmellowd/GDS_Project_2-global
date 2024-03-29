﻿using System;
using UnityEngine;
using V_AnimationSystem;
using Utilities.Utils;
using V_ObjectSystem;

public class PlayerAim_Base : MonoBehaviour {

    #region BaseSetup
    public event EventHandler<OnShootEventArgs> OnShoot;
    public class OnShootEventArgs : EventArgs {
        public Vector3 gunEndPointPosition;
        public Vector3 shootPosition;
    }

    private V_Object vObject;
    private V_UnitSkeleton unitSkeleton;
    private V_UnitAnimation unitAnimation;
    private V_UnitSkeleton_Composite_Weapon unitSkeletonCompositeWeapon;
    private V_UnitSkeleton_Composite_Walker unitSkeletonCompositeWalker_BodyHead;
    private V_UnitSkeleton_Composite_Walker unitSkeletonCompositeWalker_Feet;
    private V_IObjectTransform objectTransform;
    private bool canShoot;
    private Vector3 aimDir;
    private Vector3 lastMoveDir;

    private void Start() {
        
        vObject = CreateBasicUnit(transform, GetPosition(), 30f, null);
        unitAnimation = vObject.GetLogic<V_UnitAnimation>();
        unitSkeleton = vObject.GetLogic<V_UnitSkeleton>();
        objectTransform = vObject.GetLogic<V_IObjectTransform>();
        
        UnitAnimType idleUnitAnim = UnitAnimType.GetUnitAnimType("dBareHands_Idle");
        UnitAnimType walkUnitAnim = UnitAnimType.GetUnitAnimType("dBareHands_Walk");
        UnitAnimType hitUnitAnim = UnitAnimType.GetUnitAnimType("dBareHands_Hit");
        UnitAnimType attackUnitAnim = UnitAnimType.GetUnitAnimType("dBareHands_PunchQuickAttack");

        canShoot = true;

        unitSkeletonCompositeWeapon = new V_UnitSkeleton_Composite_Weapon(vObject, unitSkeleton, UnitAnim.GetUnitAnim("Aim_PistolRight"), UnitAnim.GetUnitAnim("Aim_PistolLeft"), UnitAnim.GetUnitAnim("Aim_PistolShootRight"), UnitAnim.GetUnitAnim("Aim_PistolShootLeft"));
        vObject.AddRelatedObject(unitSkeletonCompositeWeapon);
        unitSkeletonCompositeWeapon.SetActive();
            
        unitSkeletonCompositeWalker_BodyHead = new V_UnitSkeleton_Composite_Walker(vObject, unitSkeleton, GameAssets.UnitAnimTypeEnum.dMarine_Walk, GameAssets.UnitAnimTypeEnum.dMarine_Idle, new[] { "Body", "Head" });
        vObject.AddRelatedObject(unitSkeletonCompositeWalker_BodyHead);
            
        unitSkeletonCompositeWalker_Feet = new V_UnitSkeleton_Composite_Walker(vObject, unitSkeleton, GameAssets.UnitAnimTypeEnum.dMarine_Walk, GameAssets.UnitAnimTypeEnum.dMarine_Idle, new[] { "FootL", "FootR" });
        vObject.AddRelatedObject(unitSkeletonCompositeWalker_Feet);
    }

    private static V_Object CreateBasicUnit(Transform unitTransform, Vector3 spawnPosition, float walkerSpeed, Material materialSpriteSheet) {
        V_Object unitObject = V_Object.CreateObject(V_Main.UpdateType.Unit);
                    
        Transform instantiatedTransform = unitTransform;

        V_IObjectTransform transform = new V_ObjectTransform_LateUpdate(instantiatedTransform, () => instantiatedTransform.transform.position, V_Main.RegisterOnLateUpdate, V_Main.DeregisterOnLateUpdate);
        unitObject.AddRelatedObject(transform);
        V_IObjectTransformBody transformBody = new V_ObjectTransformBody(instantiatedTransform.Find("Body"), materialSpriteSheet);
        unitObject.AddRelatedObject(transformBody);

        V_UnitSkeleton unitSkeleton = new V_UnitSkeleton(1f, transformBody.ConvertBodyLocalPositionToWorldPosition, transformBody.SetBodyMesh);
        unitObject.AddRelatedObject(unitSkeleton);
        unitObject.AddActiveLogic(new V_ObjectLogic_SkeletonUpdater(unitSkeleton));
        V_UnitAnimation unitAnimation = new V_UnitAnimation(unitSkeleton);
        unitObject.AddRelatedObject(unitAnimation);

        unitObject.AddRelatedObject(new V_IHasWorldPosition_Class(transform.GetPosition));

        return unitObject;
    }

    public void DestroySelf() {
        vObject.DestroySelf();
    }

    private void Update() {
    }

    public V_UnitAnimation GetUnitAnimation() {
        return unitAnimation;
    }
    public Vector3 GetPosition() {
        return transform.position;
    }
    #endregion


    public void SetAimTarget(Vector3 targetPosition) {
        aimDir = (targetPosition - vObject.GetPosition()).normalized;
        unitSkeletonCompositeWeapon.SetAimTarget(targetPosition);
    }

    public void ShootTarget(Vector3 targetPosition) {
        SetAimTarget(targetPosition);
            
        Vector3 gunEndPointPosition = vObject.GetLogic<V_UnitSkeleton>().GetBodyPartPosition("MuzzleFlash");
        RaycastHit2D raycastHit = Physics2D.Raycast(gunEndPointPosition, (targetPosition - gunEndPointPosition).normalized, Vector3.Distance(gunEndPointPosition, targetPosition));
        if (raycastHit.collider != null) {
            targetPosition = (Vector3)raycastHit.point + (targetPosition - gunEndPointPosition).normalized;
        }

        canShoot = false;

        unitSkeleton.ReplaceBodyPartSkeletonAnim(GameAssets.UnitAnimTypeEnum.dMarine_Attack.GetUnitAnim(aimDir), "Body", "Head");
        unitSkeletonCompositeWeapon.Shoot(targetPosition, () => {
            canShoot = true;
        });

        Vector3 shootFlashPosition = vObject.GetLogic<V_UnitSkeleton>().GetBodyPartPosition("MuzzleFlash");
        if (OnShoot != null) OnShoot(this, new OnShootEventArgs { gunEndPointPosition = shootFlashPosition, shootPosition = targetPosition });
    }

    public void PlayMoveAnim(Vector3 moveDir) {
        lastMoveDir = moveDir;
        bool isMoving = true;

        unitSkeletonCompositeWalker_Feet.UpdateBodyParts(isMoving, moveDir);

        if (canShoot) {
            unitSkeletonCompositeWalker_BodyHead.UpdateBodyParts(isMoving, aimDir);
        }
    }

    public void PlayIdleAnim() {
        bool isMoving = false;

        unitSkeletonCompositeWalker_Feet.UpdateBodyParts(isMoving, lastMoveDir);

        if (canShoot) {
            unitSkeletonCompositeWalker_BodyHead.UpdateBodyParts(isMoving, aimDir);
        }
    }
    
}
