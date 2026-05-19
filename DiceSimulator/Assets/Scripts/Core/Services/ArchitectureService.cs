#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using System;
using System.Collections.Generic;
using Core.DependencyInjector;
using UnityEditor;
using UnityEngine;
#if UNITY_EDITOR || DEVELOPMENT_BUILD
using UnityEngine.Assertions;
#endif

namespace Core.Services
{
    /// <summary>
    /// Architecture entry point.
    /// Injects configuration files.
    /// Creates FPS Counter (if 'FPS_COUNTER' keyword added to the build).
    /// Creates DebugCommands.
    /// </summary>
    public static class ArchitectureService
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        static readonly List<Type> _usedConfigs = new();
        static int _lastFrameCount;
#endif

        static readonly string[] _configPaths =
        {
            "Assets/Settings/Configs/Boot",
            "Assets/Settings/Configs/Core",
            "Assets/Settings/Configs/GameLogic",
            "Assets/Settings/Configs/Presentation"
        };

        /// <summary>
        /// Starts dependency injection and debug commands initialization.
        /// Tapping delay is used only when the game is build on mobile devices and tells how fast player has to tap to open the debug console.
        /// </summary>
        public static void Initialize(int signalCount, string[] signalNames,
            Queue<object>[] signalQueues, List<ScriptableObject> configs)
        {
            List<string> assemblyNames = new()
            {
                "Boot",
                "Core",
                "GameLogic",
                "Presentation"
            };

            SignalService.Initialize(signalCount, signalNames, signalQueues, typeof(ReactAttribute)); // instance disposed as the object is a singleton
            DependencyInjector<ScriptableObject>.Inject(FindConfig, assemblyNames);
            DependencyInjector<ScriptableObject>.ResolveBindings();

            return;

            ScriptableObject FindConfig(Type type)
            {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                if (!_usedConfigs.Contains(type))
                    _usedConfigs.Add(type);
#endif

                // ReSharper disable once LoopCanBeConvertedToQuery
                // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
                foreach (ScriptableObject c in configs)
                    if (c.GetType() == type)
                        return c;

                return null;
            }
        }

        /// <summary>
        /// Process and execute all signals sent. Should be called only once per frame.
        /// </summary>
        public static void ExecuteSentSignals()
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            Assert.IsFalse(_lastFrameCount == Time.frameCount, "Signals should be executed once per frame.");
            _lastFrameCount = Time.frameCount;
#endif

            SignalService.ExecuteSentSignals();
        }

#if UNITY_EDITOR
        public static List<ScriptableObject> GetConfigs()
        {
            var results = new List<ScriptableObject>();
            string[] guids = AssetDatabase.FindAssets($"t:{nameof(ScriptableObject)}", _configPaths);

            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (string guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                ScriptableObject asset = AssetDatabase.LoadAssetAtPath<ScriptableObject>(assetPath);

                // skips debug configs when in a release build.
                if (!EditorUserBuildSettings.development && asset.name.Contains("Debug"))
                    continue;

                results.Add(asset);
            }

            return results;
        }
#endif
    }
}