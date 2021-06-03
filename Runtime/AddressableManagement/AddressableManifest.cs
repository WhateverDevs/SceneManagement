using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using WhateverDevs.Core.Runtime.Common;
using Version = WhateverDevs.Core.Runtime.Build.Version;

namespace WhateverDevs.SceneManagement.Runtime.AddressableManagement
{
    /// <summary>
    /// Data class to hold the information about an addressable group.
    /// </summary>
    public class AddressableManifest : LoggableScriptableObject<AddressableManifest>
    {
        /// <summary>
        /// Basic manifest version.
        /// </summary>
        [FoldoutGroup("Manifest Version")]
        public string Version;

        /// <summary>
        /// Full manifest version.
        /// </summary>
        [FoldoutGroup("Manifest Version")]
        [InfoBox("The full version gets updated automatically when building.")]
        [ReadOnly]
        public string FullVersion;

        /// <summary>
        /// Update the full manifest version.
        /// </summary>
        [FoldoutGroup("Manifest Version")]
        [Button]
        public void UpdateFullVersion() => FullVersion = Version + "." + CurrentDate;

        /// <summary>
        /// Reference to the app version.
        /// </summary>
        [FoldoutGroup("App Version")]
        [ShowIf("@AppVersionReference == null")]
        public Version AppVersionReference;

        /// <summary>
        /// Minimum app version the manifest requires.
        /// </summary>
        [FoldoutGroup("App Version")]
        [ShowIf("@AppVersionReference != null")]
        [ReadOnly]
        public string MinimumAppVersion;
        
        /// <summary>
        /// Current date in string version format.
        /// </summary>
        private static string CurrentDate => DateTime.Now.ToString("yyyyMMddhhmmss");

        /// <summary>
        /// Update the minimum app version to the current app version.
        /// </summary>
        [FoldoutGroup("App Version")]
        [ShowIf("@AppVersionReference != null")]
        [Button]
        public void UpdateMinimumToCurrentAppVersion() => MinimumAppVersion = AppVersionReference.FullVersion;

        /// <summary>
        /// List of all assets in this addressable group.
        /// This list gets updated when building bundles.
        /// </summary>
        [Tooltip("List of all assets in this addressable group.\nThis list gets updated when building bundles.")]
        [ReadOnly]
        public List<string> AssetGuids = new List<string>();
    }
}