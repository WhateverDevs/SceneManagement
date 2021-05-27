using System.IO;
using UnityEditor;
using UnityEngine;
using WhateverDevs.Core.Runtime.Common;
using WhateverDevs.SceneManagement.Runtime.SceneManagement;

namespace Editor.SceneManagement
{
    /// <summary>
    /// Create the scene manager at the location we want.
    /// </summary>
    public class SceneManagerCreator : Loggable<SceneManagerCreator>
    {
        /// <summary>
        /// Path to the data folder.
        /// </summary>
        private const string DataPath = "Assets/Data";

        /// <summary>
        /// Path to the scene management folder.
        /// </summary>
        private const string SceneManagementDataPath = DataPath + "/SceneManagement";

        /// <summary>
        /// Path to the asset.
        /// </summary>
        public const string SceneManagerPath = SceneManagementDataPath + "/SceneManager.asset";

        /// <summary>
        /// Create the manager.
        /// </summary>
        [MenuItem("WhateverDevs/Scene Management/Create Scene Manager")]
        public static void CreateSceneManager()
        {
            if (!Directory.Exists(DataPath)) Directory.CreateDirectory(DataPath);
            if (!Directory.Exists(SceneManagementDataPath)) Directory.CreateDirectory(SceneManagementDataPath);

            if (File.Exists(SceneManagerPath))
                StaticLogger.Error("Scene manager already exists!");
            else
            {
                SceneManager sceneManager = (SceneManager) ScriptableObject.CreateInstance(typeof(SceneManager));
                AssetDatabase.CreateAsset(sceneManager, SceneManagerPath);
                AssetDatabase.SaveAssets();
                StaticLogger.Info("Created SceneManager at " + SceneManagerPath + ".");
            }
        }
    }
}