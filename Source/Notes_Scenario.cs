using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using BetterNotes.Framework;
using BetterNotes.NoteClasses;
using BetterNotes.NoteClasses.CheckListHandler;

namespace BetterNotes
{
	[KSPScenario(ScenarioCreationOptions.AddToAllGames)]
	public class Notes_Scenario : ScenarioModule
	{
		private static Notes_Scenario instance;
		private Notes_Core core;

		public static Notes_Scenario Instance
		{
			get { return instance; }
		}

		public override void OnLoad(ConfigNode node)
		{
			if (core == null)
				return;

			ConfigNode vessels = node.GetNode("VESSELS");

			if (vessels != null)
			{
				for (int i = 0; i < vessels.GetNodes("VESSEL_NOTES").Length; i++)
				{
					ConfigNode vesselNotes = vessels.GetNodes("VESSEL_NOTES")[i];

					if (vesselNotes == null)
						continue;

					Vessel v;

					try
					{
						Guid vID = new Guid(vesselNotes.GetValue("VESSEL_ID"));

						if (vID == null)
							continue;

						v = FlightGlobals.Vessels.FirstOrDefault(a => a.id == vID);

						if (v == null)
							continue;
					}
					catch (Exception e)
					{
						v = null;
						Debug.LogError("BetterNotes; error while loading vessel\n" + e);
						continue;
					}

					Notes_Container n = new Notes_Container(v);

					ConfigNode vesselStats = vesselNotes.GetNode("VESSEL_STATS");

					if (vesselStats != null)
					{
						n.loadVitalStats(loadStats(vesselStats));
					}

					ConfigNode contracts = vesselNotes.GetNode("VESSEL_CONTRACTS");

					if (contracts != null)
					{
						Notes_ContractContainer c = new Notes_ContractContainer();

						n.loadContracts(c, loadContracts(contracts));
					}

					ConfigNode dataNotes = vesselNotes.GetNode("VESSEL_SCIENCE_DATA");

					if (dataNotes != null)
					{
						n.loadDataNotes(loadData(dataNotes));
					}

					ConfigNode textNotes = vesselNotes.GetNode("VESSEL_TEXT_NOTES");

					if (textNotes != null)
					{
						n.loadTextNotes(loadTextNotes(textNotes));
					}

					ConfigNode checkList = vesselNotes.GetNode("CHECK_LIST");

					if (checkList != null)
					{
						n.loadCheckList(loadCheckList(checkList, false));
					}

					ConfigNode vesselLog = vesselNotes.GetNode("VESSEL_LOG");

					if (vesselLog != null)
					{
						n.loadVesselLog(loadLog(vesselLog));
					}

					core.addNotes(n);
				}
			}

			ConfigNode archivedVessels = node.GetNode("ARCHIVED_VESSELS");

			if (archivedVessels != null)
			{
				for (int i = 0; i < archivedVessels.GetNodes("ARCHIVED_VESSEL_NOTES").Length; i++)
				{
					ConfigNode vesselNotes = archivedVessels.GetNodes("ARCHIVED_VESSEL_NOTES")[i];

					if (vesselNotes == null)
						continue;

					Guid vID;

					try
					{
						vID = new Guid(vesselNotes.GetValue("VESSEL_ID"));
					}
					catch (Exception e)
					{
						Debug.LogError("BetterNotes; error while loading archived vessel\n" + e);
						continue;
					}

					if (vID == null)
						continue;

					string name = vesselNotes.parse("VESSEL_NAME", "");
					double rTime = vesselNotes.parse("RECOVERY_TIME", 0d);
					double mTime = vesselNotes.parse("MISSION_TIME", 0d);
					VesselType vT = vesselNotes.parse("VESSEL_TYPE", VesselType.Unknown);

					Notes_Archive_Container n = new Notes_Archive_Container(vID, name, rTime, mTime, vT);

					ConfigNode crew = vesselNotes.GetNode("ARCHIVED_CREW");

					if (crew != null)
					{
						n.loadCrewNotes(loadCrew(crew));
					}

					ConfigNode contracts = vesselNotes.GetNode("VESSEL_CONTRACTS");

					if (contracts != null)
					{
						Notes_ContractContainer c = new Notes_ContractContainer();

						n.loadContracts(c, loadContracts(contracts));
					}

					ConfigNode dataNotes = vesselNotes.GetNode("VESSEL_SCIENCE_DATA");

					if (dataNotes != null)
					{
						n.loadDataNotes(loadData(dataNotes));
					}

					ConfigNode textNotes = vesselNotes.GetNode("VESSEL_TEXT_NOTES");

					if (textNotes != null)
					{
						n.loadTextNotes(loadTextNotes(textNotes));
					}

					ConfigNode checkList = vesselNotes.GetNode("CHECK_LIST");

					if (checkList != null)
					{
						n.loadCheckList(loadCheckList(checkList, true));
					}

					ConfigNode vesselLog = vesselNotes.GetNode("VESSEL_LOG");

					if (vesselLog != null)
					{
						n.loadVesselLog(loadLog(vesselLog));
					}

					core.addArchivedNotes(n);
				}
			}
		}

