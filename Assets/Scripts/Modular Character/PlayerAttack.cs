﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour {

    private void Update() {
        if (Input.GetMouseButtonDown(0)) {
            GetComponent<IAttack>().Attack(new Vector3(1, 0, 0));
        }
    }

}
