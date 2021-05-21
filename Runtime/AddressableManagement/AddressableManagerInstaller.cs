using UnityEngine;
using Zenject;

namespace WhateverDevs.SceneManagement.Runtime.AddressableManagement
{
    [CreateAssetMenu(menuName = "WhateverDevs/DI/AddressableManagement", fileName = "AddressableManagerInstaller")]
    public class AddressableManagerInstaller : ScriptableObjectInstaller
    {
        public AddressableManager AddressableManager;

        public override void InstallBindings()
        {
            AddressableManager.Reset();

            Container.QueueForInject(AddressableManager);

            Container.Bind<IAddressableManager>().FromInstance(AddressableManager).AsSingle().Lazy();
        }
    }
}