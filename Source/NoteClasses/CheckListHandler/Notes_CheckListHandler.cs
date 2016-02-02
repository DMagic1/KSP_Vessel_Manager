using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using BetterNotes.Framework;

namespace BetterNotes.NoteClasses.CheckListHandler
{
	public static class Notes_CheckListTypeHandler
	{
		private static Dictionary<Guid, Notes_CheckListItem> launchNotes = new Dictionary<Guid, Notes_CheckListItem>();
		private static Dictionary<Guid, Notes_CheckListItem> orbitNotes = new Dictionary<Guid, Notes_CheckListItem>();
		private static Dictionary<Guid, Notes_CheckListItem> enterOrbitNotes = new Dictionary<Guid, Notes_CheckListItem>();
		private static Dictionary<Guid, Notes_CheckListItem> returnToOrbitNotes = new Dictionary<Guid, Notes_CheckListItem>();
		private static Dictionary<Guid, Notes_CheckListItem> landNotes = new Dictionary<Guid, Notes_CheckListItem>();
		private static Dictionary<Guid, Notes_CheckListItem> returnHomeNotes = new Dictionary<Guid, Notes_CheckListItem>();
		private static Dictionary<Guid, Notes_CheckListItem> rendezvousVesselNotes = new Dictionary<Guid, Notes_CheckListItem>();
		private static Dictionary<Guid, Notes_CheckListItem> dockVesselNotes = new Dictionary<Guid, Notes_CheckListItem>();
		private static Dictionary<Guid, Notes_CheckListItem> rendezvousAsteroidNotes = new Dictionary<Guid, Notes_CheckListItem>();
		private static Dictionary<Guid, Notes_CheckListItem> dockAsteroidNotes = new Dictionary<Guid, Notes_CheckListItem>();
		private static Dictionary<Guid, Notes_CheckListItem> blastOffNotes = new Dictionary<Guid,Notes_CheckListItem>();
		private static Dictionary<Guid, Notes_CheckListItem> scienceNotes = new Dictionary<Guid, Notes_CheckListItem>();
		private static Dictionary<Guid, Notes_CheckListItem> flagNotes = new Dictionary<Guid, Notes_CheckListItem>();
		private static Dictionary<Guid, Notes_CheckListItem> EVAnotes = new Dictionary<Guid, Notes_CheckListItem>();
		private static Dictionary<Guid, Notes_CheckListItem> SpacewalkNotes = new Dictionary<Guid, Notes_CheckListItem>();

		public static void registerEvents()
		{
			GameEvents.onLaunch.Add(onVesselLaunch);
			GameEvents.VesselSituation.onOrbit.Add(onVesselOrbit);
			GameEvents.VesselSituation.onLand.Add(onVesselLand);
			GameEvents.onPartCouple.Add(onDock);
			GameEvents.onVesselLoaded.Add(onVesselLoad);
			GameEvents.onVesselSituationChange.Add(onSituationChange);
			GameEvents.OnScienceRecieved.Add(onScienceReceive);
			GameEvents.onFlagPlant.Add(onFlagPlant);
			GameEvents.onCrewOnEva.Add(onEVA);
		}

		public static void deRegisterEvents()
		{
			GameEvents.onLaunch.Remove(onVesselLaunch);
			GameEvents.VesselSituation.onOrbit.Remove(onVesselOrbit);
			GameEvents.VesselSituation.onLand.Remove(onVesselLand);
			GameEvents.onPartCouple.Remove(onDock);
			GameEvents.onVesselLoaded.Remove(onVesselLoad);
			GameEvents.onVesselSituationChange.Remove(onSituationChange);
			GameEvents.OnScienceRecieved.Remove(onScienceReceive);
			GameEvents.onFlagPlant.Remove(onFlagPlant);
			GameEvents.onCrewOnEva.Remove(onEVA);
		}

