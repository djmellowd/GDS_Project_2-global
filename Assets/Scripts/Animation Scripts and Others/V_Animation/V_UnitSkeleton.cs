using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace V_AnimationSystem {

    public class V_UnitSkeleton {

        private int id = UnityEngine.Random.Range(10000, 99999);

        private Mesh mesh;
        private bool hasVariableSortingOrder;
        private bool alreadyLooped;


        private UnitAnim lastUnitAnim = UnitAnim.None;

        private float frameRateMod = 1f;

        private float refreshTimer;
        private float refreshTimerMax = .016f;

        private V_ISkeleton_Updater skeletonUpdater;

        public delegate void OnAnimComplete(UnitAnim unitAnim);
        private OnAnimComplete onAnimComplete;
        public delegate void OnAnimInterrupted(UnitAnim interruptedUnitAnim, UnitAnim unitAnim, bool alreadyLooped);
        private OnAnimInterrupted onAnimInterrupted;
        public delegate void OnAnimTrigger(string trigger);
        private OnAnimTrigger onAnimTrigger;
        public delegate Vector3 DelConvertLocalPositionToWorldPosition(Vector3 position);
        private DelConvertLocalPositionToWorldPosition ConvertLocalPositionToWorldPosition;

        public delegate void OnPointerMove(Vector3 pointer, int rot);
        private OnPointerMove onPointerMove_1, onPointerMove_2, onPointerMove_3;

        private Vector3 animPointer_1, animPointer_2, animPointer_3;

        private List<V_Skeleton_Anim> tmpAnims = new List<V_Skeleton_Anim>();

        public event OnAnimComplete OnAnyAnimComplete;
        public event OnAnimInterrupted OnAnyAnimInterrupted;
        public event OnAnimTrigger OnAnyAnimTrigger;
        public event Action<UnitAnim> OnAnyPlayAnim;

        public V_UnitSkeleton(float frameRateMod, DelConvertLocalPositionToWorldPosition ConvertLocalPositionToWorldPosition, Action<Mesh> SetMesh) {
            V_Animation.Init();
            this.frameRateMod = frameRateMod;
            this.ConvertLocalPositionToWorldPosition = ConvertLocalPositionToWorldPosition;
            mesh = new Mesh();
            mesh.MarkDynamic();
            List<Vector3> vertices = new List<Vector3>();
            List<Vector2> uvs = new List<Vector2>();
            List<int> triangles = new List<int>();

            mesh.triangles = null;
            mesh.vertices = vertices.ToArray();
            mesh.uv = uvs.ToArray();
            mesh.triangles = triangles.ToArray();

            SetMesh(mesh);

            PlayAnim(UnitAnim.DefaultAnimation, 1f, null, null, null);
        }
        public void DestroySelf() {
            UnityEngine.Object.Destroy(mesh);
        }
        public Mesh GetMesh() {
            return mesh;
        }
        public V_Skeleton_Anim[] GetAnims() {
            return skeletonUpdater.GetAnims();
        }
        public V_ISkeleton_Updater GetSkeletonUpdater() {
            return skeletonUpdater;
        }
        public Vector3 GetBodyPartPosition(string bodyPartName) {
            V_Skeleton_Anim bodyPartAnim = GetSkeletonUpdater().GetAnimWithBodyPartName(bodyPartName);
            if (bodyPartAnim != null) {
                Vector3 pos = bodyPartAnim.GetCurrentAnimFrame().pos;
                return ConvertLocalPositionToWorldPosition(pos);
            } else {
                return ConvertLocalPositionToWorldPosition(Vector3.zero);
            }
        }

        public void ReplaceAllBodyPartsInAnimation(UnitAnim unitAnim) {
            foreach (V_Skeleton_Anim skeletonAnim in unitAnim.GetAnims()) {
                bool replaced = TryReplaceBodyPartSkeletonAnim(skeletonAnim);
                if (!replaced) {
                    GetSkeletonUpdater().AddAnim(skeletonAnim);
                }
            }
        }

        public void ReplaceBodyPartSkeletonAnim(UnitAnim unitAnim, params string[] bodyPartNameArr) {
            foreach (string bodyPartName in bodyPartNameArr) {
                TryReplaceBodyPartSkeletonAnim(bodyPartName, unitAnim.GetSkeletonAnim_BodyPartCustom(bodyPartName));
            }
        }
        public bool TryReplaceBodyPartSkeletonAnim(string bodyPartName, UnitAnim unitAnim) {
            return TryReplaceBodyPartSkeletonAnim(bodyPartName, unitAnim.GetSkeletonAnim_BodyPartCustom(bodyPartName));
        }
        public bool TryReplaceBodyPartSkeletonAnim(string bodyPartName, V_Skeleton_Anim anim) {
            return GetSkeletonUpdater().TryReplaceAnimOnBodyPart(bodyPartName, anim);
        }
        public bool TryReplaceBodyPartSkeletonAnim(V_Skeleton_Anim anim) {
            return GetSkeletonUpdater().TryReplaceAnimOnBodyPart(anim.bodyPart.customName, anim);
        }

        public UnitAnim GetActiveUnitAnim() {
            return lastUnitAnim;
        }
        public void SetHasVariableSortingOrder(bool set) {
            hasVariableSortingOrder = set;
            skeletonUpdater.SetHasVariableSortingOrder(set);
        }
        public void Update(float deltaTime) {
            V_ISkeleton_Updater activeSkeletonUpdater = skeletonUpdater;
            bool allLooped = skeletonUpdater.Update(deltaTime);

            if (activeSkeletonUpdater != skeletonUpdater) {
                return;
            }

            refreshTimer -= deltaTime;
            if (refreshTimer <= 0f) {
                refreshTimer = refreshTimerMax;
                skeletonUpdater.Refresh();
            }

            if (allLooped) {
                alreadyLooped = true;
            }
            if (allLooped && onAnimComplete != null) {
                OnAnimComplete backOnAnimComplete = onAnimComplete;
                onAnimComplete = null;
                backOnAnimComplete(lastUnitAnim);
                if (OnAnyAnimComplete != null) OnAnyAnimComplete(lastUnitAnim);
            } else {
            }
        }
        public void SkeletonRefresh() {
            skeletonUpdater.Refresh();
        }



        public Vector3 GetAnimPointer(int pointer) {
            switch (pointer) {
            default:
            case 1: return ConvertLocalPositionToWorldPosition(animPointer_1);
            case 2: return ConvertLocalPositionToWorldPosition(animPointer_2);
            case 3: return ConvertLocalPositionToWorldPosition(animPointer_3);
            }
        }

        public bool ClearOnAnimInterruptedIfMatches(OnAnimInterrupted onAnimInterrupted) {
            if (this.onAnimInterrupted == onAnimInterrupted) {
                this.onAnimInterrupted = null;
                return true;
            }
            return false;
        }
        public bool PlayAnimIfOnCompleteMatches(UnitAnim unitAnim, OnAnimComplete onAnimComplete) {
            if (this.onAnimComplete == onAnimComplete) {
                PlayAnim(unitAnim, 1f, null, null, null);
                return true;
            }
            return false;
        }
        public void SetAnimsArr(V_Skeleton_Anim[] anims) {
            (skeletonUpdater as V_Skeleton_Updater).SetAnimsArr(anims);
        }
        public void PlayAnim(UnitAnim unitAnim, float frameRateModModifier, OnAnimComplete onAnimComplete, OnAnimTrigger onAnimTrigger, OnAnimInterrupted onAnimInterrupted) {
            if (this.onAnimInterrupted != null) {
                OnAnimInterrupted tmpAnimInterrupted = this.onAnimInterrupted;
                this.onAnimInterrupted = null;
                tmpAnimInterrupted(lastUnitAnim, unitAnim, alreadyLooped);
                if (OnAnyAnimInterrupted != null) OnAnyAnimInterrupted(lastUnitAnim, unitAnim, alreadyLooped);
            }

            this.onAnimComplete = onAnimComplete;
            this.onAnimTrigger = onAnimTrigger;
            this.onAnimInterrupted = onAnimInterrupted;

            lastUnitAnim = unitAnim;

            alreadyLooped = false;

            V_Skeleton_Updater newSkeletonUpdater = new V_Skeleton_Updater(mesh, unitAnim.GetAnims(), frameRateMod, frameRateModModifier, onAnimTrigger, OnAnyAnimTrigger);
            newSkeletonUpdater.TestFirstFrameTrigger();
            skeletonUpdater = newSkeletonUpdater;


            if (OnAnyPlayAnim != null) OnAnyPlayAnim(unitAnim);
        }
        public void PlayAnimContinueFrames(UnitAnim unitAnim, float frameRateModModifier, OnAnimComplete onAnimComplete, OnAnimTrigger onAnimTrigger, OnAnimInterrupted onAnimInterrupted) {
            if (this.onAnimInterrupted != null) {
                OnAnimInterrupted tmpAnimInterrupted = this.onAnimInterrupted;
                this.onAnimInterrupted = null;
                tmpAnimInterrupted(lastUnitAnim, unitAnim, alreadyLooped);
                if (OnAnyAnimInterrupted != null) OnAnyAnimInterrupted(lastUnitAnim, unitAnim, alreadyLooped);
            }

            this.onAnimComplete = onAnimComplete;
            this.onAnimTrigger = onAnimTrigger;
            this.onAnimInterrupted = onAnimInterrupted;

            lastUnitAnim = unitAnim;

            alreadyLooped = false;

            if (skeletonUpdater is V_Skeleton_Updater) {
                V_Skeleton_Updater newSkeletonUpdater = new V_Skeleton_Updater(mesh, unitAnim.GetAnims(), frameRateMod, frameRateModModifier, onAnimTrigger, OnAnyAnimTrigger);
                newSkeletonUpdater.SetFramesToSame((skeletonUpdater as V_Skeleton_Updater).GetAnims());
                skeletonUpdater = newSkeletonUpdater;
            } else {
                skeletonUpdater = new V_Skeleton_Updater(mesh, unitAnim.GetAnims(), frameRateMod, frameRateModModifier, onAnimTrigger, OnAnyAnimTrigger);
            }
        }
        public void PlayAnimContinueFrames(UnitAnim unitAnim, float frameRateModModifier) {
            lastUnitAnim = unitAnim;

            alreadyLooped = false;

            if (skeletonUpdater is V_Skeleton_Updater) {
                V_Skeleton_Updater newSkeletonUpdater = new V_Skeleton_Updater(mesh, unitAnim.GetAnims(), frameRateMod, frameRateModModifier, onAnimTrigger, OnAnyAnimTrigger);
                newSkeletonUpdater.SetFramesToSame((skeletonUpdater as V_Skeleton_Updater).GetAnims());
                skeletonUpdater = newSkeletonUpdater;
            } else {
                skeletonUpdater = new V_Skeleton_Updater(mesh, unitAnim.GetAnims(), frameRateMod, frameRateModModifier, onAnimTrigger, OnAnyAnimTrigger);
            }
        }
        public static void AddSquare(int verticesIndex, Vector3[] verticesArr, int uvsIndex, Vector2[] uvsArr, int trianglesIndex, int[] trianglesArr, Vector3 localPos, Vector3 v00, Vector3 v01, Vector3 v10, Vector3 v11, Vector2[] squareUV) {
            verticesArr[verticesIndex] = localPos + v01;
            verticesArr[verticesIndex + 1] = localPos + v11;
            verticesArr[verticesIndex + 2] = localPos + v00;
            verticesArr[verticesIndex + 3] = localPos + v10;
            /* 0
             * 1
             * 2
             * 2
             * 1
             * 3 */
            trianglesArr[trianglesIndex] = verticesIndex;
            trianglesArr[trianglesIndex + 1] = verticesIndex + 1;
            trianglesArr[trianglesIndex + 2] = verticesIndex + 2;
            trianglesArr[trianglesIndex + 3] = verticesIndex + 2;
            trianglesArr[trianglesIndex + 4] = verticesIndex + 1;
            trianglesArr[trianglesIndex + 5] = verticesIndex + 3;

            for (int i = 0; i < squareUV.Length; i++) {
                uvsArr[uvsIndex + i] = squareUV[i];
            }
        }
        public static void AddSquare(List<Vector3> verticesList, List<Vector2> uvsList, List<int> trianglesList, Vector3 localPos, Vector3 v00, Vector3 v01, Vector3 v10, Vector3 v11, Vector2[] squareUV) {
            int verticesIndex = verticesList.Count;
            verticesList.Add(localPos + v01);
            verticesList.Add(localPos + v11);
            verticesList.Add(localPos + v00);
            verticesList.Add(localPos + v10);
            trianglesList.Add(verticesIndex);
            trianglesList.Add(verticesIndex + 1);
            trianglesList.Add(verticesIndex + 2);
            trianglesList.Add(verticesIndex + 2);
            trianglesList.Add(verticesIndex + 1);
            trianglesList.Add(verticesIndex + 3);

            for (int i = 0; i < squareUV.Length; i++) {
                uvsList.Add(squareUV[i]);
            }
        }

        public static Vector2[] GetUV_Type(UVType uvType) {
            return uvType.uvs;
        }
        public static Vector2[] GetUV(float x0, float x1, float y0, float y1) {
            x0 /= 1024f;
            x1 /= 1024f;
            y0 /= 1024;
            y1 /= 1024;

            return new Vector2[]{
            new Vector2(x0,y1),
            new Vector2(x1,y1),
            new Vector2(x0,y0),
            new Vector2(x1,y0),
        };
        }
    }
}