using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using BetterNotes.Framework;
using BetterNotes.NoteClasses.CheckListHandler;

namespace BetterNotes.NoteClasses
{
	public class Notes_CheckListContainer : Notes_Base
	{
		private Dictionary<Guid, Notes_CheckListItem> allChecks = new Dictionary<Guid, Notes_CheckListItem>();

		public Notes_CheckListContainer()
		{ }

		public Notes_CheckListContainer(Notes_Container n)
		{
			root = n;
			vessel = n.NotesVessel;
		}

		public Notes_CheckListContainer(Notes_CheckListContainer copy, Notes_Container n)
		{
			allChecks = copy.allChecks;
			root = n;
			vessel = n.NotesVessel;
		}

		public int noteCount
		{
			get { return allChecks.Count; }
		}

		public Notes_CheckListItem getCheckList(int index, bool warn = false)
		{
			if (allChecks.Count > index)
				return allChecks.ElementAt(index).Value;
			else if (warn)
				Debug.LogWarning("CheckList dictionary index out of range; something went wrong here...");

			return null;
		}

		public Notes_CheckListItem getCheckList(Guid id)
		{
			if (allChecks.ContainsKey(id))
				return allChecks[id];

			return null;
		}

		public void addCheckList(Notes_CheckListItem item)
		{
			if (!allChecks.ContainsKey(item.ID))
				allChecks.Add(item.ID, item);
		}
	}

	public class Notes_CheckListItem
	{
		private int order;
		private Guid id;
		private string text;
		private bool complete;
		private float? data;
		private Vessel targetVessel;
		private CelestialBody targetBody;
		private Notes_CheckListType checkType;
		private Notes_CheckListContainer root;

		private static bool loaded = false;

		private static string checkListTypeTitleLaunch = "Launch from {0}";		
		private static string checkListTypeTitleOrbit = "Orbit {0}";		
		private static string checkListTypeTitleEnterOrbit = "Enter orbit around {0}";		
		private static string checkListTypeTitleReturnToOrbit = "Return to orbit from {0}";		
		private static string checkListTypeTitleLand = "Land on {0}";		
		private static string checkListTypeTitleReturnHome = "Return to {0}";		
		private static string checkListTypeTitleRendezvousVessel = "Rendezvous with {0}\n(Approach to within 2.4km)";		
		private static string checkListTypeTitleDockVessel = "Dock with {0}";		
		private static string checkListTypeTitleRendezvousAsteroid = "Rendezvous with {0}\n(Approach to within 2.4km)";
		private static string checkListTypeTitleDockAsteroid = "Grab {0}";
		private static string checkListTypeTitleBlastOff = "Take off from {0}";
		private static string checkListTypeTitleScience = "Return {0:F0} science data";
		private static string checkListTypeTitleScienceFromPlanet = "Return {0:F0} science data from {1}";

		public Notes_CheckListItem()
		{
			if (!loaded)
				loadStrings();
		}

		public Notes_CheckListItem(int i, Vessel targetV, CelestialBody targetB, Notes_CheckListType type, Notes_CheckListContainer r, string t = "", float? d = null)
		{
			if (!loaded)
				loadStrings();
			order = i;
			checkType = type;
			root = r;
			data = d;
			id = Guid.NewGuid();

			switch (checkType)
			{
				case Notes_CheckListType.dockVessel:
				case Notes_CheckListType.dockAsteroid:
				case Notes_CheckListType.rendezvousVessel:
				case Notes_CheckListType.rendezvousAsteroid:
					targetVessel = targetV;
					targetBody = null;
					break;
				case Notes_CheckListType.land:
				case Notes_CheckListType.orbit:
				case Notes_CheckListType.enterOrbit:
				case Notes_CheckListType.returnToOrbit:
				case Notes_CheckListType.blastOff:
				case Notes_CheckListType.scienceFromPlanet:
					targetBody = targetB;
					targetVessel = null;
					break;
				case Notes_CheckListType.launch:
				case Notes_CheckListType.returnHome:
					targetBody = Planetarium.fetch.Home;
					targetVessel = null;
					break;
				case Notes_CheckListType.science:
					targetBody = null;
					targetVessel = null;
					break;
			}

			text = setTitle(t);

			Notes_CheckListTypeHandler.registerCheckList(this);
		}

		public Notes_CheckListItem(string t, int i, bool b, Vessel targetV, CelestialBody targetB, Guid g, Notes_CheckListType y, Notes_CheckListContainer r, float? d)
		{
			if (!loaded)
				loadStrings();
			text = t;
			order = i;
			complete = b;
			id = g;
			targetBody = targetB;
			targetVessel = targetV;
			checkType = y;
			root = r;
			data = d;

			Notes_CheckListTypeHandler.registerCheckList(this);
		}

