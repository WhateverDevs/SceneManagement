using UnityEngine;
using UnityEngine.SceneManagement;
using WhateverDevs.Core.Runtime.Ui;
using WhateverDevs.SceneManagement.Runtime.SceneManagement;
using Zenject;
using SceneManager = WhateverDevs.SceneManagement.Runtime.SceneManagement.SceneManager;

namespace WhateverDevs.SceneManagement.Runtime.Ui
{
    /// <summary>
    /// Load the scene set on inspector on button click.
    /// </summary>
    public class LoadSceneOnButtonClick : ActionOnButtonClick<LoadSceneOnButtonClick>
    {
        /// <summary>
        /// Scene to be loaded.
        /// </summary>
        public SceneReference SceneToLoad;

        /// <summary>
        /// Mode to load the scene into.
        /// </summary>
        public LoadSceneMode Mode;

        /// <summary>
        /// Reference to the scene manager.
        /// </summary>
        [Inject]
        [HideInInspector]
        public ISceneManager SceneManager;

        /// <summary>
        /// Load the scene when the button is clicked.
        /// </summary>
        protected override void ButtonClicked() => SceneManager.LoadScene(SceneToLoad, null, null, Mode);
    }
}