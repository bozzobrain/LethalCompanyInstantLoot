using GameNetcodeStuff;
using HarmonyLib;
using InstantLoot.EntityHelpers;
using InstantLoot.HelperFunctions;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using static InstantLoot.Networking.NetworkFunctions;
using Object = UnityEngine.Object;

namespace InstantLoot
{
	internal class InstantLootFunctions
	{
		/// <summary>
		/// Moves all loot to the ship if you are in it.
		/// Moves loot to the player if outside the ship.
		/// </summary>
		///
		internal static void RetrieveAllLoot()
		{
			HangarShipHelper hsh = new();
			if (Keybinds.localPlayerController.isInHangarShipRoom)
			{
				var allScrap = ScrapHelperFunctions.FindAllScrapOnMap();
				foreach (var obj in allScrap)
				{
					if (!obj.isInShipRoom)
					{
						hsh.MoveItemToShip(obj);
					}
				}
				LootOrganizingFunctions.OrganizeShipLoot();
			}
			else
			{
				var shipObjects = hsh.FindAllScrapShip();
				foreach (var obj in shipObjects)
				{
					Vector3 playerPosition = Keybinds.localPlayerController.gameplayCamera.transform.position;
					Vector3 targetPosition = new(playerPosition.x - 1f, playerPosition.y + 0.2f, playerPosition.z);

					NetworkingObjectManager.MakeObjectFallRpc(obj, targetPosition, true);
				}
			}
		}
	}
}