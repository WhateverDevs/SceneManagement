using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sirenix.Utilities;
using UnityEditor;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;
using Varguiniano.ExtendedEditor.Editor;
using WhateverDevs.SceneManagement.Runtime.AddressableManagement;

namespace WhateverDevs.SceneManagement.Editor.AddressablesManagement
{
    [CustomEditor(typeof(AddressableVersionDependence))]
    public class AddressableVersionDependenceEditor : ScriptableExtendedEditor<AddressableVersionDependence>
    {
        private AddressableAssetSettings addressableAssetSettings;

        private bool showSettings;

        protected override void PaintUi()
        {
            if (GUILayout.Button("Settings")) showSettings = !showSettings;

            if (showSettings)
            {
                PaintProperty("AddressableSettingsLocation");
                PaintProperty("ManifestLocation");
                PaintProperty("InitialManifestVersion");
                PaintProperty("GroupsToIgnore", true);
                return;
            }

            if (!TargetObject.AddressableSettingsLocation.IsNullOrWhitespace())
                addressableAssetSettings =
                    AssetDatabase.LoadAssetAtPath<AddressableAssetSettings>(TargetObject.AddressableSettingsLocation);

            if (addressableAssetSettings == null)
            {
                addressableAssetSettings =
                    (AddressableAssetSettings) EditorGUILayout.ObjectField("Addressable settings",
                                                                           addressableAssetSettings,
                                                                           typeof(AddressableAssetSettings),
                                                                           false);

                if (addressableAssetSettings != null)
                    TargetObject.AddressableSettingsLocation = AssetDatabase.GetAssetPath(addressableAssetSettings);

                return;
            }

            if (TargetObject.AppVersion == null)
            {
                PaintProperty("AppVersion");
                return;
            }

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Addressables");

            List<AddressableAssetGroup> groups = addressableAssetSettings.groups;

            List<string> groupNames = new List<string>();

            for (int i = 0; i < groups.Count; ++i) groupNames.Add(groups[i].Name);

            for (int i = 0; i < groups.Count; ++i)
            {
                EditorGUILayout.BeginHorizontal();

                {
                    string groupName = groups[i].Name;

                    if (!TargetObject.GroupsToIgnore.Contains(groupName))
                    {
                        EditorGUILayout.LabelField(groupName);

                        EditorGUILayout.BeginVertical();

                        {
                            ICollection<AddressableAssetEntry> assets = groups[i].entries;

                            bool hasManifestLabel = false;
                            AddressableAssetEntry manifestAsset = null;

                            foreach (AddressableAssetEntry asset in assets)
                            {
                                foreach (string _ in asset.labels.Where(label => label == "Manifest"))
                                {
                                    hasManifestLabel = true;
                                    manifestAsset = asset;
                                }
                            }

                            if (hasManifestLabel)
                            {
                                AddressableManifest manifest = (AddressableManifest) manifestAsset.MainAsset;

                                EditorGUI.BeginDisabledGroup(true);

                                {
                                    EditorGUILayout.ObjectField(manifest, typeof(AddressableManifest), false);
                                }

                                EditorGUI.EndDisabledGroup();

                                ManifestStringObjectPair dependency = null;

                                List<ManifestStringObjectPair> dependenciesToRemove =
                                    new List<ManifestStringObjectPair>();

                                for (int j = 0; j < TargetObject.Dependencies.Count; ++j)
                                {
                                    ManifestStringObjectPair dependencyCandidate = TargetObject.Dependencies[j];

                                    if (!groupNames.Contains(dependencyCandidate.Key.name))
                                        dependenciesToRemove.Add(dependencyCandidate);

                                    if (dependencyCandidate.Key == manifest) dependency = dependencyCandidate;
                                }

                                for (int j = 0; j < dependenciesToRemove.Count; ++j)
                                    TargetObject.Dependencies.Remove(dependenciesToRemove[i]);

                                if (dependency == null)
                                {
                                    dependency = new ManifestStringObjectPair {Key = manifest};
                                    TargetObject.Dependencies.Add(dependency);
                                }

                                EditorGUI.BeginDisabledGroup(true);

                                {
                                    EditorGUILayout.TextField(dependency.Value);
                                }

                                EditorGUI.EndDisabledGroup();

                                if (GUILayout.Button("Update to current manifest version."))
                                    dependency.Value = manifest.FullVersion;
                            }
                            else
                            {
                                if (GUILayout.Button("Create Manifest"))
                                {
                                    if (!Directory.Exists(TargetObject.ManifestLocation))
                                        Directory.CreateDirectory(TargetObject.ManifestLocation);

                                    AddressableManifest manifest = CreateInstance<AddressableManifest>();

                                    manifest.AppVersionReference = TargetObject.AppVersion;
                                    manifest.Version = TargetObject.InitialManifestVersion;
                                    manifest.UpdateFullVersion();
                                    manifest.UpdateMinimumToCurrentAppVersion();

                                    AssetDatabase.CreateAsset(manifest,
                                                              TargetObject.ManifestLocation + groupName + ".asset");

                                    AssetDatabase.SaveAssets();

                                    AddressableAssetEntry addressableAssetEntry = addressableAssetSettings
                                       .CreateOrMoveEntry(AssetDatabase.AssetPathToGUID(AssetDatabase
                                                             .GetAssetPath(manifest)),
                                                          groups[i]);

                                    addressableAssetEntry.SetLabel("Manifest", true);

                                    AssetDatabase.SaveAssets();
                                }
                            }
                        }

                        EditorGUILayout.EndVertical();
                    }
                }

                EditorGUILayout.EndHorizontal();
            }
        }
    }
}