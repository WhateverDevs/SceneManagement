using System;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

namespace WhateverDevs.SceneManagement.Runtime.AddressableManagement
{
    public interface IAddressableManager
    {
        void Reset();
        
        void CheckAvailableAddressables(Action<AddressableStateReport> callback);

        void LoadScene(AssetReference sceneReference,
                       LoadSceneMode loadMode,
                       Action<float> progressCallback,
                       Action<bool> sceneLoadedCallback);
    }
}