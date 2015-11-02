using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using BetterNotes.NoteClasses;

namespace BetterNotes.NoteUIObjects
{
	public class Notes_ExpButton : Notes_UIObjectBase
	{
		private Notes_Experiment expObject;
		private bool highlight;

		private void Start()
		{
			highlight = Notes_MainMenu.Settings.HighLightPart;
		}

		protected override bool assignObject(object obj)
		{
			if (obj == null || obj.GetType() != typeof(Notes_Experiment))
			{
				return false;
			}

			expObject = (Notes_Experiment)obj;

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
