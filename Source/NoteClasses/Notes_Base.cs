using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using BetterNotes.Framework;

namespace BetterNotes.NoteClasses
{
	public class Notes_Base
	{
		protected Vessel vessel;
		protected Notes_Container root;

		protected virtual void updateNotes()
		{

		}

		public Vessel RootVessel
		{
			get { return vessel; }
		}
	}
}
