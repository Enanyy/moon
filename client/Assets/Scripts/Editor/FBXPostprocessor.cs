using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEditor.Animations;

public class SUFBXPostprocessor : AssetPostprocessor
{
   
    void OnPreprocessModel()
    {
        if (!assetPath.Contains("Assets/Model")) return;
        string fileName = Path.GetFileName(assetPath);
        bool isAnimation = fileName.Contains("@");
        ModelImporter mi = (ModelImporter)assetImporter;
        mi.optimizeMesh = true;
        mi.isReadable = false;
        mi.addCollider = false;
        mi.importAnimation = true;
        mi.animationType = ModelImporterAnimationType.Generic;
       
        mi.useFileScale = true;
       

        if (isAnimation == false)
        {
            GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
            if (obj)
            {
                CreateAnimationPrefab(obj);
            }
        }
     
    }


    [MenuItem("Assets/创建模型预设", true)]
    static bool IsPrefab()
    {
        bool result = false;

        if(Selection.activeGameObject)
        {
            string path = AssetDatabase.GetAssetPath(Selection.activeGameObject);
            string fileName = Path.GetFileName(path);
            if (fileName.ToLower().Contains(".fbx") && fileName.Contains("@") == false)
            {
                result = true;
            }
        }
       
        return result;
    }
    [MenuItem("Assets/创建模型预设")]
    static void CreateAnimationPrefab()
    {
        GameObject obj = Selection.activeGameObject;

        if (obj == null)
        {
            EditorUtility.DisplayDialog("提示", "请先选择一个模型的FBX文件", "确认");
            return;
        }
        CreateAnimationPrefab(obj);
    }

    static void CreateAnimationPrefab(GameObject obj)
    {
        if(obj == null)
        {
            return;
        }
        
        string path = AssetDatabase.GetAssetPath(Selection.activeGameObject);

        string dirName = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf('/') + 1) + path.Substring(0, path.LastIndexOf('/') + 1);

        DirectoryInfo dir = new DirectoryInfo(dirName);
        FileInfo[] files = dir.GetFiles("*.fbx");

        EntityParamModel modelParam = null;
        string configFullPath = string.Format("{0}/Resources/r/config/{1}.txt", Application.dataPath, obj.name.ToLower());

        Debug.Log(configFullPath);
        if(File.Exists(configFullPath)==false)
        {
            modelParam = new EntityParamModel();
            modelParam.rect = new Rect(20, 20, TreeNode.WIDTH, TreeNode.HEIGHT);
        }
        else
        {
            modelParam = EntityParam.Create(File.ReadAllText(configFullPath)) as EntityParamModel;
        }

        modelParam.asset = obj.name.ToLower() +".prefab";

        AnimatorController animatorController = AnimatorController.CreateAnimatorControllerAtPath(path.ToLower().Replace(".fbx", ".controller"));

        AnimatorStateMachine sm = animatorController.layers[0].stateMachine;

        int leftIndex = 0;
        int rightIndex = 0;

        for (int i = 0; i < files.Length; ++i)
        {
            if (files[i].Name.Contains("@") == false)     // 跳过不是动作的文件
            {
                //Debug.Log("跳过无效文件：" + fileInfos[i].Name + "   "  + obj.name + "@");
                continue;
            }

            string animName = Path.GetFileNameWithoutExtension(files[i].Name).Split('@')[1];

            AnimatorState state = null;

            if (leftIndex < 5)
            {
                state = sm.AddState(animName, new Vector3(-140, 25 + 50 * leftIndex++, 0));
            }
            else
            {
                state = sm.AddState(animName, new Vector3(180, 25 + 50 * rightIndex++, 0));
            }
            string animationClip = files[i].FullName.Replace("\\", "/");
            animationClip = animationClip.Substring(animationClip.LastIndexOf("Assets/"));
            var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(animationClip);
            state.motion = clip;
            if (animName.Equals("idle"))
            {
                sm.defaultState = state;
            }

            if (modelParam != null)
            {
                ActionType type = EntityParamModel.GetActionType(animName);
                EntityParamAction actionParam = null;
                for(int j = 0; j < modelParam.children.Count; ++j)
                {
                    var child = modelParam.children[j] as EntityParamAction;
                    if(child!=null && type == child.action)
                    {
                        actionParam = child;break;
                    }
                }
                if(actionParam== null)
                {
                    actionParam = new EntityParamAction();
                    actionParam.rect = new Rect(300, 20, TreeNode.WIDTH, TreeNode.HEIGHT);
                    modelParam.AddChild(actionParam);
                }
                actionParam.action = type;
                actionParam.weight = EntityParamAction.ActionWeights[type];

                EntityParamAnimation animationParam = null;
                for(int j = 0; j < actionParam.children.Count; ++j)
                {
                    var child = actionParam.children[j] as EntityParamAnimation;
                    if (child != null && child.animationClip == animationClip)
                    {
                        animationParam = child; break;
                    }
                }
                if(animationParam == null)
                {
                    animationParam = new EntityParamAnimation();
                    animationParam.rect = new Rect(600, 20, TreeNode.WIDTH, TreeNode.HEIGHT);
                    modelParam.AddChild(animationParam);
                }
                animationParam.animationClip = animName;
                animationParam.length = clip.length;
                animationParam.mode = EntityParamModel.GetWrapMode(animName);
                
            }
        }
        var empty = sm.AddState("empty", new Vector3(180, 25 + 50 * rightIndex, 0));
        empty.speed = 0;
       
        
        if(File.Exists(configFullPath))
        {
            File.Delete(configFullPath);
        }
        EntityParamTool.Save(modelParam, configFullPath);
       
