using System;
using Sirenix.OdinInspector;
using WhateverDevs.Core.Runtime.Common;
using Version = WhateverDevs.Core.Runtime.Build.Version;

namespace WhateverDevs.SceneManagement.Runtime.AddressableManagement
{
    public class AddressableManifest : LoggableScriptableObject<AddressableManifest>
    {
        [FoldoutGroup("Manifest Version")]
        public string Version;

        [FoldoutGroup("Manifest Version")]
        [InfoBox("The full version gets updated automatically when building.")]
        [ReadOnly]
        public string FullVersion;

        [FoldoutGroup("Manifest Version")]
        [Button]
        public void UpdateFullVersion() => FullVersion = Version + "." + CurrentDate;

        [FoldoutGroup("App Version")]
        [ShowIf("@AppVersionReference == null")]
        public Version AppVersionReference;

        [FoldoutGroup("App Version")]
        [ShowIf("@AppVersionReference != null")]
        [ReadOnly]
        public string MinimumAppVersion;
        
        /// <summary>
        /// Current date in string version format.
        /// </summary>
        private static string CurrentDate => DateTime.Now.ToString("yyyyMMddhhmmss");

        [FoldoutGroup("App Version")]
        [ShowIf("@AppVersionReference != null")]
        [Button]
        public void UpdateMinimumToCurrentAppVersion() => MinimumAppVersion = AppVersionReference.FullVersion;
    }
}