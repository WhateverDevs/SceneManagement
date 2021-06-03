using System;
using WhateverDevs.Core.Runtime.DataStructures;

namespace WhateverDevs.SceneManagement.Runtime.AddressableManagement
{
    /// <summary>
    /// Report generated when checking if addressables are available.
    /// </summary>
    [Serializable]
    public class AddressableStateReport
    {
        /// <summary>
        /// State of each of the addressables.
        /// </summary>
        public SerializableDictionary<string, AddressableVersionState> AddressableStates;

        /// <summary>
        /// Create the dictionary on the constructor.
        /// </summary>
        public AddressableStateReport() => AddressableStates = new SerializableDictionary<string, AddressableVersionState>();
    }

    /// <summary>
    /// States an addressable can have.
    /// </summary>
    public enum AddressableVersionState
    {
        Missing,
        AddressableVersionLowerThanAppRequires,
        AppVersionLowerThanAddressableRequires,
        Correct
    }
}