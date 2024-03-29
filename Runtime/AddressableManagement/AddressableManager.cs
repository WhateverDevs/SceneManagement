using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using WhateverDevs.Core.Runtime.Common;
using WhateverDevs.SceneManagement.Runtime.Utils;
using Zenject;
using Object = UnityEngine.Object;
using Version = WhateverDevs.Core.Runtime.Build.Version;

#if ODIN_INSPECTOR_3
using Sirenix.OdinInspector;
#endif

namespace WhateverDevs.SceneManagement.Runtime.AddressableManagement
{
    /// <summary>
    /// Class that handles addressable availability checking, loading and unloading.
    /// </summary>
    [CreateAssetMenu(menuName = "WhateverDevs/SceneManagement/AddressableManager", fileName = "AddressableManager")]
    public class AddressableManager : LoggableScriptableObject<AddressableManager>, IAddressableManager
    {
        /// <summary>
        /// Reference to the version dependence library.
        /// </summary>
        public AddressableVersionDependence AddressableVersionDependence;

        /// <summary>
        /// Report generated when checking availability of addressables.
        /// </summary>
        #if ODIN_INSPECTOR_3
        [ShowInInspector]
        [ReadOnly]
        #endif
        private AddressableStateReport addressableStateReport;

        /// <summary>
        /// List of all manifests found when checking available addressables.
        /// </summary>
        #if ODIN_INSPECTOR_3
        [ShowInInspector]
        [ReadOnly]
        #endif
        private List<AddressableManifest> manifests = new();

        /// <summary>
        /// Reference to the app version.
        /// </summary>
        [HideInInspector]
        public Version Version;

        /// <summary>
        /// Flag to check report creation.
        /// </summary>
        private bool generatingReport;

        /// <summary>
        /// Reference to the callback to check the progress.
        /// </summary>
        private Action<float> checkingProgressCallback;

        /// <summary>
        /// Waiting to report creation.
        /// </summary>
        private WaitUntil creationWait;

        /// <summary>
        /// Called when injection occurs.
        /// </summary>
        /// <param name="version"></param>
        [Inject]
        public void Constructor(Version version)
        {
            AddressableVersionDependence.CleanNullDependencies();

            creationWait = new WaitUntil(() => !generatingReport);
            CheckAvailableAddressables(null);
            Version = version;
        }

        /// <summary>
        /// Reset the manager.
        /// </summary>
        #if ODIN_INSPECTOR_3
        [Button]
        #endif
        public void Reset()
        {
            addressableStateReport = null;
            generatingReport = false;
            manifests.Clear();
        }

        /// <summary>
        /// Triggers checking for available addressables.
        /// </summary>
        /// <param name="callback">Callback that sends the report back.</param>
        /// <param name="progressCallback">Callback that sends back progress.</param>
        #if ODIN_INSPECTOR_3
        [Button]
        [EnableIf("@UnityEngine.Application.isPlaying")]
        #endif
        public void CheckAvailableAddressables(Action<AddressableStateReport> callback,
                                               Action<float> progressCallback = null)
        {
            Logger.Info("Checking addressables...");

            if (progressCallback != null) checkingProgressCallback = progressCallback;

            if (addressableStateReport == null)
                CoroutineRunner.RunRoutine(CheckAvailableAddressablesRoutine(callback));
            else
                callback?.Invoke(addressableStateReport);
        }

