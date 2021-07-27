using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WhateverDevs.Core.Runtime.Build;
using WhateverDevs.Core.Runtime.Common;
using WhateverDevs.Core.Runtime.DataStructures;

namespace WhateverDevs.SceneManagement.Runtime.AddressableManagement
{
    /// <summary>
    /// Library that stores the minimum versions of each addressable the app needs.
    /// </summary>
    [CreateAssetMenu(menuName = "WhateverDevs/SceneManagement/AddressableVersionDependence",
                     fileName = "AddressableVersionDependence")]
    public class AddressableVersionDependence : LoggableScriptableObject<AddressableVersionDependence>
    {
        /// <summary>
        /// Location of the addressable asset settings.
        /// </summary>
        public string AddressableSettingsLocation = "Assets/AddressableAssetsData/AddressableAssetSettings.asset";

        /// <summary>
        /// Location of the manifests.
        /// </summary>
        public string ManifestLocation = "Assets/Data/SceneManagement/Manifests/";

        /// <summary>
        /// Groups that don't need a manifest.
        /// </summary>
        public string[] GroupsToIgnore = {"Built In Data", "Default"};

        /// <summary>
        /// Initial manifest version when creating new ones.
        /// </summary>
        public string InitialManifestVersion = "0.0.1";

        /// <summary>
        /// Reference to the app version.
        /// </summary>
        public Version AppVersion;

        /// <summary>
        /// List of minimum versions the app requires.
        /// </summary>
        public SerializableDictionary<AddressableManifest, string> Dependencies =
            new SerializableDictionary<AddressableManifest, string>();

        /// <summary>
        /// Get a manifest by its name.
        /// </summary>
        /// <param name="manifestName">Name of the manifest.</param>
        /// <returns>The manifest object.</returns>
        public AddressableManifest GetManifestByName(string manifestName)
        {
            foreach (AddressableManifest manifest in Dependencies.Keys)
                if (manifest.name == manifestName)
                    return manifest;

            return null;
        }

        /// <summary>
        /// Clean the null saved dependencies, this prevents issues with deleted addressables.
        /// </summary>
        public void CleanNullDependencies()
        {
            List<int> dependenciesToRemove = new List<int>();

            List<AddressableManifest> keys = Dependencies.Keys.ToList();

            for (int i = 0; i < keys.Count; ++i)
                if (keys[i] == null)
                    dependenciesToRemove.Add(i);

            for (int i = 0; i < dependenciesToRemove.Count; ++i) Dependencies.RemoveAt(dependenciesToRemove[i]);
        }
    }
}