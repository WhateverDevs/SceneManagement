using JetBrains.Annotations;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;
using WhateverDevs.SceneManagement.Runtime.SceneManagement;

namespace Editor.SceneManagement
{
    /// <summary>
    /// Custom drawer for scene references.
    /// </summary>
    [UsedImplicitly]
    public class SceneReferenceDrawer : OdinValueDrawer<SceneReference>
    {
        /// <summary>
        /// Reference to the scene manager.
        /// </summary>
        private SceneManager library;

        /// <summary>
        /// Paint the property.
        /// </summary>
        /// <param name="label"></param>
        protected override void DrawPropertyLayout(GUIContent label)
        {
            SceneReference reference = ValueEntry.SmartValue;

            if (library == null)
            {
                library =
                    AssetDatabase.LoadAssetAtPath<SceneManager>(SceneManagerCreator.SceneManagerPath);

                if (library == null)
                    library =
                        (SceneManager) EditorGUILayout.ObjectField(new GUIContent("Scene manager",
                                                                       "A reference to the scene manager is needed."),
                                                                   library,
                                                                   typeof(SceneManager),
                                                                   false);
            }
            else if (library.SceneNamesList.Count == 0)
                EditorGUILayout.HelpBox("There are no scenes in the library.", MessageType.Error);
            else
            {
                if (!library.SceneNamesList.Contains(reference.SceneName))
                    reference.SceneName = library.SceneNamesList[0];

                reference.SceneName =
                    library.SceneNames
                        [EditorGUILayout.Popup(label,
                                               library.SceneNamesList.IndexOf(reference.SceneName),
                                               library.SceneNames)];
            }

            ValueEntry.SmartValue = reference;
        }
    }
}