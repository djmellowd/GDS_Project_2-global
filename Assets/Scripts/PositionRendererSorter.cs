﻿using UnityEngine;

namespace Utilities.MonoBehaviours {

    public class PositionRendererSorter : MonoBehaviour {

        [SerializeField] private int sortingOrderBase = 5000;
        [SerializeField] private int offset = 0;
        [SerializeField] private bool runOnlyOnce = false;

        private float timer;
        private float timerMax = .1f;
        private Renderer myRenderer;

        private void Awake() {
            myRenderer = gameObject.GetComponent<Renderer>();
        }

        private void LateUpdate() {
            timer -= Time.deltaTime;
            if (timer <= 0f) {
                timer = timerMax;
                myRenderer.sortingOrder = (int)(sortingOrderBase - transform.position.y - offset);
                if (runOnlyOnce) {
                    Destroy(this);
                }
            }
        }

        public void SetOffset(int offset) {
            this.offset = offset;
        }

    }

}