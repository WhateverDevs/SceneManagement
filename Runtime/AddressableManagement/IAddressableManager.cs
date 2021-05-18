using System;

namespace WhateverDevs.SceneManagement.Runtime.AddressableManagement
{
    public interface IAddressableManager
    {
        void CheckAvailableAddressables(Action<AddressableStateReport> callback);
    }
}