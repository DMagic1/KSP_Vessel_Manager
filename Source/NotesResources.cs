using System;
using System.Collections.Generic;
using System.Linq;
using BetterNotes.Framework;
using UnityEngine;

namespace BetterNotes
{
	[KSPAddon(KSPAddon.Startup.MainMenu, true)]
	public class NotesResources : Notes_MBE
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
			pilotIcon = GameDatabase.Instance.GetTexture("", false);
			engineerIcon = GameDatabase.Instance.GetTexture("", false);
			scientistIcon = GameDatabase.Instance.GetTexture("", false);
			touristIcon = GameDatabase.Instance.GetTexture("", false);
			defaultIcon = GameDatabase.Instance.GetTexture("", false);
			levelZero = GameDatabase.Instance.GetTexture("", false);
			levelOne = GameDatabase.Instance.GetTexture("", false);
			levelTwo = GameDatabase.Instance.GetTexture("", false);
			levelThree = GameDatabase.Instance.GetTexture("", false);
			levelFour = GameDatabase.Instance.GetTexture("", false);
			levelFive = GameDatabase.Instance.GetTexture("", false);
		}

	}
}
