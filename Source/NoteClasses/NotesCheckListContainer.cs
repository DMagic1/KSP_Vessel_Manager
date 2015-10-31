using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using BetterNotes.Framework;
using BetterNotes.NoteClasses.CheckListHandler;

namespace BetterNotes.NoteClasses
{
	public class NotesCheckListContainer : NotesBase
	{
		private Dictionary<Guid, NotesCheckListItem> allChecks = new Dictionary<Guid, NotesCheckListItem>();

		public NotesCheckListContainer()
		{ }

		public NotesCheckListContainer(NotesContainer n)
		{
			root = n;
			vessel = n.NotesVessel;
		}

		public NotesCheckListContainer(NotesCheckListContainer copy, NotesContainer n)
		{
			allChecks = copy.allChecks;
			root = n;
			vessel = n.NotesVessel;
		}

		public int noteCount
		{
			get { return allChecks.Count; }
		}

		public NotesCheckListItem getCheckList(int index, bool warn = false)
		{
			if (allChecks.Count > index)
				return allChecks.ElementAt(index).Value;
			else if (warn)
				Debug.LogWarning("CheckList dictionary index out of range; something went wrong here...");

			return null;
		}

		public NotesCheckListItem getCheckList(Guid id)
		{
			if (allChecks.ContainsKey(id))
				return allChecks[id];

			return null;
		}

		public void addCheckList(NotesCheckListItem item)
		{
			if (!allChecks.ContainsKey(item.ID))
				allChecks.Add(item.ID, item);
		}
	}

	public class NotesCheckListItem
	{
		private int order;
		private Guid id;
		private string text;
		private bool complete;
		private float? data;
		private Vessel targetVessel;
		private CelestialBody targetBody;
		private NotesCheckListType checkType;
		private NotesCheckListContainer root;

		public NotesCheckListItem()
		{ }

		public NotesCheckListItem(int i, Vessel targetV, CelestialBody targetB, NotesCheckListType type, NotesCheckListContainer r, string t = "", float? d = null)
		{
			order = i;
			checkType = type;
			root = r;
			data = d;
			id = Guid.NewGuid();

			switch (checkType)
			{
				case NotesCheckListType.dockVessel:
				case NotesCheckListType.dockAsteroid:
				case NotesCheckListType.rendezvousVessel:
				case NotesCheckListType.rendezvousAsteroid:
					targetVessel = targetV;
					targetBody = null;
					break;
				case NotesCheckListType.land:
				case NotesCheckListType.orbit:
				case NotesCheckListType.enterOrbit:
				case NotesCheckListType.returnToOrbit:
				case NotesCheckListType.blastOff:
				case NotesCheckListType.scienceFromPlanet:
					targetBody = targetB;
					targetVessel = null;
					break;
				case NotesCheckListType.launch:
				case NotesCheckListType.returnHome:
					targetBody = Planetarium.fetch.Home;
					targetVessel = null;
					break;
				case NotesCheckListType.science:
					targetBody = null;
					targetVessel = null;
					break;
			}

			text = setTitle(t);

			NotesCheckListTypeHandler.registerCheckList(this);
		}

		public NotesCheckListItem(string t, int i, bool b, Vessel targetV, CelestialBody targetB, Guid g, NotesCheckListType y, NotesCheckListContainer r, float? d)
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

			NotesCheckListTypeHandler.registerCheckList(this);
		}

		public void setComplete()
		{
			complete = true;
			NotesCheckListTypeHandler.deRegisterCheckList(this);
		}

		private string setTitle(string custom)
		{
			switch (checkType)
			{
				case NotesCheckListType.blastOff:
					return string.Format("Take off from {0}", targetBody.theName);
				case NotesCheckListType.launch:
					return string.Format("Launch from {0}", targetBody.theName);
				case NotesCheckListType.land:
					return string.Format("Land on {0}", targetBody.theName);
				case NotesCheckListType.orbit:
					return string.Format("Orbit {0}", targetBody.theName);
				case NotesCheckListType.enterOrbit:
					return string.Format("Enter orbit around {0}", targetBody.theName);
				case NotesCheckListType.returnToOrbit:
					return string.Format("Return to orbit from {0}", targetBody.theName);
				case NotesCheckListType.returnHome:
					return string.Format("Return to {0}", targetBody.theName);
				case NotesCheckListType.dockVessel:
					return string.Format("Dock with {0}", targetVessel.vesselName);
				case NotesCheckListType.rendezvousVessel:
					return string.Format("Rendezvous with {0}\n(Approach to within 2.4km)", targetVessel.vesselName);
				case NotesCheckListType.dockAsteroid:
					return string.Format("Grab {0}", targetVessel.vesselName);
				case NotesCheckListType.rendezvousAsteroid:
					return string.Format("Rendezvous with {0}\n(Approach to within 2.4km)", targetVessel.vesselName);
				case NotesCheckListType.science:
					return string.Format("Return {0:F0} science data", data);
				case NotesCheckListType.scienceFromPlanet:
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

		public NotesCheckListContainer Root
		{
			get { return root; }
		}

		public NotesCheckListType CheckType
		{
			get { return checkType; }
		}
	}
}
