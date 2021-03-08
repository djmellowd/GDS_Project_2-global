using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey;
using CodeMonkey.Utils;
using CodeMonkey.MonoBehaviours;
using V_AnimationSystem;

namespace V_ObjectSystem {

    public class V_Object {

        private static List<V_Object> instanceList = new List<V_Object>();

        private static Dictionary<Type, List<V_Object>> typeInstanceDic = new Dictionary<Type, List<V_Object>>();


        private static int nextId;
        private static int GetNextId() {
            return nextId++;
        }

        public event EventHandler OnDestroySelf;

        public bool TESTER = false;

        public int id;

        private string name;

        private V_IObjectActiveLogic[] activeLogicArr;
        private V_IObjectActiveLogic[] activeLogicArrCopy;

        private List<V_IObjectActiveLogic> allLogicList;

        private V_Main.UpdateType updateType;

        private List<object> relatedObjectList;

        private Dictionary<Type, List<object>> logicListDic;
        private Dictionary<Type, object> logicDic;

        private float deltaTimeModifier;

        private bool isDisabled;

        public Func<Vector3> GetPosition;
        public Func<Transform> GetTransform;

        private V_Object(V_Main.UpdateType updateType) {
            V_Main.Init(); 
            instanceList.Add(this);
            id = GetNextId();

            this.updateType = updateType;

            isDestroyed = false;
            isDisabled = false;

            activeLogicArr = new V_IObjectActiveLogic[0];
            activeLogicArrCopy = new V_IObjectActiveLogic[0];

            allLogicList = new List<V_IObjectActiveLogic>();

            relatedObjectList = new List<object>();

            logicListDic = new Dictionary<Type, List<object>>();
            logicDic = new Dictionary<Type, object>();

            GetPosition = delegate () { return Vector3.zero; };
            GetTransform = delegate () { return null; };

            deltaTimeModifier = V_TimeScaleManager.GetTimeScale();
        }
        public void SetIsDisabled(bool isDisabled) {
            this.isDisabled = isDisabled;
        }
        public float GetDeltaTimeModifier() {
            return deltaTimeModifier;
        }
        public void SetDeltaTimeModifier(float f) {
            deltaTimeModifier = f;
        }
        public V_Main.UpdateType GetUpdateType() {
            return updateType;
        }
        public void SetUpdateType(V_Main.UpdateType updateType) {
            this.updateType = updateType;
        }
        public void AddActiveLogic(V_IObjectActiveLogic activeLogic) {
            List<V_IObjectActiveLogic> tmpLogicList = new List<V_IObjectActiveLogic>(activeLogicArr);
            tmpLogicList.Add(activeLogic);
            activeLogicArr = tmpLogicList.ToArray();
            activeLogicArrCopy = tmpLogicList.ToArray();

            allLogicList.Add(activeLogic);

            logicDic[activeLogic.GetType()] = activeLogic;
        }
        public void RemoveActiveLogic(V_IObjectActiveLogic activeLogic) {
            List<V_IObjectActiveLogic> tmpLogicList = new List<V_IObjectActiveLogic>(activeLogicArr);
            tmpLogicList.Remove(activeLogic);
            activeLogicArr = tmpLogicList.ToArray();
            activeLogicArrCopy = tmpLogicList.ToArray();

            allLogicList.Remove(activeLogic);
            logicDic[activeLogic.GetType()] = null;
        }
        public void AddRelatedObject(object obj) {
            if (typeof(V_IHasWorldPosition).IsAssignableFrom(obj.GetType())) {
                GetPosition = (obj as V_IHasWorldPosition).GetPosition;
            }
            if (typeof(V_IObjectTransform).IsAssignableFrom(obj.GetType())) {
                GetTransform = (obj as V_IObjectTransform).GetTransform;
            }

            relatedObjectList.Add(obj);
            logicDic[obj.GetType()] = obj;
        }
        public void RemoveRelatedObject(object obj) {
            if (typeof(V_IHasWorldPosition).IsAssignableFrom(obj.GetType())) {
                if (GetPosition == (obj as V_IHasWorldPosition).GetPosition) {
                    GetPosition = delegate () { return Vector3.zero; };
                }
            }
            if (typeof(V_IObjectTransform).IsAssignableFrom(obj.GetType())) {
                if (GetTransform == (obj as V_IObjectTransform).GetTransform) {
                    GetTransform = delegate () { return null; };
                }
            }

            relatedObjectList.Remove(obj);
            logicDic[obj.GetType()] = null;
        }


