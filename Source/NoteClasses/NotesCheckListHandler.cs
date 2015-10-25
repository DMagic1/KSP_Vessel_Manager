﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using BetterNotes.Framework;

namespace BetterNotes.NoteClasses
{
	public enum NotesCheckListType
	{
		custom = 1,
		launch = 2,
		orbit = 3,
		land = 4,
		returnHome = 5,
		rendezvousVessel = 6,
		dockVessel = 7,
		rendezvousAsteroid = 8,
		dockAsteroid = 9,
	}

	public static class NotesCheckListTypeHandler
	{
		private static Dictionary<Guid, NotesCheckListItem> launchNotes = new Dictionary<Guid, NotesCheckListItem>();
		private static Dictionary<Guid, NotesCheckListItem> orbitNotes = new Dictionary<Guid, NotesCheckListItem>();
		private static Dictionary<Guid, NotesCheckListItem> landNotes = new Dictionary<Guid, NotesCheckListItem>();
		private static Dictionary<Guid, NotesCheckListItem> returnHomeNotes = new Dictionary<Guid, NotesCheckListItem>();
		private static Dictionary<Guid, NotesCheckListItem> rendezvousVesselNotes = new Dictionary<Guid, NotesCheckListItem>();
		private static Dictionary<Guid, NotesCheckListItem> dockVesselNotes = new Dictionary<Guid, NotesCheckListItem>();
		private static Dictionary<Guid, NotesCheckListItem> rendezvousAsteroidNotes = new Dictionary<Guid, NotesCheckListItem>();
		private static Dictionary<Guid, NotesCheckListItem> dockAsteroidNotes = new Dictionary<Guid, NotesCheckListItem>();

		public static void registerEvents()
		{
			GameEvents.onLaunch.Add(onVesselLaunch);
			GameEvents.VesselSituation.onOrbit.Add(onVesselOrbit);
			GameEvents.VesselSituation.onLand.Add(onVesselLand);
			GameEvents.onPartCouple.Add(onDock);
			GameEvents.onVesselLoaded.Add(onVesselLoad);
		}

		public static void deRegisterEvents()
		{
			GameEvents.onLaunch.Remove(onVesselLaunch);
			GameEvents.VesselSituation.onOrbit.Remove(onVesselOrbit);
			GameEvents.VesselSituation.onLand.Remove(onVesselLand);
			GameEvents.onPartCouple.Remove(onDock);
			GameEvents.onVesselLoaded.Remove(onVesselLoad);
		}

		public static void registerCheckList(NotesCheckListItem n)
		{
			switch (n.CheckType)
			{
				case NotesCheckListType.launch:
					if (!launchNotes.ContainsKey(n.ID))
						launchNotes.Add(n.ID, n);
					break;
				case NotesCheckListType.orbit:
					if (!orbitNotes.ContainsKey(n.ID))
						orbitNotes.Add(n.ID, n);
					break;
				case NotesCheckListType.land:
					if (!landNotes.ContainsKey(n.ID))
						landNotes.Add(n.ID, n);
					break;
				case NotesCheckListType.returnHome:
					if (!returnHomeNotes.ContainsKey(n.ID))
						returnHomeNotes.Add(n.ID, n);
					break;
				case NotesCheckListType.rendezvousVessel:
					if (!rendezvousVesselNotes.ContainsKey(n.ID))
						rendezvousVesselNotes.Add(n.ID, n);
					break;
				case NotesCheckListType.dockVessel:
					if (!dockVesselNotes.ContainsKey(n.ID))
						dockVesselNotes.Add(n.ID, n);
					break;
				case NotesCheckListType.rendezvousAsteroid:
					if (!rendezvousAsteroidNotes.ContainsKey(n.ID))
						rendezvousAsteroidNotes.Add(n.ID, n);
					break;
				case NotesCheckListType.dockAsteroid:
					if (!dockAsteroidNotes.ContainsKey(n.ID))
						dockAsteroidNotes.Add(n.ID, n);
					break;
				default:
					break;
			}
		}

		public static void deRegisterCheckList(NotesCheckListItem n)
		{
			switch (n.CheckType)
			{
				case NotesCheckListType.launch:
					if (launchNotes.ContainsKey(n.ID))
						launchNotes.Remove(n.ID);
					break;
				case NotesCheckListType.orbit:
					if (orbitNotes.ContainsKey(n.ID))
						orbitNotes.Remove(n.ID);
					break;
				case NotesCheckListType.land:
					if (landNotes.ContainsKey(n.ID))
						landNotes.Remove(n.ID);
					break;
				case NotesCheckListType.returnHome:
					if (returnHomeNotes.ContainsKey(n.ID))
						returnHomeNotes.Remove(n.ID);
					break;
				case NotesCheckListType.rendezvousVessel:
					if (rendezvousVesselNotes.ContainsKey(n.ID))
						rendezvousVesselNotes.Remove(n.ID);
					break;
				case NotesCheckListType.dockVessel:
					if (dockVesselNotes.ContainsKey(n.ID))
						dockVesselNotes.Remove(n.ID);
					break;
				case NotesCheckListType.rendezvousAsteroid:
					if (rendezvousAsteroidNotes.ContainsKey(n.ID))
						rendezvousAsteroidNotes.Remove(n.ID);
					break;
				case NotesCheckListType.dockAsteroid:
					if (dockAsteroidNotes.ContainsKey(n.ID))
						dockAsteroidNotes.Remove(n.ID);
					break;
				default:
					break;
			}
		}

