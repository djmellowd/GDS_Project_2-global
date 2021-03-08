using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_HotkeyBar : MonoBehaviour {

    private Transform abilitySlotTemplate;
    private HotkeyAbilitySystem hotkeyAbilitySystem;

    private void Awake() {
        abilitySlotTemplate = transform.Find("abilitySlotTemplate");
        abilitySlotTemplate.gameObject.SetActive(false);
    }

    public void SetHotkeyAbilitySystem(HotkeyAbilitySystem hotkeyAbilitySystem) {
        this.hotkeyAbilitySystem = hotkeyAbilitySystem;

        hotkeyAbilitySystem.OnAbilityListChanged += HotkeyAbilitySystem_OnAbilityListChanged;

        UpdateVisual();
    }

    private void HotkeyAbilitySystem_OnAbilityListChanged(object sender, System.EventArgs e) {
        UpdateVisual();
    }

    private void UpdateVisual() {
        foreach (Transform child in transform) {
            if (child == abilitySlotTemplate) continue;
            Destroy(child.gameObject);
        }

        List<HotkeyAbilitySystem.HotkeyAbility> hotkeyAbilityList = hotkeyAbilitySystem.GetHotkeyAbilityList();
        for (int i = 0; i < hotkeyAbilityList.Count; i++) {
            HotkeyAbilitySystem.HotkeyAbility hotkeyAbility = hotkeyAbilityList[i];
            Transform abilitySlotTransform = Instantiate(abilitySlotTemplate, transform);
            abilitySlotTransform.gameObject.SetActive(true);
            RectTransform abilitySlotRectTransform = abilitySlotTransform.GetComponent<RectTransform>();
            abilitySlotRectTransform.anchoredPosition = new Vector2(100f * i, 0f);
            abilitySlotTransform.Find("itemImage").GetComponent<Image>().sprite = hotkeyAbility.GetSprite();
            abilitySlotTransform.Find("numberText").GetComponent<TMPro.TextMeshProUGUI>().SetText((i + 1).ToString());

            abilitySlotTransform.GetComponent<UI_HotkeyBarAbilitySlot>().Setup(hotkeyAbilitySystem, i, hotkeyAbility);
        }
        
        hotkeyAbilityList = hotkeyAbilitySystem.GetExtraHotkeyAbilityList();
        for (int i = 0; i < hotkeyAbilityList.Count; i++) {
            HotkeyAbilitySystem.HotkeyAbility hotkeyAbility = hotkeyAbilityList[i];
            Transform abilitySlotTransform = Instantiate(abilitySlotTemplate, transform);
            abilitySlotTransform.gameObject.SetActive(true);
            RectTransform abilitySlotRectTransform = abilitySlotTransform.GetComponent<RectTransform>();
            abilitySlotRectTransform.anchoredPosition = new Vector2(600f + 100f * i, 0f);
            abilitySlotTransform.Find("itemImage").GetComponent<Image>().sprite = hotkeyAbility.GetSprite();
            abilitySlotTransform.Find("numberText").GetComponent<TMPro.TextMeshProUGUI>().SetText("");

            abilitySlotTransform.GetComponent<UI_HotkeyBarAbilitySlot>().Setup(hotkeyAbilitySystem, -1, hotkeyAbility);
        }
    }

}
