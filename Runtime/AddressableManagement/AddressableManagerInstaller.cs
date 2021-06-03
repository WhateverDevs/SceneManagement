using UnityEngine;
using Zenject;

namespace WhateverDevs.SceneManagement.Runtime.AddressableManagement
{
    /// <summary>
    /// Installer for the default addressable manager.
    /// </summary>
    [CreateAssetMenu(menuName = "WhateverDevs/DI/AddressableManagement", fileName = "AddressableManagerInstaller")]
    public class AddressableManagerInstaller : ScriptableObjectInstaller
    {
        /// <summary>
        /// Reference to the addressable manager.
        /// </summary>
        public AddressableManager AddressableManager;

        /// <summary>
        /// First reset the manager,
        /// then inject the references to it,
        /// then trigger addressable checking
        /// and finally inject it into other classes.
        /// </summary>
        public override void InstallBindings()
        {
            AddressableManager.Reset();

            Container.QueueForInject(AddressableManager);

            Container.Bind<IAddressableManager>().FromInstance(AddressableManager).AsSingle().Lazy();
        }
    }
}