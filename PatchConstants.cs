//using System;
//using System.Linq;
//using System.Reflection;
//using Comfort.Common;
//using EFT;
//using FilesChecker;
//using UnityEngine;
////using ISession = GInterface115;

//namespace SIT.B.Tarkov.SP
//{
//    public static class PatchConstants
//    {
//        //public static BindingFlags PrivateFlags = SIT.Tarkov.Core.PatchConstants.PrivateFlags;

//        //public static Type[] EftTypes = SIT.Tarkov.Core.PatchConstants.EftTypes;
//        //public static Type[] FilesCheckerTypes = SIT.Tarkov.Core.PatchConstants.FilesCheckerTypes;
//        //public static Type LocalGameType = SIT.Tarkov.Core.PatchConstants.LocalGameType;
//        //public static Type ExfilPointManagerType = SIT.Tarkov.Core.PatchConstants.ExfilPointManagerType;
//        //public static Type BackendInterfaceType = SIT.Tarkov.Core.PatchConstants.BackendInterfaceType;
//        //public static Type SessionInterfaceType = SIT.Tarkov.Core.PatchConstants.SessionInterfaceType;
//        //public static Type BotDataInterfaceType { get; private set; }
//        //public static MethodInfo BotDataInterfaceType_ChooseProfileMethod { get; private set; }
//        ////public static MethodInfo BotDataInterfaceType_PrepareToLoadBackendMethod { get; }
//        //public static Type BotDifficultyHandlerType { get; private set; }
//        //public static Type PoolManagerType { get; private set; }
//        ////public static Type JobPriorityType { get; }

//        ////private static ISession _backEndSession;
//        ////public static ISession BackEndSession
//        ////{
//        ////    get
//        ////    {
//        ////        if (_backEndSession == null)
//        ////        {
//        ////            _backEndSession = Singleton<ClientApplication>.Instance.GetClientBackEndSession();
//        ////        }

//        ////        return _backEndSession;
//        ////    }
//        ////}

//        //public static string BackendUrl = SIT.Tarkov.Core.PatchConstants.GetBackendUrl();

//        ////private static string _backendUrl;
//        /////// <summary>
//        /////// Method that returns the Backend Url (Example: https://127.0.0.1)
//        /////// </summary>
//        ////public static string BackendUrl
//        ////{
//        ////    get
//        ////    {
//        ////        //return GClassXXX.Config.BackendUrl;
//        ////        if (_backendUrl == null)
//        ////        {
//        ////            try
//        ////            {
//        ////                var ConfigInstance = PatchConstants.EftTypes
//        ////                    .Where(type => type.GetField("DEFAULT_BACKEND_URL", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy) != null)
//        ////                    .FirstOrDefault().GetProperty("Config", BindingFlags.Static | BindingFlags.Public).GetValue(null);
//        ////                _backendUrl = HarmonyLib.Traverse.Create(ConfigInstance).Field("BackendUrl").GetValue() as string;
//        ////            }
//        ////            catch (Exception e)
//        ////            {
//        ////                Debug.LogError($"{e.Message} -> {e.StackTrace}");
//        ////            }
//        ////        }
//        ////        if (_backendUrl == null)
//        ////        {
//        ////            Debug.LogError("_backendUrl still is null");
//        ////            _backendUrl = "https://127.0.0.1";
//        ////        }
//        ////        return _backendUrl;
//        ////    }
//        ////}

//        ////#region Get MainApplication Variable
//        /////// <summary>
//        /////// Method toi get access to ClientApplication Instance
//        /////// </summary>
//        /////// <returns>ClientApplication</returns>
//        ////public static ClientApplication GetClientApp()
//        ////{
//        ////    return Singleton<ClientApplication>.Instance;
//        ////}
//        /////// <summary>
//        /////// Method to get accessto MainApplication instance
//        /////// </summary>
//        /////// <returns></returns>
//        ////public static MainApplication GetMainApp()
//        ////{
//        ////    return GetClientApp() as MainApplication;
//        ////}
//        ////#endregion

//        ////static PatchConstants()
//        ////{
//        ////    var patchConstantsLogger = BepInEx.Logging.Logger.CreateLogSource("SIT.B.Tarkov.SP.PatchConstants");
//        ////    patchConstantsLogger.LogInfo("Hiya: PatchConstants in SIT.B.Tarkov.SP");
//        ////    //_ = nameof(ISession.GetPhpSessionId);

//        ////    //PrivateFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;
//        ////    //EftTypes = typeof(MainApplication).Assembly.GetTypes();
//        ////    FilesCheckerTypes = typeof(ICheckResult).Assembly.GetTypes();
//        ////    LocalGameType = EftTypes.Single(x => x.Name == "LocalGame");
//        ////    ExfilPointManagerType = EftTypes.Single(x => x.GetMethod("InitAllExfiltrationPoints") != null);
//        ////    BackendInterfaceType = EftTypes.Single(x => x.GetMethods().Select(y => y.Name).Contains("CreateClientSession") && x.IsInterface);
//        ////    SessionInterfaceType = EftTypes.Single(x => x.GetMethods().Select(y => y.Name).Contains("GetPhpSessionId") && x.IsInterface);


//        ////    patchConstantsLogger.LogInfo("Finding BotDataInterfaceType");
//        ////    BotDataInterfaceType = EftTypes.Single(x => x.IsInterface && x.GetMethods().Any(x => x.Name == "ChooseProfile"));
//        ////    foreach(var method in BotDataInterfaceType.GetMethods())
//        ////    {
//        ////        patchConstantsLogger.LogInfo(method.Name);
//        ////    }
//        ////    if (BotDataInterfaceType_ChooseProfileMethod != null)
//        ////    {
//        ////        patchConstantsLogger.LogInfo("Finding BotDataInterfaceType.ChooseProfile");
//        ////        BotDataInterfaceType_ChooseProfileMethod = BotDataInterfaceType.GetMethods().FirstOrDefault(x => x.Name == "ChooseProfile");
//        ////        //patchConstantsLogger.LogInfo("Finding BotDataInterfaceType.PrepareToLoadBackend");
//        ////        //BotDataInterfaceType_PrepareToLoadBackendMethod = BotDataInterfaceType.GetMethods().FirstOrDefault(x => x.Name == "PrepareToLoadBackend");
//        ////    }
//        ////    patchConstantsLogger.LogInfo("Finding BotDifficultyHandlerType");
//        ////    BotDifficultyHandlerType = EftTypes.First(type => type.Name.StartsWith("GClass") && type.GetMethod("CheckOnExcude", BindingFlags.Public | BindingFlags.Static) != null);
//        ////    if (BotDifficultyHandlerType != null)
//        ////        patchConstantsLogger.LogInfo("Found BotDifficultyHandlerType");

//        ////    patchConstantsLogger.LogInfo("Finding PoolManagerType");
//        ////    PoolManagerType = EftTypes.Single(x => x.IsClass && x.GetMethods(BindingFlags.Public | BindingFlags.Instance).Any(x => x.Name == "LoadBundlesAndCreatePools") );
//        ////    if (PoolManagerType != null)
//        ////        patchConstantsLogger.LogInfo("Found PoolManagerType");

//        ////    //patchConstantsLogger.LogInfo("Finding JobPriorityType");
//        ////    //JobPriorityType = EftTypes.Single(x => x.IsClass && x.GetProperties(BindingFlags.Public | BindingFlags.Static).Any(x => x.Name == "General"));
//        ////    //if (JobPriorityType != null)
//        ////    //    patchConstantsLogger.LogInfo("Found JobPriorityType");
//        ////}
//    }
//}