		private Notes_VitalStats loadStats(ConfigNode node)
		{
			return new Notes_VitalStats();
		}

		private Notes_Archived_Crew_Container loadCrew(ConfigNode node)
		{
			Notes_Archived_Crew_Container c = new Notes_Archived_Crew_Container();

			for (int i = 0; i < node.GetValues("KERBAL").Length; i++)
			{
				string k = node.GetValues("KERBAL")[i];

				if (string.IsNullOrEmpty(k))
					continue;

				if (HighLogic.CurrentGame.CrewRoster == null)
					break;

				ProtoCrewMember p = HighLogic.CurrentGame.CrewRoster[k];

				if (p == null)
					continue;

				c.addCrewObject(p);
			}

			return c;
		}

		private List<Guid> loadContracts(ConfigNode node)
		{
			return node.parse("CONTRACTS", new List<Guid>());
		}

		private Notes_DataContainer loadData(ConfigNode node)
		{
			Notes_DataContainer d = new Notes_DataContainer();

			for (int k = 0; k < node.GetNodes("COMPLETED_DATA").Length; k++)
			{
				ConfigNode s = node.GetNodes("COMPLETED_DATA")[k];

				if (s == null)
					continue;

				ScienceSubject sub = s.parse("SCIENCE_ID", (ScienceSubject)null);

				if (sub == null)
					continue;

				float value = s.parse("SCIENCE_VALUE", 0f);
				int time = s.parse("TIME_RECEIVED", (int)0);

				Notes_ReceivedData o = new Notes_ReceivedData(sub, value, time, d);

				d.addReturnedData(o);
			}

			return d;
		}

		private Notes_TextContainer loadTextNotes(ConfigNode node)
		{
			Notes_TextContainer t = new Notes_TextContainer();

			for (int j = 0; j < node.GetNodes("NOTE").Length; j++)
			{
				ConfigNode note = node.GetNodes("NOTE")[j];

				if (note == null)
					continue;

				if (!note.HasValue("TITLE"))
					continue;
				if (!note.HasValue("NOTE"))
					continue;

				string title = note.GetValue("TITLE");
				string text = note.GetValue("NOTE");
				Guid id = note.parse("KEY", Guid.NewGuid());
				DateTime create = note.parse("CREATE_TIME", new DateTime());
				DateTime edit = note.parse("EDIT_TIME", new DateTime());

				Notes_TextItem newNote = new Notes_TextItem(text, title, id, create, edit, t);

				t.addNote(newNote);
			}

			return t;
		}

		private Notes_CheckListContainer loadCheckList(ConfigNode node, bool archived)
		{
			Notes_CheckListContainer c = new Notes_CheckListContainer();

			for (int j = 0; j < node.GetNodes("CHECK_LIST_ITEM").Length; j++)
			{
				ConfigNode checkListItem = node.GetNodes("CHECK_LIST_ITEM")[j];

				if (checkListItem == null)
					continue;

				if (!checkListItem.HasValue("NOTE"))
					continue;

				string text = checkListItem.GetValue("NOTE");
				int order = checkListItem.parse("ORDER", j);
				bool complete = checkListItem.parse("COMPLETE", false);
				float? data = checkListItem.parse("DATA", (float?)null);
				Guid id = checkListItem.parse("KEY", Guid.NewGuid());
				Notes_CheckListType type = checkListItem.parse("TYPE", Notes_CheckListType.custom);

				Vessel targetV = null;
				CelestialBody targetB = null;

				if (!archived)
				{
					targetV = checkListItem.parse("TARGET_VESSEL", (Vessel)null);
					targetB = checkListItem.parse("TARGET_BODY", (CelestialBody)null);
				}

				if (!complete && !archived)
				{
					switch (type)
					{
						case Notes_CheckListType.dockVessel:
						case Notes_CheckListType.dockAsteroid:
						case Notes_CheckListType.rendezvousVessel:
						case Notes_CheckListType.rendezvousAsteroid:
							if (targetV == null)
								continue;
							break;
						case Notes_CheckListType.enterOrbit:
						case Notes_CheckListType.land:
						case Notes_CheckListType.orbit:
						case Notes_CheckListType.blastOff:
						case Notes_CheckListType.returnToOrbit:
						case Notes_CheckListType.scienceFromPlanet:
							if (targetB == null)
								continue;
							break;
						case Notes_CheckListType.launch:
						case Notes_CheckListType.returnHome:
							if (targetB == null)
								targetB = Planetarium.fetch.Home;
							break;
					}
				}

				Notes_CheckListItem newCheckListItem = new Notes_CheckListItem(text, order, complete, targetV, targetB, id, type, c, data);

				c.addCheckList(newCheckListItem);
			}

			return c;
		}

