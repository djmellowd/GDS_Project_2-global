﻿#define SILENT
#define OVERWRITE_DEFAULT_ANIMATIONS

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Reflection;

namespace V_AnimationSystem {

    public class UnitAnim {

        public const string VALID_NAME_DIC = "abcdefghijklmnopqrstuvxywzABCDEFGHIJKLMNOPQRSTUVXYWZ0123456789_";

        public enum AnimDir {
            Down,
            Up,
            Left,
            Right,
            DownLeft,
            DownRight,
            UpLeft,
            UpRight,
        }
        
        public static AnimDir GetAnimDirFromVectorLimit4Directions(Vector3 dir) {
            return GetAnimDirFromAngleLimit4Directions(V_UnitAnimation.GetAngleFromVector(dir));
        }
        public static AnimDir GetAnimDirFromAngleLimit4Directions(int angle) {
            switch (GetAnimDirFromAngle(angle)) {
            default:
            case AnimDir.Down:
            case AnimDir.DownLeft:
            case AnimDir.DownRight:
                return AnimDir.Down;
            case AnimDir.Up:
            case AnimDir.UpRight:
            case AnimDir.UpLeft:
                return AnimDir.Up;
            case AnimDir.Left:
                return AnimDir.Left;
            case AnimDir.Right:
                return AnimDir.Right;
            }
        }
        public static AnimDir GetAnimDirFromVector(Vector3 dir) {
            return GetAnimDirFromAngle(V_UnitAnimation.GetAngleFromVector(dir));
        }
        public static AnimDir GetAnimDirFromAngle(int angle) {
            switch (angle) {
            case 2:
                return AnimDir.Up;
            case 1:
                return AnimDir.UpRight;
            case 0:
            case 8:
                return AnimDir.Right;
            case 7:
                return AnimDir.DownRight;
            default:
            case 6:
                return AnimDir.Down;
            case 5:
                return AnimDir.DownLeft;
            case 4:
                return AnimDir.Left;
            case 3:
                return AnimDir.UpLeft;
            }
        }
        public static int GetAngleFromAnimDir(AnimDir animDir) {
            switch (animDir) {
            case AnimDir.Up: return 2;
            case AnimDir.UpRight: return 1;
            case AnimDir.Right: return 0;
            case AnimDir.DownRight: return 7;
            default:
            case AnimDir.Down: return 6;
            case AnimDir.DownLeft: return 5;
            case AnimDir.Left: return 4;
            case AnimDir.UpLeft: return 3;
            }
        }
        public static UnitAnim GetUnitAnim(UnitAnimType animType, int angle) {
            AnimDir animDir = GetAnimDirFromAngle(angle);
            return animType.GetUnitAnim(animDir);
        }
        
        public static UnitAnim GetUnitAnim(string unitAnimName) {
            foreach (UnitAnim unitAnim in unitAnimList) {
                if (unitAnim.name == unitAnimName) {
                    return unitAnim;
                }
            }
            return null;
        }

        public static UnitAnim Create(string name, V_Skeleton_Anim[] anims) {
            return new UnitAnim(name, anims);
        }

        private string name;
        private bool disableOverwrite;
        private V_Skeleton_Anim[] anims;

        private UnitAnim(string name, V_Skeleton_Anim[] anims) {
            this.name = name;
            this.anims = anims;
        }
        public bool CanOverwrite() {
#if OVERWRITE_DEFAULT_ANIMATIONS
        return true;
#else
            return !disableOverwrite;
#endif
        }

        public string GetName() {
            return name;
        }

        public V_Skeleton_Anim GetSkeletonAnim_BodyPartPreset(int bodyPartPreset) {
            foreach (V_Skeleton_Anim skeletonAnim in anims) {
                if (skeletonAnim.bodyPart.preset == bodyPartPreset) {
                    return skeletonAnim;
                }
            }
            return null;
        }

        public V_Skeleton_Anim GetSkeletonAnim_BodyPartCustom(string bodyPartCustom) {
            foreach (V_Skeleton_Anim skeletonAnim in anims) {
                if (skeletonAnim.bodyPart.customName == bodyPartCustom) {
                    return skeletonAnim;
                }
            }
            return null;
        }

        public V_Skeleton_Anim GetSkeletonAnim(BodyPart bodyPart) {
            foreach (V_Skeleton_Anim skeletonAnim in anims) {
                if (skeletonAnim.bodyPart == bodyPart) {
                    return skeletonAnim;
                }
            }
            return null;
        }

