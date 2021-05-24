using UnityEngine;
using Zenject;

namespace WhateverDevs.SceneManagement.Runtime.SceneManagement
{
    /// <summary>
    /// Installer for the scene manager.
    /// </summary>
    [CreateAssetMenu(menuName = "WhateverDevs/DI/SceneManager", fileName = "SceneManagerInstaller")]
    public class SceneManagerInstaller : ScriptableObjectInstaller
    {
        /// <summary>
        /// Reference to the scene manager.
        /// </summary>
        public SceneManager SceneManager;

        /// <summary>
        /// Queue for inject and then inject as lazy singleton.
        /// </summary>
        public override void InstallBindings()
        {
            Container.QueueForInject(SceneManager);

            Container.Bind<SceneManager>().FromInstance(SceneManager).AsSingle().Lazy();
        }
    }
}