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

			if (vessels == null)
				return;

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


					Notes_VitalStats s = new Notes_VitalStats();

					n.loadVitalStats(s);
				}

				ConfigNode contracts = vesselNotes.GetNode("VESSEL_CONTRACTS");

				if (contracts != null)
				{
					Notes_ContractContainer c = new Notes_ContractContainer();

					List<Guid> contractIDs = contracts.parse("CONTRACTS", new List<Guid>());

					n.loadContracts(c, contractIDs);
				}

				ConfigNode dataNotes = vesselNotes.GetNode("VESSEL_SCIENCE_DATA");

				if (dataNotes != null)
				{
					Notes_DataContainer d = new Notes_DataContainer();

					for (int k = 0; k < dataNotes.GetNodes("COMPLETED_DATA").Length; k++)
					{
						ConfigNode s = dataNotes.GetNodes("COMPLETED_DATA")[k];

						if (s == null)
							continue;

						ScienceSubject sub = s.parse("SCIENCE_ID", (ScienceSubject)null);

						if (sub == null)
							continue;

						float value = s.parse("SCIENCE_VALUE", 0f);
						int time = s.parse("TIME_RECEIVED", (int)0);

						Notes_ReceivedData o = new Notes_ReceivedData(sub, value, time);

						d.addReturnedData(o);
					}

					n.loadDataNotes(d);
				}

				ConfigNode textNotes = vesselNotes.GetNode("VESSEL_TEXT_NOTES");

				if (textNotes != null)
				{
					Notes_TextContainer t = new Notes_TextContainer();

					for (int j = 0; j < textNotes.GetNodes("NOTE").Length; j++)
					{
						ConfigNode note = textNotes.GetNodes("NOTE")[j];

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

						Notes_TextItem newNote = new Notes_TextItem(text, title, id, create, edit);

						t.addNote(newNote);
					}

					n.loadTextNotes(t);
				}

				ConfigNode checkList = vesselNotes.GetNode("CHECK_LIST");

				if (checkList != null)
				{
					Notes_CheckListContainer c = new Notes_CheckListContainer();

					for (int j = 0; j < checkList.GetNodes("CHECK_LIST_ITEM").Length; j++)
					{
						ConfigNode checkListItem = textNotes.GetNodes("CHECK_LIST_ITEM")[j];

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
						Vessel targetV = checkListItem.parse("TARGET_VESSEL", (Vessel)null);
						CelestialBody targetB = checkListItem.parse("TARGET_BODY", (CelestialBody)null);

						if (!complete)
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

					n.loadCheckList(c);
				}

				ConfigNode vesselLog = vesselNotes.GetNode("VESSEL_LOG");

				if (vesselLog != null)
				{
					Notes_VesselLog l = new Notes_VesselLog();

					Vector2d target = vesselLog.parse("TARGET", new Vector2d());

					l.setTarget(target);

					n.loadVesselLog(l);
				}

				core.addNotes(n);
			}
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
					ConfigNode vesselStats = new ConfigNode("VESSEL_STATS");

					vesselNotes.AddNode(vesselStats);
				}

				if (n.Contracts != null && n.Contracts.contractCount > 0)
				{
					ConfigNode contracts = new ConfigNode("VESSEL_CONTRACTS");
					contracts.AddValue("CONTRACTS", n.Contracts.getAllContractIDs.stringConcat());
					vesselNotes.AddNode(contracts);
				}

				if (n.Data != null && n.Data.NotesDataCompletedCount > 0)
				{
					ConfigNode data = new ConfigNode("VESSEL_SCIENCE_DATA");

					for (int j = 0; j < n.Data.NotesDataCompletedCount; j++)
					{
						Notes_ReceivedData d = n.Data.getReturnedNotesData(j);

						if (d == null)
							continue;

						ConfigNode partData = new ConfigNode("COMPLETED_DATA");
						partData.AddValue("SCIENCE_ID", d.ID);
						partData.AddValue("SCIENCE_VALUE", d.ScienceValue);
						partData.AddValue("TIME_RECEIVED", d.ReceivedTime);
						data.AddNode(partData);
					}

					if (data.CountNodes > 0)
						vesselNotes.AddNode(data);
				}

				if (n.Notes != null && n.Notes.noteCount > 0)
				{
					ConfigNode textNotes = new ConfigNode("VESSEL_TEXT_NOTES");

					for (int j = 0; j < n.Notes.noteCount; j++)
					{
						Notes_TextItem t = n.Notes.getNote(j);

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

					vesselNotes.AddNode(textNotes);
				}

				if (n.CheckList != null && n.CheckList.noteCount > 0)
				{
					ConfigNode checkList = new ConfigNode("CHECK_LIST");

					for (int j = 0; j < n.CheckList.noteCount; j++)
					{
						Notes_CheckListItem c = n.CheckList.getCheckList(j);

						if (c == null)
							continue;

						ConfigNode checkItem = new ConfigNode("CHECK_LIST_ITEM");

						checkItem.AddValue("KEY", c.ID);
						checkItem.AddValue("ORDER", c.Order);
						checkItem.AddValue("NOTE", c.Text);
						checkItem.AddValue("COMPLETE", c.Complete);
						checkItem.AddValue("TYPE", c.CheckType);

						switch(c.CheckType)
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
								if (c.TargetBody == null)
									continue;
								if (c.Data != null)
									checkItem.AddValue("DATA", c.Data);
								checkItem.AddValue("TARGET_BODY", c.TargetBody.name);
								break;
						}

						checkItem.AddNode(checkItem);
					}

					vesselNotes.AddNode(checkList);
				}

				if (n.Log != null && n.Log.targetCount > 0)
				{
					ConfigNode vesselLog = new ConfigNode("VESSEL_LOG");

					vesselLog.AddValue("TARGET", n.Log.TargetLocation.vector2ToString());

					vesselNotes.AddNode(vesselLog);
				}

				if (vesselNotes.CountNodes > 0)
					vessels.AddNode(vesselNotes);
			}

			if (vessels.CountNodes > 0)
				node.AddNode(vessels);
		}

		public override void OnAwake()
		{
			instance = this;
			core = Notes_Core.Instance;
		}
	}
}
