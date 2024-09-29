﻿using HarmonyLib;
using Kitchen;
using KitchenData;
using KitchenMods;
using KitchenSpeedrunLayouts.Patches;
using KitchenSpeedrunLayouts.Utils;
using System.Linq;
using System.Reflection;

namespace KitchenSpeedrunLayouts
{
    public abstract class BaseMain : IModInitializer
    {
        private readonly string ModGuid;
        private readonly string ModName;
        private readonly string ModVersion;

        Harmony _harmony;

        private bool _isPostActivating = false;

        public BaseMain(string modGuid, string modName, string modVersion, Assembly assembly)
        {
            ModGuid = modGuid;
            ModName = modName;
            ModVersion = modVersion;

            _harmony = new Harmony(modGuid);
            _harmony.PatchAll(assembly);
        }

        public void PostActivate(Mod mod)
        {
            _isPostActivating = true;
            Debug.LogWarning($"{ModGuid} v{ModVersion} in use!");
            OnPostActivate(mod);
            _isPostActivating = false;
        }

        public abstract void OnPostActivate(Mod mod);

        public virtual void PreInject()
        {
        }

        public virtual void PostInject()
        {
        }

        protected GameDataObject AddGameDataObject(GameDataObject gameDataObject)
        {
            if (!_isPostActivating)
            {
                Main.LogError("GameDataObjects can only be added while PostActivating!");
                return null;
            }
            return GameDataConstructor_Patch.AddGameDataObject(gameDataObject);
        }

        protected T AddGameDataObject<T>(string name) where T : GameDataObject, new()
        {
            if (!_isPostActivating)
            {
                Main.LogError("GameDataObjects can only be added while PostActivating!");
                return null;
            }
            T gdo = GameDataConstructor_Patch.Add<T>();
            gdo.ID = HashUtils.GetID($"{ModGuid}:{name}");
            gdo.name = $"{ModGuid} - {name}";
            return gdo;
        }

        protected bool AddTextLocalisation(string localisationKey, params (Locale locale, string localisation)[] localisations)
        {
            if (!_isPostActivating)
            {
                Main.LogError("Global localisation can only be added while PostActivating!");
                return false;
            }
            return GameDataConstructor_Patch.AddTextLocalisation(localisationKey, localisations.ToDictionary(
                localisationItem => localisationItem.locale,
                localisationItem => localisationItem.localisation));
        }

        protected bool AddPopupTextLocalisation(PopupType localisationKey, params (Locale locale, PopupDetails localisation)[] localisations)
        {
            if (!_isPostActivating)
            {
                Main.LogError("Popup text localisation can only be added while PostActivating!");
                return false;
            }
            return GameDataConstructor_Patch.AddPopupTextLocalisation(localisationKey, localisations.ToDictionary(
                localisationItem => localisationItem.locale,
                localisationItem => localisationItem.localisation));
        }

        protected bool AddStartDayWarningLocalisation(StartDayWarning localisationKey, params (Locale locale, GenericLocalisationStruct localisation)[] localisations)
        {
            if (!_isPostActivating)
            {
                Main.LogError("Start day warning localisation can only be added while PostActivating!");
                return false;
            }
            return GameDataConstructor_Patch.AddStartDayWarningLocalisation(localisationKey, localisations.ToDictionary(
                localisationItem => localisationItem.locale,
                localisationItem => localisationItem.localisation));
        }

        #region Logging
        private void LogInfo(string _log) { Debug.Log($"[{ModName}] " + _log); }
        private void LogWarning(string _log) { Debug.LogWarning($"[{ModName}] " + _log); }
        private void LogError(string _log) { Debug.LogError($"[{ModName}] " + _log); }
        private void LogInfo(object _log) { LogInfo(_log.ToString()); }
        private void LogWarning(object _log) { LogWarning(_log.ToString()); }
        private void LogError(object _log) { LogError(_log.ToString()); }
        #endregion
    }
}
