using BepInEx;
using HarmonyLib;
using InstantLoot.Configuration;
using System.Reflection;

namespace InstantLoot
{
	[BepInPlugin(GUID, NAME, VERSION)]
	internal class InstantLoot : BaseUnityPlugin
	{
		public static InstantLoot instance;
		private const string GUID = "InstantLoot";
		private const string NAME = "InstantLoot";
		private const string VERSION = "2.1.1";

		public static void Log(string message)
		{
			instance.Logger.LogInfo((object)message);
		}

		private void Awake()
		{
			instance = this;
			ConfigSettings.BindConfigSettings();

			// Plugin startup logic
			Logger.LogInfo($"Plugin {GUID} is loaded!");

			Harmony harmony = new Harmony(GUID);
			harmony.PatchAll(Assembly.GetExecutingAssembly());
		}
	}
}