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
		private const string settingsNode = "NotesSettings";

		private static Notes_Settings settings;

		public static Notes_Settings Settings
		{
			get { return settings; }
		}

		protected override void Start()
		{
			if (loaded)
				return;

			loaded = true;

			settings = new Notes_Settings(settingsFilePath, settingsNode);
		}
	}
}
