using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using BetterNotes.Framework;
using BetterNotes.NoteClasses;
using BetterNotes.NoteClasses.CheckListHandler;
using Contracts;

namespace BetterNotes
{
	[KSPAddon(KSPAddon.Startup.MainMenu, true)]
	public class NotesCore : Notes_MBE
	{
		private static bool loaded = false;
		private static NotesCore instance;
		private NotesCheckListMonoBehaviour checkListMono;
		private Dictionary<Guid, Vessel> activeVessels = new Dictionary<Guid, Vessel>();
		private Dictionary<Guid, Vessel> allVessels = new Dictionary<Guid, Vessel>();
		private Dictionary<Guid, NotesContainer> allNotes = new Dictionary<Guid, NotesContainer>();
		private Dictionary<Guid, NotesContractInfo> allContracts = new Dictionary<Guid, NotesContractInfo>();

		private Vessel activeVessel;

		public static NotesCore Instance
		{
			get { return instance; }
		}

		public NotesCheckListMonoBehaviour CheckListMono
		{
			get { return checkListMono; }
		}

		public void addNotes(NotesContainer n)
		{
			if (allNotes.ContainsKey(n.ID))
			{
				Debug.LogWarning("BetterNotes: Note already present with key: " + n.ID);
			}
			else
				allNotes.Add(n.ID, n);
		}

		public NotesContainer getNotes(Guid id)
		{
			if (allNotes.ContainsKey(id))
				return allNotes[id];

			return null;
		}

		public int notesCount
		{
			get { return allNotes.Count; }
		}

		public NotesContainer getNotes(int index, bool warn = false)
		{
			if (allNotes.Count > index)
				return allNotes.ElementAt(index).Value;
			else if (warn)
				Debug.LogWarning("Notes dictionary index out of range; something went wrong here...");

			return null;
		}

		public void addContract(NotesContractInfo C)
		{
			if (!allContracts.ContainsKey(C.ID))
				allContracts.Add(C.ID, C);
		}

		public void removeContract(NotesContractInfo C)
		{
			if (allContracts.ContainsKey(C.ID))
				allContracts.Remove(C.ID);
		}

		public NotesContractInfo getContract(Guid id)
		{
			if (allContracts.ContainsKey(id))
				return allContracts[id];

			return null;
		}

		protected override void Start()
		{
			if (!loaded)
			{
				DontDestroyOnLoad(this);
				loaded = true;
				startup();

				checkListMono = gameObject.AddComponent<NotesCheckListMonoBehaviour>();
			}
		}

		private void startup()
		{
			instance = this;
			GameEvents.onNewVesselCreated.Add(vesselRefresh);
			GameEvents.onVesselWasModified.Add(vesselRefresh);
			GameEvents.onVesselChange.Add(vesselRefresh);
			GameEvents.OnScienceRecieved.Add(onScienceTransmit);
			GameEvents.Contract.onAccepted.Add(onAddContract);
			GameEvents.Contract.onFinished.Add(onFinishContract);
			GameEvents.Contract.onContractsLoaded.Add(onLoadContracts);

			NotesCheckListTypeHandler.registerEvents();

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
			GameEvents.Contract.onAccepted.Remove(onAddContract);
			GameEvents.Contract.onFinished.Remove(onFinishContract);
			GameEvents.Contract.onContractsLoaded.Remove(onLoadContracts);

			NotesCheckListTypeHandler.deRegisterEvents();

			if (checkListMono != null)
				Destroy(checkListMono);
		}

		private void onAddContract(Contract c)
		{
			NotesContractInfo N = new NotesContractInfo(c);

			addContract(N);

			refreshContracts();
		}

		private void onFinishContract(Contract c)
		{
			NotesContractInfo C = getContract(c.ContractGuid);

			if (C != null)
				removeContract(C);

			refreshContracts();
		}

		private void refreshContracts()
		{
			NotesContainer n = getNotes(activeVessel.id);

			if (n == null)
				return;

			n.contractsRefresh();
		}

		private void vesselRefresh(Vessel v)
		{
			activeVessel = v;

			NotesContainer n = getNotes(v.id);

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

			NotesContainer n = getNotes(pV.vesselID);

			if (n == null)
				return;

			if (n.Data == null)
				return;

			double time = Planetarium.GetUniversalTime();

			NotesReceivedData o = new NotesReceivedData(sub, value, (int)time);

			n.Data.addReturnedData(o);
		}

		private void onLoadContracts()
		{
			StartCoroutine(loadContracts());
		}

		private IEnumerator loadContracts()
		{
			int t = 0;

			//Agency modifiers don't seem to work unless I wait a few frames before loading contracts
			while (t < 5)
			{
				t++;
				yield return null;
			}

			for (int i = 0; i < ContractSystem.Instance.Contracts.Count; i++)
			{
				Contract c = ContractSystem.Instance.Contracts[i];

				if (c == null)
					continue;

				NotesContractInfo n = new NotesContractInfo(c);

				if (c == null)
					continue;

				if (c.Root == null)
					continue;

				addContract(n);
			}

			LogFormatted("{0} Contracts Loaded", allContracts.Count);

			foreach (NotesContainer n in allNotes.Values)
			{
				if (n == null)
					continue;

				n.contractsRefresh();
			}
		}
	}
}
