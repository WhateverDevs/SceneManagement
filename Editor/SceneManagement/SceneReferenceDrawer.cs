using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;
using WhateverDevs.SceneManagement.Runtime.SceneManagement;

namespace Editor.SceneManagement
{
    /// <summary>
    /// Custom drawer for scene references.
    /// </summary>
    // ReSharper disable once UnusedType.Global
    public class SceneReferenceDrawer : OdinValueDrawer<SceneReference>
    {
        /// <summary>
        /// Paint the property.
        /// </summary>
        /// <param name="label"></param>
        protected override void DrawPropertyLayout(GUIContent label)
        {
            SceneReference reference = ValueEntry.SmartValue;

            if (reference.Library == null)
            {
                reference.Library =
                    AssetDatabase.LoadAssetAtPath<SceneManager>(SceneManagerCreator.SceneManagerPath);

                if (reference.Library == null)
                    reference.Library =
                        (SceneManager) EditorGUILayout.ObjectField(new GUIContent("Scene manager",
                                                                       "A reference to the scene manager is needed."),
                                                                   reference.Library,
                                                                   typeof(SceneManager),
                                                                   false);
            }
            else if (reference.Library.SceneNamesList.Count == 0)
                EditorGUILayout.HelpBox("There are no scenes in the library.", MessageType.Error);
            else
            {
                if (!reference.Library.SceneNamesList.Contains(reference.SceneName))
                    reference.SceneName = reference.Library.SceneNamesList[0];

                reference.SceneName =
                    reference.Library.SceneNames
                        [EditorGUILayout.Popup(label,
                                               reference.Library.SceneNamesList.IndexOf(reference.SceneName),
                                               reference.Library.SceneNames)];
            }

            ValueEntry.SmartValue = reference;
        }
    }
}