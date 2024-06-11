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
        if (inSlot.Empty || inSlot?.Itemstack == null)
        {
            return;
        }
        try
        {
            EntityProperties entityType = world.GetEntityType(inSlot.Itemstack.Collectible.Code);
            if (entityType != null && entityType.IsBoat() && HarmonyPatches.ConfigSwimmingSpeed.SpeedMultiplier.TryGetValue(entityType.Code.ToString(), out float speed))
            {
                dsc.AppendLineOnce();
                dsc.Append(Lang.Get("walk-multiplier") + speed);
            }
        }
        catch (System.Exception e)
        {
            world.Logger.Error("[Configure Everything] Prevented crash:");
            world.Logger.Error(e);
        }
    }
}