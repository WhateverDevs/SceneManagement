using UnityEngine;
using WhateverDevs.Core.Runtime.Build;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace WhateverDevs.SceneManagement.Runtime.BuildHooks
{
    /// <summary>
    /// Hook to warn the dev about building addressables before building the app.
    /// </summary>
    [CreateAssetMenu(menuName = "WhateverDevs/SceneManagement/BuildHooks/WarnAboutBuildBundlesBeforeBuild",
                     fileName = "WarnAboutBuildBundlesBeforeBuild")]
    public class WarnAboutBuildBundlesBeforeBuild : BuildProcessorHook
    {
        /// <summary>
        /// Toggle not reminding again.
        /// </summary>
        public bool DontRemindAgain;

        /// <summary>
        /// Run the hook.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public override bool RunHook(string path)
        {
            #if UNITY_EDITOR

            if (!DontRemindAgain)
                DontRemindAgain = !EditorUtility.DisplayDialog("Build bundles",
                                                               "Remember that you need to rebuild the app everytime you build the bundles. "
                                                             + "Build cannot be stopped now if you forgot, but just as a reminder.",
                                                               "Ok",
                                                               "Don't remind me again");
            
            AssetDatabase.SaveAssets();

            #endif

            return true;
        }
    }
}