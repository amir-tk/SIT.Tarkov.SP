using System;
using System.Linq;
using System.Reflection;
using Comfort.Common;
using EFT;
using FilesChecker;
using UnityEngine;
using ISession = GInterface115;

namespace SIT.Tarkov.Core
{
    public static class PatchConstants
    {
        public static BindingFlags PrivateFlags { get; private set; }
        public static Type[] EftTypes { get; private set; }
        public static Type[] FilesCheckerTypes { get; private set; }
        public static Type LocalGameType { get; private set; }
        public static Type ExfilPointManagerType { get; private set; }
        public static Type BackendInterfaceType { get; private set; }
        public static Type SessionInterfaceType { get; private set; }

        private static ISession _backEndSession;
        public static ISession BackEndSession
        {
            get
            {
                if (_backEndSession == null)
                {
                    _backEndSession = Singleton<ClientApplication>.Instance.GetClientBackEndSession();
                }

                return _backEndSession;
            }
        }

        private static string _backendUrl;
        /// <summary>
        /// Method that returns the Backend Url (Example: https://127.0.0.1)
        /// </summary>
        public static string BackendUrl
        {
            get
            {
                //return GClassXXX.Config.BackendUrl;
                if (_backendUrl == null)
                {
                    try
                    {
                        var ConfigInstance = PatchConstants.EftTypes
                            .Where(type => type.GetField("DEFAULT_BACKEND_URL", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy) != null)
                            .FirstOrDefault().GetProperty("Config", BindingFlags.Static | BindingFlags.Public).GetValue(null);
                        _backendUrl = HarmonyLib.Traverse.Create(ConfigInstance).Field("BackendUrl").GetValue() as string;
                    }
                    catch (Exception e)
                    {
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

        static PatchConstants()
        {
            _ = nameof(ISession.GetPhpSessionId);

            PrivateFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;
            EftTypes = typeof(AbstractGame).Assembly.GetTypes();
            FilesCheckerTypes = typeof(ICheckResult).Assembly.GetTypes();
            LocalGameType = EftTypes.Single(x => x.Name == "LocalGame");
            ExfilPointManagerType = EftTypes.Single(x => x.GetMethod("InitAllExfiltrationPoints") != null);
            BackendInterfaceType = EftTypes.Single(x => x.GetMethods().Select(y => y.Name).Contains("CreateClientSession") && x.IsInterface);
            SessionInterfaceType = EftTypes.Single(x => x.GetMethods().Select(y => y.Name).Contains("GetPhpSessionId") && x.IsInterface);
        }
    }
}
