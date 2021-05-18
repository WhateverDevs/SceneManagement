using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using WhateverDevs.Core.Runtime.Common;

namespace WhateverDevs.SceneManagement.Runtime.AddressableManagement
{
    [CreateAssetMenu(menuName = "WhateverDevs/SceneManagement/AddressableManager", fileName = "AddressableManager")]
    public class AddressableManager : LoggableScriptableObject<AddressableManager>, IAddressableManager
    {
        [Button]
        [EnableIf("@UnityEngine.Application.isPlaying")]
        public void CheckAvailableAddressables(Action<AddressableStateReport> callback)
        {
            Logger.Info("Checking addressables...");

            CoroutineRunner.Instance.RunRoutine(CheckAvailableAddressablesRoutine(callback));
        }

        private IEnumerator CheckAvailableAddressablesRoutine(Action<AddressableStateReport> callback)
        {
            yield return new WaitForSeconds(1);
            
            AddressableStateReport report = new AddressableStateReport();
            
            AsyncOperationHandle<IList<IResourceLocation>> handle = Addressables.LoadResourceLocationsAsync("Manifest");

            yield return handle;

            foreach (IResourceLocation location in handle.Result)
            {
                Logger.Info("Checking " + location.PrimaryKey + "...");

                AsyncOperationHandle<AddressableManifest> manifestHandle =
                    Addressables.LoadAssetAsync<AddressableManifest>(location);

                yield return manifestHandle;

                AddressableManifest manifest = manifestHandle.Result;

                if (manifest == null)
                {
                    Logger.Error(location.PrimaryKey + " is missing!");
                    report.AddressableStates[location.PrimaryKey] = AddressableState.Missing;
                }
                else
                {
                    Logger.Info(location.PrimaryKey + " found.");
                }
            }

            Addressables.Release(handle);
        }
    }
}