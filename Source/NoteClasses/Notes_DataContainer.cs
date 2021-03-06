﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using BetterNotes.Framework;

namespace BetterNotes.NoteClasses
{
	public class Notes_DataContainer : Notes_PartBase
	{
		private Dictionary<uint, Notes_DataPart> allData = new Dictionary<uint, Notes_DataPart>();
		private Dictionary<string, Notes_ReceivedData> returnedData = new Dictionary<string, Notes_ReceivedData>();
		private bool archived;
		public bool TransferActive { get; set; }

		public Notes_DataContainer()
		{}

		public Notes_DataContainer(Notes_Container n)
		{
			root = n;
			vessel = n.NotesVessel;
		}

		public Notes_DataContainer(Notes_Archive_Container n)
		{
			archive_Root = n;
			vessel = null;
			archived = true;
		}

		public Notes_DataContainer(Notes_DataContainer copy, Notes_Container n)
		{
			allData = copy.allData;
			returnedData = copy.returnedData;
			root = n;
			vessel = n.NotesVessel;
		}

		public Notes_DataContainer(Notes_DataContainer copy, Notes_Archive_Container n)
		{
			returnedData = copy.returnedData;
			archive_Root = n;
			vessel = null;
			archived = true;
		}

		public int NotesDataCount
		{
			get { return allData.Count; }
		}

		public int NotesDataCompletedCount
		{
			get { return returnedData.Count; }
		}

		public Notes_ReceivedData getReturnedNotesData(int index, bool warn = false)
		{
			if (returnedData.Count > index)
				return returnedData.ElementAt(index).Value;
			else if (warn)
				Debug.LogWarning("Notes Data dictionary index out of range; something went wrong here...");

			return null;
		}

		public void addReturnedData(Notes_ReceivedData n)
		{
			if (!returnedData.ContainsKey(n.ID))
				returnedData.Add(n.ID, n);
			else
				returnedData[n.ID].updateData(n);
		}

		public Notes_DataPart getNotesData(uint id)
		{
			if (allData.ContainsKey(id))
				return allData[id];

			return null;
		}

		protected override void scanVessel()
		{
			if (vessel == null)
				return;

			if (archived)
				return;

			validParts.Clear();

			for (int i = 0; i < vessel.Parts.Count; i++)
			{
				Part p = vessel.Parts[i];

				if (p == null)
					continue;

				if (p.FindModulesImplementing<IScienceDataContainer>().Count > 0)
					validParts.Add(p);
			}
		}

		protected override void updateValidParts()
		{
			if (archived)
				return;

			if (validParts.Count <= 0)
				return;

			for (int i = 0; i < validParts.Count; i++)
			{
				Part p = validParts[i];

				if (p == null)
					continue;

				Notes_DataPart n = getNotesData(p.flightID);

				if (n == null)
					n = new Notes_DataPart(p, this);

				n.clearData();

				for (int k = 0; k < p.FindModulesImplementing<IScienceDataContainer>().Count; k++)
				{
					IScienceDataContainer container = p.FindModulesImplementing<IScienceDataContainer>()[k];

					if (container == null)
						continue;

					for (int j = 0; j < container.GetScienceCount(); j++)
					{
						ScienceData d = container.GetData()[j];

						if (d == null)
							continue;

						n.addPartData(d, container);
					}
				}

				if (n.DataCount > 0)
				{
					if (!allData.ContainsKey(p.flightID))
						allData.Add(p.flightID, n);
				}
				else if (allData.ContainsKey(p.flightID))
					allData.Remove(p.flightID);
			}
		}

		public bool Archived
		{
			get { return archived; }
		}
	}

	public class Notes_DataPart
	{
		private List<Notes_DataObject> partData = new List<Notes_DataObject>();
		private Part part;
		private uint id;
		private Notes_DataContainer root;

		public Notes_DataPart(Part p, Notes_DataContainer r)
		{
			part = p;
			id = p.flightID;
			root = r;
		}

		public void addPartData(ScienceData Data, IScienceDataContainer Container)
		{
			Notes_DataObject n = new Notes_DataObject(Data, this, Container);

			if (!partData.Contains(n))
				partData.Add(n);
		}

		public void clearData()
		{
			partData.Clear();
		}

		public int DataCount
		{
			get { return partData.Count; }
		}

		public List<Notes_DataObject> PartData
		{
			get { return partData; }
		}

		public Part Part
		{
			get { return part; }
		}

		public uint ID
		{
			get { return id; }
		}

		public Notes_DataContainer Root
		{
			get { return root; }
		}
	}

