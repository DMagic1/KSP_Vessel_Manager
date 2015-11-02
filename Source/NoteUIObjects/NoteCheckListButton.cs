using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using BetterNotes.NoteClasses;
using BetterNotes.NoteClasses.CheckListHandler;

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

		protected override bool assignObject(object obj)
		{
			if (obj == null || obj.GetType() != typeof(NotesCheckListItem))
			{
				return false;
			}

			checkListItem = (NotesCheckListItem)obj;

			return true;
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
