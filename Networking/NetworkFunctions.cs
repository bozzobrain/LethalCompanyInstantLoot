﻿using GameNetcodeStuff;
using HarmonyLib;
using Unity.Netcode;
using UnityEngine;

namespace InstantLoot.Networking
{
	internal class NetworkFunctions
	{
		public class NetworkingObjectManager : NetworkBehaviour
		{
			public static PlayerControllerB localPlayerController;

			public static NetworkingObjectManager GetNetworkingObjectManager()
			{
				GameObject terminal = GameObject.Find("/Environment/HangarShip/Terminal");
				if (terminal != null)
				{
					InstantLoot.Log($"Terminal found {terminal.name}");
					return terminal.GetComponentInChildren<NetworkingObjectManager>();
				}
				return null;
			}

			public static void MakeObjectFallRpc(GrabbableObject obj, Vector3 placementPosition, bool shipParent)
			{
				var pni = GetNetworkingObjectManager();

				if (pni != null)
				{
					InstantLoot.Log($"NetworkingObjectManager - Network behavior found {pni.name}");
					pni.RunClientRpc(obj.NetworkObject, placementPosition, shipParent);
				}
				else
				{
					InstantLoot.Log($"NetworkingObjectManager not found ");
				}
			}

			public static void NetworkManagerInit()
			{
				InstantLoot.Log("Registering named message");
				NetworkManager.Singleton.CustomMessagingManager.RegisterNamedMessageHandler("MakeObjectFall", (senderClientId, reader) =>
				{
					if (senderClientId != localPlayerController.playerClientId)
					{
						reader.ReadValueSafe(out NetworkObjectReference value, default);
						reader.ReadValueSafe(out Vector3 value3);
						reader.ReadValueSafe(out bool shipParent);
						if (value.TryGet(out var networkObject))
						{
							GrabbableObject component = networkObject.GetComponent<GrabbableObject>();

							GetNetworkingObjectManager().MakeObjectFall(component, value3, shipParent);
						}
					}
				});
			}

			[HarmonyPatch(typeof(PlayerControllerB), "ConnectClientToPlayerObject")]
			[HarmonyPostfix]
			public static void OnLocalPlayerConnect(PlayerControllerB __instance)
			{
				localPlayerController = __instance;
				if (localPlayerController.IsClient)
					NetworkManagerInit();
			}

			public void MakeObjectFall(GrabbableObject obj, Vector3 placementPosition, bool shipParent)
			{
				GameObject ship = GameObject.Find("/Environment/HangarShip");
				GameObject storageCloset = GameObject.Find("/Environment/HangarShip/StorageCloset");
				string debugLocation = string.Empty;
				Vector3 targetlocation = new();
				if (shipParent)
				{
					if (obj.gameObject.transform.GetParent() == null || obj.gameObject.transform.GetParent().name != "HangarShip")
					{
						obj.gameObject.transform.SetParent(ship.transform);
					}
					targetlocation = ship.transform.position;
					debugLocation = "ship";
				}
				else
				{
					if (obj.gameObject.transform.GetParent() == null || obj.gameObject.transform.GetParent().name != "StorageCloset")
					{
						obj.gameObject.transform.SetParent(storageCloset.transform);
					}
					targetlocation = storageCloset.transform.position;
					debugLocation = "storage";
				}
				InstantLoot.Log($"Request to make GrabbableObject {obj.name} fall to ground in {debugLocation} - {targetlocation.x},{targetlocation.y},{targetlocation.z}");
				obj.gameObject.transform.SetPositionAndRotation(placementPosition, obj.transform.rotation);
				obj.hasHitGround = false;
				obj.startFallingPosition = placementPosition;
				if (obj.transform.parent != null)
				{
					obj.startFallingPosition = obj.transform.parent.InverseTransformPoint(obj.startFallingPosition);
				}
				obj.FallToGround(false);
			}

			[ClientRpc]
			public void MakeObjectFallClientRpc(NetworkObjectReference obj, Vector3 placementPosition, bool shipParent)
			{
				NetworkManager networkManager = NetworkManager;
				if ((object)networkManager == null || !networkManager.IsListening)
				{
					return;
				}

				FastBufferWriter bufferWriter = new FastBufferWriter(256, Unity.Collections.Allocator.Temp);
				bufferWriter.WriteValueSafe(in obj, default);
				bufferWriter.WriteValueSafe(in placementPosition);
				bufferWriter.WriteValueSafe(shipParent);
				NetworkManager.Singleton.CustomMessagingManager.SendNamedMessageToAll("MakeObjectFall", bufferWriter, NetworkDelivery.Reliable);

				if (obj.TryGet(out var networkObject))
				{
					GrabbableObject component = networkObject.GetComponent<GrabbableObject>();
					if (!IsOwner)
					{
						MakeObjectFall(component, placementPosition, shipParent);
					}
				}
			}

			[ServerRpc]
			public void MakeObjectFallServerRpc(NetworkObjectReference obj, Vector3 placementPosition, bool shipParent)
			{
				NetworkManager networkManager = NetworkManager;
				if ((object)networkManager == null || !networkManager.IsListening)
				{
					return;
				}

				if (OwnerClientId != networkManager.LocalClientId)
				{
					if (networkManager.LogLevel <= LogLevel.Normal)
					{
						Debug.LogError("Only the owner can invoke a ServerRpc that requires ownership!");
					}

					return;
				}

				FastBufferWriter bufferWriter = new FastBufferWriter(256, Unity.Collections.Allocator.Temp);
				bufferWriter.WriteValueSafe(in obj, default);
				bufferWriter.WriteValueSafe(placementPosition);
				bufferWriter.WriteValueSafe(shipParent);
				NetworkManager.Singleton.CustomMessagingManager.SendNamedMessageToAll("MakeObjectFall", bufferWriter, NetworkDelivery.Reliable);

				if (obj.TryGet(out var networkObject))
				{
					GrabbableObject component = networkObject.GetComponent<GrabbableObject>();
					if (!IsOwner)
					{
						MakeObjectFall(component, placementPosition, shipParent);
					}
				}
			}

			[ClientRpc]
			public void RunClientRpc(NetworkObjectReference obj, Vector3 placementPosition, bool shipParent)
			{
				MakeObjectFallServerRpc(obj, placementPosition, shipParent);
			}
		}
	}
}