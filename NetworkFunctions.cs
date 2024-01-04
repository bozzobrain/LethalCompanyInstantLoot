using System;
using System.Collections.Generic;
using System.Text;
using Unity.Netcode;
using UnityEngine;

namespace InstantLoot
{
	internal class NetworkFunctions
	{
		public class NetworkingObjectManager : NetworkBehaviour
		{
			[ServerRpc]
			public void MakeObjectFallServerRpc(NetworkObjectReference obj, Vector3 placementPosition)
			{
				NetworkManager networkManager = base.NetworkManager;
				if ((object)networkManager == null || !networkManager.IsListening)
				{
					return;
				}

				if (base.OwnerClientId != networkManager.LocalClientId)
				{
					if (networkManager.LogLevel <= Unity.Netcode.LogLevel.Normal)
					{
						Debug.LogError("Only the owner can invoke a ServerRpc that requires ownership!");
					}

					return;
				}

				FastBufferWriter bufferWriter = new FastBufferWriter(256, Unity.Collections.Allocator.Temp);
				bufferWriter.WriteValueSafe(in obj, default(FastBufferWriter.ForNetworkSerializable));
				bufferWriter.WriteValueSafe(placementPosition);
				NetworkManager.Singleton.CustomMessagingManager.SendNamedMessageToAll("MakeObjectFall", bufferWriter, NetworkDelivery.Reliable);

				if (obj.TryGet(out var networkObject))
				{
					GrabbableObject component = networkObject.GetComponent<GrabbableObject>();
					if (!base.IsOwner)
					{
						MakeObjectFall(component, placementPosition);
					}
				}
			}
			[ClientRpc]
			public void MakeObjectFallClientRpc(NetworkObjectReference obj, Vector3 placementPosition)
			{
				NetworkManager networkManager = base.NetworkManager;
				if ((object)networkManager == null || !networkManager.IsListening)
				{
					return;
				}

				FastBufferWriter bufferWriter = new FastBufferWriter(256, Unity.Collections.Allocator.Temp);
				bufferWriter.WriteValueSafe(in obj, default(FastBufferWriter.ForNetworkSerializable));
				bufferWriter.WriteValueSafe(in placementPosition);
				NetworkManager.Singleton.CustomMessagingManager.SendNamedMessageToAll("MakeObjectFall", bufferWriter, NetworkDelivery.Reliable);

				if (obj.TryGet(out var networkObject))
				{
					GrabbableObject component = networkObject.GetComponent<GrabbableObject>();
					if (!base.IsOwner)
					{
						MakeObjectFall(component, placementPosition);
					}
				}
			}

			public void MakeObjectFall(GrabbableObject obj, Vector3 placementPosition)
			{
				GameObject ship = GameObject.Find("/Environment/HangarShip");

				InstantLoot.Log($"Request to make GrabbableObject {obj.name} fall to ground");
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
			public void RunClientRpc(NetworkObjectReference obj, Vector3 placementPosition)
			{
				MakeObjectFallServerRpc(obj, placementPosition);
			}
		}
	}
}
