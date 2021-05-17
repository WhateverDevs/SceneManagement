using System;
using Sirenix.OdinInspector;
using UnityEngine;
using WhateverDevs.Core.Runtime.Common;
using Version = WhateverDevs.Core.Runtime.Build.Version;

namespace WhateverDevs.SceneManagement.Runtime.AddressableManagement
{
    [CreateAssetMenu(menuName = "WhateverDevs/SceneManagement/AddressableManifest", fileName = "AddressableManifest")]
    public class AddressableManifest : LoggableScriptableObject<AddressableManifest>
    {
        public string Version;

        [InfoBox("The full version gets updated when building.")]
        [ReadOnly]
        public string FullVersion;

        [Button]
        public void UpdateFullVersion() => FullVersion = Version + "." + CurrentDate;

        [ShowIf("@AppVersionReference == null")]
        public Version AppVersionReference;

        [ShowIf("@AppVersionReference != null")]
        public string MinimumAppVersion;
        
        /// <summary>
        /// Current date in string version format.
        /// </summary>
        private static string CurrentDate => DateTime.Now.ToString("yyyyMMddhhmmss");

        [ShowIf("@AppVersionReference != null")]
        [Button]
        public void UpdateMinimumToCurrentAppVersion() => MinimumAppVersion = AppVersionReference.FullVersion;
    }
}