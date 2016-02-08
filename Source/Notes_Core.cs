using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using BetterNotes.Framework;
using BetterNotes.NoteClasses;
using BetterNotes.NoteClasses.CheckListHandler;
using Contracts;
using ContractParser;
using ProgressParser;

namespace BetterNotes
{
	[KSPAddon(KSPAddon.Startup.MainMenu, true)]
	public class Notes_Core : Notes_MBE
	{
		private static bool loaded = false;
		private static Notes_Core instance;
		private Notes_CheckListMonoBehaviour checkListMono;
		private Dictionary<Guid, Vessel> activeVessels = new Dictionary<Guid, Vessel>();
		private Dictionary<Guid, Vessel> allVessels = new Dictionary<Guid, Vessel>();
		private Dictionary<Guid, Notes_Archive_Container> archivedNotes = new Dictionary<Guid, Notes_Archive_Container>();
		private Dictionary<Guid, Notes_Container> allNotes = new Dictionary<Guid, Notes_Container>();
		private Dictionary<string, List<Guid>> CWmissionLists = new Dictionary<string, List<Guid>>();

		private Vessel activeVessel;

		public static Notes_Core Instance
		{
			get { return instance; }
		}

		public Notes_CheckListMonoBehaviour CheckListMono
		{
			get { return checkListMono; }
		}

		public void addNotes(Notes_Container n)
		{
			if (allNotes.ContainsKey(n.ID))
				Debug.LogWarning("BetterNotes: Note already present with key: " + n.ID);
			else
				allNotes.Add(n.ID, n);
		}

		public Notes_Container getNotes(Guid id)
		{
			if (allNotes.ContainsKey(id))
				return allNotes[id];

			return null;
		}

		public int notesCount
		{
			get { return allNotes.Count; }
		}

		public Notes_Container getNotes(int index, bool warn = false)
		{
			if (allNotes.Count > index)
				return allNotes.ElementAt(index).Value;
			else if (warn)
				Debug.LogWarning("Notes dictionary index out of range; something went wrong here...");

			return null;
		}

		public void addArchivedNotes(Notes_Archive_Container n)
		{
			if (archivedNotes.ContainsKey(n.ID))
				Debug.LogWarning("BetterNotes: Note already present with key: " + n.ID);
			else
				archivedNotes.Add(n.ID, n);
		}

		public Notes_Archive_Container getArchivedNotes(Guid id)
		{
			if (archivedNotes.ContainsKey(id))
				return archivedNotes[id];

			return null;
		}

		public int archivedNotesCount
		{
			get { return archivedNotes.Count; }
		}

		public Notes_Archive_Container getArchivedNotes(int index, bool warn = false)
		{
			if (archivedNotes.Count > index)
				return archivedNotes.ElementAt(index).Value;
			else if (warn)
				Debug.LogWarning("Archived Notes dictionary index out of range; something went wrong here...");

			return null;
		}

		protected override void Start()
		{
			if (!loaded)
			{
				DontDestroyOnLoad(this);
				loaded = true;
				startup();

				checkListMono = gameObject.AddComponent<Notes_CheckListMonoBehaviour>();
			}
		}

		private void startup()
		{
			instance = this;
			GameEvents.onNewVesselCreated.Add(vesselRefresh);
			GameEvents.onVesselWasModified.Add(vesselRefresh);
			GameEvents.onVesselChange.Add(vesselRefresh);
			GameEvents.OnScienceRecieved.Add(onScienceTransmit);
			GameEvents.onVesselRecovered.Add(onVesselRecovered);
			GameEvents.Contract.onAccepted.Add(onAddContract);
			GameEvents.Contract.onFinished.Add(onFinishContract);
			contractParser.onContractsParsed.Add(onLoadContracts);
			progressParser.onProgressParsed.Add(onLoadProgress);

			Notes_CheckListTypeHandler.registerEvents();

			if (HighLogic.LoadedSceneIsFlight)
				activeVessel = FlightGlobals.ActiveVessel;

			for (int i = 0; i < FlightGlobals.Vessels.Count; i++)
			{
				Vessel v = FlightGlobals.Vessels[i];

				if (v == null)
					continue;

				if (activeVessels.ContainsKey(v.id))
					continue;

				activeVessels.Add(v.id, v);
			}
		}

