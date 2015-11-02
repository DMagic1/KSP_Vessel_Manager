using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using BetterNotes.Framework;

namespace BetterNotes.NoteClasses
{
	public class Notes_Container
	{
		private Notes_ExpContainer experiments;
		private Notes_DataContainer data;
		private Notes_ContractContainer contracts;
		private Notes_CrewContainer crew;
		private Notes_TextContainer notes;
		private Notes_CheckListContainer checkList;
		private Notes_VitalStats stats;
		private Notes_VesselLog log;

		private Vessel vessel;
		private Guid id;
		private Notes_Container container;

		public Notes_Container(Vessel v)
		{
			vessel = v;
			id = v.id;
			container = this;
			stats = new Notes_VitalStats(container);
			log = new Notes_VesselLog(container);
			notes = new Notes_TextContainer(container);
			checkList = new Notes_CheckListContainer(container);
			crew = new Notes_CrewContainer(container);
			contracts = new Notes_ContractContainer(container);
			data = new Notes_DataContainer(container);
			experiments = new Notes_ExpContainer(container);
		}

		public void loadVitalStats(Notes_VitalStats s)
		{
			stats = new Notes_VitalStats(s, this);
		}

		public void loadVesselLog(Notes_VesselLog l)
		{
			log = new Notes_VesselLog(l, this);
		}

		public void loadContracts(Notes_ContractContainer c, List<Guid> ids)
		{
			contracts = new Notes_ContractContainer(c, ids, this);
		}

		public void loadTextNotes(Notes_TextContainer t)
		{
			notes = new Notes_TextContainer(t, this);
		}

		public void loadCheckList(Notes_CheckListContainer c)
		{
			checkList = new Notes_CheckListContainer(c, this);
		}

		public void loadDataNotes(Notes_DataContainer d)
		{
			data = new Notes_DataContainer(d, this);
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

		public Notes_ExpContainer Experiments
		{
			get { return experiments; }
		}
		public Notes_DataContainer Data
		{
			get { return data; }
		}
		public Notes_ContractContainer Contracts
		{
			get { return contracts; }
		}
		public Notes_CrewContainer Crew
		{
			get { return crew; }
		}
		public Notes_TextContainer Notes
		{
			get { return notes; }
		}
		public Notes_CheckListContainer CheckList
		{
			get { return checkList; }
		}
		public Notes_VitalStats Stats
		{
			get { return stats; }
		}
		public Notes_VesselLog Log
		{
			get { return log; }
		}
	}
}
