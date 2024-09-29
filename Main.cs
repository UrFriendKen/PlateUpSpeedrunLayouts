using Kitchen;
using KitchenData;
using KitchenSpeedrunLayouts.Utils;
using PreferenceSystem;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace KitchenSpeedrunLayouts
{
    public class Main : BaseMain
    {
        public const string MOD_GUID = $"IcedMilo.PlateUp.{MOD_NAME}";
        public const string MOD_NAME = "Speedrun Layouts";
        public const string MOD_VERSION = "0.1.2";

        internal static readonly int RefreshLayoutsButtonID = HashUtils.GetID($"{MOD_GUID}:RefreshLayoutsButton");

        static GameObject _prefabContainer;

        internal static PreferenceSystemManager PrefManager;

        internal const string SELECTED_LAYOUT_PROFILE_ID = "selectedLayoutProfile";

        public Main() : base(MOD_GUID, MOD_NAME, MOD_VERSION, Assembly.GetExecutingAssembly())
        {
        }

        public override void OnPostActivate(KitchenMods.Mod mod)
        {
            PrefManager = new PreferenceSystemManager(MOD_GUID, MOD_NAME);
            PrefManager
                .AddLabel("Layout Profile")
                .AddOption<int>(
                    SELECTED_LAYOUT_PROFILE_ID,
                    0,
                    LayoutProfileOptions.SelectableLayoutProfiles.Keys.ToArray(),
                    LayoutProfileOptions.SelectableLayoutProfiles.Values.ToArray())
                .AddSpacer()
                .AddSpacer();

            PrefManager.RegisterMenu(PreferenceSystemManager.MenuType.PauseMenu);

            Appliance refreshLayoutButton = SerializedScriptableObject.CreateInstance<Appliance>();
            refreshLayoutButton.name = "Refresh Layouts Button";
            refreshLayoutButton.ID = RefreshLayoutsButtonID;
            refreshLayoutButton.Info = new LocalisationObject<ApplianceInfo>();

            ApplianceInfo refreshLayoutButtonEnglishApplianceInfo = SerializedScriptableObject.CreateInstance<ApplianceInfo>();
            refreshLayoutButtonEnglishApplianceInfo.Name = "Refresh Layout Button";
            refreshLayoutButtonEnglishApplianceInfo.Description = "Why are you looking at this???";
            refreshLayoutButtonEnglishApplianceInfo.Sections = new List<Appliance.Section>();
            refreshLayoutButtonEnglishApplianceInfo.Tags = new List<string>();

            refreshLayoutButton.Info.Add(Locale.English, refreshLayoutButtonEnglishApplianceInfo);
            AddGameDataObject(refreshLayoutButton);
        }

        private void RegisterCustomRestaurantSettings()
        {
            if (GameData.Main.TryGet(507410699, out RestaurantSetting lakeSetting))
                Registry.GrantCustomSetting(lakeSetting);

            if (GameData.Main.TryGet(-851159532, out RestaurantSetting northPoleSetting) &&
                GameData.Main.TryGet(-791067106, out LayoutProfile northPoleLayout))
            {
                Registry.GrantCustomSetting(northPoleSetting);
                Registry.AddSettingLayout(northPoleSetting, northPoleLayout);
            }
        }

        public override void PreInject()
        {
            RegisterCustomRestaurantSettings();

            if (!_prefabContainer)
            {
                _prefabContainer = new GameObject("Speedrun Layouts Prefabs");
                _prefabContainer.SetActive(false);
                _prefabContainer.hideFlags = HideFlags.HideAndDontSave;
            }

            if (GameData.Main.TryGet(RefreshLayoutsButtonID, out Appliance refreshLayoutsButtonAppliance, warn_if_fail: true) &&
                GameData.Main.TryGet(-1425710426, out Appliance workshopCraftButton, warn_if_fail: true))
            {
                GameObject prefab = GameObject.Instantiate(workshopCraftButton.Prefab);
                prefab.name = "Refresh Layouts Button";
                GameObject bezierCurve = prefab.GetChild("WorkshopActivator/BezierCurve");
                GameObject.Destroy(bezierCurve);

                MeshRenderer buttonRenderer = prefab.GetChild("WorkshopActivator/Button")?.GetComponent<MeshRenderer>();
                Material buttonReadyMaterial = Resources.FindObjectsOfTypeAll<Material>().Where(material => material.name.StartsWith("Button Ready")).FirstOrDefault();
                if (buttonRenderer != null && buttonRenderer != null)
                    buttonRenderer.material = buttonReadyMaterial;
                prefab.hideFlags = HideFlags.HideAndDontSave;
                prefab.transform.SetParent(_prefabContainer.transform);
                prefab.transform.Reset();

                refreshLayoutsButtonAppliance.Prefab = prefab;
                Main.LogInfo("Add Component");
                refreshLayoutsButtonAppliance.Properties.Add(default(CRefreshLayoutsActivator));
            }
        }
        #region Logging
        public static void LogInfo(string _log) { Debug.Log($"[{MOD_NAME}] " + _log); }
        public static void LogWarning(string _log) { Debug.LogWarning($"[{MOD_NAME}] " + _log); }
        public static void LogError(string _log) { Debug.LogError($"[{MOD_NAME}] " + _log); }
        public static void LogInfo(object _log) { LogInfo(_log.ToString()); }
        public static void LogWarning(object _log) { LogWarning(_log.ToString()); }
        public static void LogError(object _log) { LogError(_log.ToString()); }
        #endregion
    }
}