		private Notes_VesselLog loadLog(ConfigNode node)
		{
			Notes_VesselLog l = new Notes_VesselLog();

			Vector2d target = node.parse("TARGET", new Vector2d());

			l.setTarget(target);

			return l;
		}

		public override void OnSave(ConfigNode node)
		{
			if (core == null)
				return;

			ConfigNode vessels = new ConfigNode("VESSELS");

			for (int i = 0; i < core.notesCount; i++)
			{
				Notes_Container n = core.getNotes(i);

				if (n == null)
					continue;

				ConfigNode vesselNotes = new ConfigNode("VESSEL_NOTES");

				vesselNotes.AddValue("VESSEL_ID", n.ID);

				if (n.Stats != null && true)
				{
					saveStats(vesselNotes, n.Stats);
				}

				if (n.Contracts != null && n.Contracts.contractCount > 0)
				{
					saveContracts(vesselNotes, n.Contracts);
				}

				if (n.Data != null && n.Data.NotesDataCompletedCount > 0)
				{
					saveData(vesselNotes, n.Data);
				}

				if (n.Notes != null && n.Notes.noteCount > 0)
				{
					saveTextNotes(vesselNotes, n.Notes);
				}

				if (n.CheckList != null && n.CheckList.noteCount > 0)
				{
					saveChecklist(vesselNotes, n.CheckList, false);
				}

				if (n.Log != null && n.Log.targetCount > 0)
				{
					saveLog(vesselNotes, n.Log);
				}

				if (vesselNotes.CountNodes > 0)
					vessels.AddNode(vesselNotes);
			}

			ConfigNode archivedVessels = new ConfigNode("ARCHIVED_VESSELS");

			for (int i = 0; i < core.archivedNotesCount; i++)
			{
				Notes_Archive_Container n = core.getArchivedNotes(i);

				if (n == null)
					continue;

				ConfigNode archivedVesselNotes = new ConfigNode("ARCHIVED_VESSEL_NOTES");

				archivedVesselNotes.AddValue("VESSEL_ID", n.ID);
				archivedVesselNotes.AddValue("VESSEL_NAME", n.VesselName);
				archivedVesselNotes.AddValue("RECOVERY_TIME", n.RecoveryTime.ToString("F2"));
				archivedVesselNotes.AddValue("MISSION_TIME", n.MET.ToString("F2"));
				archivedVesselNotes.AddValue("VESSEL_TYPE", (int)n.VType);

				if (n.Crew != null && n.Crew.getCrewCount > 0)
				{
					saveCrew(archivedVesselNotes, n.Crew);
				}

				if (n.Contracts != null && n.Contracts.contractCount > 0)
				{
					saveContracts(archivedVesselNotes, n.Contracts);
				}

				if (n.Data != null && n.Data.NotesDataCompletedCount > 0)
				{
					saveData(archivedVesselNotes, n.Data);
				}

				if (n.Notes != null && n.Notes.noteCount > 0)
				{
					saveTextNotes(archivedVesselNotes, n.Notes);
				}

				if (n.CheckList != null && n.CheckList.noteCount > 0)
				{
					saveChecklist(archivedVesselNotes, n.CheckList, true);
				}

				if (n.Log != null && n.Log.targetCount > 0)
				{
					saveLog(archivedVesselNotes, n.Log);
				}

				if (archivedVesselNotes.CountNodes > 0)
					archivedVessels.AddNode(archivedVesselNotes);
			}

			if (vessels.CountNodes > 0)
				node.AddNode(vessels);

			if (archivedVessels.CountNodes > 0)
				node.AddNode(archivedVessels);
		}

		private void saveStats(ConfigNode node, Notes_VitalStats stats)
		{
			ConfigNode vesselStats = new ConfigNode("VESSEL_STATS");

			node.AddNode(vesselStats);
		}