		private void loadStrings()
		{
			loaded = true;

			if (Notes_MainMenu.Active_Localization_Pack == null)
				return;

			checkListTypeTitleLaunch = Notes_MainMenu.Active_Localization_Pack.CheckListTypeTitleLaunch;
			checkListTypeTitleOrbit = Notes_MainMenu.Active_Localization_Pack.CheckListTypeTitleOrbit;
			checkListTypeTitleEnterOrbit = Notes_MainMenu.Active_Localization_Pack.CheckListTypeTitleEnterOrbit;
			checkListTypeTitleReturnToOrbit = Notes_MainMenu.Active_Localization_Pack.CheckListTypeTitleReturnToOrbit;
			checkListTypeTitleLand = Notes_MainMenu.Active_Localization_Pack.CheckListTypeTitleLand;
			checkListTypeTitleReturnHome = Notes_MainMenu.Active_Localization_Pack.CheckListTypeTitleReturnHome;
			checkListTypeTitleRendezvousVessel = Notes_MainMenu.Active_Localization_Pack.CheckListTypeTitleRendezvousVessel;
			checkListTypeTitleDockVessel = Notes_MainMenu.Active_Localization_Pack.CheckListTypeTitleDockVessel;
			checkListTypeTitleRendezvousAsteroid = Notes_MainMenu.Active_Localization_Pack.CheckListTypeTitleRendezvousAsteroid;
			checkListTypeTitleDockAsteroid = Notes_MainMenu.Active_Localization_Pack.CheckListTypeTitleDockAsteroid;
			checkListTypeTitleBlastOff = Notes_MainMenu.Active_Localization_Pack.CheckListTypeTitleBlastOff;
			checkListTypeTitleScience = Notes_MainMenu.Active_Localization_Pack.CheckListTypeTitleScience;
			checkListTypeTitleScienceFromPlanet = Notes_MainMenu.Active_Localization_Pack.CheckListTypeTitleScienceFromPlanet;
		}

		public void setComplete()
		{
			complete = true;
			Notes_CheckListTypeHandler.deRegisterCheckList(this);
		}

		private string setTitle(string custom)
		{
			switch (checkType)
			{
				case Notes_CheckListType.blastOff:
					return string.Format(checkListTypeTitleBlastOff, targetBody.theName);
				case Notes_CheckListType.launch:
					return string.Format(checkListTypeTitleLaunch, targetBody.theName);
				case Notes_CheckListType.land:
					return string.Format(checkListTypeTitleLand, targetBody.theName);
				case Notes_CheckListType.orbit:
					return string.Format(checkListTypeTitleOrbit, targetBody.theName);
				case Notes_CheckListType.enterOrbit:
					return string.Format(checkListTypeTitleEnterOrbit, targetBody.theName);
				case Notes_CheckListType.returnToOrbit:
					return string.Format(checkListTypeTitleReturnToOrbit, targetBody.theName);
				case Notes_CheckListType.returnHome:
					return string.Format(checkListTypeTitleReturnHome, targetBody.theName);
				case Notes_CheckListType.dockVessel:
					return string.Format(checkListTypeTitleDockVessel, targetVessel.vesselName);
				case Notes_CheckListType.rendezvousVessel:
					return string.Format(checkListTypeTitleRendezvousVessel, targetVessel.vesselName);
				case Notes_CheckListType.dockAsteroid:
					return string.Format(checkListTypeTitleDockAsteroid, targetVessel.vesselName);
				case Notes_CheckListType.rendezvousAsteroid:
					return string.Format(checkListTypeTitleRendezvousAsteroid, targetVessel.vesselName);
				case Notes_CheckListType.science:
					return string.Format(checkListTypeTitleScience, data);
				case Notes_CheckListType.scienceFromPlanet:
					return string.Format(checkListTypeTitleScienceFromPlanet, data, targetBody.theName);
				default:
					return custom;
			}
		}

		public string Text
		{
			get { return text; }
			set { text = value; }
		}

		public float? Data
		{
			get { return data; }
			set
			{
				if (value != null && data != null)
				{
					if (value <= 0)
						data = 0;
					else if (value < data)
						data = value;
				}
			}
		}

		public int Order
		{
			get { return order; }
		}

		public bool Complete
		{
			get { return complete; }
		}

		public Guid ID
		{
			get { return id; }
		}

		public Vessel TargetVessel
		{
			get { return targetVessel; }
		}

		public CelestialBody TargetBody
		{
			get { return targetBody; }
		}

		public Notes_CheckListContainer Root
		{
			get { return root; }
		}

		public Notes_CheckListType CheckType
		{
			get { return checkType; }
		}
	}
}