        public V_Skeleton_Anim[] GetAnims() {
            return anims;
        }

        public UnitAnimType GetUnitAnimType() {
            return null;
        }

        public float GetFrameRateOriginal() {
            return anims[0].GetFrameRateOriginal();
        }

        public int GetTotalFrameCount() {
            int ret = 0;
            foreach (V_Skeleton_Anim skeletonAnim in anims) {
                ret += skeletonAnim.GetTotalFrameCount();
            }
            return ret;
        }

        public List<string> GetTriggerList() {
            List<string> ret = new List<string>();
            foreach (V_Skeleton_Anim skeletonAnim in anims) {
                ret.AddRange(skeletonAnim.GetTriggerList());
            }
            return ret;
        }

        public void ApplyAimDir(Vector3 dir, Vector3 positionOffset, int bonusSortingOrder) {
            float aimAngle = GetAngleFromVectorFloat(dir);

            for (int i=0; i<anims.Length; i++) {
                V_Skeleton_Anim skeletonAnim = anims[i];

                V_Skeleton_Frame[] skeletonFrameArr = skeletonAnim.GetFrames();
                for (int j=0; j<skeletonFrameArr.Length; j++) {
                    V_Skeleton_Frame skeletonFrame = skeletonFrameArr[j];
                    Vector3 framePosition = skeletonFrame.GetBasePosition();
                    Vector3 newFramePosition = ApplyRotationToVector(framePosition, dir);
                    skeletonFrame.ApplyTemporaryPosition(newFramePosition + positionOffset);

                    skeletonFrame.ApplyBonusRotation(aimAngle);

                    skeletonFrame.ApplyBonusSortingOrder(bonusSortingOrder);
                }
            }
        }

        public void ApplyAimDir(Vector3 dir, Vector3 positionOffset, int bonusSortingOrder, params string[] bodyPartNameArr) {
        }

        public void ApplyBonusRotation(float rot) {
            foreach (V_Skeleton_Anim skeletonAnim in anims) {
                foreach (V_Skeleton_Frame skeletonFrame in skeletonAnim.GetFrames()) {
                    skeletonFrame.ApplyBonusRotation(rot);
                }
            }
        }

        public void ResetAnims() {
            foreach (V_Skeleton_Anim skeletonAnim in anims) {
                skeletonAnim.Reset();
            }
        }

        public override string ToString() {
            return name;
        }


        public UnitAnim Clone() {
            V_Skeleton_Anim[] skeletonAnimArray = new V_Skeleton_Anim[anims.Length];
            for (int i = 0; i < anims.Length; i++) {
                skeletonAnimArray[i] = anims[i].Clone();
            }
            UnitAnim clone = new UnitAnim(name, skeletonAnimArray);
            return clone;
        }
        
        public UnitAnim CloneDeep() {
            V_Skeleton_Anim[] skeletonAnimArray = new V_Skeleton_Anim[anims.Length];
            for (int i = 0; i < anims.Length; i++) {
                skeletonAnimArray[i] = anims[i].CloneDeep();
            }
            UnitAnim clone = new UnitAnim(name, skeletonAnimArray);
            return clone;
        }
        



        private static float GetAngleFromVectorFloat(Vector3 dir) {
            dir = dir.normalized;
            float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            if (n < 0) n += 360;

            return n;
        }
        private static Vector3 ApplyRotationToVector(Vector3 vec, Vector3 vecRotation) {
            return ApplyRotationToVector(vec, GetAngleFromVectorFloat(vecRotation));
        }
        private static Vector3 ApplyRotationToVector(Vector3 vec, float angle) {
            return Quaternion.Euler(0,0,angle) * vec;
        }





        private const int ANIMATION_FRAME_MULTIPLIER = 2;

