using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using BetterNotes.NoteClasses;

namespace BetterNotes.NoteUIObjects
{
	public class NoteCheckListButton : NoteUIObjectBase
	{
		private NotesCheckListItem checkListItem;

		protected override void OnLeftClick()
		{
			switch (checkListItem.CheckType)
			{
				case NotesCheckListType.custom:
					checkListItem.setComplete();
					break;
				default:
					return;
			}

		}

		protected override void OnRightClick()
		{
			throw new NotImplementedException();
		}

		protected override void ToolTip()
		{
			throw new NotImplementedException();
		}

		protected override void OnMouseIn()
		{
			throw new NotImplementedException();
		}

		protected override void OnMouseOut()
		{
			throw new NotImplementedException();
		}
	}
}
