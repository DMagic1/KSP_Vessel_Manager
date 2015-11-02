﻿using System;
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

		public Notes_CheckListItem()
		{ }

		public Notes_CheckListItem(int i, Vessel targetV, CelestialBody targetB, Notes_CheckListType type, Notes_CheckListContainer r, string t = "", float? d = null)
		{
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
					return string.Format("Take off from {0}", targetBody.theName);
				case Notes_CheckListType.launch:
					return string.Format("Launch from {0}", targetBody.theName);
				case Notes_CheckListType.land:
					return string.Format("Land on {0}", targetBody.theName);
				case Notes_CheckListType.orbit:
					return string.Format("Orbit {0}", targetBody.theName);
				case Notes_CheckListType.enterOrbit:
					return string.Format("Enter orbit around {0}", targetBody.theName);
				case Notes_CheckListType.returnToOrbit:
					return string.Format("Return to orbit from {0}", targetBody.theName);
				case Notes_CheckListType.returnHome:
					return string.Format("Return to {0}", targetBody.theName);
				case Notes_CheckListType.dockVessel:
					return string.Format("Dock with {0}", targetVessel.vesselName);
				case Notes_CheckListType.rendezvousVessel:
					return string.Format("Rendezvous with {0}\n(Approach to within 2.4km)", targetVessel.vesselName);
				case Notes_CheckListType.dockAsteroid:
					return string.Format("Grab {0}", targetVessel.vesselName);
				case Notes_CheckListType.rendezvousAsteroid:
					return string.Format("Rendezvous with {0}\n(Approach to within 2.4km)", targetVessel.vesselName);
				case Notes_CheckListType.science:
					return string.Format("Return {0:F0} science data", data);
				case Notes_CheckListType.scienceFromPlanet:
					return string.Format("Return {0:F0} science data from {1}", data, targetBody.theName);
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