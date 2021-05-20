using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using WhateverDevs.Core.Runtime.Common;
using Zenject;
using Version = WhateverDevs.Core.Runtime.Build.Version;

namespace WhateverDevs.SceneManagement.Runtime.AddressableManagement
{
    [CreateAssetMenu(menuName = "WhateverDevs/SceneManagement/AddressableManager", fileName = "AddressableManager")]
    public class AddressableManager : LoggableScriptableObject<AddressableManager>, IAddressableManager
    {
        public AddressableVersionDependence AddressableVersionDependence;
        
        [Inject]
        [HideInInspector]
        public Version Version;
        
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

            AsyncOperationHandle<IList<IResourceLocation>> manifestsHandle =
                Addressables.LoadResourceLocationsAsync("Manifest");

            yield return manifestsHandle;

            Logger.Info("Found " + manifestsHandle.Result.Count + " manifest locations.");

            foreach (IResourceLocation location in manifestsHandle.Result)
            {
                Logger.Info("Checking " + location.PrimaryKey + " manifest...");

                AsyncOperationHandle<AddressableManifest> manifestHandle =
                    Addressables.LoadAssetAsync<AddressableManifest>(location);

                yield return manifestHandle;

                AddressableManifest manifest = manifestHandle.Result;

                if (manifest == null)
                {
                    Logger.Error(location.PrimaryKey + " manifest is missing!");
                    report.AddressableStates[location.PrimaryKey] = AddressableState.Missing;
                }
                else
                {
                    Logger.Info(location.PrimaryKey + " manifest found.");
                    Logger.Info(manifest.name + " version is " + manifest.Version + ".");
                    
                    
                    
                }
            }

            Addressables.Release(manifestsHandle);
        }
    }
}