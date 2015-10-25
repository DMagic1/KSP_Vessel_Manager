using System;
using System.Collections.Generic;
using System.Linq;
using BetterNotes.Framework;
using UnityEngine;

namespace BetterNotes.NoteClasses
{
	public class NotesVitalStats : NotesBase
	{
		private int contractsAssigned;
		private int experimentsOnBoard;
		private int dataOnBoard;
		private double deltaV;

		public NotesVitalStats()
		{ }

		public NotesVitalStats(NotesContainer n)
		{
			root = n;
			vessel = n.NotesVessel;
		}

		public NotesVitalStats(NotesVitalStats copy, NotesContainer n)
		{
			deltaV = copy.deltaV;
			root = n;
			vessel = n.NotesVessel;
		}


	}
}