		public static void registerCheckList(Notes_CheckListItem n)
		{
			switch (n.CheckType)
			{
				case Notes_CheckListType.launch:
					if (!launchNotes.ContainsKey(n.ID))
						launchNotes.Add(n.ID, n);
					break;
				case Notes_CheckListType.orbit:
					if (!orbitNotes.ContainsKey(n.ID))
						orbitNotes.Add(n.ID, n);
					break;
				case Notes_CheckListType.enterOrbit:
					if (!enterOrbitNotes.ContainsKey(n.ID))
						enterOrbitNotes.Add(n.ID, n);
					break;
				case Notes_CheckListType.returnToOrbit:
					if (!returnToOrbitNotes.ContainsKey(n.ID))
						returnToOrbitNotes.Add(n.ID, n);
					break;
				case Notes_CheckListType.land:
					if (!landNotes.ContainsKey(n.ID))
						landNotes.Add(n.ID, n);
					break;
				case Notes_CheckListType.plantFlag:
					if (!flagNotes.ContainsKey(n.ID))
						flagNotes.Add(n.ID, n);
					break;
				case Notes_CheckListType.spacewalk:
					if (!SpacewalkNotes.ContainsKey(n.ID))
						SpacewalkNotes.Add(n.ID, n);
					break;
				case Notes_CheckListType.surfaceEVA:
					if (!EVAnotes.ContainsKey(n.ID))
						EVAnotes.Add(n.ID, n);
					break;
				case Notes_CheckListType.returnHome:
					if (!returnHomeNotes.ContainsKey(n.ID))
						returnHomeNotes.Add(n.ID, n);
					break;
				case Notes_CheckListType.rendezvousVessel:
					if (!rendezvousVesselNotes.ContainsKey(n.ID))
						rendezvousVesselNotes.Add(n.ID, n);
					break;
				case Notes_CheckListType.dockVessel:
					if (!dockVesselNotes.ContainsKey(n.ID))
						dockVesselNotes.Add(n.ID, n);
					break;
				case Notes_CheckListType.rendezvousAsteroid:
					if (!rendezvousAsteroidNotes.ContainsKey(n.ID))
						rendezvousAsteroidNotes.Add(n.ID, n);
					break;
				case Notes_CheckListType.dockAsteroid:
					if (!dockAsteroidNotes.ContainsKey(n.ID))
						dockAsteroidNotes.Add(n.ID, n);
					break;
				case Notes_CheckListType.blastOff:
					if (!blastOffNotes.ContainsKey(n.ID))
						blastOffNotes.Add(n.ID, n);
					break;
				case Notes_CheckListType.science:
				case Notes_CheckListType.scienceFromPlanet:
					if (!scienceNotes.ContainsKey(n.ID))
						scienceNotes.Add(n.ID, n);
					break;
				default:
					break;
			}
		}

