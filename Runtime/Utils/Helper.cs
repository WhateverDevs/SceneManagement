namespace WhateverDevs.SceneManagement.Runtime.Utils
{
    /// <summary>
    /// Class with utility functions and extensions.
    /// </summary>
    public static class Helper
    {
        /// <summary>
        /// Checks if a string version is newer than other
        /// </summary>
        /// <param name="version1">The version to check.</param>
        /// <param name="version2">The version to check.</param>
        /// <returns>0 if equal, -1 version1 newer , 1 version2 newer, -2 error.</returns>
        public static int CheckVersion(string version1, string version2)
        {
            string[] splitVersion1 = version1.Split('.');
            string[] splitVersion2 = version2.Split('.');

            if (splitVersion1.Length != splitVersion2.Length) return -2;

            if (char.IsLetter(splitVersion1[2][splitVersion1[2].Length - 1])
             && char.IsLetter(splitVersion2[2][splitVersion2[2].Length - 1]))
            {
                if (splitVersion1[2][splitVersion1[2].Length - 1] < splitVersion2[2][splitVersion2[2].Length - 1])
                    return 1;

                if (splitVersion1[2][splitVersion1[2].Length - 1] > splitVersion2[2][splitVersion2[2].Length - 1])
                    return -1;
            }

            if (char.IsLetter(splitVersion1[2][splitVersion1[2].Length - 1]))
                splitVersion1[2] = splitVersion1[2].Remove(splitVersion1[2].Length - 1, 1);

            if (char.IsLetter(splitVersion2[2][splitVersion2[2].Length - 1]))
                splitVersion2[2] = splitVersion2[2].Remove(splitVersion2[2].Length - 1, 1);

            for (int i = 0; i < splitVersion1.Length; ++i)
            {
                if (long.Parse(splitVersion1[i]) > long.Parse(splitVersion2[i])) return -1;
                if (long.Parse(splitVersion1[i]) < long.Parse(splitVersion2[i])) return 1;
            }

            return 0;
        }
    }
}