using System.Collections.Generic;
using UnityEngine;
using WhateverDevs.Core.Runtime.Common;
using WhateverDevs.Core.Runtime.DataStructures;

namespace WhateverDevs.SceneManagement.Runtime.AddressableManagement
{
    [CreateAssetMenu(menuName = "WhateverDevs/SceneManagement/AddressableVersionDependence",
                     fileName = "AddressableVersionDependence")]
    public class AddressableVersionDependence : LoggableScriptableObject<AddressableVersionDependence>
    {
        public string SettingsLocation;
    
        public List<ManifestStringObjectPair> Dependencies = new List<ManifestStringObjectPair>();
    }

    public class ManifestStringObjectPair : ObjectPair<AddressableManifest, string>
    {
    }
}