		private static void onVesselOrbit(Vessel v, CelestialBody c)
		{
			if (v == null)
				return;

			if (c == null)
				return;

			IEnumerable<KeyValuePair<Guid, NotesCheckListItem>> notes = orbitNotes.Where(n => n.Value.Root.RootVessel.id == v.id);
			for (int i = 0; i < notes.Count(); i++)
			{
				NotesCheckListItem n = notes.ElementAt(i).Value;

				if (n == null)
					continue;

				if (n.TargetBody != c)
					continue;

				n.setComplete();
			}
		}

		private static void onVesselLand(Vessel v, CelestialBody c)
		{
			if (v == null)
				return;

			if (c == null)
				return;

			IEnumerable<KeyValuePair<Guid, NotesCheckListItem>> notes;

			if (c == Planetarium.fetch.Home)
			{
				notes = returnHomeNotes.Where(n => n.Value.Root.RootVessel.id == v.id);
				for (int i = 0; i < notes.Count(); i++)
				{
					NotesCheckListItem n = notes.ElementAt(i).Value;

					if (n == null)
						continue;

					n.setComplete();
				}
			}

			notes = landNotes.Where(n => n.Value.Root.RootVessel.id == v.id);
			for (int i = 0; i < notes.Count(); i++)
			{
				NotesCheckListItem n = notes.ElementAt(i).Value;

				if (n == null)
					continue;

				if (n.TargetBody != c)
					continue;

				n.setComplete();
			}
		}

		private static void onVesselLaunch(EventReport e)
		{
			Vessel v = FlightGlobals.ActiveVessel;

			if (v == null)
				return;

			IEnumerable<KeyValuePair<Guid, NotesCheckListItem>> notes = launchNotes.Where(n => n.Value.Root.RootVessel.id == v.id);
			for (int i = 0; i < notes.Count(); i++)
			{
				NotesCheckListItem n = notes.ElementAt(i).Value;

				if (n == null)
					continue;

				n.setComplete();
			}
		}

		private static void onDock(GameEvents.FromToAction<Part, Part> F)
		{
			if (F.from == null || F.to == null)
				return;

			if (F.from.vessel == null || F.to.vessel == null)
				return;

			IEnumerable<KeyValuePair<Guid, NotesCheckListItem>> fromNotes;
			IEnumerable<KeyValuePair<Guid, NotesCheckListItem>> toNotes;

			if (F.to.vessel.FindPartModulesImplementing<ModuleAsteroid>().Count > 0)
			{
				fromNotes = dockAsteroidNotes.Where(n => n.Value.Root.RootVessel.id == F.from.vessel.id);
			}
			else
			{
				fromNotes = dockVesselNotes.Where(n => n.Value.Root.RootVessel.id == F.from.vessel.id);
			}

			if (F.from.vessel.FindPartModulesImplementing<ModuleAsteroid>().Count > 0)
			{
				toNotes = dockAsteroidNotes.Where(n => n.Value.Root.RootVessel.id == F.to.vessel.id);
			}
			else
			{
				toNotes = dockVesselNotes.Where(n => n.Value.Root.RootVessel.id == F.to.vessel.id);
			}

			if (fromNotes.Count() > 0)
			{
				for (int i = 0; i < fromNotes.Count(); i++)
				{
					NotesCheckListItem n = fromNotes.ElementAt(i).Value;

					if (n == null)
						continue;

					if (n.TargetVessel == null)
						continue;

					if (n.TargetVessel.id == F.to.vessel.id)
						n.setComplete();
				}
			}

			if (toNotes.Count() > 0)
			{
				for (int i = 0; i < toNotes.Count(); i++)
				{
					NotesCheckListItem n = toNotes.ElementAt(i).Value;

					if (n == null)
						continue;

					if (n.TargetVessel == null)
						continue;

					if (n.TargetVessel.id == F.from.vessel.id)
						n.setComplete();
				}
			}
		}

		private static void onVesselLoad(Vessel v)
		{
			IEnumerable<KeyValuePair<Guid, NotesCheckListItem>> activeNotes;
			IEnumerable<KeyValuePair<Guid, NotesCheckListItem>> loadedNotes;

			Vessel active = FlightGlobals.ActiveVessel;

			if (active.FindPartModulesImplementing<ModuleAsteroid>().Count > 0)
			{
				activeNotes = rendezvousAsteroidNotes.Where(n => n.Value.Root.RootVessel.id == active.id);
			}
			else
			{
				activeNotes = rendezvousVesselNotes.Where(n => n.Value.Root.RootVessel.id == active.id);
			}

			if (v.FindPartModulesImplementing<ModuleAsteroid>().Count > 0)
			{
				loadedNotes = rendezvousAsteroidNotes.Where(n => n.Value.Root.RootVessel.id == v.id);
			}
			else
			{
				loadedNotes = rendezvousVesselNotes.Where(n => n.Value.Root.RootVessel.id == v.id);
			}

			if (activeNotes.Count() > 0)
			{
				for (int i = 0; i < activeNotes.Count(); i++)
				{
					NotesCheckListItem n = activeNotes.ElementAt(i).Value;

					if (n == null)
						continue;

					if (n.TargetVessel == null)
						continue;

					if (n.TargetVessel.id == v.id)
						n.setComplete();
				}
			}

			if (loadedNotes.Count() > 0)
			{
				for (int i = 0; i < loadedNotes.Count(); i++)
				{
					NotesCheckListItem n = loadedNotes.ElementAt(i).Value;

					if (n == null)
						continue;

					if (n.TargetVessel == null)
						continue;

					if (n.TargetVessel.id == active.id)
						n.setComplete();
				}
			}
		}

	}
}