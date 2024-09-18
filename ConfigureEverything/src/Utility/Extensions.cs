using Newtonsoft.Json.Linq;
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

    public static CollectibleObject GetCollectible(this ICoreAPI api, string key)
    {
        return (CollectibleObject)api.World.GetBlock(new AssetLocation(key)) ?? api.World.GetItem(new AssetLocation(key));
    }
}