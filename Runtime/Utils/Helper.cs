using System;
using System.Text.RegularExpressions;

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
        /// <returns>0 if equal, -1 verion1 newer , 1 version2 newer, -2 error.</returns>
        public static int CheckVersion(string version1, string version2)
        {
            string[] splitversion1 = version1.Split('.');
            string[] splitversion2 = version2.Split('.');

            if (splitversion1.Length != splitversion2.Length) return -2;

            if (Char.IsLetter(splitversion1[2][splitversion1[2].Length-1]) && Char.IsLetter(splitversion2[2][splitversion2[2].Length-1]))
            {
                if (splitversion1[2][splitversion1[2].Length-1] > splitversion2[2][splitversion2[2].Length-1]) return 1;
                if (splitversion1[2][splitversion1[2].Length-1] < splitversion2[2][splitversion2[2].Length-1]) return -1;

                splitversion1[2] = splitversion1[2].Remove(splitversion1[2].Length - 1,1);
                splitversion2[2] = splitversion2[2].Remove(splitversion2[2].Length - 1,1);
            }
                
            
            for (int i = 0; i < splitversion1.Length; ++i)
            {
                if(long.Parse(splitversion1[i]) > long.Parse(splitversion2[i]))
                    return -1;
                if(long.Parse(splitversion1[i]) > long.Parse(splitversion2[i]))
                    return 1;
            }

            return 0;
        }
    }
}