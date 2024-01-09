using GameNetcodeStuff;
using HarmonyLib;
using InstantLoot.Configuration;
using InstantLoot.HelperFunctions;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

namespace InstantLoot
{
	[HarmonyPatch]
	internal static class Keybinds
	{
		public static PlayerControllerB localPlayerController;

		private static InputAction InstantLootGrabLoot;

		[HarmonyPatch(typeof(PlayerControllerB), "OnDisable")]
		[HarmonyPostfix]
		public static void OnDisable(PlayerControllerB __instance)
		{
			if (InstantLootGrabLoot != null && !((Object)(object)__instance != (Object)(object)localPlayerController))
			{
				InstantLootGrabLoot.performed -= OnInstantLootCalled;
				InstantLootGrabLoot.Disable();
			}
		}

		[HarmonyPatch(typeof(PlayerControllerB), "OnEnable")]
		[HarmonyPostfix]
		public static void OnEnable(PlayerControllerB __instance)
		{
			if ((Object)(object)__instance == (Object)(object)localPlayerController)
			{
				SubscribeToEvents();
			}
		}

		[HarmonyPatch(typeof(PlayerControllerB), "ConnectClientToPlayerObject")]
		[HarmonyPostfix]
		public static void OnLocalPlayerConnect(PlayerControllerB __instance)
		{
			localPlayerController = __instance;
			InstantLootGrabLoot = new InputAction(null, 0, ConfigSettings.InstantLootInputAction.Key.Value, "Press", null, null);

			if (localPlayerController.gameObject.activeSelf)
			{
				SubscribeToEvents();
			}
		}

		private static void OnInstantLootCalled(CallbackContext context)
		{
			if ((Object)(object)localPlayerController == null || !localPlayerController.isPlayerControlled || localPlayerController.inTerminalMenu || localPlayerController.IsServer && !localPlayerController.isHostPlayerObject)
			{
				return;
			}
			InstantLoot.Log("Get all loot");
			InstantLootFunctions.RetrieveAllLoot();
		}

		private static void SubscribeToEvents()
		{
			if (InstantLootGrabLoot != null)
			{
				InstantLootGrabLoot.Enable();

				InstantLootGrabLoot.performed += OnInstantLootCalled;
			}
		}
	}
}