        /// <summary>
        /// Routine to asynchronously check available addressables.
        /// </summary>
        /// <param name="callback">Callback that sends the report back.</param>
        /// <returns></returns>
        private IEnumerator CheckAvailableAddressablesRoutine(Action<AddressableStateReport> callback)
        {
            if (generatingReport)
            {
                // wait until creation
                yield return creationWait;
                callback?.Invoke(addressableStateReport);
                yield break;
            }

            generatingReport = true;

            AddressableStateReport newReport = new();

            AsyncOperationHandle<IList<IResourceLocation>> manifestsHandle =
                Addressables.LoadResourceLocationsAsync("Manifest");

            yield return manifestsHandle;

            Logger.Info("Found " + manifestsHandle.Result.Count + " manifest locations.");

            for (int i = 0; i < manifestsHandle.Result.Count; i++)
            {
                IResourceLocation location = manifestsHandle.Result[i];
                Logger.Info("Checking " + location.PrimaryKey + " manifest...");

                checkingProgressCallback?.Invoke((float)i / manifestsHandle.Result.Count);

                AsyncOperationHandle<AddressableManifest> manifestHandle =
                    Addressables.LoadAssetAsync<AddressableManifest>(location);

                yield return manifestHandle;

                AddressableManifest manifest = manifestHandle.Result;

                if (manifest == null)
                {
                    Logger.Error(location.PrimaryKey + " manifest is missing!");

                    newReport.AddressableStates[LocationPrimaryKeyToGroupName(location.PrimaryKey)] =
                        AddressableVersionState.Missing;
                }
                else
                {
                    Logger.Info(location.PrimaryKey + " manifest found.");

                    if (!manifests.Contains(manifest)) manifests.Add(manifest);

                    string requiredVersion = AddressableVersionDependence.Dependencies[AddressableVersionDependence
                       .GetManifestByName(manifest.name)];

                    if (Helper.CheckVersion(manifest.FullVersion, requiredVersion) > 0)
                    {
                        Logger.Warn(manifest.name
                                  + " version("
                                  + manifest.FullVersion
                                  + ") is older than required("
                                  + requiredVersion
                                  + ").");

                        newReport.AddressableStates[LocationPrimaryKeyToGroupName(location.PrimaryKey)] =
                            AddressableVersionState.AddressableVersionLowerThanAppRequires;
                    }
                    else
                    {
                        if (Helper.CheckVersion(Version.FullVersion, manifest.MinimumAppVersion) > 0)
                        {
                            Logger.Warn("App version("
                                      + Version.FullVersion
                                      + ") is older than what "
                                      + manifest.name
                                      + " requires("
                                      + manifest.MinimumAppVersion
                                      + ").");

                            newReport.AddressableStates
                                    [LocationPrimaryKeyToGroupName(location.PrimaryKey)] =
                                AddressableVersionState.AppVersionLowerThanAddressableRequires;
                        }
                        else
                        {
                            // TODO: Check dependencies between bundles. I'm not sure if this can be done in runtime.
                            // TODO: Perhaps precached when building?

                            Logger.Info(manifest.name + " version is compatible with the app.");
                            Logger.Info("App version is compatible with " + manifest.name + ".");

                            newReport.AddressableStates
                                    [LocationPrimaryKeyToGroupName(location.PrimaryKey)] =
                                AddressableVersionState.Correct;
                        }
                    }
                }
            }

            Addressables.Release(manifestsHandle);

            addressableStateReport = newReport;

            Logger.Info("Cached addressables manifest state.");

            callback?.Invoke(addressableStateReport);

            generatingReport = false;
        }

        /// <summary>
        /// Check if a scene is in a valid manifest and ready to be loaded.
        /// </summary>
        /// <param name="scene">Scene to check.</param>
        /// <returns>True if available.</returns>
        public bool IsAssetAvailable(AssetReference scene)
        {
            if (addressableStateReport != null) return IsAssetInAValidManifest(scene);
            Logger.Error("Addressables have not been scanned yet!");
            return false;
        }

        /// <summary>
        /// Load the given scene by its asset reference.
        /// </summary>
        /// <param name="sceneReference">Scene to load.</param>
        /// <param name="sceneName">Scene name, only used for logs, can be null as long as the reference is okay.</param>
        /// <param name="loadMode">Mode to load the scene into.</param>
        /// <param name="progressCallback">Callback that reports the loading progress every frame.</param>
        /// <param name="sceneLoadedCallback">Callback called when the scene is loaded or when there has been an error.</param>
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