		private void saveCrew(ConfigNode node, Notes_Archived_Crew_Container c)
		{
			ConfigNode crew = new ConfigNode("ARCHIVED_CREW");

			for (int i = 0; i < c.getCrewCount; i++)
			{
				Notes_CrewObject o = c.getCrewNotes(i);

				if (o == null)
					continue;

				crew.AddValue("KERBAL", o.Crew.name);
			}

			node.AddNode(crew);
		}

		private void saveContracts(ConfigNode node, Notes_ContractContainer c)
		{
			ConfigNode contracts = new ConfigNode("VESSEL_CONTRACTS");

			contracts.AddValue("CONTRACTS", c.getAllContractIDs.stringConcat());


			node.AddNode(contracts);
		}

		private void saveData(ConfigNode node, Notes_DataContainer d)
		{
			ConfigNode data = new ConfigNode("VESSEL_SCIENCE_DATA");

			for (int j = 0; j < d.NotesDataCompletedCount; j++)
			{
				Notes_ReceivedData dat = d.getReturnedNotesData(j);

				if (d == null)
					continue;

				ConfigNode partData = new ConfigNode("COMPLETED_DATA");
				partData.AddValue("SCIENCE_ID", dat.ID);
				partData.AddValue("SCIENCE_VALUE", dat.ScienceValue);
				partData.AddValue("TIME_RECEIVED", dat.ReceivedTime);
				data.AddNode(partData);
			}

			node.AddNode(data);
		}

		private void saveTextNotes(ConfigNode node, Notes_TextContainer text)
		{
			ConfigNode textNotes = new ConfigNode("VESSEL_TEXT_NOTES");

			for (int j = 0; j < text.noteCount; j++)
			{
				Notes_TextItem t = text.getNote(j);

				if (t == null)
					continue;

				ConfigNode textNote = new ConfigNode("NOTE");

				textNote.AddValue("KEY", t.ID);
				textNote.AddValue("CREATE_TIME", t.CreateTime);
				textNote.AddValue("EDIT_TIME", t.EditTime);
				textNote.AddValue("TITLE", t.Title);
				textNote.AddValue("NOTE", t.Text);

				textNotes.AddNode(textNote);
			}

			node.AddNode(textNotes);
		}

		private void saveChecklist(ConfigNode node, Notes_CheckListContainer check, bool archived)
		{
			ConfigNode checkList = new ConfigNode("CHECK_LIST");

			for (int j = 0; j < check.noteCount; j++)
			{
				Notes_CheckListItem c = check.getCheckList(j);

				if (c == null)
					continue;

				ConfigNode checkItem = new ConfigNode("CHECK_LIST_ITEM");

				checkItem.AddValue("KEY", c.ID);
				checkItem.AddValue("ORDER", c.Order);
				checkItem.AddValue("NOTE", c.Text);
				checkItem.AddValue("COMPLETE", c.Complete);
				checkItem.AddValue("TYPE", c.CheckType);

				switch (c.CheckType)
				{
					case Notes_CheckListType.dockVessel:
					case Notes_CheckListType.dockAsteroid:
					case Notes_CheckListType.rendezvousVessel:
					case Notes_CheckListType.rendezvousAsteroid:
						if (c.TargetVessel == null)
							continue;
						checkItem.AddValue("TARGET_VESSEL", c.TargetVessel.id);
						break;
					case Notes_CheckListType.launch:
					case Notes_CheckListType.land:
					case Notes_CheckListType.blastOff:
					case Notes_CheckListType.orbit:
					case Notes_CheckListType.enterOrbit:
					case Notes_CheckListType.returnToOrbit:
					case Notes_CheckListType.returnHome:
						if (c.TargetBody == null)
							continue;
						checkItem.AddValue("TARGET_BODY", c.TargetBody.name);
						break;
					case Notes_CheckListType.science:
						if (c.Data != null)
							checkItem.AddValue("DATA", c.Data);
						break;
					case Notes_CheckListType.scienceFromPlanet:
						if (c.TargetBody == null && !archived)
							continue;
						if (c.Data != null)
							checkItem.AddValue("DATA", c.Data);
						if (!archived)
							checkItem.AddValue("TARGET_BODY", c.TargetBody.name);
						break;
				}

				checkList.AddNode(checkItem);
			}

			node.AddNode(checkList);
		}

		private void saveLog(ConfigNode node, Notes_VesselLog log)
		{
			ConfigNode vesselLog = new ConfigNode("VESSEL_LOG");

			vesselLog.AddValue("TARGET", log.TargetLocation.vector2ToString());

			node.AddNode(vesselLog);
		}

		public override void OnAwake()
		{
			instance = this;
			core = Notes_Core.Instance;
		}
	}
}