		protected override void OnDestroy()
		{
			GameEvents.onNewVesselCreated.Remove(vesselRefresh);
			GameEvents.onVesselWasModified.Remove(vesselRefresh);
			GameEvents.onVesselChange.Remove(vesselRefresh);
			GameEvents.OnScienceRecieved.Remove(onScienceTransmit);
			GameEvents.onVesselRecovered.Add(onVesselRecovered);
			GameEvents.Contract.onAccepted.Remove(onAddContract);
			GameEvents.Contract.onFinished.Remove(onFinishContract);
			contractParser.onContractsParsed.Remove(onLoadContracts);
			progressParser.onProgressParsed.Remove(onLoadProgress);

			Notes_CheckListTypeHandler.deRegisterEvents();

			if (checkListMono != null)
				Destroy(checkListMono);
		}

		private void onAddContract(Contract c)
		{
			refreshContracts();
		}

		private void onFinishContract(Contract c)
		{
			refreshContracts();
		}

		private void refreshContracts()
		{
			Notes_Container n = getNotes(activeVessel.id);

			if (n == null)
				return;

			n.contractsRefresh();
		}

		private void vesselRefresh(Vessel v)
		{
			activeVessel = v;

			Notes_Container n = getNotes(v.id);

			if (n == null)
				return;

			n.vesselRefresh();
		}

		private void onScienceTransmit(float value, ScienceSubject sub, ProtoVessel pV, bool b)
		{
			if (pV == null)
				return;

			if (pV.vesselID == null)
				return;

			Notes_Container n = getNotes(pV.vesselID);

			if (n == null)
				return;

			if (n.Data == null)
				return;

			double time = Planetarium.GetUniversalTime();

			Notes_ReceivedData o = new Notes_ReceivedData(sub, value, (int)time, n.Data);

			n.Data.addReturnedData(o);
		}

		private void onVesselRecovered(ProtoVessel v)
		{
			Notes_Container container = getNotes(v.vesselID);

			if (container == null)
				return;

			Notes_Archive_Container n = new Notes_Archive_Container(v.vesselID, v.vesselName, Planetarium.GetUniversalTime(), v.missionTime, v.vesselType);

			n.loadCheckList(container.CheckList);
			n.loadContracts(container.Contracts, container.Contracts.getAllActiveContractIDs.ToList());
			n.loadDataNotes(container.Data);
			n.loadTextNotes(container.Notes);
			n.loadVesselLog(container.Log);

			addArchivedNotes(n);
		}

		private void onLoadProgress()
		{
			StartCoroutine(loadProgress());
		}

		private IEnumerator loadProgress()
		{
			int i = 0;

			while (!progressParser.Loaded && i < 200)
			{
				i++;
				yield return null;
			}


		}

		private void onLoadContracts()
		{
			StartCoroutine(loadContracts());
		}

		private IEnumerator loadContracts()
		{
			int t = 0;

			foreach (Notes_Container n in allNotes.Values)
			{
				if (n == null)
					continue;

				n.contractsRefresh();
			}

			if (Notes_MainMenu.ContractsPlusLoaded)
			{
				t = 0;
				while (!Notes_AssemblyLoad.ContractMissionListsLoaded() && t < 550)
				{
					t++;
					yield return null;
				}

				loadCWmissionLists();
			}
		}

		private void loadCWmissionLists()
		{
			if (!Notes_MainMenu.ContractsPlusLoaded)
				return;

			CWmissionLists.Clear();

			var strings = Notes_AssemblyLoad.GetContractMissionNames();

			for (int i = 0; i < strings.Count(); i++)
			{
				string n = strings.ElementAt(i);

				if (string.IsNullOrEmpty(n))
					continue;

				var ids = Notes_AssemblyLoad.GetContractMission(n);

				if (ids == null)
					continue;

				addMissionList(n, ids.ToList());
			}
		}

		private void addMissionList(string name, List<Guid> ids)
		{
			if (!CWmissionLists.ContainsKey(name))
				CWmissionLists.Add(name, ids);
			else
				CWmissionLists[name] = ids;
		}

		private void removeMissionList(string name)
		{
			if (CWmissionLists.ContainsKey(name))
				CWmissionLists.Remove(name);
		}

		public void refreshMissionLists()
		{
			if (!Notes_MainMenu.ContractsPlusLoaded)
				return;

			loadCWmissionLists();
		}

	}
}
