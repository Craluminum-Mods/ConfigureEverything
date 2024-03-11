using System;
using HarmonyLib;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.GameContent;

namespace ConfigureEverything.HarmonyPatches;

public static class EntityBehaviorControlledPhysics_Patch
{
    [HarmonyPatch(typeof(EntityBehaviorControlledPhysics), MethodType.Constructor, new Type[] { typeof(Entity) })]
    public static void Postfix(EntityBehaviorControlledPhysics __instance, Entity entity)
    {
        if (entity is not EntityPlayer)
        {
            return;
        }

        // reversed because why not
        float climbUpSpeed = HarmonyPatches.ConfigClimbingSpeed.DownSpeed;
        float climbDownSpeed = HarmonyPatches.ConfigClimbingSpeed.UpSpeed;
        __instance.climbUpSpeed = climbUpSpeed;
        __instance.climbDownSpeed = climbDownSpeed;
    }
}