using System;

namespace WhateverDevs.SceneManagement.Runtime.SceneManagement
{
    /// <summary>
    /// Struct that can be used on the inspector to reference a scene with a cool drawer.
    /// </summary>
    [Serializable]
    public struct SceneReference
    {
        /// <summary>
        /// Name of the scene.
        /// </summary>
        public string SceneName;
    }
}