﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using WhateverDevs.Core.Runtime.Common;
using WhateverDevs.Core.Runtime.DataStructures;
using WhateverDevs.SceneManagement.Runtime.AddressableManagement;
using Zenject;

namespace WhateverDevs.SceneManagement.Runtime.SceneManagement
{
    /// <summary>
    /// Class that stores the scene references and loads them on runtime.
    /// </summary>
    [CreateAssetMenu(menuName = "WhateverDevs/SceneManagement/SceneManager", fileName = "SceneManager")]
    public class SceneManager : LoggableScriptableObject<SceneManager>
    {
        /// <summary>
        /// List of non addressable scenes that go into the build.
        /// </summary>
        public List<string> NonAddressableScenes = new List<string>();

        /// <summary>
        /// List of non addressable scenes that go in separate bundles.
        /// </summary>
        public List<AssetReference> AddressableScenes = new List<AssetReference>();

        /// <summary>
        /// Dictionary with the equivalence between addressable scene names and their guids.
        /// </summary>
        public SerializableDictionary<string, string> SceneNameGuidDictionary =
            new SerializableDictionary<string, string>();

        /// <summary>
        /// Array of all the scenes, both in build and addressable.
        /// </summary>
        public string[] SceneNames => SceneNamesList.ToArray();

        /// <summary>
        /// List of all the scenes, both in build and addressable.
        /// </summary>
        public List<string> SceneNamesList
        {
            get
            {
                List<string> list = new List<string>();

                for (int i = 0; i < NonAddressableScenes.Count; ++i)
                    list.Add(Path.GetFileNameWithoutExtension(NonAddressableScenes[i]));

                for (int i = 0; i < AddressableScenes.Count; ++i)
                    list.Add(GetSceneNameFromGuid(AddressableScenes[i].RuntimeKey.ToString()));

                return list;
            }
        }

        /// <summary>
        /// Reference to the addressable manager.
        /// </summary>
        [Inject]
        public IAddressableManager AddressableManager;

        /// <summary>
        /// Load a scene by its name.
        /// </summary>
        /// <param name="sceneName">Name of the scene to load.</param>
        /// <param name="progressCallback">Called to update on the progress of loading.</param>
        /// <param name="callback">Callback called when the scene is loaded.</param>
        /// <param name="mode">Loading mode.</param>
        public void LoadScene(string sceneName,
                              Action<float> progressCallback,
                              Action<bool> callback,
                              LoadSceneMode mode = LoadSceneMode.Additive) =>
            CoroutineRunner.Instance.RunRoutine(LoadSceneRoutine(sceneName, mode, progressCallback, callback));

        /// <summary>
        /// Load a scene by its name.
        /// </summary>
        /// <param name="sceneName">Name of the scene to load.</param>
        /// <param name="mode">Loading mode.</param>
        /// <param name="progressCallback">Called to update on the progress of loading.</param>
        /// <param name="callback">Callback called when the scene is loaded.</param>
        private IEnumerator LoadSceneRoutine(string sceneName,
                                             LoadSceneMode mode,
                                             Action<float> progressCallback,
                                             Action<bool> callback)
        {
            Logger.Info("Attempting to load scene " + sceneName + ".");

            int sceneIndex = GetSceneIndex(sceneName);

            if (sceneIndex == -1)
            {
                Logger.Info("Scene not found in build, checking addressables...");

                AssetReference scene = GetSceneAddressable(sceneName);

                if (scene == null)
                {
                    Logger.Error("Scene is nowhere to be found in the scene library, not loading.");
                    callback?.Invoke(false);
                    yield break;
                }

                AddressableManager.LoadScene(scene,
                                             GetSceneNameFromGuid(scene.RuntimeKey.ToString()),
                                             mode,
                                             progressCallback,
                                             callback);

                // TODO: Keep track of loaded scenes.
            }
            else
            {
                AsyncOperation operation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneIndex, mode);

                while (!operation.isDone)
                {
                    GetLogger().Info("Loading scene - " + operation.progress);
                    progressCallback?.Invoke(operation.progress);
                    yield return new WaitForEndOfFrame();
                }

                // TODO: Keep track of loaded scenes.

                Logger.Info("Scene " + sceneName + " loaded.");

                callback?.Invoke(true);
            }
        }

        /// <summary>
        /// Get the index of a non addressable scene by its name.
        /// </summary>
        /// <param name="sceneName"></param>
        /// <returns></returns>
        private int GetSceneIndex(string sceneName)
        {
            for (int i = 0; i < NonAddressableScenes.Count; ++i)
                if (sceneName == Path.GetFileNameWithoutExtension(NonAddressableScenes[i]))
                    return i;

            return -1;
        }

        /// <summary>
        /// Get the AssetReference of an addressable scene by its name.
        /// </summary>
        /// <param name="sceneName"></param>
        /// <returns></returns>
        private AssetReference GetSceneAddressable(string sceneName)
        {
            string guid = SceneNameGuidDictionary[sceneName];

            for (int i = 0; i < AddressableScenes.Count; ++i)
                if (guid == AddressableScenes[i].RuntimeKey.ToString())
                    return AddressableScenes[i];

            return null;
        }

        /// <summary>
        /// Get the name of a scene from its guid.
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        private string GetSceneNameFromGuid(string guid)
        {
            foreach (KeyValuePair<string, string> keyValuePair in SceneNameGuidDictionary)
                if (keyValuePair.Value == guid)
                    return keyValuePair.Key;

            throw new KeyNotFoundException();
        }
    }
}