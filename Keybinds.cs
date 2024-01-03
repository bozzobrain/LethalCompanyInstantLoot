using GameNetcodeStuff;
using HarmonyLib;
using System.Collections.Generic;
using System.Text;
using static UnityEngine.InputSystem.InputAction;
using Unity.Netcode;
using UnityEngine.InputSystem;
using UnityEngine;

namespace InstantLoot
{
    [HarmonyPatch]
    internal static class Keybinds
    {
        public static PlayerControllerB localPlayerController;

        private static InputAction instantLootGrabLoot;

		[HarmonyPatch(typeof(PlayerControllerB), "ConnectClientToPlayerObject")]
        [HarmonyPostfix]
        public static void OnLocalPlayerConnect(PlayerControllerB __instance)
        {
            localPlayerController = __instance;
            instantLootGrabLoot = new InputAction(null, 0, ConfigSettings.activateInstantLootKey.Value, "Press", null, null);
			if (localPlayerController.gameObject.activeSelf)
            {
                SubscribeToEvents();
            }
        }
         
        private static void SubscribeToEvents()
        {
            if (instantLootGrabLoot != null)
            {
                instantLootGrabLoot.Enable();

				instantLootGrabLoot.performed += OnInstantLootCalled;

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

        [HarmonyPatch(typeof(PlayerControllerB), "OnDisable")]
        [HarmonyPostfix]
        public static void OnDisable(PlayerControllerB __instance)
		{
			if (instantLootGrabLoot != null && !((Object)(object)__instance != (Object)(object)localPlayerController))
			{
				instantLootGrabLoot.performed -= OnInstantLootCalled;
				instantLootGrabLoot.Disable();
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
	
	}

}
