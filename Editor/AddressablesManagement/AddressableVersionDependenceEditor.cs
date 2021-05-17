using System.Collections.Generic;
using Sirenix.Utilities;
using UnityEditor;
using UnityEditor.AddressableAssets.Settings;
using Varguiniano.ExtendedEditor.Editor;
using WhateverDevs.SceneManagement.Runtime.AddressableManagement;

namespace WhateverDevs.SceneManagement.Editor.AddressablesManagement
{
    [CustomEditor(typeof(AddressableVersionDependence))]
    public class AddressableVersionDependenceEditor : ScriptableExtendedEditor<AddressableVersionDependence>
    {
        private AddressableAssetSettings settings;

        protected override void PaintUi()
        {
            if (!TargetObject.SettingsLocation.IsNullOrWhitespace())
                settings = AssetDatabase.LoadAssetAtPath<AddressableAssetSettings>(TargetObject.SettingsLocation);

            if (settings == null)
            {
                settings =
                    (AddressableAssetSettings) EditorGUILayout.ObjectField("Addressable settings",
                                                                           settings,
                                                                           typeof(AddressableAssetSettings),
                                                                           false);

                if (settings != null) TargetObject.SettingsLocation = AssetDatabase.GetAssetPath(settings);

                return;
            }

            List<AddressableAssetGroup> groups = settings.groups;

            for (int i = 0; i < groups.Count; ++i) EditorGUILayout.LabelField(groups[i].Name);
        }
    }
}