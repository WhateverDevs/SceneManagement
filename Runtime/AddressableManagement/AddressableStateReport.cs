using System;
using WhateverDevs.Core.Runtime.DataStructures;

namespace WhateverDevs.SceneManagement.Runtime.AddressableManagement
{
    [Serializable]
    public class AddressableStateReport
    {
        public SerializableDictionary<string, AddressableVersionState> AddressableStates;

        public AddressableStateReport() => AddressableStates = new SerializableDictionary<string, AddressableVersionState>();
    }

    public enum AddressableVersionState
    {
        Missing,
        AddressableVersionLowerThanAppRequires,
        AppVersionLowerThanAddressableRequires,
        Correct
    }
}