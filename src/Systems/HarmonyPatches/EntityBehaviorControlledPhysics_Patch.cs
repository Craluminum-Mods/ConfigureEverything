using System.Reflection;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.GameContent;

namespace ConfigureEverything.HarmonyPatches;

public static class EntityBehaviorControlledPhysics_Patch
{
    public static ConstructorInfo TargetMethod()
    {
        return typeof(EntityBehaviorControlledPhysics).GetConstructor(new[] { typeof(Entity) });
    }

    public static MethodInfo GetPostfix() => typeof(EntityBehaviorControlledPhysics_Patch).GetMethod(nameof(Postfix));

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