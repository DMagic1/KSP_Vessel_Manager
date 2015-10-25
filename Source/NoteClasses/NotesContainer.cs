using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using BetterNotes.Framework;

namespace BetterNotes.NoteClasses
{
	public class NotesContainer
	{
		private NotesExpContainer experiments;
		private NotesDataContainer data;
		private NotesContractContainer contracts;
		private NotesCrewContainer crew;
		private NotesTextContainer notes;
		private NotesCheckListContainer checkList;
		private NotesVitalStats stats;
		private NotesVesselLog log;

		private Vessel vessel;
		private Guid id;
		private NotesContainer container;

		public NotesContainer(Vessel v)
		{
			vessel = v;
			id = v.id;
			container = this;
			stats = new NotesVitalStats(container);
			log = new NotesVesselLog(container);
			notes = new NotesTextContainer(container);
			checkList = new NotesCheckListContainer(container);
			crew = new NotesCrewContainer(container);
			contracts = new NotesContractContainer(container);
			data = new NotesDataContainer(container);
			experiments = new NotesExpContainer(container);
		}

		public void loadVitalStats(NotesVitalStats s)
		{
			stats = new NotesVitalStats(s, this);
		}

		public void loadVesselLog(NotesVesselLog l)
		{
			log = new NotesVesselLog(l, this);
		}

		public void loadContracts(NotesContractContainer c, List<Guid> ids)
		{
			contracts = new NotesContractContainer(c, ids, this);
		}

		public void loadTextNotes(NotesTextContainer t)
		{
			notes = new NotesTextContainer(t, this);
		}

		public void loadCheckList(NotesCheckListContainer c)
		{
			checkList = new NotesCheckListContainer(c, this);
		}

		public void loadDataNotes(NotesDataContainer d)
		{
			data = new NotesDataContainer(d, this);
		}

		public void vesselRefresh()
		{

		}

		public void contractsRefresh()
		{
			if (contracts == null)
				return;

			contracts.contractsRefresh();
		}

		public Guid ID
		{
			get { return id; }
		}

		public Vessel NotesVessel
		{
			get { return vessel; }
		}

		public NotesExpContainer Experiments
		{
			get { return experiments; }
		}
		public NotesDataContainer Data
		{
			get { return data; }
		}
		public NotesContractContainer Contracts
		{
			get { return contracts; }
		}
		public NotesCrewContainer Crew
		{
			get { return crew; }
		}
		public NotesTextContainer Notes
		{
			get { return notes; }
		}
		public NotesCheckListContainer CheckList
		{
			get { return checkList; }
		}
		public NotesVitalStats Stats
		{
			get { return stats; }
		}
		public NotesVesselLog Log
		{
			get { return log; }
		}
	}
}
