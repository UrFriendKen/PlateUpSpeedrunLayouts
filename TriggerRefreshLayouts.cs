using Kitchen;
using KitchenMods;

namespace KitchenSpeedrunLayouts
{
    public class TriggerRefreshLayouts : ItemInteractionSystem, IModSystem
    {
        protected override bool IsPossible(ref InteractionData data)
        {
            if (!Has<CRefreshLayoutsActivator>(data.Target))
                return false;
            return true;
        }

        protected override void Perform(ref InteractionData data)
        {
            Set(default(HandleLayoutRequests.SLayoutRequest));
        }
    }
}
