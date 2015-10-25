using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using BetterNotes.Framework;
using Experience;
using Experience.Effects;

namespace BetterNotes.NoteClasses
{
	public class NotesCrewContainer : NotesPartBase
	{
		private Dictionary<uint, NotesCrewPart> allCrew = new Dictionary<uint, NotesCrewPart>();

		public NotesCrewContainer()
		{ }

		public NotesCrewContainer(NotesContainer n)
		{
			root = n;
			vessel = n.NotesVessel;
		}

		public NotesCrewPart getCrewNotes(uint id)
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

				NotesCrewPart n = getCrewNotes(p.flightID);

				if (n == null)
					n = new NotesCrewPart(p);

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

	public class NotesCrewPart
	{
		private List<NotesCrewObject> partCrew = new List<NotesCrewObject>();
		private Part part;

		public NotesCrewPart(Part p)
		{
			part = p;
		}

		public void clearCrew()
		{
			partCrew.Clear();
		}

		public void addPartCrew(ProtoCrewMember c)
		{
			NotesCrewObject n = new NotesCrewObject(c, this);

			if (!partCrew.Contains(n))
				partCrew.Add(n);
		}

		public int CrewCount
		{
			get { return partCrew.Count; }
		}

		public List<NotesCrewObject> PartCrew
		{
			get { return partCrew; }
		}

		public Part Part
		{
			get { return part; }
		}
	}

	public class NotesCrewObject
	{
		private ProtoCrewMember crew;
		private NotesCrewPart root;
		private Texture2D profIcon;
		private Texture2D levelIcon;
		private Color32 iconColor;

		public NotesCrewObject(ProtoCrewMember c, NotesCrewPart r)
		{
			crew = c;
			root = r;
			profIcon = assignPIcon(crew.experienceTrait);
			levelIcon = assignLIcon(crew.experienceLevel);
		}

		private Texture2D assignPIcon(ExperienceTrait t)
		{
			switch(t.Title)
			{
				case "Pilot":
					iconColor = NotesMainMenu.Settings.PilotIconColor;
					return new Texture2D(1, 1);
				case "Engineer":
					iconColor = NotesMainMenu.Settings.EngineerIconColor;
					return new Texture2D(1, 1);
				case "Scientist":
					iconColor = NotesMainMenu.Settings.ScientistIconColor;
					return new Texture2D(1, 1);
				case "Tourist":
					iconColor = NotesMainMenu.Settings.TouristIconColor;
					return new Texture2D(1, 1);
				default:
					iconColor = XKCDColors.White;
					return new Texture2D(1, 1);
			}
		}

		private Texture2D assignLIcon(int i)
		{
			switch(i)
			{
				case 0:
					return new Texture2D(1, 1);
				case 1:
					return new Texture2D(1, 1);
				case 2:
					return new Texture2D(1, 1);
				case 3:
					return new Texture2D(1, 1);
				case 4:
					return new Texture2D(1, 1);
				case 5:
					return new Texture2D(1, 1);
				default:
					return new Texture2D(1, 1);
			}
		}

		public ProtoCrewMember Crew
		{
			get { return crew; }
		}

		public NotesCrewPart Root
		{
			get { return root; }
		}

		public Part RootPart
		{
			get { return root.Part; }
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