	public class Notes_DataObject
	{
		private ScienceData data;
		private ScienceSubject sub;
		private Notes_DataPart root;
		private IScienceDataContainer container;
		private float returnValue;
		private float transmitValue;
		private float remainingValue;
		private string title;
		private string text;
		private Notes_ScienceTransfer scienceTransfer;

		public Notes_DataObject(ScienceData d, Notes_DataPart r, IScienceDataContainer c)
		{
			data = d;
			root = r;
			container = c;
			sub = ResearchAndDevelopment.GetSubjectByID(d.subjectID);
			if (sub != null)
			{
				returnValue = ResearchAndDevelopment.GetScienceValue(data.dataAmount, sub, 1f) * HighLogic.CurrentGame.Parameters.Career.ScienceGainMultiplier;
				transmitValue = ResearchAndDevelopment.GetScienceValue(data.dataAmount, sub, data.transmitValue) * HighLogic.CurrentGame.Parameters.Career.ScienceGainMultiplier;
				remainingValue = Math.Min(sub.scienceCap, Math.Max(0f, sub.scienceCap * sub.scientificValue)) * HighLogic.CurrentGame.Parameters.Career.ScienceGainMultiplier;
				text = ResearchAndDevelopment.GetResults(sub.id);
			}
			title = d.title;
		}

		public void updateScience()
		{
			if (sub == null)
				return;

			returnValue = ResearchAndDevelopment.GetScienceValue(data.dataAmount, sub, 1f) * HighLogic.CurrentGame.Parameters.Career.ScienceGainMultiplier;
			transmitValue = ResearchAndDevelopment.GetScienceValue(data.dataAmount, sub, data.transmitValue) * HighLogic.CurrentGame.Parameters.Career.ScienceGainMultiplier;
			remainingValue = Math.Min(sub.scienceCap, Math.Max(0f, sub.scienceCap * sub.scientificValue)) * HighLogic.CurrentGame.Parameters.Career.ScienceGainMultiplier;
		}

		public void reviewData()
		{
			if (data == null)
				return;

			if (container == null)
				return;

			container.ReviewData();
		}

		public void transferData()
		{
			scienceTransfer = Notes_ScienceTransfer.Create(RootPart, container, onTransferDismiss);
			RootContainer.TransferActive = true;
		}

		public void onTransferDismiss(CrewTransfer.DismissAction d)
		{
			scienceTransfer = null;
			RootContainer.TransferActive = false;
		}

		public ScienceData Data
		{
			get { return data; }
		}

		public bool TransferActive
		{
			get { return RootContainer.TransferActive; }
		}

		public float ReturnValue
		{
			get { return returnValue; }
		}

		public float TransmitValue
		{
			get { return transmitValue; }
		}

		public float RemainingValue
		{
			get { return remainingValue; }
		}

		public Notes_DataPart Root
		{
			get { return root; }
		}

		public Part RootPart
		{
			get { return root.Part; }
		}

		public Notes_DataContainer RootContainer
		{
			get { return root.Root; }
		}

		public string Title
		{
			get { return title; }
		}

		public string Text
		{
			get { return text; }
		}
	}

	public class Notes_ReceivedData
	{
		private ScienceSubject sub;
		private float scienceValue;
		private float remainingValue;
		private int receivedTime;
		private string date;
		private string title;
		private string text;
		private Notes_DataContainer rootContainer;

		public Notes_ReceivedData(ScienceSubject id, float value, int time, Notes_DataContainer r)
		{
			sub = id;
			scienceValue = value;
			receivedTime = time;
			date = KSPUtil.PrintDateCompact(receivedTime, false, false);
			remainingValue = Math.Min(sub.scienceCap, Math.Max(0f, sub.scienceCap * sub.scientificValue));
			text = ResearchAndDevelopment.GetResults(sub.id);
			title = sub.title;
			rootContainer = r;
		}

		public void updateData(Notes_ReceivedData d)
		{
			scienceValue += d.scienceValue;
			receivedTime = d.receivedTime;
			date = KSPUtil.PrintDateCompact(receivedTime, false, false);
			remainingValue = Math.Min(sub.scienceCap, Math.Max(0f, sub.scienceCap * sub.scientificValue));
		}

		public float ScienceValue
		{
			get { return scienceValue; }
		}

		public float RemainingValue
		{
			get { return remainingValue; }
		}

		public int ReceivedTime
		{
			get { return receivedTime; }
		}

		public string Title
		{
			get { return title; }
		}

		public string Text
		{
			get { return text; }
		}

		public string Date
		{
			get { return date; }
		}

		public string ID
		{
			get { return sub.id; }
		}

		public Notes_DataContainer RootContainer
		{
			get { return rootContainer; }
		}
	}
}
