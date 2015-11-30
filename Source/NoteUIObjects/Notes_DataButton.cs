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
		private bool allowTransfer;

		private void Start()
		{
			highlight = Notes_MainMenu.Settings.HighLightPart;
			allowTransfer = Notes_MainMenu.Settings.AllowScienceTransfer;
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

			if (dataObject.TransferActive)
				return;

			if (allowTransfer)
			{
				dataObject.RootPart.SetHighlight(false, false);
				dataObject.transferData();
			}
			else
				dataObject.reviewData();
		}

		protected override void OnRightClick()
		{
			//Part Right-Click menu
		}

		protected override void OnMouseIn()
		{
			if (dataObject == null)
				return;

			if (dataObject.TransferActive)
				return;

			if (highlight)
				dataObject.RootPart.SetHighlight(true, false);
		}

		protected override void OnMouseOut()
		{
			if (dataObject == null)
				return;

			if (dataObject.TransferActive)
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