		public static void deRegisterCheckList(Notes_CheckListItem n)
		{
			switch (n.CheckType)
			{
				case Notes_CheckListType.launch:
					if (launchNotes.ContainsKey(n.ID))
						launchNotes.Remove(n.ID);
					break;
				case Notes_CheckListType.orbit:
					if (orbitNotes.ContainsKey(n.ID))
						orbitNotes.Remove(n.ID);
					break;
				case Notes_CheckListType.enterOrbit:
					if (enterOrbitNotes.ContainsKey(n.ID))
						enterOrbitNotes.Remove(n.ID);
					break;
				case Notes_CheckListType.returnToOrbit:
					if (returnToOrbitNotes.ContainsKey(n.ID))
						returnToOrbitNotes.Remove(n.ID);
					break;
				case Notes_CheckListType.land:
					if (landNotes.ContainsKey(n.ID))
						landNotes.Remove(n.ID);
					break;
				case Notes_CheckListType.plantFlag:
					if (flagNotes.ContainsKey(n.ID))
						flagNotes.Remove(n.ID);
					break;
				case Notes_CheckListType.spacewalk:
					if (SpacewalkNotes.ContainsKey(n.ID))
						SpacewalkNotes.Remove(n.ID);
					break;
				case Notes_CheckListType.surfaceEVA:
					if (EVAnotes.ContainsKey(n.ID))
						EVAnotes.Remove(n.ID);
					break;
				case Notes_CheckListType.returnHome:
					if (returnHomeNotes.ContainsKey(n.ID))
						returnHomeNotes.Remove(n.ID);
					break;
				case Notes_CheckListType.rendezvousVessel:
					if (rendezvousVesselNotes.ContainsKey(n.ID))
						rendezvousVesselNotes.Remove(n.ID);
					break;
				case Notes_CheckListType.dockVessel:
					if (dockVesselNotes.ContainsKey(n.ID))
						dockVesselNotes.Remove(n.ID);
					break;
				case Notes_CheckListType.rendezvousAsteroid:
					if (rendezvousAsteroidNotes.ContainsKey(n.ID))
						rendezvousAsteroidNotes.Remove(n.ID);
					break;
				case Notes_CheckListType.dockAsteroid:
					if (dockAsteroidNotes.ContainsKey(n.ID))
						dockAsteroidNotes.Remove(n.ID);
					break;
				case Notes_CheckListType.blastOff:
					if (blastOffNotes.ContainsKey(n.ID))
						blastOffNotes.Remove(n.ID);
					break;
				case Notes_CheckListType.science:
				case Notes_CheckListType.scienceFromPlanet:
					if (scienceNotes.ContainsKey(n.ID))
						scienceNotes.Remove(n.ID);
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

			IEnumerable<KeyValuePair<Guid, Notes_CheckListItem>> notes = orbitNotes.Where(n => n.Value.Root.RootVessel.id == v.id);
			for (int i = 0; i < notes.Count(); i++)
			{
				Notes_CheckListItem n = notes.ElementAt(i).Value;

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

			IEnumerable<KeyValuePair<Guid, Notes_CheckListItem>> notes;

			if (c == Planetarium.fetch.Home)
			{
				notes = returnHomeNotes.Where(n => n.Value.Root.RootVessel.id == v.id);
				for (int i = 0; i < notes.Count(); i++)
				{
					Notes_CheckListItem n = notes.ElementAt(i).Value;

					if (n == null)
						continue;

					n.setComplete();
				}
			}

			notes = landNotes.Where(n => n.Value.Root.RootVessel.id == v.id);
			for (int i = 0; i < notes.Count(); i++)
			{
				Notes_CheckListItem n = notes.ElementAt(i).Value;

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

			IEnumerable<KeyValuePair<Guid, Notes_CheckListItem>> notes = launchNotes.Where(n => n.Value.Root.RootVessel.id == v.id);
			for (int i = 0; i < notes.Count(); i++)
			{
				Notes_CheckListItem n = notes.ElementAt(i).Value;

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

			IEnumerable<KeyValuePair<Guid, Notes_CheckListItem>> fromNotes;
			IEnumerable<KeyValuePair<Guid, Notes_CheckListItem>> toNotes;

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
					Notes_CheckListItem n = fromNotes.ElementAt(i).Value;

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
					Notes_CheckListItem n = toNotes.ElementAt(i).Value;

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
			IEnumerable<KeyValuePair<Guid, Notes_CheckListItem>> activeNotes;
			IEnumerable<KeyValuePair<Guid, Notes_CheckListItem>> loadedNotes;

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
					Notes_CheckListItem n = activeNotes.ElementAt(i).Value;

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
					Notes_CheckListItem n = loadedNotes.ElementAt(i).Value;

					if (n == null)
						continue;

					if (n.TargetVessel == null)
						continue;

					if (n.TargetVessel.id == active.id)
						n.setComplete();
				}
			}
		}

		private static void onSituationChange(GameEvents.HostedFromToAction<Vessel, Vessel.Situations> VS)
		{
			if (VS.host == null)
				return;

			if (VS.from == Vessel.Situations.LANDED || VS.from == Vessel.Situations.SPLASHED)
			{
				if (VS.to == Vessel.Situations.FLYING || VS.to == Vessel.Situations.SUB_ORBITAL)
				{
					IEnumerable<KeyValuePair<Guid, Notes_CheckListItem>> notes = blastOffNotes.Where(n => n.Value.Root.RootVessel.id == VS.host.id);
					for (int i = 0; i < notes.Count(); i++)
					{
						Notes_CheckListItem n = notes.ElementAt(i).Value;

						if (n == null)
							continue;

						if (n.TargetBody != VS.host.mainBody)
							continue;

						Notes_Core.Instance.CheckListMono.startBlastOffWatcher(VS.host, n);
					}
				}
			}

			if (VS.from == Vessel.Situations.SUB_ORBITAL || VS.from == Vessel.Situations.FLYING)
			{
				if (VS.to == Vessel.Situations.ORBITING)
				{
					IEnumerable<KeyValuePair<Guid, Notes_CheckListItem>> notes = returnToOrbitNotes.Where(n => n.Value.Root.RootVessel.id == VS.host.id);
					for (int i = 0; i < notes.Count(); i++)
					{
						Notes_CheckListItem n = notes.ElementAt(i).Value;

						if (n == null)
							continue;

						if (n.TargetBody != VS.host.mainBody)
							continue;

						n.setComplete();
					}
				}
			}

			if (VS.from == Vessel.Situations.ESCAPING)
			{
				if (VS.to == Vessel.Situations.ORBITING)
				{
					IEnumerable<KeyValuePair<Guid, Notes_CheckListItem>> notes = enterOrbitNotes.Where(n => n.Value.Root.RootVessel.id == VS.host.id);
					for (int i = 0; i < notes.Count(); i++)
					{
						Notes_CheckListItem n = notes.ElementAt(i).Value;

						if (n == null)
							continue;

						if (n.TargetBody != VS.host.mainBody)
							continue;

						n.setComplete();
					}
				}
			}
		}

		private static void onFlagPlant(Vessel v)
		{
			if (v == null)
				return; ;

			IEnumerable<KeyValuePair<Guid, Notes_CheckListItem>> notes = flagNotes.Where(n => n.Value.TargetBody == v.mainBody);
			for (int i = 0; i < notes.Count(); i++)
			{
				Notes_CheckListItem n = notes.ElementAt(i).Value;

				if (n == null)
					continue;

				n.setComplete();
			}
		}

		private static void onEVA(GameEvents.FromToAction<Part, Part> f)
		{
			if (f.from == null)
				return;

			if (f.from.vessel == null)
				return;

			Vessel.Situations sit = f.from.vessel.situation;

			if (f.from.vessel.LandedOrSplashed)
			{
				IEnumerable<KeyValuePair<Guid, Notes_CheckListItem>> notes = EVAnotes.Where(n => n.Value.TargetBody == f.from.vessel.mainBody);
				for (int i = 0; i < notes.Count(); i++)
				{
					Notes_CheckListItem n = notes.ElementAt(i).Value;

					if (n == null)
						continue;

					n.setComplete();
				}
			}
			else if (sit == Vessel.Situations.ESCAPING || sit == Vessel.Situations.ORBITING || sit == Vessel.Situations.SUB_ORBITAL)
			{
				IEnumerable<KeyValuePair<Guid, Notes_CheckListItem>> notes = SpacewalkNotes.Where(n => n.Value.TargetBody == f.from.vessel.mainBody);
				for (int i = 0; i < notes.Count(); i++)
				{
					Notes_CheckListItem n = notes.ElementAt(i).Value;

					if (n == null)
						continue;

					n.setComplete();
				}
			}
		}

		private static void onScienceReceive(float data, ScienceSubject sub, ProtoVessel pv, bool b)
		{
			if (pv == null)
				return;

			if (sub == null)
				return;

			if (pv.vesselRef.mainBody == null)
				return;

			IEnumerable<KeyValuePair<Guid, Notes_CheckListItem>> notes = scienceNotes.Where(n => n.Value.Root.RootVessel.id == pv.vesselID);
			for (int i = 0; i < notes.Count(); i++)
			{
				Notes_CheckListItem n = notes.ElementAt(i).Value;

				if (n == null)
					continue;

				if (n.Data == null)
					continue;

				if (n.CheckType == Notes_CheckListType.scienceFromPlanet)
				{
					if (n.TargetBody == null)
						continue;

					if (!sub.IsFromBody(n.TargetBody))
						continue;
				}

				n.Data -= data;

				switch (n.CheckType)
				{
					case Notes_CheckListType.science:
						n.Text =  string.Format("Return {0:F0} more science data", n.Data);
						break;
					case Notes_CheckListType.scienceFromPlanet:
						n.Text = string.Format("Return {0:F0} more science data from {1}", n.Data, n.TargetBody.theName);
						break;
				}

				if (n.Data <= 0)
					n.setComplete();
			}
		}

	}
}
