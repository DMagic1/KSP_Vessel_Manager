using System;
using System.Collections.Generic;
using System.Linq;
using BetterNotes.Framework;
using UnityEngine;

namespace BetterNotes.NoteClasses
{
	public class Notes_VitalStats : Notes_Base
	{
		private int contractsAssigned;
		private int experimentsOnBoard;
		private int dataOnBoard;
		private double deltaV;

		public Notes_VitalStats()
		{ }

		public Notes_VitalStats(Notes_Container n)
		{
			root = n;
			vessel = n.NotesVessel;
		}

		public Notes_VitalStats(Notes_VitalStats copy, Notes_Container n)
		{
			deltaV = copy.deltaV;
			root = n;
			vessel = n.NotesVessel;
		}


	}
}
