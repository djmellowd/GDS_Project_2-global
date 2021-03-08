using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UI_HotkeyBarAbilitySlot : MonoBehaviour, IPointerDownHandler, IDragHandler, IDropHandler, IBeginDragHandler, IEndDragHandler {

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Canvas canvas;
    private HotkeyAbilitySystem.HotkeyAbility hotkeyAbility;
    private HotkeyAbilitySystem hotkeySystem;
    private int abilityIndex;

    private Vector2 startAnchoredPosition;

    private void Awake() {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

        Transform testCanvasTransform = transform;
        do {
            testCanvasTransform = testCanvasTransform.parent;
            canvas = testCanvasTransform.GetComponent<Canvas>();
        } while (canvas == null);
    }

    private void Start() {
        startAnchoredPosition = rectTransform.anchoredPosition;
    }

    public int GetAbilityIndex() {
        return abilityIndex;
    }

    public HotkeyAbilitySystem.HotkeyAbility GetHotkeyAbility() {
        return hotkeyAbility;
    }

    public void Setup(HotkeyAbilitySystem hotkeySystem, int abilityIndex, HotkeyAbilitySystem.HotkeyAbility hotkeyAbility) {
        this.hotkeySystem = hotkeySystem;
        this.abilityIndex = abilityIndex;
        this.hotkeyAbility = hotkeyAbility;
    }

    public void OnPointerDown(PointerEventData eventData) {
        hotkeyAbility.activateAbilityAction();
    }

    public void OnDrop(PointerEventData eventData) {
        if (eventData.pointerDrag != null) {
            UI_HotkeyBarAbilitySlot uiHotkeyBarAbilitySlot = eventData.pointerDrag.GetComponent<UI_HotkeyBarAbilitySlot>();
            if (uiHotkeyBarAbilitySlot != null) {
                hotkeySystem.SwapAbility(hotkeyAbility, uiHotkeyBarAbilitySlot.GetHotkeyAbility());
            }
        }
    }

    public void OnDrag(PointerEventData eventData) {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData) {
        rectTransform.anchoredPosition = startAnchoredPosition;
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
    }

    public void OnBeginDrag(PointerEventData eventData) {
        canvasGroup.alpha = .6f;
        canvasGroup.blocksRaycasts = false;
        transform.SetAsLastSibling();
    }

}
