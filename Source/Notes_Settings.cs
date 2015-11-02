using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using BetterNotes.Framework;

namespace BetterNotes
{
	public class Notes_Settings : Notes_ConfigNodeStorage
	{
		[Persistent]
		private bool showDebris = false;
		[Persistent]
		private bool showFlags = true;
		[Persistent]
		private bool showEVA = false;
		[Persistent]
		private bool showAsteroids = false;
		[Persistent]
		private bool highlightPart = true;
		[Persistent]
		private Color32 pilotIconColor = XKCDColors.PastelRed;
		[Persistent]
		private Color32 engineerIconColor = XKCDColors.DarkYellow;
		[Persistent]
		private Color32 scientistIconColor = XKCDColors.DirtyBlue;
		[Persistent]
		private Color32 touristIconColor = XKCDColors.SapGreen;

		public Notes_Settings(string path, string node)
		{
			FilePath = path;
			NodeName = path + "/" + node;

			if (!Load())
				Save();
		}

		public bool ShowDebris
		{
			get { return showDebris; }
		}

		public bool ShowFlags
		{
			get { return showFlags; }
		}

		public bool ShowEVA
		{
			get { return showEVA; }
		}

		public bool ShowAsteroids
		{
			get { return showAsteroids; }
		}

		public bool HighLightPart
		{
			get { return highlightPart; }
		}

		public Color32 PilotIconColor
		{
			get { return pilotIconColor; }
		}

		public Color32 EngineerIconColor
		{
			get { return engineerIconColor; }
		}

		public Color32 ScientistIconColor
		{
			get { return scientistIconColor; }
		}

		public Color32 TouristIconColor
		{
			get { return touristIconColor; }
		}
	}
}
