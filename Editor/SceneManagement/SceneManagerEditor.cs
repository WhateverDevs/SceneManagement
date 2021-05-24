using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Varguiniano.ExtendedEditor.Editor;
using WhateverDevs.Core.Runtime.Common;
using WhateverDevs.SceneManagement.Runtime.SceneManagement;

namespace Editor.SceneManagement
{
    /// <summary>
    /// Custom editor for the scene manager.
    /// </summary>
    [CustomEditor(typeof(SceneManager))]
    public class SceneManagerEditor : ScriptableExtendedEditor<SceneManager>
    {
        /// <summary>
        /// List that saves the scene assets.
        /// </summary>
        private List<SceneAsset> sceneList = new List<SceneAsset>();

        /// <summary>
        /// Reorderable list to display the scene list.
        /// </summary>
        private ReorderableList reorderableList;

        private string selectedScene;

        /// <summary>
        /// Called when this object is enabled.
        /// </summary>
        private void OnEnable()
        {
            LoadList();
            LoadListEditor();
        }

        /// <summary>
        /// Paint the UI.
        /// </summary>
        protected override void PaintUi()
        {
            if (Application.isPlaying)
            {
                if (selectedScene.IsNullEmptyOrWhiteSpace()) selectedScene = TargetObject.SceneNames[0];

                EditorGUILayout.BeginHorizontal();

                {
                    selectedScene =
                        TargetObject.SceneNames
                            [EditorGUILayout.Popup(TargetObject.SceneNamesList.IndexOf(selectedScene),
                                                   TargetObject.SceneNames)];

                    if (GUILayout.Button("Load")) TargetObject.LoadScene(selectedScene, null, null);
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUI.BeginDisabledGroup(Application.isPlaying);

            {
                EditorGUILayout.BeginVertical("Box");

                {
                    EditorGUILayout.LabelField("Non addressable scenes");

                    EditorGUILayout
                       .HelpBox("Be careful not to add addressable scenes to this list!",
                                MessageType.Warning);

                    EditorGUILayout
                       .HelpBox("Remember to click the save button after editing!",
                                MessageType.Warning);

                    if (GUILayout.Button("Save library and build settings")) SaveList();
                    reorderableList.DoLayoutList();
                }

                EditorGUILayout.EndVertical();

                EditorGUILayout.Space();
                EditorGUILayout.Space();

                EditorGUILayout.BeginVertical("Box");

                {
                    EditorGUILayout.LabelField("Addressable scenes");

                    EditorGUILayout
                       .HelpBox("Keep in mind that if you add a nonaddressable scene to this list it will become addressable.",
                                MessageType.Warning);

                    EditorGUILayout
                       .HelpBox("Also don't add non scene assets here.",
                                MessageType.Warning);

                    PaintProperty("AddressableScenes", true);
                }

                EditorGUILayout.EndVertical();
            }

            EditorGUI.EndDisabledGroup();
        }

        /// <summary>
        /// Loads the scene list.
        /// </summary>
        private void LoadList()
        {
            sceneList.Clear();
            List<string> scenesToRemove = new List<string>();

            for (int i = 0; i < TargetObject.NonAddressableScenes.Count; ++i)
            {
                SceneAsset scene = AssetDatabase.LoadAssetAtPath<SceneAsset>(TargetObject.NonAddressableScenes[i]);

                if (scene == null)
                {
                    TargetObject.Logger.Info("Library contained a path to a scene that doesn't exist, deleting it.");
                    scenesToRemove.Add(TargetObject.NonAddressableScenes[i]);
                    continue;
                }

                sceneList.Add(scene);
            }

            for (int i = 0; i < scenesToRemove.Count; ++i) TargetObject.NonAddressableScenes.Remove(scenesToRemove[i]);
        }

        /// <summary>
        /// Saves the scene list.
        /// </summary>
        private void SaveList()
        {
            List<string> scenesToSave = new List<string>();
            List<EditorBuildSettingsScene> scenesToBuild = new List<EditorBuildSettingsScene>();

            for (int i = 0; i < sceneList.Count; ++i)
            {
                if (sceneList[i] == null) continue;
                string path = AssetDatabase.GetAssetPath(sceneList[i]);

                scenesToSave.Add(path);

                scenesToBuild.Add(new EditorBuildSettingsScene(path, true));
            }

            TargetObject.NonAddressableScenes = scenesToSave;
            EditorBuildSettings.scenes = scenesToBuild.ToArray();
        }

        /// <summary>
        /// Loads the editor for the Exercise List.
        /// </summary>
        private void LoadListEditor() =>
            reorderableList =
                new ReorderableList(sceneList, typeof(SceneAsset))
                {
                    drawHeaderCallback = rect => EditorGUI.LabelField(rect, "Scene list"),
                    drawElementCallback = (rect, index, isActive, isFocused) =>
                                              sceneList[index] = (SceneAsset) EditorGUI.ObjectField(rect,
                                                  sceneList[index],
                                                  typeof(SceneAsset),
                                                  false),
                    onAddCallback = list => sceneList.Add(null)
                };
    }
}