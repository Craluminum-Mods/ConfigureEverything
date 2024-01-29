using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using Vintagestory.GameContent;

namespace ConfigureEverything.HarmonyPatches;

public static class ConrolledPhysics_Collision_Patch
{
    [HarmonyPatch(typeof(EntityBehaviorControlledPhysics), nameof(EntityBehaviorControlledPhysics.DisplaceWithBlockCollision))]
    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        double descendingSpeed = HarmonyPatches.ConfigClimbingSpeed.DescendingSpeed;
        double ascendingSpeed = HarmonyPatches.ConfigClimbingSpeed.AscendingSpeed;

        bool found = false;
        List<CodeInstruction> codes = new(instructions);

        for (int i = 0; i < codes.Count; i++)
        {
            if (!found && codes[i].Is(OpCodes.Ldc_R8, -0.07))
            {
                codes[i].operand = -descendingSpeed;
                yield return codes[i];
                continue;
            }
            if (!found && codes[i].Is(OpCodes.Ldc_R8, 0.07))
            {
                codes[i].operand = descendingSpeed;
                yield return codes[i];
                continue;
            }
            if (!found && codes[i].Is(OpCodes.Ldc_R8, 0.035))
            {
                codes[i].operand = ascendingSpeed;
                yield return codes[i];
                found = true;
                continue;
            }
            yield return codes[i];
        }
    }
}