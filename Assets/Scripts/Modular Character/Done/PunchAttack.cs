using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;
using V_AnimationSystem;

public class PunchAttack : MonoBehaviour, IAttack {

    private enum State {
        Normal,
        Attacking
    }

    private Character_Base characterBase;
    private State state;

    private void Awake() {
        characterBase = GetComponent<Character_Base>();
        SetStateNormal();
    }

    private void SetStateAttacking() {
        state = State.Attacking;
        GetComponent<IMoveVelocity>().Disable();
    }

    private void SetStateNormal() {
        state = State.Normal;
        GetComponent<IMoveVelocity>().Enable();
    }

    public void Attack(Vector3 attackDir) {
        SetStateAttacking();

        characterBase.PlayAttackAnimation(attackDir, SetStateNormal);
    }

    private Vector3 GetPosition() {
        return transform.position;
    }

}
