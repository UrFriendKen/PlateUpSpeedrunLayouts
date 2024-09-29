using HarmonyLib;
using Kitchen;
using KitchenSpeedrunLayouts.Utils;
using KitchenData;
using KitchenEditor;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KitchenSpeedrunLayouts.Patches
{
    [HarmonyPatch]
    internal static class GameDataConstructor_Patch
    {
        private const int MAX_ID_CONFLICT_REATTEMPTS = 1000;

        static List<(GameDataObject gdo, bool isNonPersistent)> GDOsToRegister = new List<(GameDataObject, bool)>();

        static Dictionary<string, Dictionary<Locale, string>> TextLocalisationsToRegister = new Dictionary<string, Dictionary<Locale, string>>();
        static Dictionary<PopupType, Dictionary<Locale, PopupDetails>> PopupTextLocalisationsToRegister = new Dictionary<PopupType, Dictionary<Locale, PopupDetails>>();
        static Dictionary<StartDayWarning, Dictionary<Locale, GenericLocalisationStruct>> StartDayWarningLocalisationsToRegister = new Dictionary<StartDayWarning, Dictionary<Locale, GenericLocalisationStruct>>();

        [HarmonyPatch(typeof(GameDataConstructor), "BuildGameData")]
        [HarmonyPrefix]
        [HarmonyPriority(int.MaxValue)]
        static void BuildGameData_Prefix(List<GameDataObject> ___GameDataObjects, Dictionary<int, GameDataObject> ___All, List<ProcessGraph> ___ProcessGraphs)
        {
            ResourceUtils.Init();

            GlobalLocalisation globalLocalisationGDO = (GlobalLocalisation)___GameDataObjects.Where(gdo => gdo is GlobalLocalisation).FirstOrDefault();
            RegisterTextLocalisations(globalLocalisationGDO);
            RegisterPopupTextLocalisations(globalLocalisationGDO);

            RegisterCustomGDOs(___GameDataObjects, ___All);
        }

        private static void RegisterTextLocalisations(GlobalLocalisation globalLocalisation)
        {
            LocalisationObject<DictionaryInfo> globalLocalisationInfo = globalLocalisation?.LocalisationInfo;
            if (globalLocalisationInfo == null)
                return;
            foreach (string localisationKey in TextLocalisationsToRegister.Keys)
            {
                foreach (Locale locale in globalLocalisationInfo.GetLocales())
                {
                    DictionaryInfo dictionaryInfo = globalLocalisationInfo.Get(locale);
                    if (dictionaryInfo?.Text == null || dictionaryInfo.Text.ContainsKey(localisationKey))
                        continue;
                    if (!TextLocalisationsToRegister[localisationKey].TryGetValue(locale, out string buttonText))
                        continue;
                    dictionaryInfo.Text[localisationKey] = buttonText;
                }
            }
        }

        private static void RegisterPopupTextLocalisations(GlobalLocalisation globalLocalisation)
        {
            LocalisationObject<PopupText> popupTextLocalisationInfo = globalLocalisation?.PopupTextLocalisation?.LocalisationInfo;
            if (popupTextLocalisationInfo == null)
                return;
            foreach (PopupType localisationKey in PopupTextLocalisationsToRegister.Keys)
            {
                foreach (Locale locale in popupTextLocalisationInfo.GetLocales())
                {
                    PopupText popupText = popupTextLocalisationInfo.Get(locale);
                    if (popupText?.Text == null || popupText.Text.ContainsKey(localisationKey))
                        continue;
                    if (!PopupTextLocalisationsToRegister[localisationKey].TryGetValue(locale, out PopupDetails popupDetails))
                        continue;
                    popupText.Text[localisationKey] = popupDetails;
                }
            }
        }

        private static void RegisterStartDayWarningLocalisations(GlobalLocalisation globalLocalisation)
        {
            LocalisationObject<StartDayWarningInfo> startDayWarningLocalisationInfo = globalLocalisation?.StartDayWarningLocalisation?.LocalisationInfo;
            if (startDayWarningLocalisationInfo == null)
                return;
            foreach (StartDayWarning localisationKey in StartDayWarningLocalisationsToRegister.Keys)
            {
                foreach (Locale locale in startDayWarningLocalisationInfo.GetLocales())
                {
                    StartDayWarningInfo startDayWarningInfo = startDayWarningLocalisationInfo.Get(locale);
                    if (startDayWarningInfo?.Text == null || startDayWarningInfo.Text.ContainsKey(localisationKey))
                        continue;
                    if (!StartDayWarningLocalisationsToRegister[localisationKey].TryGetValue(locale, out GenericLocalisationStruct genericLocalisationStruct))
                        continue;
                    startDayWarningInfo.Text[localisationKey] = genericLocalisationStruct;
                }
            }
        }

        private static void RegisterCustomGDOs(List<GameDataObject> gameDataObjectsList, Dictionary<int, GameDataObject> allGameDataObjectsDict)
        {
            foreach (bool isNonPersistent in new bool[] { false, true })
            {
                foreach (var item in GDOsToRegister)
                {
                    if (item.isNonPersistent != isNonPersistent)
                        continue;
                    if (item.gdo.ID == 0)
                    {
                        Main.LogError($"Failed to register {item.gdo.GetType()}! ID cannot be 0.");
                        continue;
                    }

                    bool shouldRegister = true;
                    if (allGameDataObjectsDict.ContainsKey(item.gdo.ID))
                    {
                        if (item.isNonPersistent)
                        {
                            item.gdo.ID = Random.Range(int.MinValue, int.MaxValue);
                            for (int i = 0; i < MAX_ID_CONFLICT_REATTEMPTS; i++)
                            {
                                item.gdo.ID++;
                                if (!allGameDataObjectsDict.ContainsKey(item.gdo.ID))
                                    break;
                                if (i == MAX_ID_CONFLICT_REATTEMPTS - 1)
                                {
                                    shouldRegister = false;
                                    Main.LogError($"Failed to register {item.gdo.GetType()}! ID {item.gdo.ID - MAX_ID_CONFLICT_REATTEMPTS} to {item.gdo.ID} already in use.");
                                }
                            }
                        }
                        else
                        {
                            shouldRegister = false;
                            Main.LogError($"Failed to register {item.gdo.GetType()}! ID {item.gdo.ID} already in use.");
                        }
                    }

                    if (shouldRegister)
                    {
                        Main.LogWarning($"Added GDO {item.gdo.ID}");
                        gameDataObjectsList.Add(item.gdo);
                        allGameDataObjectsDict.Add(item.gdo.ID, item.gdo);
                    }
                }
            }
        }

        public static GameDataObject AddGameDataObject(GameDataObject gameDataObject)
        {
            GDOsToRegister.Add((gameDataObject, false));
            return gameDataObject;
        }

        public static T Add<T>() where T : GameDataObject, new()
        {
            T gdo = ScriptableObject.CreateInstance<T>();
            return (T)AddGameDataObject(gdo);
        }

        public static bool AddTextLocalisation(string localisationKey, Dictionary<Locale, string> localisations)
        {
            if (TextLocalisationsToRegister.ContainsKey(localisationKey))
                return false;
            TextLocalisationsToRegister.Add(localisationKey, localisations);
            return true;
        }

        public static bool AddPopupTextLocalisation(PopupType localisationKey, Dictionary<Locale, PopupDetails> localisations)
        {
            if (PopupTextLocalisationsToRegister.ContainsKey(localisationKey))
                return false;
            PopupTextLocalisationsToRegister.Add(localisationKey, localisations);
            return true;
        }

        public static bool AddStartDayWarningLocalisation(StartDayWarning localisationKey, Dictionary<Locale, GenericLocalisationStruct> localisations)
        {
            if (StartDayWarningLocalisationsToRegister.ContainsKey(localisationKey))
                return false;
            StartDayWarningLocalisationsToRegister.Add(localisationKey, localisations);
            return true;
        }
    }
}
