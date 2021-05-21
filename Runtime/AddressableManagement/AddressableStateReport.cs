using System;
using WhateverDevs.Core.Runtime.DataStructures;

namespace WhateverDevs.SceneManagement.Runtime.AddressableManagement
{
    [Serializable]
    public class AddressableStateReport
    {
        public SerializableDictionary<string, AddressableState> AddressableStates;

        public AddressableStateReport() => AddressableStates = new SerializableDictionary<string, AddressableState>();
    }

    public enum AddressableState
    {
        Missing,
        AddressableVersionLowerThanAppRequires,
        AppVersionLowerThanAddressableRequires,
        Correct
    }
}