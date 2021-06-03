using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace WhateverDevs.SceneManagement.Runtime.SceneManagement
{
    /// <summary>
    /// Interface that stores the scene references and loads them on runtime.
    /// </summary>
    public interface ISceneManager
    {
        /// <summary>
        /// List of all the currently loaded scenes.
        /// </summary>
        List<string> LoadedScenes { get; }
        
        /// <summary>
        /// Array of all the scenes, both in build and addressable.
        /// </summary>
        public string[] SceneNames { get; }

        /// <summary>
        /// Check if a scene is available to be loaded or its asset is missing.
        /// </summary>
        /// <param name="scene">Scene to check.</param>
        /// <returns>True if it is available.</returns>
        public bool IsSceneAvailable(SceneReference scene);

        /// <summary>
        /// Check if a scene is available to be loaded or its asset is missing.
        /// </summary>
        /// <param name="scene">Scene to check.</param>
        /// <returns>True if it is available.</returns>
        public bool IsSceneAvailable(string scene);

        /// <summary>
        /// Load a scene by its reference.
        /// </summary>
        /// <param name="sceneReference">Reference of the scene to load.</param>
        /// <param name="progressCallback">Called to update on the progress of loading.</param>
        /// <param name="callback">Callback called when the scene is loaded.</param>
        /// <param name="mode">Loading mode.</param>
        void LoadScene(SceneReference sceneReference,
                       Action<float> progressCallback,
                       Action<bool> callback,
                       LoadSceneMode mode = LoadSceneMode.Additive);

        /// <summary>
        /// Load a scene by its name.
        /// </summary>
        /// <param name="sceneName">Name of the scene to load.</param>
        /// <param name="progressCallback">Called to update on the progress of loading.</param>
        /// <param name="callback">Callback called when the scene is loaded.</param>
        /// <param name="mode">Loading mode.</param>
        void LoadScene(string sceneName,
                       Action<float> progressCallback,
                       Action<bool> callback,
                       LoadSceneMode mode = LoadSceneMode.Additive);

        /// <summary>
        /// Unload a scene by its reference.
        /// </summary>
        /// <param name="sceneReference">Reference of the scene to unload.</param>
        /// <param name="progressCallback">Called to update on the progress of unloading.</param>
        /// <param name="callback">Callback called when the scene is unloaded.</param>
        void UnloadScene(SceneReference sceneReference,
                         Action<float> progressCallback,
                         Action<bool> callback);

        /// <summary>
        /// Unload a scene by its name.
        /// </summary>
        /// <param name="sceneName">Name of the scene to unload.</param>
        /// <param name="progressCallback">Called to update on the progress of unloading.</param>
        /// <param name="callback">Callback called when the scene is unloaded.</param>
        void UnloadScene(string sceneName,
                         Action<float> progressCallback,
                         Action<bool> callback);

        /// <summary>
        /// Set a scene as the active scene.
        /// </summary>
        /// <param name="scene">Scene to set active.</param>
        void SetActiveScene(SceneReference scene);

        /// <summary>
        /// Set a scene as the active scene.
        /// </summary>
        /// <param name="sceneName">Scene to set active.</param>
        void SetActiveScene(string sceneName);
    }
}