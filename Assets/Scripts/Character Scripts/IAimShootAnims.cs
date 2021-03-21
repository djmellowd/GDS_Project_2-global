using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAimShootAnims {

    event EventHandler<CharacterAim_Base.OnShootEventArgs> OnShoot;

    void SetAimTarget(Vector3 targetPosition);
    void ShootTarget(Vector3 targetPosition, Action onShootComplete);

}
