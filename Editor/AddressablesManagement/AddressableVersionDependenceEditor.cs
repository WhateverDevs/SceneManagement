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

        // ReSharper disable once CyclomaticComplexity
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
            EditorGUILayout.Space();

            List<AddressableAssetGroup> groups = addressableAssetSettings.groups;

            List<string> groupNames = new List<string>();

            for (int i = 0; i < groups.Count; ++i) groupNames.Add(groups[i].Name);

            for (int i = 0; i < groups.Count; ++i)
            {
                string groupName = groups[i].Name;

                if (TargetObject.GroupsToIgnore.Contains(groupName)) continue;

                EditorGUILayout.BeginVertical("box");

                {
                    EditorGUILayout.LabelField(groupName);
                    EditorGUILayout.Space();

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

                        manifest.Version =
                            EditorGUILayout.TextField(new GUIContent("Manifest Version",
                                                                     "The current version of this addressable."),
                                                      manifest.Version);

                        EditorGUILayout.HelpBox("The full version gets updated automatically when building.",
                                                MessageType.Info);

                        EditorGUI.BeginDisabledGroup(true);

                        {
                            EditorGUILayout.TextField(new GUIContent("Manifest full version",
                                                                     "The current full version of this addressable."),
                                                      manifest.FullVersion);
                        }

                        EditorGUI.EndDisabledGroup();

                        if (GUILayout.Button("Update manifest full version")) manifest.UpdateFullVersion();

                        EditorGUI.BeginDisabledGroup(true);

                        {
                            manifest.MinimumAppVersion =
                                EditorGUILayout.TextField(new GUIContent("Minimum app version",
                                                                         "Minimum version of the app required by this addressable."),
                                                          manifest.MinimumAppVersion);
                        }

                        EditorGUI.EndDisabledGroup();

                        if (GUILayout.Button("Update minimum app version"))
                            manifest.MinimumAppVersion = manifest.AppVersionReference.FullVersion;

                        EditorUtility.SetDirty(manifest);

                        ManifestStringObjectPair dependency = null;

                        List<ManifestStringObjectPair> dependenciesToRemove =
                            new List<ManifestStringObjectPair>();

                        for (int j = 0; j < TargetObject.Dependencies.Count; ++j)
                        {
                            ManifestStringObjectPair dependencyCandidate = TargetObject.Dependencies[j];

                            if (dependencyCandidate.Key == null || !groupNames.Contains(dependencyCandidate.Key.name))
                                dependenciesToRemove.Add(dependencyCandidate);

                            if (dependencyCandidate.Key == manifest) dependency = dependencyCandidate;
                        }

                        for (int j = 0; j < dependenciesToRemove.Count; ++j)
                            TargetObject.Dependencies.Remove(dependenciesToRemove[j]);

                        if (dependency == null)
                        {
                            dependency = new ManifestStringObjectPair {Key = manifest};
                            TargetObject.Dependencies.Add(dependency);
                        }

                        EditorGUI.BeginDisabledGroup(true);

                        {
                            EditorGUILayout.TextField(new GUIContent("Minimum manifest version required",
                                                                     "Minimum version of this addressable required by the app."),
                                                      dependency.Value);
                        }

                        EditorGUI.EndDisabledGroup();

                        if (GUILayout.Button("Update to current manifest version")
                         || dependency.Value.IsNullOrWhitespace())
                            dependency.Value = manifest.FullVersion;
                    }
                    else
                    {
                        EditorGUILayout.HelpBox("Missing manifest!", MessageType.Error);

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

                            addressableAssetEntry.address = "Manifests/" + groupName;

                            if (!addressableAssetSettings.GetLabels().Contains("Manifest"))
                                addressableAssetSettings.AddLabel("Manifest");

                            addressableAssetEntry.SetLabel("Manifest", true);

                            AssetDatabase.SaveAssets();
                        }
                    }
                }

                EditorGUILayout.EndVertical();
            }
        }
    }
}