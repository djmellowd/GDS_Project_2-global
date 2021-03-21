using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DissolveAnimate : MonoBehaviour {

    private Material material;
    private DissolveAction dissolveAction;
    private float dissolveAmount;
    private float dissolveSpeed;

    private void Update() {
        dissolveAmount += dissolveSpeed * Time.deltaTime;
        SetDissolveAmount();

        if (dissolveAction != null) {
            dissolveAction.Update(dissolveAmount);
        }
    }

    private void SetDissolveAmount() {
        SetDissolveAmount(dissolveAmount);
    }

    private void SetDissolveAmount(float dissolveAmount) {
        this.dissolveAmount = dissolveAmount;
        if (material != null) material.SetFloat("_DissolveAmount", dissolveAmount);
    }

    public void StartDissolve(float startDissolveAmount, float dissolveSpeed, DissolveAction dissolveAction = null) {
        this.dissolveSpeed = dissolveSpeed;
        dissolveAmount = startDissolveAmount;
        this.dissolveAction = dissolveAction;

        RefreshMaterial();
        SetDissolveAmount();
    }

    private void RefreshMaterial() {
        if (transform.Find("Body") != null) {
            material = transform.Find("Body").GetComponent<MeshRenderer>().material;
        }
        if (GetComponent<Renderer>() != null) {
            material = GetComponent<Renderer>().material;
        }
    }

    public class DissolveAction {

        public enum Condition {
            Greater,
            Less,
        }

        private float targetDissolveAmount;
        private Condition condition;
        private Action action;
        private bool triggerOnce;
        private bool alreadyTriggeredOnce;

        public DissolveAction(float targetDissolveAmount, Condition condition, Action action, bool triggerOnce) {
            this.targetDissolveAmount = targetDissolveAmount;
            this.condition = condition;
            this.action = action;
            this.triggerOnce = triggerOnce;
            alreadyTriggeredOnce = false;
        }

        public void Update(float dissolveAmount) {
            if (triggerOnce && alreadyTriggeredOnce) {
                return;
            }

            bool isConditionTrue = false;

            switch (condition) {
            default:
            case Condition.Greater: isConditionTrue = dissolveAmount >= targetDissolveAmount; break;
            case Condition.Less:    isConditionTrue = dissolveAmount <= targetDissolveAmount; break;
            }

            if (isConditionTrue) {
                action();
                alreadyTriggeredOnce = true;
            }
        }

    }

}
