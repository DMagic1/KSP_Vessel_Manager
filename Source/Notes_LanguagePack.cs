using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Reflection;
using BetterNotes.Framework;

namespace BetterNotes
{
	public class Notes_LanguagePack : Notes_ConfigNodeStorage
	{
		[Persistent]
		private bool activePack;
		[Persistent]
		private string language = "English(USA)";

		//In all cases [x] in strings are replaced with {x} for string.Format; where x is a numeral

		//Check List Type Titles
		[Persistent]
		private string checkListTypeTitleLaunch = "Launch from {0}";
		[Persistent]
		private string checkListTypeTitleOrbit = "Orbit {0}";
		[Persistent]
		private string checkListTypeTitleEnterOrbit = "Enter orbit around {0}";
		[Persistent]
		private string checkListTypeTitleReturnToOrbit = "Return to orbit from {0}";
		[Persistent]
		private string checkListTypeTitleLand = "Land on {0}";
		[Persistent]
		private string checkListTypeTitleReturnHome = "Return to {0}";
		[Persistent]
		private string checkListTypeTitleRendezvousVessel = "Rendezvous with {0}\n(Approach to within 2.4km)";
		[Persistent]
		private string checkListTypeTitleDockVessel = "Dock with {0}";
		[Persistent]
		private string checkListTypeTitleRendezvousAsteroid = "Rendezvous with {0}\n(Approach to within 2.4km)";
		[Persistent]
		private string checkListTypeTitleDockAsteroid = "Grab {0}";
		[Persistent]
		private string checkListTypeTitleBlastOff = "Take off from {0}";
		[Persistent]
		private string checkListTypeTitleScience = "Return {0:F0} science data";
		[Persistent]
		private string checkListTypeTitleScienceFromPlanet = "Return {0:F0} science data from {1}";

		public override void OnDecodeFromConfigNode()
		{
			Regex openBracket = new Regex(@"\[(?=\d+:?\w{0,3}\])");

			Regex closeBraket = new Regex(@"(?<=\{\d+:?\w{0,3})\]");

			Regex newLines = new Regex(@"\\n");

			var stringFields = this.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public).Where(a => a.FieldType == typeof(string)).ToList();

			for (int i = 0; i < stringFields.Count(); i++)
			{
				FieldInfo f = stringFields[i];

				f.SetValue(this, openBracket.Replace((string)f.GetValue(this), "{"));

				f.SetValue(this, closeBraket.Replace((string)f.GetValue(this), "}"));

				f.SetValue(this, newLines.Replace((string)f.GetValue(this), Environment.NewLine));
			}
		}

		public override void OnEncodeToConfigNode()
		{
			Regex openCurlyBracket = new Regex(@"\{(?=\d+:?\w{0,3}\})");

			Regex closeCurlyBraket = new Regex(@"(?<=\[\d+:?\w{0,3})\}");

			Regex newLines = new Regex(@"\n");

			var stringFields = this.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public).Where(a => a.FieldType == typeof(string)).ToList();

			for (int i = 0; i < stringFields.Count(); i++)
			{
				FieldInfo f = stringFields[i];

				f.SetValue(this, openCurlyBracket.Replace((string)f.GetValue(this), "["));

				f.SetValue(this, closeCurlyBraket.Replace((string)f.GetValue(this), "]"));

				f.SetValue(this, newLines.Replace((string)f.GetValue(this), @"\n"));
			}
		}

		public bool ActivePack
		{
			get { return activePack; }
		}

		public string CheckListTypeTitleLaunch
		{
			get { return checkListTypeTitleLaunch; }
		}
		
		public string CheckListTypeTitleOrbit
		{
			get { return checkListTypeTitleOrbit; }
		}
		
		public string CheckListTypeTitleEnterOrbit
		{
			get { return checkListTypeTitleEnterOrbit; }
		}
		
		public string CheckListTypeTitleReturnToOrbit
		{
			get { return checkListTypeTitleReturnToOrbit; }
		}
		
		public string CheckListTypeTitleLand
		{
			get { return checkListTypeTitleLand; }
		}
		
		public string CheckListTypeTitleReturnHome
		{
			get { return checkListTypeTitleReturnHome; }
		}
		
		public string CheckListTypeTitleRendezvousVessel
		{
			get { return checkListTypeTitleRendezvousVessel; }
		}
		
		public string CheckListTypeTitleDockVessel
		{
			get { return checkListTypeTitleDockVessel; }
		}
		
		public string CheckListTypeTitleRendezvousAsteroid
		{
			get { return checkListTypeTitleRendezvousAsteroid; }
		}
		
		public string CheckListTypeTitleDockAsteroid
		{
			get { return checkListTypeTitleDockAsteroid; }
		}
		
		public string CheckListTypeTitleBlastOff
		{
			get { return checkListTypeTitleBlastOff; }
		}

		public string CheckListTypeTitleScience
		{
			get { return checkListTypeTitleScience; }
		}
		
		public string CheckListTypeTitleScienceFromPlanet
		{
			get { return checkListTypeTitleScienceFromPlanet;}
		}

	}
}
