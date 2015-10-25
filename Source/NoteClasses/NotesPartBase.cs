using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using BetterNotes.Framework;

namespace BetterNotes.NoteClasses
{
	public class NotesPartBase : NotesBase
	{
		protected List<Part> validParts = new List<Part>();

		protected virtual void scanVessel()
		{

		}

		protected virtual void updateValidParts()
		{

		}

	}
}
