using UnityEngine;
using WhateverDevs.Core.Runtime.Common;

namespace WhateverDevs.SceneManagement.Runtime.AddressableManagement
{
    [CreateAssetMenu(menuName = "WhateverDevs/SceneManagement/AddressablesBuilder",
                     fileName = "AddressablesBuilder")]
    public class AddressablesBuilder : LoggableScriptableObject<AddressablesBuilder>
    {
        public AddressableVersionDependence VersionDependence;
    }
}