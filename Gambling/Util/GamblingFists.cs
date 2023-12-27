using HarmonyLib;
using UnityEngine;

namespace Gambling.Util;

public class GamblingFists : GrabbableObject
{
    private static Item theseFists;

    public static void addItem()
    {
        LC_API.GameInterfaceAPI.Features.Item item = new LC_API.GameInterfaceAPI.Features.Item
        {
            Name = "Left Right Goodnight.",
            GrabbableObject =
            {
                itemProperties = getItem(),
                grabbable = false,
                scrapValue = 0
            }
        };
    }

    public static Item getItem()
    {
        if (theseFists == null)
        {
            theseFists = (Item)ScriptableObject.CreateInstance(typeof(Item));
            theseFists.spawnPrefab = GamblingPlugin.Instance.Bundle.GetAsset<GameObject>("Fists");
            theseFists.canBeGrabbedBeforeGameStart = false;
            theseFists.allowDroppingAheadOfPlayer = false;
            theseFists.itemIcon = GamblingPlugin.Instance.Bundle.GetAsset<Sprite>("FistIcon");
            theseFists.weight = 0;
            theseFists.itemSpawnsOnGround = false;
            theseFists.name = "Left Right Good Night.";
            theseFists.itemName = "Left Right Good Night.";
            theseFists.creditsWorth = 0;
            theseFists.toolTips = new[] { "CATCH THESE HANDS!" };
            theseFists.twoHanded = true;
            theseFists.holdButtonUse = true;
        }

        return theseFists;
    }
}