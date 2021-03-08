using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using V_AnimationSystem;

public interface ICharacterAnims {
    
    void PlayIdleAnim();
    void PlayMoveAnim(Vector3 animDir);
    V_UnitAnimation GetUnitAnimation();

}
