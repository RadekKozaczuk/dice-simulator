#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core.Services
{
    /// <summary>
    /// In our architecture every project starts from BootScene.
    /// Designers, programmers, and artists alike occasionally jump between scenes while using the editor.
    /// Sometimes they may forget to come back to BootScene before pressing the Play button.
    /// For their convenience this script automates this process as, at least for now, there is zero reason to not start from BootScene.
    /// </summary>
    [InitializeOnLoad]
    class BootSceneStartService : AssetPostprocessor
    {
        // static constructor is called only on
        // - Unity restart
        // - domain reload
        static BootSceneStartService()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            EditorSceneManager.sceneOpened += static (scene, _) =>
            {
                if (!scene.name.Contains("BootScene"))
                    return;

                ReloadConfigs(scene);
            };
            EditorApplication.update += OneTimeEditorStartup;
        }

        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets,
            string[] movedAssets, string[] movedFromAssetPaths, bool didDomainReload)
        {
            if (!didDomainReload)
                return;

            // AssetDatabase may not be read at the moment when InitializeOnLoad is called
            // but, it should on domain reload
            SceneAsset entryScene = AssetDatabase.LoadAssetAtPath<SceneAsset>("Assets/Scenes/BootScene.unity");
            if (entryScene)
            {
                EditorSceneManager.playModeStartScene = entryScene;
                Scene scene = SceneManager.GetActiveScene();
                ReloadConfigs(scene);
            }
        }

        static void OneTimeEditorStartup()
        {
            Scene scene = SceneManager.GetActiveScene();
            if (scene.name.Contains("BootScene"))
                ReloadConfigs(scene);

            EditorApplication.update -= OneTimeEditorStartup;
        }

        static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.EnteredPlayMode)
            {
                Scene scene = SceneManager.GetActiveScene();
                ReloadConfigs(scene);
            }
        }

        static void ReloadConfigs(Scene scene)
        {
            GameObject[] objects = scene.GetRootGameObjects();

            foreach (GameObject obj in objects)
            {
                if (obj.name != "Boot")
                    continue;

                Component component = obj.GetComponents<Component>()[1];
                Type type = component.GetType();
                FieldInfo field = type.GetField("_configs", BindingFlags.NonPublic | BindingFlags.Instance)!;

                List<ScriptableObject> c = ArchitectureService.GetConfigs();
                field.SetValue(component, c);
            }
        }
    }
}
#endif