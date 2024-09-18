using HarmonyLib;
using System.Reflection;
using System.Text;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.Util;

namespace ConfigureEverything.HarmonyPatches;

public static class CollectibleObject_GetHeldItemInfo_Patch
{
    public static MethodBase TargetMethod()
    {
        return AccessTools.Method(typeof(CollectibleObject), nameof(CollectibleObject.GetHeldItemInfo), new[] { typeof(ItemSlot), typeof(StringBuilder), typeof(IWorldAccessor), typeof(bool) });
    }

    public static MethodInfo GetPostfix() => typeof(CollectibleObject_GetHeldItemInfo_Patch).GetMethod(nameof(Postfix));

    public static void Postfix(ItemSlot inSlot, StringBuilder dsc, IWorldAccessor world)
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