        // 如果目录下预制体不存在，则创建预制体
        string prefabPath = string.Format("Assets/Resources/r/model/{0}.prefab", obj.name.ToLower());

        string prefabFullPath = string.Format("{0}/Resources/r/model/{1}.prefab", Application.dataPath, obj.name.ToLower());
        if (File.Exists(prefabFullPath))
        {

            bool delete = EditorUtility.DisplayDialog("提示", string.Format("预设{0}.prefab已存在，是否替换？", obj.name), "确认","取消");
            if (delete)
            {
                File.Delete(prefabFullPath);
            }
            else
            {
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                return;
            }
        }

        obj.GetComponent<Animator>().runtimeAnimatorController = animatorController;
        GameObject go = PrefabUtility.CreatePrefab(prefabPath, obj);
        go.transform.position = Vector3.zero;
        go.transform.rotation = Quaternion.identity;
        go.transform.localScale = Vector3.one;


        int layer = LayerMask.NameToLayer("Default");
        Transform[] transforms = go.GetComponentsInChildren<Transform>();
        for (int i = 0; i < transforms.Length; ++i)
        {
            transforms[i].gameObject.layer = layer;

            SetBonePoint(transforms[i]);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
    static void SetBonePoint(Transform t)
    {
        string name = t.name;
        if (name.Contains("Root_node"))
        {
            BoneObject bp = t.GetComponent<BoneObject>();
            if (bp == null)
                bp = t.gameObject.AddComponent<BoneObject>();

            bp.type = BonePoint.Root;
        }
        else if (name.Contains("Bone_Buff"))
        {
            BoneObject bp = t.GetComponent<BoneObject>();
            if (bp == null)
                bp = t.gameObject.AddComponent<BoneObject>();

            bp.type = BonePoint.Buff;
        }
        else if (name.Contains("Bone_Head"))
        {
            BoneObject bp = t.GetComponent<BoneObject>();
            if (bp == null)
                bp = t.gameObject.AddComponent<BoneObject>();

            bp.type = BonePoint.Head;
        }

        else if (name.Contains("Bone_Hit"))
        {
            BoneObject bp = t.GetComponent<BoneObject>();
            if (bp == null)
                bp = t.gameObject.AddComponent<BoneObject>();

            bp.type = BonePoint.Hit;
        }
        else if (name.Contains("Bip001 Prop1"))
        {
            BoneObject bp = t.GetComponent<BoneObject>();
            if (bp == null)
                bp = t.gameObject.AddComponent<BoneObject>();

            bp.type = BonePoint.Weapon1;
        }
        else if (name.Contains("Bip001 Prop2"))
        {
            BoneObject bp = t.GetComponent<BoneObject>();
            if (bp == null)
                bp = t.gameObject.AddComponent<BoneObject>();

            bp.type = BonePoint.Weapon2;
        }

        else if (name.Contains("Bip001 Prop3"))
        {
            BoneObject bp = t.GetComponent<BoneObject>();
            if (bp == null)
                bp = t.gameObject.AddComponent<BoneObject>();

            bp.type = BonePoint.Weapon3;
        }
    }


    void OnPreprocessAnimation()
    {
        if (!assetPath.Contains("Assets/Model")) return;

        ModelImporter modelImporter = assetImporter as ModelImporter;
        if (modelImporter.clipAnimations.Length == 0)
        {
            modelImporter.clipAnimations = modelImporter.defaultClipAnimations;
        }

        ModelImporterClipAnimation[] clipAnimations = modelImporter.clipAnimations;
        for (int i = 0; i < clipAnimations.Length; i++)
        {
            ModelImporterClipAnimation clip = clipAnimations[i];
            string clipName = clip.name;
            int indexOf = clipName.IndexOf('@');
            if (indexOf > 0)
            {
                clipName = clipName.Substring(indexOf + 1, clipName.Length - indexOf - 1);
            }
           
            if (EntityParamModel.IsLoop(clipName))
            {
                clip.loopTime = true;
            }
        }
        modelImporter.clipAnimations = clipAnimations;
    }

    
   
}

