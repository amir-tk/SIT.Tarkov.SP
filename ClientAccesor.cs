using UnityEngine;
using Comfort.Common;
using EFT;
using JET.Utility.Patching;
using System.Linq;
using System.Reflection;
using System;

namespace SIT.Tarkov.Core
{
    public static class ClientAccesor
    {
        #region Get MainApplication Variable
        /// <summary>
        /// Method toi get access to ClientApplication Instance
        /// </summary>
        /// <returns>ClientApplication</returns>
        public static ClientApplication GetClientApp()
        {
            return Singleton<ClientApplication>.Instance;
        }
        /// <summary>
        /// Method to get accessto MainApplication instance
        /// </summary>
        /// <returns></returns>
        public static MainApplication GetMainApp()
        {
            return GetClientApp() as MainApplication;
        }
        #endregion 

        #region Get GameVersion String
        private static string _gameVersion = "";
        /// <summary>
        /// Method that returns a Game version extracted from Assembl-CSharp dll
        /// </summary>
        public static string GameVersion
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(_gameVersion)) return _gameVersion;
                var list = Constants.Instance.TargetAssembly.GetTypes()
                    .Where(type =>
                        type.Name.StartsWith("Class") &&
                        type.GetField("string_0", BindingFlags.NonPublic | BindingFlags.Static) != null &&
                        type.GetMethods().Length == 4 &&
                        type.GetProperties().Length == 0 &&
                        type.GetMethods(BindingFlags.NonPublic | BindingFlags.Static).Length == 0 &&
                        type.GetProperties(BindingFlags.NonPublic | BindingFlags.Static).Length == 0)
                    .ToList();
                if (list.Count > 0)
                    _gameVersion = list[0].GetField("string_0", BindingFlags.NonPublic | BindingFlags.Static)?.GetValue(null).ToString();
                return _gameVersion;
            }
        }
        #endregion

        #region PreloaderUI Instance
        /// <summary>
        /// Property to get access to PreloaderUI Instance
        /// </summary>
        public static EFT.UI.PreloaderUI PreloaderUI 
        {
            get 
            {
                return Singleton<EFT.UI.PreloaderUI>.Instance;
            }
        }

        #endregion

        #region Get BetaVersionText Variable
        static EFT.UI.LocalizedText localizedText;
        /// <summary>
        /// Access to UI LocalixedText of Betaversion text label
        /// </summary>
        internal static EFT.UI.LocalizedText BetaVersionLabel {
            get
            {
                if (localizedText == null && PreloaderUI != null)
                {
                    if (typeof(EFT.UI.PreloaderUI).GetField("_alphaVersionLabel", BindingFlags.NonPublic | BindingFlags.Instance) == null)
                        return null;
                    localizedText = typeof(EFT.UI.PreloaderUI)
                    .GetField("_alphaVersionLabel", BindingFlags.NonPublic | BindingFlags.Instance)
                    .GetValue(PreloaderUI) as EFT.UI.LocalizedText;
                }
                return localizedText;
            }
        }

        #endregion

        private static string _backendUrl;
        /// <summary>
        /// Method that returns the Backend Url (Example: https://127.0.0.1)
        /// </summary>
        public static string BackendUrl {
            get 
            {
                //return GClassXXX.Config.BackendUrl;
                if (_backendUrl == null)
                {
                    try
                    {
                        var ConfigInstance = Constants.Instance.TargetAssemblyTypes
                            .Where(type => type.GetField("DEFAULT_BACKEND_URL", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy) != null)
                            .FirstOrDefault().GetProperty("Config", BindingFlags.Static | BindingFlags.Public).GetValue(null);
                        _backendUrl = HarmonyLib.Traverse.Create(ConfigInstance).Field("BackendUrl").GetValue() as string;
                    }
                    catch (Exception e) {
                        Debug.LogError($"{e.Message} -> {e.StackTrace}");
                    }
                }
                if (_backendUrl == null)
                {
                    Debug.LogError("_backendUrl still is null");
                    _backendUrl = "https://127.0.0.1";
                }
                return _backendUrl;
            }
        }

    }
}
