using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using WhateverDevs.Core.Runtime.Common;
using Zenject;
using Version = WhateverDevs.Core.Runtime.Build.Version;

namespace WhateverDevs.SceneManagement.Runtime.AddressableManagement
{
    [CreateAssetMenu(menuName = "WhateverDevs/SceneManagement/AddressableManager", fileName = "AddressableManager")]
    public class AddressableManager : LoggableScriptableObject<AddressableManager>, IAddressableManager
    {
        public AddressableVersionDependence AddressableVersionDependence;

        [ShowInInspector]
        [ReadOnly]
        private AddressableStateReport addressableStateReport;

        [Inject]
        [HideInInspector]
        public Version Version;

        [Button]
        public void Reset() => addressableStateReport = null;

        [Button]
        [EnableIf("@UnityEngine.Application.isPlaying")]
        public void CheckAvailableAddressables(Action<AddressableStateReport> callback)
        {
            Logger.Info("Checking addressables...");

            if (addressableStateReport == null)
                CoroutineRunner.Instance.RunRoutine(CheckAvailableAddressablesRoutine(callback));
            else
                callback?.Invoke(addressableStateReport);
        }

        private IEnumerator CheckAvailableAddressablesRoutine(Action<AddressableStateReport> callback)
        {
            addressableStateReport = new AddressableStateReport();

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

                    addressableStateReport.AddressableStates[LocationPrimaryKeyToGroupName(location.PrimaryKey)] =
                        AddressableState.Missing;
                }
                else
                {
                    Logger.Info(location.PrimaryKey + " manifest found.");

                    string requiredVersion = AddressableVersionDependence.Dependencies[AddressableVersionDependence
                       .GetManifestByName(manifest.name)];

                    if (string.Compare(manifest.FullVersion,
                                       requiredVersion,
                                       StringComparison.Ordinal)
                      < 0)
                    {
                        Logger.Warn(manifest.name
                                  + " version("
                                  + manifest.FullVersion
                                  + ") is older than required("
                                  + requiredVersion
                                  + ").");

                        addressableStateReport.AddressableStates[LocationPrimaryKeyToGroupName(location.PrimaryKey)] =
                            AddressableState.AddressableVersionLowerThanAppRequires;
                    }
                    else
                    {
                        if (string.Compare(Version.FullVersion,
                                           manifest.MinimumAppVersion,
                                           StringComparison.Ordinal)
                          < 0)
                        {
                            Logger.Warn("App version("
                                      + Version.FullVersion
                                      + ") is older than what "
                                      + manifest.name
                                      + " requires("
                                      + manifest.MinimumAppVersion
                                      + ").");

                            addressableStateReport.AddressableStates
                                    [LocationPrimaryKeyToGroupName(location.PrimaryKey)] =
                                AddressableState.AppVersionLowerThanAddressableRequires;
                        }
                        else
                        {
                            Logger.Info(manifest.name + " version is compatible with the app.");
                            Logger.Info("App version is compatible with " + manifest.name + ".");

                            addressableStateReport.AddressableStates
                                    [LocationPrimaryKeyToGroupName(location.PrimaryKey)] =
                                AddressableState.Correct;
                        }
                    }
                }
            }

            Addressables.Release(manifestsHandle);

            callback?.Invoke(addressableStateReport);
        }

        public void LoadScene(AssetReference sceneReference,
                              string sceneName,
                              LoadSceneMode loadMode,
                              Action<float> progressCallback,
                              Action<bool> sceneLoadedCallback)
        {
            if (sceneReference == null)
            {
                Logger.Error("Given scene is null!");
                sceneLoadedCallback?.Invoke(false);
                return;
            }
            
            // TODO: How the fuck do we get the labels of the scene from the asset reference?
            // TODO: We can't check its manifest without its labels.

            CoroutineRunner.Instance.RunRoutine(LoadSceneRoutine(sceneReference,
                                                                 sceneName,
                                                                 loadMode,
                                                                 progressCallback,
                                                                 sceneLoadedCallback));
        }

        private IEnumerator LoadSceneRoutine(AssetReference sceneReference,
                                             string sceneName,
                                             LoadSceneMode loadMode,
                                             Action<float> progressCallback,
                                             Action<bool> sceneLoadedCallback)
        {
            AsyncOperationHandle<SceneInstance> operation;

            try
            {
                operation = sceneReference.LoadSceneAsync(loadMode);
            }
            catch (Exception e)
            {
                Logger.Fatal(e.Message + "\n" + e.StackTrace);
                sceneLoadedCallback?.Invoke(false);
                yield break;
            }

            while (!operation.IsDone)
            {
                Logger.Info("Loading " + sceneReference.SubObjectName + " scene - " + operation.PercentComplete + ".");
                progressCallback?.Invoke(operation.PercentComplete);
                yield return new WaitForEndOfFrame();
            }
            
            Logger.Info("Scene " + sceneName + " loaded.");

            sceneLoadedCallback?.Invoke(true);
        }

        private static string LocationPrimaryKeyToGroupName(string primaryKey) => primaryKey.Split('/')[1];
    }
}