            if (IsAssetInAValidManifest(sceneReference))
                CoroutineRunner.RunRoutine(LoadSceneRoutine(sceneReference,
                                                            sceneName,
                                                            loadMode,
                                                            progressCallback,
                                                            sceneLoadedCallback));
            else
            {
                Logger.Error("Scene " + sceneName + " does not have a valid manifest!");
                sceneLoadedCallback?.Invoke(false);
            }
        }

        /// <summary>
        /// Load the given scene by its asset reference.
        /// </summary>
        /// <param name="sceneReference">Scene to load.</param>
        /// <param name="sceneName">Scene name, only used for logs, can be null as long as the reference is okay.</param>
        /// <param name="loadMode">Mode to load the scene into.</param>
        /// <param name="progressCallback">Callback that reports the loading progress every frame.</param>
        /// <param name="sceneLoadedCallback">Callback called when the scene is loaded or when there has been an error.</param>
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

        /// <summary>
        /// Load the given asset by its reference.
        /// </summary>
        /// <param name="assetReference">Reference to the asset to load.</param>
        /// <param name="loadedCallback">Callback raised when the asset is loaded, the bool represents success.</param>
        /// <typeparam name="T">Type of asset to load.</typeparam>
        public void LoadAsset<T>(AssetReference assetReference, Action<bool, T> loadedCallback) where T : Object
        {
            if (assetReference == null)
            {
                Logger.Error("Given asset reference is null!");
                loadedCallback?.Invoke(false, null);
                return;
            }

            if (IsAssetInAValidManifest(assetReference))
                CoroutineRunner.RunRoutine(LoadAssetRoutine(assetReference, loadedCallback));
            else
            {
                Logger.Error("Given asset reference does not have a valid manifest!");
                loadedCallback?.Invoke(false, null);
            }
        }

        /// <summary>
        /// Load the given asset by its reference.
        /// </summary>
        /// <param name="assetReference">Reference to the asset to load.</param>
        /// <param name="loadedCallback">Callback raised when the asset is loaded, the bool represents success.</param>
        /// <typeparam name="T">Type of asset to load.</typeparam>
        private IEnumerator LoadAssetRoutine<T>(IKeyEvaluator assetReference, Action<bool, T> loadedCallback)
            where T : Object
        {
            AsyncOperationHandle<T> operationHandle = default;
            bool success = true;

            try
            {
                operationHandle = Addressables.LoadAssetAsync<T>(assetReference);
            }
            catch (Exception e)
            {
                Logger.Fatal(e.Message + "\n" + e.StackTrace);
                loadedCallback?.Invoke(false, null);
                success = false;
            }

            yield return new WaitUntil(() => operationHandle.IsDone);

            if (operationHandle.Status == AsyncOperationStatus.Failed)
            {
                Logger.Fatal("Error retrieving the asset.");
                loadedCallback?.Invoke(false, null);
                success = false;
            }

            if (!success) yield break;

            Logger.Info("Loaded asset " + assetReference.RuntimeKey + ".");

            loadedCallback?.Invoke(true, operationHandle.Result);
        }

        /// <summary>
        /// Translate the primary key of an asset to its group name.
        /// </summary>
        /// <param name="primaryKey"></param>
        /// <returns></returns>
        private static string LocationPrimaryKeyToGroupName(string primaryKey) => primaryKey.Split('/')[1];

        /// <summary>
        /// Check if an asset is in a valid manifest.
        /// </summary>
        /// <param name="asset"></param>
        /// <returns></returns>
        private bool IsAssetInAValidManifest(IKeyEvaluator asset)
        {
            for (int i = 0; i < manifests.Count; ++i)
                if (manifests[i].AssetGuids.Contains(asset.RuntimeKey.ToString()))
                    return addressableStateReport.AddressableStates[manifests[i].name]
                        == AddressableVersionState.Correct;

            Logger.Error("No manifest found for asset!");

            return false;
        }
    }
}