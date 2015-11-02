using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using BetterNotes.NoteClasses;

namespace BetterNotes.NoteUIObjects
{
	public class NoteExpButton : NoteUIObjectBase
	{
		private NotesExperiment expObject;
		private bool highlight;

		private void Start()
		{
			highlight = NotesMainMenu.Settings.HighLightPart;
		}

		protected override bool assignObject(object obj)
		{
			if (obj == null || obj.GetType() != typeof(NotesExperiment))
			{
				return false;
			}

			expObject = (NotesExperiment)obj;

			return true;
		}

		protected override void OnLeftClick()
		{
			if (expObject.deployExperiment())
			{
				//log success
			}
			else
			{
				//log fail
			}
		}

		protected override void OnRightClick()
		{
			//Part Right-Click menu
		}

		protected override void OnMouseIn()
		{
			if (highlight)
				expObject.RootPart.SetHighlight(true, false);
		}

		protected override void OnMouseOut()
		{
			if (highlight)
				expObject.RootPart.SetHighlight(false, false);
		}

		protected override void ToolTip()
		{
			throw new NotImplementedException();
		}
	}
}
