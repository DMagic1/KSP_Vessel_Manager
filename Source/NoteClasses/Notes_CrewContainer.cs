using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using BetterNotes.Framework;
using Experience;
using Experience.Effects;

namespace BetterNotes.NoteClasses
{
	public class Notes_CrewContainer : Notes_PartBase
	{
		private Dictionary<uint, Notes_CrewPart> allCrew = new Dictionary<uint, Notes_CrewPart>();
		public bool TransferActive { get; set; }

		public Notes_CrewContainer()
		{ }

		public Notes_CrewContainer(Notes_Container n)
		{
			root = n;
			vessel = n.NotesVessel;
		}

		public Notes_CrewPart getCrewNotes(uint id)
		{
			if (allCrew.ContainsKey(id))
				return allCrew[id];

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

				if (p.CrewCapacity > 0)
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

				Notes_CrewPart n = getCrewNotes(p.flightID);

				if (n == null)
					n = new Notes_CrewPart(p, this);

				n.clearCrew();

				for (int j = 0; j < p.protoModuleCrew.Count; j++)
				{
					ProtoCrewMember c = p.protoModuleCrew[j];

					if (c == null)
						continue;

					n.addPartCrew(c);
				}

				if (n.CrewCount > 0)
				{
					if (!allCrew.ContainsKey(p.flightID))
						allCrew.Add(p.flightID, n);
				}
				else if (allCrew.ContainsKey(p.flightID))
					allCrew.Remove(p.flightID);
			}
		}
	}

	public class Notes_CrewPart
	{
		private List<Notes_CrewObject> partCrew = new List<Notes_CrewObject>();
		private Part part;
		private Notes_CrewContainer root;

		public Notes_CrewPart(Part p, Notes_CrewContainer r)
		{
			part = p;
			root = r;
		}

		public void clearCrew()
		{
			partCrew.Clear();
		}

		public void addPartCrew(ProtoCrewMember c)
		{
			Notes_CrewObject n = new Notes_CrewObject(c, this);

			if (!partCrew.Contains(n))
				partCrew.Add(n);
		}

		public int CrewCount
		{
			get { return partCrew.Count; }
		}

		public List<Notes_CrewObject> PartCrew
		{
			get { return partCrew; }
		}

		public Part Part
		{
			get { return part; }
		}

		public Notes_CrewContainer Root
		{
			get { return root; }
		}
	}

	public class Notes_Archived_Crew_Container: Notes_Base
	{
		private Dictionary<string, Notes_CrewObject> allCrew = new Dictionary<string, Notes_CrewObject>();

		public Notes_Archived_Crew_Container() { }
		
		public Notes_Archived_Crew_Container(Notes_Archive_Container n)
		{
			archive_Root = n;
			vessel = null;
		}

		public Notes_Archived_Crew_Container(Notes_Archived_Crew_Container copy, Notes_Archive_Container n)
		{
			allCrew = copy.allCrew;
			archive_Root = n;
			vessel = null;
		}

		public int getCrewCount
		{
			get { return allCrew.Count; }
		}

		public Notes_CrewObject getCrewNotes(string id)
		{
			if (allCrew.ContainsKey(id))
				return allCrew[id];

			return null;
		}

		public Notes_CrewObject getCrewNotes(int index)
		{
			if (allCrew.Count > index)
				return allCrew.ElementAt(index).Value;

			return null;
		}

		public void addCrewObject(ProtoCrewMember c)
		{
			Notes_CrewObject o = new Notes_CrewObject(c, this);

			if (!allCrew.ContainsKey(c.name))
				allCrew.Add(c.name, o);
		}
	}

	public class Notes_CrewObject
	{
		private ProtoCrewMember crew;
		private Notes_CrewPart root;
		private Texture2D profIcon;
		private Texture2D levelIcon;
		private Color32 iconColor;
		private CrewTransfer transfer;
		private Notes_Archived_Crew_Container archive_Root;

		public Notes_CrewObject(ProtoCrewMember c, Notes_CrewPart r)
		{
			crew = c;
			root = r;
			profIcon = assignPIcon(crew.experienceTrait);
			levelIcon = assignLIcon(crew.experienceLevel);
		}

		public Notes_CrewObject(ProtoCrewMember c, Notes_Archived_Crew_Container r)
		{
			crew = c;
			root = null;
			archive_Root = r;
			profIcon = assignPIcon(crew.experienceTrait);
			levelIcon = assignLIcon(crew.experienceLevel);
		}

		public void transferCrew()
		{
			if (root == null)
				return;

			transfer = CrewTransfer.Create(RootPart, crew, onTransferDismiss);
			RootContainer.TransferActive = true;
		}

		public void onTransferDismiss(CrewTransfer.DismissAction d)
		{
			transfer = null;
			RootContainer.TransferActive = false;
		}

		private Texture2D assignPIcon(ExperienceTrait t)
		{
			switch(t.Title)
			{
				case "Pilot":
					iconColor = Notes_MainMenu.Settings.PilotIconColor;
					return Notes_Resources.pilotIcon;
				case "Engineer":
					iconColor = Notes_MainMenu.Settings.EngineerIconColor;
					return Notes_Resources.engineerIcon;
				case "Scientist":
					iconColor = Notes_MainMenu.Settings.ScientistIconColor;
					return Notes_Resources.scientistIcon;
				case "Tourist":
					iconColor = Notes_MainMenu.Settings.TouristIconColor;
					return Notes_Resources.touristIcon;
				default:
					iconColor = XKCDColors.White;
					return Notes_Resources.defaultIcon;
			}
		}

		private Texture2D assignLIcon(int i)
		{
			switch(i)
			{
				case 0:
					return Notes_Resources.levelZero;
				case 1:
					return Notes_Resources.levelOne;
				case 2:
					return Notes_Resources.levelTwo;
				case 3:
					return Notes_Resources.levelThree;
				case 4:
					return Notes_Resources.levelFour;
				case 5:
					return Notes_Resources.levelFive;
				default:
					return Notes_Resources.levelZero;
			}
		}

		public ProtoCrewMember Crew
		{
			get { return crew; }
		}

		public bool TransferActive
		{
			get { return RootContainer.TransferActive; }
		}

		public Notes_CrewPart Root
		{
			get { return root; }
		}

		public Part RootPart
		{
			get
			{
				if (root == null)
					return null;

				return root.Part;
			}
		}

		public Notes_CrewContainer RootContainer
		{
			get
			{
				if (root == null)
					return null;

				return root.Root;
			}
		}

		public Notes_Archived_Crew_Container ArchiveRoot
		{
			get { return archive_Root; }
		}

		public Texture2D ProfIcon
		{
			get { return profIcon; }
		}

		public Texture2D LevelIcon
		{
			get { return levelIcon; }
		}
	}
}
