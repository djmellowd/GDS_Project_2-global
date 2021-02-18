using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

public class UnitGridCombat : MonoBehaviour
{
    [SerializeField] private Team team;
    private Character_Base characterBase;
    private HealthSystem healthSystem;
    private GameObject selectedGameObject;
    private MovePositionPathfinding movePosition;
    private State state;
    private World_Bar healthBar;
    public enum Team { };
    private enum State { };

    private void Awake()
    {

    }

    private void HealthSystem_OnHealthChanged(object sender, EventArgs e)
    {

    }

    private void Update()
    {

    }

    public void SetSelectedVisible(bool visible)
    {

    }

    public void MoveTo(Vector3 targetPosition, Action onReachedPosition)
    {

    }

    public bool CanAttackUnit(UnitGridCombat unitGridCombat)
    {
        return Vector3.Distance(GetPosition(), unitGridCombat.GetPosition()) < 60f;
    }

    public void AttackUnit(UnitGridCombat unitGridCombat, Action onAttackComplete)
    {

    }

    private void ShootUnit(UnitGridCombat unitGridCombat, Action onShotComplete)
    {

    }

    public void Damage(UnitGridCombat attacker, int damageAmount)
    {

    }

    public bool isDead()
    {

    }

    public Vector3 GetPosition()
    {

    }
}
