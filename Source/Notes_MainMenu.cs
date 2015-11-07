using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using BetterNotes.Framework;

namespace BetterNotes
{
	[KSPAddon(KSPAddon.Startup.MainMenu, true)]
	public class Notes_MainMenu : Notes_MBE
	{
		private bool loaded;

		private const string settingsFilePath = "BetterNotes/Settings";
		private const string settingsNode = "Notes_Settings";

		private const string localizationFilePath = "BetterNotes/Localization";
		private const string localizationNode = "Notes_Localization";

		private static bool contractsPlusLoaded;

		private static Notes_Settings settings;
		private static Notes_Localization localization;

		public static bool ContractsPlusLoaded
		{
			get { return contractsPlusLoaded; }
		}

		public static Notes_Settings Settings
		{
			get { return settings; }
		}

		public static Notes_Localization Localization
		{
			get { return localization; }
		}

		public static Notes_LanguagePack Active_Localization_Pack
		{
			get { return localization.ActivePack; }
		}

		protected override void Start()
		{
			if (loaded)
				return;

			loaded = true;

			settings = new Notes_Settings(settingsFilePath, settingsNode);

			localization = new Notes_Localization(localizationFilePath, localizationNode);

			contractsPlusLoaded = Notes_AssemblyLoad.loadMethods();
		}
	}
}
