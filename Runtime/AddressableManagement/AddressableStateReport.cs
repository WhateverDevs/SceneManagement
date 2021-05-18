using System.Collections.Generic;

namespace WhateverDevs.SceneManagement.Runtime.AddressableManagement
{
    public class AddressableStateReport
    {
        public Dictionary<string, AddressableState> AddressableStates;

        public AddressableStateReport() => AddressableStates = new Dictionary<string, AddressableState>();
    }

    public enum AddressableState
    {
        Missing,
        AddressableVersionLowerThanAppRequires,
        AppVersionLowerThanAddressableRequires,
        Correct
    }
}