        public static void Static_Update(float deltaTime, Dictionary<V_Main.UpdateType, float> updateTimeModDic) {
            List<V_Object> tmpInstanceList = new List<V_Object>(instanceList);
            for (int i = 0; i < tmpInstanceList.Count; i++) {
                tmpInstanceList[i].Update(deltaTime * updateTimeModDic[tmpInstanceList[i].updateType]);
            }
        }
        public static V_Object GetObject(string name) {
            foreach (V_Object vObject in instanceList) {
                if (vObject.name == name)
                    return vObject;
            }
            return null;
        }
        private void Update(float deltaTime) {
            if (isDestroyed) return;
            if (isDisabled) return;

            deltaTime = deltaTime * deltaTimeModifier;

            for (int i = 0; i < activeLogicArrCopy.Length; i++) {
                activeLogicArrCopy[i].Update(deltaTime);
                if (isDestroyed) return;
            }
        }


        public bool HasLogic<T>() {
            return GetLogic<T>() != null;
        }

        public bool HasAllLogic<T1, T2>() {
            if (!HasLogic<T1>()) return false;
            if (!HasLogic<T2>()) return false;
            return true;
        }

        public bool HasAllLogic<T1, T2, T3>() {
            if (!HasLogic<T1>()) return false;
            if (!HasLogic<T2>()) return false;
            if (!HasLogic<T3>()) return false;
            return true;
        }

        public bool TryGetLogic<T>(out T t) {
            t = GetLogic<T>();
            return t != null;
        }

        public T GetLogic<T>() {
            object objectInstance;
            if (logicDic.TryGetValue(typeof(T), out objectInstance)) {
                return (T)objectInstance;
            }

            for (int i = 0; i < allLogicList.Count; i++) {
                if (typeof(T).IsAssignableFrom(allLogicList[i].GetType())) {
                    logicDic[typeof(T)] = allLogicList[i];
                    return (T)allLogicList[i];
                }
            }
            for (int i = 0; i < relatedObjectList.Count; i++) {
                if (typeof(T).IsAssignableFrom(relatedObjectList[i].GetType())) {
                    logicDic[typeof(T)] = relatedObjectList[i];
                    return (T)relatedObjectList[i];
                }
            }

            return default(T);
        }
        public void GetLogic<T>(out T ret) {
            ret = GetLogic<T>();
        }



        public bool isDestroyed;
        public void DestroySelf() {
            if (isDestroyed) return;

            isDestroyed = true;
            instanceList.Remove(this);
            foreach (System.Type type in typeInstanceDic.Keys) {
                typeInstanceDic[type].Remove(this);
            }

            // Clean up related objects
            for (int i = 0; i < allLogicList.Count; i++) {
                if (typeof(V_IDestroySelf).IsAssignableFrom(allLogicList[i].GetType())) {
                    (allLogicList[i] as V_IDestroySelf).DestroySelf();
                }
            }
            for (int i = 0; i < relatedObjectList.Count; i++) {
                if (typeof(V_IDestroySelf).IsAssignableFrom(relatedObjectList[i].GetType())) {
                    (relatedObjectList[i] as V_IDestroySelf).DestroySelf();
                }
            }

            if (OnDestroySelf != null) OnDestroySelf(this, EventArgs.Empty);
        }




        
        public static void TryDestroyObject(string name) {
            V_Object vObject = GetObject(name);
            if (vObject != null) {
                vObject.DestroySelf();
            }
        }

