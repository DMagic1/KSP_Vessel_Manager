using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using BetterNotes.NoteClasses;

namespace BetterNotes.NoteUIObjects
{
	public class NoteCrewButton : NoteUIObjectBase
	{
		private NotesCrewObject crewObject;
		private bool highlight;

		private void Start()
		{
			highlight = NotesMainMenu.Settings.HighLightPart;
		}

		protected override void OnLeftClick()
		{
			throw new NotImplementedException();
		}

		protected override void OnRightClick()
		{
			throw new NotImplementedException();
		}

		protected override void OnMouseIn()
		{
			if (highlight)
				crewObject.RootPart.SetHighlight(true, false);
		}

		protected override void OnMouseOut()
		{
			if (highlight)
				crewObject.RootPart.SetHighlight(false, false);
		}

		protected override void ToolTip()
		{
			throw new NotImplementedException();
		}
	}
}