        public string Save() {
            if (unitAnimList.IndexOf(this) == -1) {
                unitAnimList.Add(this);
            }
            List<V_Skeleton_Anim> animKeyframes = new List<V_Skeleton_Anim>();
            foreach (V_Skeleton_Anim anim in GetAnims()) {
                V_Skeleton_Anim clonedKeyframes = anim.CloneOnlyKeyframes();
                animKeyframes.Add(clonedKeyframes);
            }

            foreach (V_Skeleton_Anim anim in animKeyframes) {
                foreach (V_Skeleton_Frame frame in anim.frames) {
                    frame.frameCount = frame.frameCount / ANIMATION_FRAME_MULTIPLIER;
                }
                anim.SetFrameRateOriginal(anim.GetFrameRateOriginal() * ANIMATION_FRAME_MULTIPLIER);
            }



            string[] content = new string[]{
            ""+V_Animation.Save_List<V_Skeleton_Anim>(animKeyframes, V_Skeleton_Anim.Save_Static, "#SKELETONANIMLIST#"),
            ""+name,
        };
            return string.Join("#ANIMATION#", content);
        }
        public static UnitAnim Load(string save) {
            string[] content = V_Animation.SplitString(save, "#ANIMATION#");
            UnitAnim unitAnim = new UnitAnim(content[1], V_Animation.V_LoadAnimationString(content[0], ANIMATION_FRAME_MULTIPLIER));
            return unitAnim;
        }







        public static List<UnitAnim> GetUnitAnimList() {
            return unitAnimList;
        }
        private static List<UnitAnim> unitAnimList;
        private static bool isDirty;
        public static void SetDirty() {
            isDirty = true;
        }
        public static void CheckDirty() {
            if (isDirty) {
                ReInit();
            }
        }
        private static void ReInit() {
            unitAnimList = null;
            Init();
        }
        public static void Init() {
            if (unitAnimList != null) return;
            isDirty = false;
            unitAnimList = new List<UnitAnim>();

            if (V_Animation.DATA_LOCATION == V_Animation.DataLocation.Assets) {
                LoadFromDataFolder();
            } else {
                LoadFromResources();
            }


            DefaultAnimation = UnitAnim.GetUnitAnim("DefaultAnimation");
            
#if !SILENT
            Debug.Log("Loaded Animations: "+unitAnimList.Count);
#endif
        }



        private static void LoadFromDataFolder() {
            DirectoryInfo defaultDirMain = new DirectoryInfo(V_Animation.LOC_DEFAULT_ANIMATIONS);
            DirectoryInfo[] defaultDirArr = defaultDirMain.GetDirectories();
            foreach (DirectoryInfo defaultDir in defaultDirArr) {
                List<FileInfo> defaultDirFileInfoList = new List<FileInfo>(defaultDir.GetFiles("*." + V_Animation.fileExtention_Animation));
                foreach (FileInfo fileInfo in defaultDirFileInfoList) {
                    string readAllText;
                    SaveSystem.FileData fileData;
                    if (SaveSystem.Load(V_Animation.LOC_DEFAULT_ANIMATIONS + defaultDir.Name + "/", fileInfo.Name, out fileData)) {
                        readAllText = fileData.save;
                    } else {
                        continue;
                    }
                    UnitAnim unitAnim = UnitAnim.Load(readAllText);
                    unitAnim.disableOverwrite = true;
                    unitAnimList.Add(unitAnim);
                }
            }
            List<FileInfo> defaultFileInfoList = new List<FileInfo>(defaultDirMain.GetFiles("*." + V_Animation.fileExtention_Animation));
            foreach (FileInfo fileInfo in defaultFileInfoList) {
                string readAllText;
                SaveSystem.FileData fileData;
                if (SaveSystem.Load(V_Animation.LOC_DEFAULT_ANIMATIONS + "/", fileInfo.Name, out fileData)) {
                    readAllText = fileData.save;
                } else {
                    continue;
                }
                UnitAnim unitAnim = UnitAnim.Load(readAllText);
                unitAnim.disableOverwrite = true;
                unitAnimList.Add(unitAnim);
            }

            DirectoryInfo dir = new DirectoryInfo(V_Animation.LOC_ANIMATIONS);
            List<FileInfo> fileInfoList = new List<FileInfo>(dir.GetFiles("*." + V_Animation.fileExtention_Animation));

            Debug.Log("ANIMATIONS FOUND: " + fileInfoList.Count);
            float startTime = Time.realtimeSinceStartup;
            foreach (FileInfo fileInfo in fileInfoList) {
                string readAllText;
                SaveSystem.FileData fileData;
                if (SaveSystem.Load(V_Animation.LOC_ANIMATIONS, fileInfo.Name, out fileData)) {
                    // Loaded!
                    readAllText = fileData.save;
                } else {
                    continue;
                }
                UnitAnim unitAnim = UnitAnim.Load(readAllText);

                unitAnimList.Add(unitAnim);
            }
            Debug.Log("Loaded animations " + ((Time.realtimeSinceStartup - startTime) * 1000f) + "ms");
        }

