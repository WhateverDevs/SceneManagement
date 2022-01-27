using System;
using System.Collections.Generic;
using UnityEngine;
using WhateverDevs.Core.Runtime.Common;
using Version = WhateverDevs.Core.Runtime.Build.Version;

#if ODIN_INSPECTOR_3
using Sirenix.OdinInspector;
#endif

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
        #if ODIN_INSPECTOR_3
        [FoldoutGroup("Manifest Version")]
        #endif
        public string Version;

        /// <summary>
        /// Full manifest version.
        /// </summary>
        #if ODIN_INSPECTOR_3
        [FoldoutGroup("Manifest Version")]
        [InfoBox("The full version gets updated automatically when building.")]
        [ReadOnly]
        #endif
        public string FullVersion;

        /// <summary>
        /// Update the full manifest version.
        /// </summary>
        #if ODIN_INSPECTOR_3
        [FoldoutGroup("Manifest Version")]
        [Button]
        #endif
        public void UpdateFullVersion() => FullVersion = Version + "." + CurrentDate;

        /// <summary>
        /// Reference to the app version.
        /// </summary>
        #if ODIN_INSPECTOR_3
        [FoldoutGroup("App Version")]
        [ShowIf("@AppVersionReference == null")]
        #endif
        public Version AppVersionReference;

        /// <summary>
        /// Minimum app version the manifest requires.
        /// </summary>
        #if ODIN_INSPECTOR_3
        [FoldoutGroup("App Version")]
        [ShowIf("@AppVersionReference != null")]
        [ReadOnly]
        #endif
        public string MinimumAppVersion;

        /// <summary>
        /// Current date in string version format.
        /// </summary>
        private static string CurrentDate => DateTime.Now.ToString("yyyyMMddHHmmss");

        /// <summary>
        /// Update the minimum app version to the current app version.
        /// </summary>
        #if ODIN_INSPECTOR_3
        [FoldoutGroup("App Version")]
        [ShowIf("@AppVersionReference != null")]
        [Button]
        #endif
        public void UpdateMinimumToCurrentAppVersion() => MinimumAppVersion = AppVersionReference.FullVersion;

        /// <summary>
        /// List of all assets in this addressable group.
        /// This list gets updated when building bundles.
        /// </summary>
        [Tooltip("List of all assets in this addressable group.\nThis list gets updated when building bundles.")]
        #if ODIN_INSPECTOR_3
        [ReadOnly]
        #endif
        public List<string> AssetGuids = new List<string>();
    }
}