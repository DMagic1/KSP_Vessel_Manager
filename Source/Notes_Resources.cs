using System;
using System.Collections.Generic;
using System.Linq;
using BetterNotes.Framework;
using UnityEngine;

namespace BetterNotes
{
	[KSPAddon(KSPAddon.Startup.MainMenu, true)]
	public class Notes_Resources : Notes_MBE
	{
		internal static Texture2D pilotIcon;
		internal static Texture2D engineerIcon;
		internal static Texture2D scientistIcon;
		internal static Texture2D touristIcon;
		internal static Texture2D defaultIcon;
		internal static Texture2D levelZero;
		internal static Texture2D levelOne;
		internal static Texture2D levelTwo;
		internal static Texture2D levelThree;
		internal static Texture2D levelFour;
		internal static Texture2D levelFive;


		protected override void OnGUIOnceOnly()
		{
			loadTextures();
		}

		private void loadTextures()
		{
			pilotIcon = GameDatabase.Instance.GetTexture("DMagicUtilities/BetterNotes/", false);
			engineerIcon = GameDatabase.Instance.GetTexture("DMagicUtilities/BetterNotes/", false);
			scientistIcon = GameDatabase.Instance.GetTexture("DMagicUtilities/BetterNotes/", false);
			touristIcon = GameDatabase.Instance.GetTexture("DMagicUtilities/BetterNotes/", false);
			defaultIcon = GameDatabase.Instance.GetTexture("DMagicUtilities/BetterNotes/", false);
			levelZero = GameDatabase.Instance.GetTexture("DMagicUtilities/BetterNotes/", false);
			levelOne = GameDatabase.Instance.GetTexture("DMagicUtilities/BetterNotes/", false);
			levelTwo = GameDatabase.Instance.GetTexture("DMagicUtilities/BetterNotes/", false);
			levelThree = GameDatabase.Instance.GetTexture("DMagicUtilities/BetterNotes/", false);
			levelFour = GameDatabase.Instance.GetTexture("DMagicUtilities/BetterNotes/", false);
			levelFive = GameDatabase.Instance.GetTexture("DMagicUtilities/BetterNotes/", false);
		}

	}
}