        public static void CompileResourcesAnimationFile() {
            Debug.Log("########## CompileResourcesAnimationFile");
        }

        public static void CopyAnimationsIntoResourcesAndRename() {
            Debug.Log("########## CopyAnimationsIntoResourcesAndRename");

            string animationResourcesPath = Application.dataPath + @"/Data/Animations/";
            DirectoryInfo animationDataDir = new DirectoryInfo(animationResourcesPath);
            List<FileInfo> fileInfoList = new List<FileInfo>();
            fileInfoList.AddRange(animationDataDir.GetFiles("*." + V_Animation.fileExtention_Animation));
            foreach (FileInfo fileInfo in fileInfoList) {
                string newFileName = fileInfo.Name.Substring(0, fileInfo.Name.Length - 3) + "bytes";
                newFileName = Application.dataPath + @"/V_Animation/Resources/AnimationData/Animations/" + newFileName;
                if (!File.Exists(newFileName) || 
                    fileInfo.LastWriteTime > new FileInfo(newFileName).LastWriteTime) {
                    Debug.Log("Copying Anim: " + fileInfo.Name + "\n" + fileInfo.FullName + " -> " + newFileName);
                    try {
                        File.Copy(fileInfo.FullName, newFileName, true);
                    } catch (System.Exception e) {
                        Debug.LogError("Error: " + e);
                    }
                }
            }
            
            animationResourcesPath = Application.dataPath + @"/Data/AnimationTypes/";
            animationDataDir = new DirectoryInfo(animationResourcesPath);
            fileInfoList = new List<FileInfo>();
            fileInfoList.AddRange(animationDataDir.GetFiles("*." + V_Animation.fileExtention_AnimationType));
            foreach (FileInfo fileInfo in fileInfoList) {
                string newFileName = fileInfo.Name.Substring(0, fileInfo.Name.Length - 3) + "bytes";
                newFileName = Application.dataPath + @"/V_Animation/Resources/AnimationData/AnimationTypes/" + newFileName;
                if (!File.Exists(newFileName) || 
                    fileInfo.LastWriteTime > new FileInfo(newFileName).LastWriteTime) {
                    Debug.Log("Copying AnimType: " + fileInfo.Name + "\n" + fileInfo.FullName +" -> "+ newFileName);
                    try {
                        File.Copy(fileInfo.FullName, newFileName, true);
                    } catch (System.Exception e) {
                        Debug.LogError("Error: " + e);
                    }
                }
            }
        }
        
        public static void RenameAnimationsInResources() {
            Debug.Log("########## RenameAnimationsInResources");
            string animationResourcesPath = Application.dataPath + @"/V_Animation/Resources/AnimationData/";
            DirectoryInfo animationDataDir = new DirectoryInfo(animationResourcesPath);
            DirectoryInfo[] dirArr = animationDataDir.GetDirectories("*", SearchOption.AllDirectories);
            foreach (DirectoryInfo dir in dirArr) {
                List<FileInfo> fileInfoList = new List<FileInfo>();
                fileInfoList.AddRange(dir.GetFiles("*." + V_Animation.fileExtention_Animation));
                fileInfoList.AddRange(dir.GetFiles("*." + V_Animation.fileExtention_AnimationType));
                foreach (FileInfo fileInfo in fileInfoList) {
                    string newFileName = fileInfo.FullName.Substring(0, fileInfo.FullName.Length - 3) + "bytes";
                    Debug.Log(fileInfo.FullName +" -> "+ newFileName);
                    File.Move(fileInfo.FullName, newFileName);
                }
            }
        }
        private static void SaveResourcesTree() {
            string treeString = "";

            string animationResourcesPath = Application.dataPath + @"/V_Animation/Resources/AnimationData/";

            DirectoryInfo animationDataDir = new DirectoryInfo(animationResourcesPath);
            DirectoryInfo[] dirArr = animationDataDir.GetDirectories("*", SearchOption.AllDirectories);
            foreach (DirectoryInfo dir in dirArr) {
                string fullPath = dir.FullName;
                fullPath = fullPath.Replace("\\", "/");
                fullPath = fullPath.Replace(animationResourcesPath, "");
                treeString += fullPath + ",";
            }

            File.WriteAllText(animationResourcesPath + "animationResourceTreeTextAsset.txt", treeString);
        }
        private static void LoadFromResources() {
            string folderCSV = "_Default/Animations,Animations";

            string[] folderArr = V_Animation.SplitString(folderCSV, ",");
            foreach (string folder in folderArr) {
                if (folder != "") {
                    TextAsset[] textAssetArr = Resources.LoadAll<TextAsset>("AnimationData/" + folder);
                    foreach (TextAsset textAsset in textAssetArr) {
                        byte[] byteArr = textAsset.bytes;

                        string readAllText;
                        SaveSystem.FileData fileData;
                        if (SaveSystem.Load(byteArr, out fileData)) {
                            readAllText = fileData.save;
                        } else {
                            readAllText = null;
                        }
                        UnitAnim unitAnim = UnitAnim.Load(readAllText);

                        unitAnimList.Add(unitAnim);
                    }
                }
            }
        }



