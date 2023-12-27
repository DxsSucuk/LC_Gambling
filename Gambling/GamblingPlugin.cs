using BepInEx;
using BepInEx.Logging;
using GameNetcodeStuff;
using HarmonyLib;
using Gambling.Util;
using LC_API.BundleAPI;
using LCSoundTool;
using UnityEngine;

namespace Gambling
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInDependency("LCSoundTool")]
    [BepInDependency("LC_API")]
    [BepInDependency("com.rune580.LethalCompanyInputUtils")]
    public class GamblingPlugin : BaseUnityPlugin
    {
        public readonly ManualLogSource Log = new ManualLogSource(" Always bet on Hakari");
        private readonly Harmony _harmony = new Harmony(PluginInfo.PLUGIN_GUID);
        internal static GamblingInput InputActionsInstance = new GamblingInput();

        public static GamblingPlugin Instance;

        public LoadedAssetBundle Bundle;
        public bool shouldDebug = true;
        
        public AudioClip gambit;

        void Awake()
        {
            if (Instance == null)
                Instance = this;

            //Bundle = BundleLoader.LoadAssetBundle("C:\\Users\\Admin\\AppData\\Roaming\\r2modmanPlus-local\\LethalCompany\\profiles\\Modding\\BepInEx\\plugins\\JJK\\bundle");
            
            BepInEx.Logging.Logger.Sources.Add(Log);

            // Plugin startup logic
            Log.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");

            gambit = SoundTool.GetAudioClip("Presti-Gambling", "gambit.mp3");

            Log.LogInfo("Loaded Gambit Audio.");
            
            _harmony.PatchAll(typeof(GamblingPlugin));
            _harmony.PatchAll(typeof(PlayerControllerPatch));
            _harmony.PatchAll(typeof(DamagePatch));
            Log.LogInfo("Finished patching.");
        }
        
        [HarmonyPatch(typeof(PlayerControllerB))]
        internal class PlayerControllerPatch
        {
            [HarmonyPatch("Awake")]
            [HarmonyPostfix]
            public static void GambitPatch(PlayerControllerB __instance)
            {
                __instance.gameObject.AddComponent<GamblingController>();
            }
        }
        
        [HarmonyPatch(typeof(EnemyAI))]
        internal class DamagePatch
        {
            [HarmonyPatch("PlayerIsTargetable")]
            [HarmonyPrefix]
            public static bool KillPatch(EnemyAI __instance, ref bool __result, PlayerControllerB playerScript)
            {
                if (!(__instance is FlowermanAI) && !(__instance is CentipedeAI)) return true;
                
                GamblingPlugin.Instance.Log.LogInfo("called x-x");
                playerScript.TryGetComponent(out GamblingController controller);

                if (controller is not null)
                {
                    GamblingPlugin.Instance.Log.LogInfo("called y-y");
                    if (controller.domainActive && controller.domainUser == playerScript.actualClientId)
                    {
                        GamblingPlugin.Instance.Log.LogInfo("called z-z");
                        __result = false;
                        return false;
                    }
                }

                return true;
            }
        }
    }
}