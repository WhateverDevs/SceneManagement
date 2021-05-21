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
    [CustomEditor(typeof(AddressablesBuilder))]
    public class AddressablesBuilderEditor : ScriptableExtendedEditor<AddressablesBuilder>
    {
        private AddressableAssetSettings addressableAssetSettings;

        private const string ProgressBarTitle = "Building addressables";

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

            PaintProperty("FolderToBuildTo");

            if (GUILayout.Button("Build")) Build();
        }

        private void Build()
        {
            try
            {
                EditorUtility.DisplayProgressBar(ProgressBarTitle, "Creating tags for groups..", .0f);

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
                        asset.SetLabel(addressableAssetGroup.Name, true);
                }

                AddressableAssetSettings.BuildPlayerContent();
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }
    }
}