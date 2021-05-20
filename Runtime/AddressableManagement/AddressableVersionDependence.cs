using System.Collections.Generic;
using UnityEngine;
using WhateverDevs.Core.Runtime.Build;
using WhateverDevs.Core.Runtime.Common;
using WhateverDevs.Core.Runtime.DataStructures;

namespace WhateverDevs.SceneManagement.Runtime.AddressableManagement
{
    [CreateAssetMenu(menuName = "WhateverDevs/SceneManagement/AddressableVersionDependence",
                     fileName = "AddressableVersionDependence")]
    public class AddressableVersionDependence : LoggableScriptableObject<AddressableVersionDependence>
    {
        public string AddressableSettingsLocation = "Assets/AddressableAssetsData/AddressableAssetSettings.asset";

        public string ManifestLocation = "Assets/Data/SceneManagement/Manifests/";

        public string[] GroupsToIgnore = {"Built In Data", "Default"};

        public string InitialManifestVersion = "0.0.1";

        public Version AppVersion;
    
        public List<ManifestStringObjectPair> Dependencies = new List<ManifestStringObjectPair>();
    }

    public class ManifestStringObjectPair : ObjectPair<AddressableManifest, string>
    {
    }
}