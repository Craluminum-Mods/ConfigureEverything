using ConfigureEverything.Configuration;
using ConfigureEverything.Configuration.ConfigClimbingSpeed;
using ConfigureEverything.Configuration.ConfigSwimmingSpeed;
using HarmonyLib;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.GameContent;

namespace ConfigureEverything.HarmonyPatches;

public class HarmonyPatches : ModSystem
{
    public string HarmonyID => Mod.Info.ModID;
    public Harmony HarmonyInstance => new(HarmonyID);

    public static ConfigClimbingSpeed ConfigClimbingSpeed { get; private set; }
    public static ConfigSwimmingSpeed ConfigSwimmingSpeed { get; private set; }

    public override void Dispose()
    {
        UnpatchAll();
    }

    public override void AssetsFinalize(ICoreAPI api)
    {
        PatchAll(api);
    }

    private void PatchAll(ICoreAPI api)
    {
        if (api.Side.IsServer())
        {
            ConfigClimbingSpeed = ModConfig.ReadConfig<ConfigClimbingSpeed>(api, $"ConfigureEverything/{api.Side}/ClimbingSpeed.json");
            if (ConfigClimbingSpeed?.Enabled == true)
            {
                HarmonyInstance.Patch(original: typeof(EntityBehaviorControlledPhysics).GetMethod(nameof(EntityBehaviorControlledPhysics.DisplaceWithBlockCollision)), transpiler: typeof(ConrolledPhysics_Collision_Patch).GetMethod(nameof(ConrolledPhysics_Collision_Patch.Transpiler)));
            }

            ConfigSwimmingSpeed = ModConfig.ReadConfig<ConfigSwimmingSpeed>(api, $"ConfigureEverything/{api.Side}/SwimmingSpeed.json");
            if (ConfigSwimmingSpeed?.Enabled == true)
            {
                HarmonyInstance.Patch(
                    original: AccessTools.PropertyGetter(typeof(EntityBoat), nameof(EntityBoat.SpeedMultiplier)),
                    postfix: typeof(EntityBoat_SpeedMultiplier_Patch).GetMethod(nameof(EntityBoat_SpeedMultiplier_Patch.Postfix)));

                HarmonyInstance.Patch(original: typeof(Entity).GetMethod(nameof(Entity.GetInfoText)), postfix: typeof(Entity_Info_Patch).GetMethod(nameof(Entity_Info_Patch.Postfix)));
                HarmonyInstance.Patch(original: typeof(CollectibleObject).GetMethod(nameof(CollectibleObject.GetHeldItemInfo)), postfix: typeof(CollectibleObject_Info_Patch).GetMethod(nameof(CollectibleObject_Info_Patch.Postfix)));
            }
        }
    }

    private void UnpatchAll()
    {
        if (ConfigClimbingSpeed?.Enabled == true)
        {
            HarmonyInstance.Unpatch(original: typeof(EntityBehaviorControlledPhysics).GetMethod(nameof(EntityBehaviorControlledPhysics.DisplaceWithBlockCollision)), HarmonyPatchType.All, HarmonyID);
        }
        if (ConfigSwimmingSpeed?.Enabled == true)
        {
            HarmonyInstance.Unpatch(original: AccessTools.PropertyGetter(typeof(EntityBoat), nameof(EntityBoat.SpeedMultiplier)), HarmonyPatchType.All, HarmonyID);
            HarmonyInstance.Unpatch(original: typeof(Entity).GetMethod(nameof(Entity.GetInfoText)), HarmonyPatchType.All, HarmonyID);
            HarmonyInstance.Unpatch(original: typeof(CollectibleObject).GetMethod(nameof(CollectibleObject.GetHeldItemInfo)), HarmonyPatchType.All, HarmonyID);
        }
    }
}