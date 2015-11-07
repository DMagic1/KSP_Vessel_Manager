using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using BetterNotes.NoteClasses;

namespace BetterNotes.NoteUIObjects
{
	public class Notes_DataButton : Notes_UIObjectBase
	{
		private Notes_DataObject dataObject;
		private bool highlight;

		private void Start()
		{
			highlight = Notes_MainMenu.Settings.HighLightPart;
		}

		protected override bool assignObject(object obj)
		{
			if (obj == null || obj.GetType() != typeof(Notes_DataObject))
			{
				return false;
			}

			dataObject = (Notes_DataObject)obj;

			return true;
		}

		protected override void OnLeftClick()
		{
			if (dataObject == null)
				return;

			dataObject.reviewData();
		}

		protected override void OnRightClick()
		{
			throw new NotImplementedException();
		}

		protected override void OnMouseIn()
		{
			if (dataObject == null)
				return;

			if (highlight)
				dataObject.RootPart.SetHighlight(true, false);
		}

		protected override void OnMouseOut()
		{
			if (dataObject == null)
				return;

			if (highlight)
				dataObject.RootPart.SetHighlight(false, false);
		}

		protected override void ToolTip()
		{
			throw new NotImplementedException();
		}
	}
}
