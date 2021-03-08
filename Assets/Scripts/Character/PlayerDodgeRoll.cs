using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

public class PlayerDodgeRoll : MonoBehaviour {

    private const float ROLL_SPEED = 250f;

    private enum State {
        Normal,
        Rolling
    }

    private PlayerMain playerMain;

    private State state;
    private Vector3 rollDir;
    private float rollSpeed;

    private void Awake() {
        playerMain = GetComponent<PlayerMain>();
        state = State.Normal;
    }

    private void Update() {
        switch (state) {
        default:
        case State.Normal:
            HandleInput();
            break;
        case State.Rolling:
            HandleRolling();
            break;
        }
    }

    private void FixedUpdate() {
        switch (state) {
        case State.Rolling:
            playerMain.PlayerRigidbody2D.velocity = rollDir * rollSpeed;
            break;
        }
    }

    private void HandleInput() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            rollDir = playerMain.PlayerMovementHandler.GetLastMoveDir();
            rollSpeed = ROLL_SPEED;
            state = State.Rolling;
            playerMain.PlayerMovementHandler.Disable();
        }
    }

    private void HandleRolling() {
        float rollSpeedDropMultiplier = 5f;
        rollSpeed -= rollSpeed * rollSpeedDropMultiplier * Time.deltaTime;

        float rollSpeedMinimum = 50f;
        if (rollSpeed < rollSpeedMinimum) {
            state = State.Normal;
            playerMain.PlayerMovementHandler.Enable();
        }
    }
}