        private static void LoadOldVersionSaves() { 
            string dirPath = V_Animation.LOC_ANIMATIONS + "OldVersion/";
            DirectoryInfo dir = new DirectoryInfo(dirPath);
            List<FileInfo> fileInfoList = new List<FileInfo>(dir.GetFiles("*.txt"));
            foreach (FileInfo fileInfo in fileInfoList) {
                string readAllText = File.ReadAllText(fileInfo.FullName);
                UnitAnim unitAnim = UnitAnim.Load(readAllText);
                string prefix = "dSwordTwoHandedBack_";
                unitAnim.name = prefix + unitAnim.name.Replace(" ", "");
                
                V_Skeleton_Anim[] anims = unitAnim.GetAnims();
                for (int i = 0; i < anims.Length; i++) {
                    V_Skeleton_Anim skeletonAnim = anims[i];
                    foreach (V_Skeleton_Frame frame in skeletonAnim.frames) {
                        switch (skeletonAnim.bodyPart.customName) {
                        case "FootL":   frame.SetNewSize(frame.GetSize() * .5f); break;
                        case "FootR":   frame.SetNewSize(frame.GetSize() * .5f); break;
                        case "Body":    frame.SetNewSize(frame.GetSize() * 0.85f); break;
                        case "Head":    frame.SetNewSize(frame.GetSize() * 0.916f); break;
                        case "Sword":   frame.SetNewSize(frame.GetSize() * 1.03f); break;
                        case "HandL":   frame.SetNewSize(frame.GetSize() * .5f); break;
                        case "HandR":   frame.SetNewSize(frame.GetSize() * .5f); break;
                        }
                        frame.RefreshVertices();
                    }
                    skeletonAnim.RemakeTween();
                }
                List<V_Skeleton_Anim> animsList = new List<V_Skeleton_Anim>(anims);
                for (int i = 0; i < animsList.Count; i++) {
                    V_Skeleton_Anim skeletonAnim = animsList[i];
                    if (skeletonAnim.bodyPart.customName == "Pointer_1" || 
                        skeletonAnim.bodyPart.customName == "Pointer_2" || 
                        skeletonAnim.bodyPart.customName == "Pointer_3") {
                        animsList.RemoveAt(i);
                        i--;
                    }
                }
                unitAnim = new UnitAnim(unitAnim.name, animsList.ToArray());

                string saveString = unitAnim.Save();
                SaveSystem.Save(V_Animation.LOC_ANIMATIONS, unitAnim.name+"."+V_Animation.fileExtention_Animation, SaveSystem.FileData.FileType.Animation, saveString);
            }
        }

        public static string LoadAnimString(string fileNameWithExtension) {
            return LoadAnimString(V_Animation.LOC_ANIMATIONS, fileNameWithExtension);
        }
        public static string LoadAnimString(string parentFolder, string fileNameWithExtension) {
            string readAllText;
            SaveSystem.FileData fileData;
            if (SaveSystem.Load(parentFolder, fileNameWithExtension, out fileData)) {
                readAllText = fileData.save;
            } else {
                return null;
            }
            return readAllText;
        }

        public static UnitAnim dMinion_IdleDown;

        public static UnitAnim DefaultAnimation;

        public static UnitAnim None;

        private class UnitAnimEnum {

            static UnitAnimEnum() {
                FieldInfo[] fieldInfoArr = typeof(UnitAnimEnum).GetFields(BindingFlags.Static | BindingFlags.Public);
                foreach (FieldInfo fieldInfo in fieldInfoArr) {
                    if (fieldInfo != null) {
                        fieldInfo.SetValue(null, UnitAnim.GetUnitAnim(fieldInfo.Name));
                    }
                }
            }
        }

    }




}