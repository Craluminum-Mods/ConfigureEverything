using HarmonyLib;
using System.Text;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.Util;

namespace ConfigureEverything.HarmonyPatches;

public static class CollectibleObject_Info_Patch
{
    [HarmonyPatch(typeof(CollectibleObject), nameof(CollectibleObject.GetHeldItemInfo))]
    public static void Postfix(CollectibleObject __instance, ItemSlot inSlot, StringBuilder dsc, IWorldAccessor world)
    {
        EntityProperties entityType = world.GetEntityType(inSlot.Itemstack.Collectible.Code);
        if (entityType != null && entityType.IsBoat() && HarmonyPatches.ConfigSwimmingSpeed.SpeedMultiplier.ContainsKey(entityType.Code.ToString()))
        {
            dsc.AppendLineOnce();
            dsc.Append(Lang.Get("walk-multiplier") + HarmonyPatches.ConfigSwimmingSpeed.SpeedMultiplier.GetValueSafe(entityType.Code.ToString()));
        }
    }
}