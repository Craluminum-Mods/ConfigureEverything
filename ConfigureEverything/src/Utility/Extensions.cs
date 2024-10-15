using Newtonsoft.Json.Linq;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Datastructures;
using Vintagestory.API.Util;
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

    public static AssetLocation GetCompactCode(this AssetLocation location)
    {
        if (location.FirstCodePart() == location.SecondCodePart())
        {
            return location;
        }
        return location.CopyWithPath(location.FirstCodePart() + "-*");
    }

    public static bool WildCardMatchExt(this CollectibleObject obj, AssetLocation location)
    {
        return obj.WildCardMatch(location);
    }

    public static bool WildCardMatchExt(this CollectibleObject obj, string location)
    {
        return obj.WildCardMatch(AssetLocation.Create(location));
    }

    public static bool WildCardMatchExt(this EntityProperties obj, string location)
    {
        return WildcardUtil.Match(location, obj.Code.ToString());
    }
}