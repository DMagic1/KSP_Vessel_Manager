using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using BetterNotes.Framework;

namespace BetterNotes
{
	[KSPAddon(KSPAddon.Startup.MainMenu, true)]
	public class NotesMainMenu : Notes_MBE
	{
		private bool loaded;

		private const string settingsFilePath = "BetterNotes/Settings";
		private const string settingsNode = "NotesSettings";

		private static NotesSettings settings;

		public static NotesSettings Settings
		{
			get { return settings; }
		}

		protected override void Start()
		{
			if (loaded)
				return;

			loaded = true;

			settings = new NotesSettings(settingsFilePath, settingsNode);
		}
	}
}
