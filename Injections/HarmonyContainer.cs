using HarmonyLib;
using System.Reflection;

namespace KitchenModName.Injections
{
    internal class HarmonyContainer : BaseModContainer<HarmonyContainer.HarmonyArgs>
    {
        public override string DependencyName => "Harmony";
        public object HarmonyInstance;

        public class HarmonyArgs : IInitArgs
        {
            public string ID;
        }

        public HarmonyContainer(string id) : base(
            new HarmonyArgs()
            {
                ID = id
            })
        {
        }

        protected override void Init(HarmonyArgs args)
        {
            HarmonyInstance = new Harmony(args.ID);
        }

        public void PatchAll(Assembly assembly)
        {
            ((Harmony)HarmonyInstance).PatchAll(assembly);
        }
    }
}
