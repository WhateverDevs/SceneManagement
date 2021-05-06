using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.SceneManagement;
using WhateverDevs.Core.Runtime.Common;

namespace WhateverDevs.SceneManagement.Runtime.AddressableManagement
{
    [CreateAssetMenu(menuName = "WhateverDevs/SceneManagement/AddressableManager", fileName = "AddressableManager")]
    public class AddressableManager : LoggableScriptableObject<AddressableManager>, IAddressableManager
    {
        [Button]
        [EnableIf("@UnityEngine.Application.isPlaying")]
        public void CheckAvailableScenes()
        {
            Logger.Info("Checking addressables...");

            CoroutineRunner.Instance.RunRoutine(CheckAvailableScenesRoutine());
        }

        private IEnumerator CheckAvailableScenesRoutine()
        {
            AsyncOperationHandle<IList<IResourceLocation>> handle = Addressables.LoadResourceLocationsAsync("Scenario");

            yield return handle;

            foreach (IResourceLocation location in handle.Result)
            {
                Logger.Info(location.PrimaryKey);

                try
                {
                    Addressables.LoadSceneAsync(location.PrimaryKey, LoadSceneMode.Additive);
                }
                catch (Exception)
                {
                    Logger.Error("Addressable not available!");
                }
            }

            Addressables.Release(handle);
        }
    }
}