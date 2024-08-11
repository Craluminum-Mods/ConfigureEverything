using System.Reflection;
using HarmonyLib;
using Vintagestory.GameContent;

namespace ConfigureEverything.HarmonyPatches;

public static class EntityBoat_SpeedMultiplier_Patch
{
    public static MethodBase TargetMethod()
    {
        return AccessTools.PropertyGetter(typeof(EntityBoat), nameof(EntityBoat.SpeedMultiplier));
    }

    public static MethodInfo GetPostfix() => typeof(EntityBoat_SpeedMultiplier_Patch).GetMethod(nameof(Postfix));

    [HarmonyPatch(typeof(EntityBoat), nameof(EntityBoat.SpeedMultiplier), MethodType.Getter)]
    public static void Postfix(ref float __result, EntityBoat __instance)
    {
        if (HarmonyPatches.ConfigSwimmingSpeed.SpeedMultiplier.ContainsKey(__instance.Code.ToString()))
        {
            __result = HarmonyPatches.ConfigSwimmingSpeed.SpeedMultiplier.GetValueSafe(__instance.Code.ToString());
        }
    }
}
