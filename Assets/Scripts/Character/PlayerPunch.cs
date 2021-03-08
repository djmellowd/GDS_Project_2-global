using System;
using UnityEngine;
using V_AnimationSystem;
using CodeMonkey.Utils;

public class PlayerPunch : MonoBehaviour {
    
    public static PlayerPunch instance;

    private const float SPEED = 50f;
    
    private PlayerMain playerMain;
    private Character_Base characterBase;
    private State state;
    private Material material;
    private Color materialTintColor;

    private enum State {
        Normal,
        Attacking,
    }

    private void Awake() {
        instance = this;
        playerMain = GetComponent<PlayerMain>();
        characterBase = gameObject.GetComponent<Character_Base>();
        material = transform.Find("Body").GetComponent<MeshRenderer>().material;
        materialTintColor = new Color(1, 0, 0, 0);
    }

    private void Start() {
        SetStateNormal();
    }

    private void Update() {
        switch (state) {
        case State.Normal:
            HandleAttack();
            break;
        case State.Attacking:
            HandleAttack();
            break;
        }

        if (materialTintColor.a > 0) {
            float tintFadeSpeed = 6f;
            materialTintColor.a -= tintFadeSpeed * Time.deltaTime;
            material.SetColor("_Tint", materialTintColor);
        }
    }
    
    private void SetStateNormal() {
        state = State.Normal;
        playerMain.PlayerMovementHandler.Enable();
    }

    private void SetStateAttacking() {
        state = State.Attacking;
        playerMain.PlayerMovementHandler.Disable();
    }

    private void HandleAttack() {
        if (Input.GetMouseButtonDown(0)) {
            SetStateAttacking();
            
            Vector3 attackDir = (UtilsClass.GetMouseWorldPosition() - GetPosition()).normalized;

            EnemyHandler enemyHandler = EnemyHandler.GetClosestEnemy(GetPosition() + attackDir * 4f, 20f);
            bool hitEnemy;
            if (enemyHandler != null) {
                hitEnemy = true;
                attackDir = (enemyHandler.GetPosition() - GetPosition()).normalized;
                transform.position = enemyHandler.GetPosition() + attackDir * -12f;
            } else {
                hitEnemy = false;
                transform.position = transform.position + attackDir * 4f;
            }

            float attackAngle = UtilsClass.GetAngleFromVectorFloat(attackDir);

            if (characterBase.IsPlayingPunchAnimation()) {
                characterBase.PlayKickAnimation(attackDir, (Vector3 impactPosition) => {
                    if (hitEnemy) {
                        impactPosition += UtilsClass.GetVectorFromAngle((int)attackAngle) * 4f;
                        Transform impactEffect = Instantiate(GameAssets.i.pfImpactEffect, impactPosition, Quaternion.identity);
                        impactEffect.eulerAngles = new Vector3(0, 0, attackAngle - 90);
                    }
                }, SetStateNormal);
            } else {
                characterBase.PlayPunchAnimation(attackDir, (Vector3 impactPosition) => {
                    if (hitEnemy) {
                        impactPosition += UtilsClass.GetVectorFromAngle((int)attackAngle) * 4f;
                        Transform impactEffect = Instantiate(GameAssets.i.pfImpactEffect, impactPosition, Quaternion.identity);
                        impactEffect.eulerAngles = new Vector3(0, 0, attackAngle - 90);
                    }
                }, SetStateNormal);
            }
        }
    }
    
    private void DamageFlash() {
        materialTintColor = new Color(1, 0, 0, 1f);
        material.SetColor("_Tint", materialTintColor);
    }

    public void DamageKnockback(Vector3 knockbackDir, float knockbackDistance) {
        transform.position += knockbackDir * knockbackDistance;
        DamageFlash();
    }

    public Vector3 GetPosition() {
        return transform.position;
    }
        
}
