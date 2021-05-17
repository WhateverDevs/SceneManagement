using Sirenix.OdinInspector;
using UnityEngine;
using WhateverDevs.Core.Runtime.Build;
using WhateverDevs.Core.Runtime.Common;

namespace WhateverDevs.SceneManagement.Runtime.AddressableManagement
{
    [CreateAssetMenu(menuName = "WhateverDevs/SceneManagement/AddressableManifest", fileName = "AddressableManifest")]
    public class AddressableManifest : LoggableScriptableObject<AddressableManifest>
    {
        public string Version;

        [ShowIf("@AppVersionReference == null")]
        public Version AppVersionReference;

        [ShowIf("@AppVersionReference != null")]
        public string MinimumAppVersion;

        [ShowIf("@AppVersionReference != null")]
        [Button]
        public void UpdateMinimumToCurrentAppVersion() => MinimumAppVersion = AppVersionReference.FullVersion;
    }
}