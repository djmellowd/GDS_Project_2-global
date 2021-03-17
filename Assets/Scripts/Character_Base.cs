﻿using System;
using UnityEngine;
using V_AnimationSystem;
using Utilities.Utils;

public class Character_Base : MonoBehaviour, ICharacterAnims, IGetPosition {
    
    #region BaseSetup
    private V_UnitSkeleton unitSkeleton;
    private V_UnitAnimation unitAnimation;
    private AnimatedWalker animatedWalker;
    [SerializeField] private string idleAnimation;
    [SerializeField] private string walkAnimation;
    
    private bool lastAttackWasPunch;

    private void Awake() {
        Transform bodyTransform = transform.Find("Body");
        unitSkeleton = new V_UnitSkeleton(1f, bodyTransform.TransformPoint, (Mesh mesh) => bodyTransform.GetComponent<MeshFilter>().mesh = mesh);
        unitAnimation = new V_UnitAnimation(unitSkeleton);
        
        UnitAnimType idleUnitAnim = UnitAnimType.GetUnitAnimType(idleAnimation);
        UnitAnimType walkUnitAnim = UnitAnimType.GetUnitAnimType(walkAnimation);
        UnitAnimType hitUnitAnim = UnitAnimType.GetUnitAnimType("dBareHands_Hit");
        UnitAnimType attackUnitAnim = UnitAnimType.GetUnitAnimType("dBareHands_PunchQuickAttack");

        animatedWalker = new AnimatedWalker(unitAnimation, idleUnitAnim, walkUnitAnim, 1f, 1f);
    }

    private void Update() {
        unitSkeleton.Update(Time.deltaTime);
    }
    
    public void SetIdleWalkAnims(UnitAnimType idleUnitAnim, UnitAnimType walkUnitAnim) {
        animatedWalker = new AnimatedWalker(unitAnimation, idleUnitAnim, walkUnitAnim, 1f, 1f);
    }

    public void RefreshBodySkeletonMesh() {
        Transform bodyTransform = transform.Find("Body");
        bodyTransform.GetComponent<MeshFilter>().mesh = unitSkeleton.GetMesh();
    }

    public V_UnitAnimation GetUnitAnimation() {
        return unitAnimation;
    }

    public Vector3 GetPosition() {
        return transform.position;
    }
    #endregion


    public void PlayMoveAnim(Vector3 moveDir) {
        animatedWalker.SetMoveVector(moveDir);
    }

    public void PlayIdleAnim() {
        animatedWalker.SetMoveVector(Vector3.zero);
    }

    public void PlayJumpAnim(Vector3 moveDir) {
        if (moveDir.x >= 0) {
            unitAnimation.PlayAnim(UnitAnim.GetUnitAnim("dBareHands_JumpRight"));
        } else {
            unitAnimation.PlayAnim(UnitAnim.GetUnitAnim("dBareHands_JumpLeft"));
        }
    }
    
    public bool IsPlayingPunchAnimation() {
        return unitAnimation.GetActiveAnimType().GetName() == "dBareHands_PunchQuick";
    }

    public bool IsPlayingKickAnimation() {
        return unitAnimation.GetActiveAnimType().GetName() == "dBareHands_KickQuick";
    }

    public void PlayAttackAnimation(Vector3 dir, Action onAnimComplete) {
        lastAttackWasPunch = !lastAttackWasPunch;
        if (lastAttackWasPunch) {
            PlayKickAnimation(dir, (Vector3 hitPosition) => {
                float attackAngle = UtilsClass.GetAngleFromVectorFloat(dir);
                hitPosition += UtilsClass.GetVectorFromAngle((int)attackAngle) * 4f;
                Transform impactEffect = Instantiate(GameAssets.i.pfImpactEffect, hitPosition, Quaternion.identity);
                impactEffect.eulerAngles = new Vector3(0, 0, attackAngle - 90);
            }, onAnimComplete);
        } else {
            PlayPunchAnimation(dir, (Vector3 hitPosition) => {
                float attackAngle = UtilsClass.GetAngleFromVectorFloat(dir);
                hitPosition += UtilsClass.GetVectorFromAngle((int)attackAngle) * 4f;
                Transform impactEffect = Instantiate(GameAssets.i.pfImpactEffect, hitPosition, Quaternion.identity);
                impactEffect.eulerAngles = new Vector3(0, 0, attackAngle - 90);
            }, onAnimComplete);
        }
    }