        public static V_Object CreateObject(V_Main.UpdateType updateType = V_Main.UpdateType.Main) {
            return new V_Object(updateType);
        }
        public static V_Object CreateObject(string name, V_Main.UpdateType updateType = V_Main.UpdateType.Main) {
            V_Object vObject = CreateObject(updateType);
            vObject.name = name;
            return vObject;
        }
        public static V_Object CreateObjectPrefab(GameObject prefab, Vector3 position) {
            V_Object instancedObject = V_Object.CreateObject();
            Transform instancedTransform = UnityEngine.Object.Instantiate(prefab, position, Quaternion.identity).transform;
            V_IObjectTransform instanceTransform = new V_ObjectTransform(instancedTransform);
            instancedObject.AddRelatedObject(instanceTransform);
            instancedObject.AddRelatedObject(new V_IHasWorldPosition_Class(instanceTransform.GetPosition));
            return instancedObject;
        }
        public static V_Object CreateObjectGameObject(GameObject gameObject) {
            V_Object instancedObject = V_Object.CreateObject();
            Transform instancedTransform = gameObject.transform;
            V_IObjectTransform instanceTransform = new V_ObjectTransform(instancedTransform);
            instancedObject.AddRelatedObject(instanceTransform);
            instancedObject.AddRelatedObject(new V_IHasWorldPosition_Class(instanceTransform.GetPosition));
            return instancedObject;
        }
        public static V_Object CreateSkeleton(Vector3 position, Material material) {
            V_Object instancedObject = V_Object.CreateObject();
            GameObject gameObject = new GameObject("Skeleton", typeof(MeshFilter), typeof(MeshRenderer));
            Transform instantiatedTransform = gameObject.transform;
            instantiatedTransform.position = position;
            V_IObjectTransform transform = new V_ObjectTransform(instantiatedTransform);
            instancedObject.AddRelatedObject(transform);
            V_IObjectTransformBody transformBody = new V_ObjectTransformBody(instantiatedTransform, material);
            instancedObject.AddRelatedObject(transformBody);
            instancedObject.AddRelatedObject(new V_IHasWorldPosition_Class(transform.GetPosition));

            V_UnitSkeleton unitSkeleton = new V_UnitSkeleton(1f, transformBody.ConvertBodyLocalPositionToWorldPosition, transformBody.SetBodyMesh);
            instancedObject.AddRelatedObject(unitSkeleton);
            instancedObject.AddActiveLogic(new V_ObjectLogic_SkeletonUpdater(unitSkeleton));
            V_UnitAnimation unitAnimation = new V_UnitAnimation(unitSkeleton);
            instancedObject.AddRelatedObject(unitAnimation);

            return instancedObject;
        }
        public static V_Object CreateSkeletonPresetAnim(Vector3 position, Material material, UnitAnimType unitAnimType, Vector3 dir, float frameRateMod) {
            V_Object instancedObject = CreateSkeleton(position, material);
            V_UnitSkeleton.OnAnimComplete onAnimComplete = delegate (UnitAnim unitAnim) {
                instancedObject.DestroySelf();
            };
            instancedObject.GetLogic<V_UnitAnimation>().PlayAnimForced(unitAnimType, dir, frameRateMod, onAnimComplete, null, null);
            return instancedObject;
        }
        public static V_Object CreateSkeletonPresetAnim(Vector3 position, Material material, UnitAnimType unitAnimType, Vector3 dir, float frameRateMod, Vector3 rotatePrefabDir) {
            V_Object instancedObject = CreateSkeleton(position, material);
            V_UnitSkeleton.OnAnimComplete onAnimComplete = delegate (UnitAnim unitAnim) {
                instancedObject.DestroySelf();
            };
            instancedObject.GetLogic<V_UnitAnimation>().PlayAnimForced(unitAnimType, dir, frameRateMod, onAnimComplete, null, null);
            instancedObject.GetLogic<V_IObjectTransform>().SetEulerZ(GetAngleFromVector(rotatePrefabDir));
            return instancedObject;
        }
        public static V_Object CreateSkeletonPresetAnim(Vector3 position, Material material, UnitAnimType unitAnimType, Vector3 dir, float frameRateMod, Vector3 rotatePrefabDir, Vector3 scalePrefab) {
            V_Object instancedObject = CreateSkeleton(position, material);
            V_UnitSkeleton.OnAnimComplete onAnimComplete = delegate (UnitAnim unitAnim) {
                instancedObject.DestroySelf();
            };
            instancedObject.GetLogic<V_UnitAnimation>().PlayAnimForced(unitAnimType, dir, frameRateMod, onAnimComplete, null, null);
            instancedObject.GetLogic<V_IObjectTransform>().SetEulerZ(GetAngleFromVector(rotatePrefabDir));
            instancedObject.GetLogic<V_IObjectTransform>().SetScale(scalePrefab);
            return instancedObject;
        }
        public static V_Object CreateSkeletonPresetAnim(Vector3 position, Material material, UnitAnim unitAnim, float frameRateMod, Vector3 rotatePrefabDir, Vector3 scalePrefab, int sortingOrderOffset) {
            V_Object instancedObject = CreateSkeleton(position, material);
            V_UnitSkeleton.OnAnimComplete onAnimComplete = delegate (UnitAnim u) {
                instancedObject.DestroySelf();
            };
            instancedObject.GetLogic<V_IObjectTransform>().GetTransform().GetComponent<CodeMonkey.MonoBehaviours.PositionRendererSorter>().SetOffset(sortingOrderOffset);
            instancedObject.GetLogic<V_UnitAnimation>().PlayAnimForced(unitAnim, frameRateMod, onAnimComplete, null, null);
            instancedObject.GetLogic<V_IObjectTransform>().SetEulerZ(GetAngleFromVector(rotatePrefabDir));
            instancedObject.GetLogic<V_IObjectTransform>().SetScale(scalePrefab);
            return instancedObject;
        }
        public static V_Object CreateSkeletonPresetAnim_DebugDash(Vector3 position, Vector3 rotationDir, Vector3 localScale) {
            V_Object vObject = V_Object.CreateSkeletonPresetAnim(position, GetWhitePixelMaterial(), UnitAnim.GetUnitAnim("EffectDash"), 1f, rotationDir, localScale, 100);
            return vObject;
        }
        public static V_Object CreateSkeletonPresetAnim_DebugDash(Vector3 position) {
            V_Object vObject = V_Object.CreateSkeletonPresetAnim(position, GetWhitePixelMaterial(), UnitAnim.GetUnitAnim("EffectDash"), 1f, new Vector3(0, 1, 0), new Vector3(3f, 1f, 1f), 100);
            return vObject;
        }
        public static V_Object CreateSkeletonPresetAnim_DebugDash(Vector3 position, Color color, float frameRate = 1f) {
            Material material = new Material(GetWhitePixelMaterial());
            material.color = color;
            V_Object vObject = V_Object.CreateSkeletonPresetAnim(position, material, UnitAnim.GetUnitAnim("EffectDash"), frameRate, new Vector3(0, 1, 0), new Vector3(3f, 1f, 1f), 100);
            return vObject;
        }





