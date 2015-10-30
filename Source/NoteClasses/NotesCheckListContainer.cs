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
		private Vessel targetVessel;
		private CelestialBody targetBody;
		private NotesCheckListType checkType;
		private NotesCheckListContainer root;

		public NotesCheckListItem()
		{ }

		public NotesCheckListItem(string t, int i, Vessel targetV, CelestialBody targetB, NotesCheckListType type, NotesCheckListContainer r)
		{
			text = t;
			order = i;
			checkType = type;
			root = r;
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
				case NotesCheckListType.blastOff:
					targetBody = targetB;
					targetVessel = null;
					break;
				case NotesCheckListType.launch:
				case NotesCheckListType.returnHome:
					targetBody = Planetarium.fetch.Home;
					targetVessel = null;
					break;
			}

			NotesCheckListTypeHandler.registerCheckList(this);
		}

		public NotesCheckListItem(string t, int i, bool b, Vessel targetV, CelestialBody targetB, Guid g, NotesCheckListType y, NotesCheckListContainer r)
		{
			text = t;
			order = i;
			complete = b;
			id = g;
			targetBody = targetB;
			targetVessel = targetV;
			checkType = y;
			root = r;

			NotesCheckListTypeHandler.registerCheckList(this);
		}

		public void setComplete()
		{
			complete = true;
			NotesCheckListTypeHandler.deRegisterCheckList(this);
		}

		public string Text
		{
			get { return text; }
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
