using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Datastructures;
using Vintagestory.GameContent;

namespace ConfigureEverything;

public static class Extensions
{
    public static void EnsureAttributesNotNull(this CollectibleObject obj)
    {
        obj.Attributes ??= new JsonObject(new JObject());
    }

    public static void EnsureAttributesNotNull(this EntityProperties obj)
    {
        obj.Attributes ??= new JsonObject(new JObject());
    }

    public static bool ContainsAny(this string value, params string[] values)
    {
        foreach (string one in values)
        {
            if (value.Contains(one))
            {
                return true;
            }
        }
        return false;
    }

    public static bool IsBoat(this EntityProperties entityType)
    {
        return entityType.Class.ContainsAny(nameof(EntityBoat), "boat", "raft");
    }

    public static string GetCompactBlockCode(this AssetLocation location, bool removeOnlyDomain = false)
    {
        string code = location.ToString();

        if (location.Domain == "game")
        {
            code = code.Replace("game:", "");
        }

        if (removeOnlyDomain)
        {
            return code;
        }

        if (code.Contains("-north")) code = code.Replace("-north", "-*");
        if (code.Contains("-east")) code = code.Replace("-east", "-*");
        if (code.Contains("-south")) code = code.Replace("-south", "-*");
        if (code.Contains("-west")) code = code.Replace("-west", "-*");
        if (code.Contains("-up")) code = code.Replace("-up", "-*");
        if (code.Contains("-down")) code = code.Replace("-down", "-*");

        if (code.Contains("-base")) code = code.Replace("-base", "-*");
        if (code.Contains("-top")) code = code.Replace("-top", "-*");
        if (code.Contains("-middle")) code = code.Replace("-middle", "-*");
        if (code.Contains("-bottom")) code = code.Replace("-bottom", "-*");

        if (code.Contains("-snow")) code = code.Replace("-snow", "-*");
        if (code.Contains("-free")) code = code.Replace("-free", "-*");

        // replace a number or a row of numbers with wildcard
        code = Regex.Replace(code, @"\d+(-\d+)*", "*");

        // replace a row of wildcards with wildcard
        code = Regex.Replace(code, @"(\*-(\*-?)+)", "*");
        return code;
    }
}