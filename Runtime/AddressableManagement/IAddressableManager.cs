using System;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace WhateverDevs.SceneManagement.Runtime.AddressableManagement
{
    /// <summary>
    /// Interface that handles addressable availability checking, loading and unloading.
    /// </summary>
    public interface IAddressableManager
    {
        /// <summary>
        /// Reset the manager.
        /// </summary>
        void Reset();

        /// <summary>
        /// Triggers checking for available addressables.
        /// </summary>
        /// <param name="callback">Callback that sends the report back.</param>
        /// <param name="progressCallback">Callback that sends back progress.</param>
        void CheckAvailableAddressables(Action<AddressableStateReport> callback,
                                        Action<float> progressCallback = null);

        /// <summary>
        /// Check if an asset is in a valid manifest and ready to be loaded.
        /// </summary>
        /// <param name="asset">Asset to check.</param>
        /// <returns>True if available.</returns>
        public bool IsAssetAvailable(AssetReference asset);

        /// <summary>
        /// Load the given scene by its asset reference.
        /// </summary>
        /// <param name="sceneReference">Scene to load.</param>
        /// <param name="sceneName">Scene name, only used for logs, can be null as long as the reference is okay.</param>
        /// <param name="loadMode">Mode to load the scene into.</param>
        /// <param name="progressCallback">Callback that reports the loading progress every frame.</param>
        /// <param name="sceneLoadedCallback">Callback called when the scene is loaded or when there has been an error.</param>
        void LoadScene(AssetReference sceneReference,
                       string sceneName,
                       LoadSceneMode loadMode,
                       Action<float> progressCallback,
                       Action<bool> sceneLoadedCallback);

        /// <summary>
        /// Load the given asset by its reference.
        /// </summary>
        /// <param name="assetReference">Reference to the asset to load.</param>
        /// <param name="loadedCallback">Callback raised when the asset is loaded, the bool represents success.</param>
        /// <typeparam name="T">Type of asset to load.</typeparam>
        void LoadAsset<T>(AssetReference assetReference, Action<bool, T> loadedCallback) where T : Object;
    }
}