using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotkeyAbilitySystem {

    public event EventHandler OnAbilityListChanged;

    public enum AbilityType {
        Pistol,
        Shotgun,
        Sword,
        Punch,
        HealthPotion,
        ManaPotion,
    }

    private List<HotkeyAbility> hotkeyAbilityList;
    private List<HotkeyAbility> extraHotkeyAbilityList;

    public HotkeyAbilitySystem() {
        hotkeyAbilityList = new List<HotkeyAbility>();
        extraHotkeyAbilityList = new List<HotkeyAbility>();

        hotkeyAbilityList.Add(new HotkeyAbility {
            abilityType = AbilityType.HealthPotion,
            activateAbilityAction = () => { }
        });

        
        extraHotkeyAbilityList.Add(new HotkeyAbility { 
            abilityType = AbilityType.ManaPotion, 
            activateAbilityAction = () => { }
        });
    }

    public void Update() {
        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            hotkeyAbilityList[0].activateAbilityAction();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2)) {
            hotkeyAbilityList[1].activateAbilityAction();
        }
        if (Input.GetKeyDown(KeyCode.Alpha3)) {
            hotkeyAbilityList[2].activateAbilityAction();
        }
        if (Input.GetKeyDown(KeyCode.Alpha4)) {
            hotkeyAbilityList[3].activateAbilityAction();
        }
        if (Input.GetKeyDown(KeyCode.Alpha5)) {
            hotkeyAbilityList[4].activateAbilityAction();
        }
    }
    
    public List<HotkeyAbility> GetHotkeyAbilityList() {
        return hotkeyAbilityList;
    }
    
    public List<HotkeyAbility> GetExtraHotkeyAbilityList() {
        return extraHotkeyAbilityList;
    }

    public void SwapAbility(int abilityIndexA, int abilityIndexB) {
        HotkeyAbility hotkeyAbility = hotkeyAbilityList[abilityIndexA];
        hotkeyAbilityList[abilityIndexA] = hotkeyAbilityList[abilityIndexB];
        hotkeyAbilityList[abilityIndexB] = hotkeyAbility;
        OnAbilityListChanged?.Invoke(this, EventArgs.Empty);
    }
    
    public void SwapAbility(HotkeyAbility hotkeyAbilityA, HotkeyAbility hotkeyAbilityB) {
        if (extraHotkeyAbilityList.Contains(hotkeyAbilityA)) {
            int indexB = hotkeyAbilityList.IndexOf(hotkeyAbilityB);
            hotkeyAbilityList[indexB] = hotkeyAbilityA;

            extraHotkeyAbilityList.Remove(hotkeyAbilityA);
            extraHotkeyAbilityList.Add(hotkeyAbilityB);
        } else {
            if (extraHotkeyAbilityList.Contains(hotkeyAbilityB)) {
                int indexA = hotkeyAbilityList.IndexOf(hotkeyAbilityA);
                hotkeyAbilityList[indexA] = hotkeyAbilityB;

                extraHotkeyAbilityList.Remove(hotkeyAbilityB);
                extraHotkeyAbilityList.Add(hotkeyAbilityA);
            } else {
                int indexA = hotkeyAbilityList.IndexOf(hotkeyAbilityA);
                int indexB = hotkeyAbilityList.IndexOf(hotkeyAbilityB);
                HotkeyAbility tmp = hotkeyAbilityList[indexA];
                hotkeyAbilityList[indexA] = hotkeyAbilityList[indexB];
                hotkeyAbilityList[indexB] = tmp;
            }
        }

        OnAbilityListChanged?.Invoke(this, EventArgs.Empty);
    }

    public class HotkeyAbility {
        public AbilityType abilityType;
        public Action activateAbilityAction;

        public Sprite GetSprite() {
            switch (abilityType) {
            default:
            case AbilityType.HealthPotion:  return null;
            }
        }

        public override string ToString() {
            return abilityType.ToString();
        }
    }

}
