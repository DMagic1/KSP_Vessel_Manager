using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using BetterNotes.NoteClasses;

namespace BetterNotes.NoteUIObjects
{
	public class Notes_CrewButton : Notes_UIObjectBase
	{
		private Notes_CrewObject crewObject;
		private bool highlight;

		private void Start()
		{
			highlight = Notes_MainMenu.Settings.HighLightPart;
		}

		protected override bool assignObject(object obj)
		{
			if (obj == null || obj.GetType() != typeof(Notes_CrewObject))
			{
				return false;
			}

			crewObject = (Notes_CrewObject)obj;

			return true;
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