    public void PlayPunchAnimation(Vector3 dir, Action<Vector3> onHit, Action onAnimComplete, float frameRateMod = 1f) {
        unitAnimation.PlayAnimForced(UnitAnimType.GetUnitAnimType("dBareHands_PunchQuick"), dir, 1f, (UnitAnim unitAnim2) => {
            if (onAnimComplete != null) onAnimComplete();
        }, (string trigger) => {
            string hitBodyPartName = trigger == "HIT" ? "HandR" : "HandL";
            Vector3 impactPosition = unitSkeleton.GetBodyPartPosition(hitBodyPartName);
            if (onHit != null) {
                onHit(impactPosition);
            }
        }, null);
    }
    
    public void PlayKickAnimation(Vector3 dir, Action<Vector3> onHit, Action onAnimComplete) {
        unitAnimation.PlayAnimForced(UnitAnimType.GetUnitAnimType("dBareHands_KickQuick"), dir, 1f, (UnitAnim unitAnim2) => {
            if (onAnimComplete != null) onAnimComplete();
        }, (string trigger) => {
            string hitBodyPartName = trigger == "HIT" ? "FootL" : "FootR";
            Vector3 impactPosition = unitSkeleton.GetBodyPartPosition(hitBodyPartName);
            if (onHit != null) {
                onHit(impactPosition);
            }
        }, null);
    }

    public void PlayPunchSlowAnimation(Vector3 dir, Action<Vector3> onHit, Action onAnimComplete, float frameRateMod = 1f) {
        unitAnimation.PlayAnimForced(UnitAnimType.GetUnitAnimType("dBareHands_Punch"), dir, 1f, (UnitAnim unitAnim2) => {
            if (onAnimComplete != null) onAnimComplete();
        }, (string trigger) => {
            string hitBodyPartName = trigger == "HIT" ? "HandR" : "HandL";
            Vector3 impactPosition = unitSkeleton.GetBodyPartPosition(hitBodyPartName);
            if (onHit != null) {
                onHit(impactPosition);
            }
        }, null);
    }

    public void PlayShootAnimation(Vector3 dir, Action<Vector3> onHit, Action onAnimComplete) {
        unitAnimation.PlayAnimForced(UnitAnimType.GetUnitAnimType("dMarine_Attack"), dir, 1f, (UnitAnim unitAnim2) => {
            if (onAnimComplete != null) onAnimComplete();
        }, (string trigger) => {
            Vector3 shootGunEndPointPosition = unitSkeleton.GetBodyPartPosition("MuzzleFlash");
            if (onHit != null) {
                onHit(shootGunEndPointPosition);
            }
        }, null);
    }

    public void PlayWebZipShootAnimation(Vector3 dir) {
        unitAnimation.PlayAnimForced(UnitAnimType.GetUnitAnimType("Spiderman_ShootWebZip"), dir, 1f, null, null, null);
    }

    public void PlayWebZipFlyingAnimation(Vector3 dir) {
        unitAnimation.PlayAnimForced(UnitAnimType.GetUnitAnimType("Spiderman_WebZipFlying"), dir, 1f, null, null, null);
    }

    public void PlaySlidingAnimation(Vector3 dir) {
        unitAnimation.PlayAnimForced(UnitAnimType.GetUnitAnimType("Spiderman_Sliding"), dir, 1f, null, null, null);
    }

    public Vector3 GetHandLPosition() {
        return unitSkeleton.GetBodyPartPosition("HandL");
    }

    public Vector3 GetHandRPosition() {
        return unitSkeleton.GetBodyPartPosition("HandR");
    }
    
    public void PlayDodgeAnimation(Vector3 dir) {
        unitAnimation.PlayAnimForced(UnitAnimType.GetUnitAnimType("dBareHands_Roll"), dir, 1f, null, null, null);
    }
    
    public void PlayRollAnimation(Vector3 dir) {
        unitAnimation.PlayAnimForced(UnitAnimType.GetUnitAnimType("dBareHands_Roll"), dir, 1f, null, null, null);
    }
    
    public void PlayWinAnimation() {
        unitAnimation.PlayAnimForced(UnitAnim.GetUnitAnim("dBareHands_Victory"), 1f, null);
    }
    
    public void PlayHitAnimation(Vector3 dir, Action onAnimComplete) {
        unitAnimation.PlayAnimForced(UnitAnimType.GetUnitAnimType("dBareHands_Hit"), dir, 1f, (UnitAnim unitAnim2) => {
            if (onAnimComplete != null) onAnimComplete();
        }, (string trigger) => {
        }, null);
    }

}
