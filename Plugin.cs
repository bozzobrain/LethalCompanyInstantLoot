using BepInEx;
using BepInEx.Logging;
using Dissonance;
using HarmonyLib;
using System;
using System.Reflection;

namespace InstantLoot
{

	[BepInPlugin(GUID, NAME, VERSION)]
	internal class InstantLoot : BaseUnityPlugin
	{
		private const string GUID = "InstantLoot";
		private const string NAME = "InstantLoot";
		private const string VERSION = "1.1.0";

		public static InstantLoot instance;
		private void Awake()
		{
			instance = this;
			ConfigSettings.BindConfigSettings();

			// Plugin startup logic
			Logger.LogInfo($"Plugin {GUID} is loaded!");

			Harmony harmony = new Harmony(GUID);
			harmony.PatchAll(Assembly.GetExecutingAssembly());


		}
		public static void Log(string message)
		{
			instance.Logger.LogInfo((object)message);
		}
	}
}
