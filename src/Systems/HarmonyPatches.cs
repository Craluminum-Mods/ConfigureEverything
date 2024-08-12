using ConfigureEverything.Configuration;
using ConfigureEverything.Configuration.ConfigClimbingSpeed;
using ConfigureEverything.Configuration.ConfigSwimmingSpeed;
using HarmonyLib;
using Vintagestory.API.Common;

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
            ConfigSwimmingSpeed = ModConfig.ReadConfig<ConfigSwimmingSpeed>(api, $"ConfigureEverything/{api.Side}/SwimmingSpeed.json");

            if (ConfigClimbingSpeed?.Enabled == true)
            {
                HarmonyInstance.Patch(original: EntityBehaviorControlledPhysics_Patch.TargetMethod(), postfix: EntityBehaviorControlledPhysics_Patch.GetPostfix());
            }
            if (ConfigSwimmingSpeed?.Enabled == true)
            {
                HarmonyInstance.Patch(original: EntityBoat_SpeedMultiplier_Patch.TargetMethod(), postfix: EntityBoat_SpeedMultiplier_Patch.GetPostfix());
                HarmonyInstance.Patch(original: Entity_GetInfoText_Patch.TargetMethod(), postfix: Entity_GetInfoText_Patch.GetPostfix());
                HarmonyInstance.Patch(original: CollectibleObject_GetHeldItemInfo_Patch.TargetMethod(), postfix: CollectibleObject_GetHeldItemInfo_Patch.GetPostfix());
            }
        }
    }

    private void UnpatchAll()
    {
        if (ConfigClimbingSpeed?.Enabled == true)
        {
            HarmonyInstance.Unpatch(original: EntityBehaviorControlledPhysics_Patch.TargetMethod(), HarmonyPatchType.All, HarmonyID);
        }
        if (ConfigSwimmingSpeed?.Enabled == true)
        {
            HarmonyInstance.Unpatch(original: EntityBoat_SpeedMultiplier_Patch.TargetMethod(), HarmonyPatchType.All, HarmonyID);
            HarmonyInstance.Unpatch(original: Entity_GetInfoText_Patch.TargetMethod(), HarmonyPatchType.All, HarmonyID);
            HarmonyInstance.Unpatch(original: CollectibleObject_GetHeldItemInfo_Patch.TargetMethod(), HarmonyPatchType.All, HarmonyID);
        }
    }
}