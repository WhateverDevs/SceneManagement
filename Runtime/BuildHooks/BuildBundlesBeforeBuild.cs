using WhateverDevs.Core.Runtime.Build;

namespace WhateverDevs.SceneManagement.Runtime.BuildHooks
{
    public class BuildBundlesBeforeBuild : BuildProcessorHook
    {
        public override bool RunHook()
        {
            return true;
        }
    }
}