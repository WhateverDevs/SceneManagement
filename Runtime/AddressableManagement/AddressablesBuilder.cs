using UnityEngine;
using WhateverDevs.Core.Runtime.Common;

namespace WhateverDevs.SceneManagement.Runtime.AddressableManagement
{
    /// <summary>
    /// Scriptable that triggers addressable building.
    /// </summary>
    [CreateAssetMenu(menuName = "WhateverDevs/SceneManagement/AddressablesBuilder",
                     fileName = "AddressablesBuilder")]
    public class AddressablesBuilder : LoggableScriptableObject<AddressablesBuilder>
    {
        /// <summary>
        /// Reference to the version dependence scriptable.
        /// </summary>
        public AddressableVersionDependence VersionDependence;
    }
}