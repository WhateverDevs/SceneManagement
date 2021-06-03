using System.Collections.Generic;
using System.Linq;
using Sirenix.Utilities;
using UnityEditor;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;
using Varguiniano.ExtendedEditor.Editor;
using WhateverDevs.SceneManagement.Runtime.AddressableManagement;

namespace WhateverDevs.SceneManagement.Editor.AddressablesManagement
{
    /// <summary>
    /// Custom editor for the addressable builder.
    /// This is were the building code is.
    /// TODO: Change the building code to a static class.
    /// </summary>
    [CustomEditor(typeof(AddressablesBuilder))]
    public class AddressablesBuilderEditor : ScriptableExtendedEditor<AddressablesBuilder>
    {
        /// <summary>
        /// Reference to the addressable asset settings.
        /// </summary>
        private AddressableAssetSettings addressableAssetSettings;

        /// <summary>
        /// Title for the progress bar.
        /// </summary>
        private const string ProgressBarTitle = "Building addressables";

        /// <summary>
        /// Paint the ui.
        /// </summary>
        protected override void PaintUi()
        {
            if (TargetObject.VersionDependence == null)
            {
                PaintProperty("VersionDependence");
                return;
            }

            if (!TargetObject.VersionDependence.AddressableSettingsLocation.IsNullOrWhitespace())
                addressableAssetSettings =
                    AssetDatabase.LoadAssetAtPath<AddressableAssetSettings>(TargetObject.VersionDependence
                                                                               .AddressableSettingsLocation);

            if (addressableAssetSettings == null)
            {
                EditorGUILayout
                   .HelpBox("Fix the addressable settings location reference in the version dependence scriptable!",
                            MessageType.Error);

                return;
            }

            EditorGUILayout
               .HelpBox("Remember that the build location can't be changed by this tool. You have to set it on the group schemata.",
                        MessageType.Info);

            if (GUILayout.Button("Build")) Build();
        }

        /// <summary>
        /// Build the addressables.
        /// </summary>
        private void Build()
        {
            try
            {
                EditorUtility.DisplayProgressBar(ProgressBarTitle, "Creating tags for groups...", .33f);

                List<AddressableAssetGroup> groups = addressableAssetSettings.groups;

                for (int i = 0; i < groups.Count; ++i)
                {
                    AddressableAssetGroup addressableAssetGroup = groups[i];

                    if (TargetObject.VersionDependence.GroupsToIgnore.Contains(addressableAssetGroup.Name)) continue;

                    EditorUtility.DisplayProgressBar(ProgressBarTitle,
                                                     "Creating tag for group" + addressableAssetGroup.Name + "...",
                                                     (float) i / groups.Count);

                    if (!addressableAssetSettings.GetLabels().Contains(addressableAssetGroup.Name))
                        addressableAssetSettings.AddLabel(addressableAssetGroup.Name);

                    EditorUtility.DisplayProgressBar(ProgressBarTitle,
                                                     "Adding tag to assets in group "
                                                   + addressableAssetGroup.Name
                                                   + "...",
                                                     (float) i / groups.Count);

                    foreach (AddressableAssetEntry asset in addressableAssetGroup.entries)
                    {
                        asset.SetLabel(addressableAssetGroup.Name, true);

                        EditorUtility.DisplayProgressBar(ProgressBarTitle,
                                                         "Registering assets to manifest "
                                                       + addressableAssetGroup.Name
                                                       + "...",
                                                         (float) i / groups.Count);

                        if (!asset.labels.Contains("Manifest")) continue;
                        AddressableManifest manifest = (AddressableManifest) asset.MainAsset;
                        manifest.UpdateFullVersion();
                        manifest.AssetGuids.Clear();

                        foreach (AddressableAssetEntry entry in addressableAssetGroup.entries)
                            manifest.AssetGuids.Add(entry.guid);

                        EditorUtility.SetDirty(manifest);
                    }

                    AssetDatabase.Refresh();
                }

                AddressableAssetSettings.BuildPlayerContent();
                
                TargetObject.Logger.Info("Finished addressables build.");
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }
    }
}