        public static List<V_Object> GetObjectsWithAllTheseLogicTypes<T>() {
            List<V_Object> filteredList;
            if (typeInstanceDic.TryGetValue(typeof(T), out filteredList)) {
            } else {
                filteredList = FilterObjectsWithTheseLogicTypes<T>(instanceList);
                typeInstanceDic[typeof(T)] = filteredList;
            }
            return filteredList;
        }

        public static List<V_Object> FilterObjectsWithTheseLogicTypes<T>(List<V_Object> objectList) {
            List<V_Object> ret = new List<V_Object>();
            for (int i = 0; i < objectList.Count; i++) {
                V_Object objectInstance = objectList[i];
                T logic = objectInstance.GetLogic<T>();
                if (logic != null) {
                    ret.Add(objectInstance);
                }
            }
            return ret;
        }


        public static List<V_Object> FilterObjectList(List<V_Object> objectList, Func<V_Object, bool> ValidateObjectFunc) {
            for (int i = 0; i < objectList.Count; i++) {
                if (!ValidateObjectFunc(objectList[i])) {
                    objectList.RemoveAt(i);
                    i--;
                }
            }
            return objectList;
        }

        public static List<V_Object> FilterObjectList_Distance(List<V_Object> objectList, Vector3 position, float distance) {
            return FilterObjectList(objectList, GetValidateObjectFuncTestDistance(position, distance));
        }




        public static V_Object GetClosestFromList(Vector3 position, List<V_Object> objectList) {
            V_Object closest = null;
            for (int i = 0; i < objectList.Count; i++) {
                if (closest == null) {
                    closest = objectList[i];
                } else {
                    if (Vector3.Distance(position, objectList[i].GetPosition()) < Vector3.Distance(position, closest.GetPosition())) {
                        closest = objectList[i];
                    }
                }
            }
            return closest;
        }
        public static Func<V_Object, bool> GetValidateObjectFuncTestDistance(Vector3 position, float distance) {
            return delegate (V_Object vObject) {
                return Vector3.Distance(position, vObject.GetPosition()) <= distance;
            };
        }



        public static void SetAllDeltaTimeModifier(float deltaTimeModifier) {
            foreach (V_Object singleObject in instanceList) {
                singleObject.SetDeltaTimeModifier(deltaTimeModifier);
            }
        }

        
        private static int GetAngleFromVector(Vector3 dir) {
            dir = dir.normalized;
            float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            if (n < 0) n += 360;
            int angle = Mathf.RoundToInt(n);

            return angle;
        }
        private static Material GetWhitePixelMaterial() {
            return CodeMonkey.Assets.i.m_White;
        }


        public static void TriggerDestroySelfOnObjectsWithLogic<T>() {
            List<V_Object> tmpObjectList = new List<V_Object>(GetObjectsWithAllTheseLogicTypes<T>());
            foreach (V_Object objectInstance in tmpObjectList) {
                objectInstance.DestroySelf();
            }
        }


        public static void CleanUpAllObjects() {
            List<V_Object> tmpInstanceList = new List<V_Object>(instanceList);
            foreach (V_Object objectInstance in tmpInstanceList) {
                objectInstance.DestroySelf();
            }
        }

    }


}