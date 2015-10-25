using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using BetterNotes.Framework;

namespace BetterNotes.NoteClasses
{
	public class NotesBase
	{
		protected Vessel vessel;
		protected NotesContainer root;

		protected virtual void updateNotes()
		{

		}

		public Vessel RootVessel
		{
			get { return vessel; }
		}
	}
}
