using Kitchen;
using KitchenMods;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace KitchenSpeedrunLayouts
{
    [UpdateAfter(typeof(CreateOffice))]
    public class CreateRefreshLayoutsButton : FranchiseFirstFrameSystem, IModSystem
    {
        protected override void OnUpdate()
        {
            CreateButton(LobbyPositionAnchors.Office + new Vector3(-4f, 0f, -2f));
        }

        private void CreateButton(Vector3 location)
        {
            Main.LogInfo("Create Button");
            EntityManager entityManager = base.EntityManager;
            Entity entity = entityManager.CreateEntity(typeof(CCreateAppliance), typeof(CPosition), typeof(CItemHolder), typeof(CDoNotPersist));
            entityManager.SetComponentData(entity, new CCreateAppliance
            {
                ID = Main.RefreshLayoutsButtonID
            });
            entityManager.SetComponentData(entity, new CPosition(location, quaternion.LookRotation(new float3(1f, 0f, 0f), new float3(0f, 1f, 0f))));
        }
    }
}
