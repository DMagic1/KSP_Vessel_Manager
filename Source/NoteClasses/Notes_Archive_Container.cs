using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using BetterNotes.Framework;

namespace BetterNotes.NoteClasses
{
	public class Notes_Archive_Container
	{
		private Notes_DataContainer data;
		private Notes_ContractContainer contracts;
		private Notes_TextContainer notes;
		private Notes_CheckListContainer checkList;
		private Notes_VesselLog log;
		private Notes_Archived_Crew_Container crew;

		private Guid id;
		private string vesselName;
		private double recoveryTime;
		private double met;
		private VesselType vtype;
		private Notes_Archive_Container container;

		public Notes_Archive_Container(Guid gid, string name, double time, double m, VesselType t)
		{
			id = gid;
			vesselName = name;
			recoveryTime = time;
			met = m;
			vtype = t;
			container = this;
			log = new Notes_VesselLog(container);
			notes = new Notes_TextContainer(container);
			checkList = new Notes_CheckListContainer(container);
			contracts = new Notes_ContractContainer(container);
			data = new Notes_DataContainer(container);
			crew = new Notes_Archived_Crew_Container(container);
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

		public void loadCrewNotes(Notes_Archived_Crew_Container c)
		{
			crew = new Notes_Archived_Crew_Container(c, this);
		}

		public Guid ID
		{
			get { return id; }
		}
		public string VesselName
		{
			get { return vesselName; }
		}
		public VesselType VType
		{
			get { return vtype; }
		}
		public double MET
		{
			get { return met; }
		}
		public double RecoveryTime
		{
			get { return recoveryTime; }
		}

		public Notes_DataContainer Data
		{
			get { return data; }
		}
		public Notes_ContractContainer Contracts
		{
			get { return contracts; }
		}
		public Notes_TextContainer Notes
		{
			get { return notes; }
		}
		public Notes_CheckListContainer CheckList
		{
			get { return checkList; }
		}
		public Notes_VesselLog Log
		{
			get { return log; }
		}
		public Notes_Archived_Crew_Container Crew
		{
			get { return crew; }
		}
	}
}
