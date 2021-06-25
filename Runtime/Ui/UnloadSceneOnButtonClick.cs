using UnityEngine.SceneManagement;
using WhateverDevs.Core.Runtime.Ui;
using WhateverDevs.SceneManagement.Runtime.SceneManagement;
using Zenject;

namespace WhateverDevs.SceneManagement.Runtime.Ui
{
    /// <summary>
    /// Load the scene set on inspector on button click.
    /// </summary>
    public class UnloadSceneOnButtonClick : ActionOnButtonClick<LoadSceneOnButtonClick>
    {
        /// <summary>
        /// Scene to be unloaded.
        /// </summary>
        public SceneReference SceneToUnload;

        /// <summary>
        /// Mode to load the scene into.
        /// </summary>
        public LoadSceneMode Mode;

        /// <summary>
        /// Reference to the scene manager.
        /// </summary>
        [Inject]
        public ISceneManager SceneManager;

        /// <summary>
        /// Load the scene when the button is clicked.
        /// </summary>
        protected override void ButtonClicked() => SceneManager.UnloadScene(SceneToUnload, null, null);
    }
}