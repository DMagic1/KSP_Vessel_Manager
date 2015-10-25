using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using BetterNotes.Framework;

namespace BetterNotes.NoteClasses
{
	public class NotesDataContainer : NotesPartBase
	{
		private Dictionary<uint, NotesDataPart> allData = new Dictionary<uint, NotesDataPart>();
		private Dictionary<string, NotesReceivedData> returnedData = new Dictionary<string, NotesReceivedData>();

		public NotesDataContainer()
		{}

		public NotesDataContainer(NotesContainer n)
		{
			root = n;
			vessel = n.NotesVessel;
		}

		public NotesDataContainer(NotesDataContainer copy, NotesContainer n)
		{
			allData = copy.allData;
			returnedData = copy.returnedData;
			root = n;
			vessel = n.NotesVessel;
		}

		public int NotesDataCount
		{
			get { return allData.Count; }
		}

		public int NotesDataCompletedCount
		{
			get { return returnedData.Count; }
		}

		public NotesReceivedData getReturnedNotesData(int index, bool warn = false)
		{
			if (returnedData.Count > index)
				return returnedData.ElementAt(index).Value;
			else if (warn)
				Debug.LogWarning("Notes Data dictionary index out of range; something went wrong here...");

			return null;
		}

		public void addReturnedData(NotesReceivedData n)
		{
			if (!returnedData.ContainsKey(n.ID))
				returnedData.Add(n.ID, n);
			else
			{
				returnedData[n.ID].updateData(n);
			}
		}

		public NotesDataPart getNotesData(uint id)
		{
			if (allData.ContainsKey(id))
				return allData[id];

			return null;
		}

		protected override void scanVessel()
		{
			if (vessel == null)
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
			if (validParts.Count <= 0)
				return;

			for (int i = 0; i < validParts.Count; i++)
			{
				Part p = validParts[i];

				if (p == null)
					continue;

				NotesDataPart n = getNotesData(p.flightID);

				if (n == null)
					n = new NotesDataPart(p);

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

						n.addPartData(d);
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
	}

	public class NotesDataPart
	{
		private List<NotesDataObject> partData = new List<NotesDataObject>();
		private Part part;
		private uint id;

		public NotesDataPart(Part p)
		{
			part = p;
			id = p.flightID;
		}

		public void addPartData(ScienceData data)
		{
			NotesDataObject n = new NotesDataObject(data, this);

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

		public List<NotesDataObject> PartData
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
	}

	public class NotesDataObject
	{
		private ScienceData data;
		private ScienceSubject sub;
		private NotesDataPart root;
		private float returnValue;
		private float transmitValue;
		private float remainingValue;
		private string title;
		private string text;

		public NotesDataObject(ScienceData d, NotesDataPart r)
		{
			data = d;
			root = r;
			sub = ResearchAndDevelopment.GetSubjectByID(d.subjectID);
			if (sub != null)
			{
				returnValue = ResearchAndDevelopment.GetNextScienceValue(data.dataAmount, sub, 1f) * HighLogic.CurrentGame.Parameters.Career.ScienceGainMultiplier;
				transmitValue = ResearchAndDevelopment.GetNextScienceValue(data.dataAmount, sub, data.transmitValue) * HighLogic.CurrentGame.Parameters.Career.ScienceGainMultiplier;
				remainingValue = Math.Min(sub.scienceCap, Math.Max(0f, sub.scienceCap * sub.scientificValue)) * HighLogic.CurrentGame.Parameters.Career.ScienceGainMultiplier;
				text = ResearchAndDevelopment.GetResults(sub.id);
			}
			title = d.title;
		}

		public void updateScience()
		{
			if (sub == null)
				return;

			returnValue = ResearchAndDevelopment.GetNextScienceValue(data.dataAmount, sub, 1f) * HighLogic.CurrentGame.Parameters.Career.ScienceGainMultiplier;
			transmitValue = ResearchAndDevelopment.GetNextScienceValue(data.dataAmount, sub, data.transmitValue) * HighLogic.CurrentGame.Parameters.Career.ScienceGainMultiplier;
			remainingValue = Math.Min(sub.scienceCap, Math.Max(0f, sub.scienceCap * sub.scientificValue)) * HighLogic.CurrentGame.Parameters.Career.ScienceGainMultiplier;
		}

		public ScienceData Data
		{
			get { return data; }
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

		public NotesDataPart Root
		{
			get { return root; }
		}

		public Part RootPart
		{
			get { return root.Part; }
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

	public class NotesReceivedData
	{
		private ScienceSubject sub;
		private float scienceValue;
		private float remainingValue;
		private int receivedTime;
		private string date;
		private string title;
		private string text;

		public NotesReceivedData(ScienceSubject id, float value, int time)
		{
			sub = id;
			scienceValue = value;
			receivedTime = time;
			date = KSPUtil.PrintDateCompact(receivedTime, false, false);
			remainingValue = Math.Min(sub.scienceCap, Math.Max(0f, sub.scienceCap * sub.scientificValue));
			text = ResearchAndDevelopment.GetResults(sub.id);
			title = sub.title;
		}

		public void updateData(NotesReceivedData d)
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
	}
}
