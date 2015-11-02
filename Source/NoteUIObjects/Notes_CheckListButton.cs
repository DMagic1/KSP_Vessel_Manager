using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using BetterNotes.NoteClasses;
using BetterNotes.NoteClasses.CheckListHandler;

namespace BetterNotes.NoteUIObjects
{
	public class Notes_CheckListButton : Notes_UIObjectBase
	{
		private Notes_CheckListItem checkListItem;

		protected override void OnLeftClick()
		{
			switch (checkListItem.CheckType)
			{
				case Notes_CheckListType.custom:
					checkListItem.setComplete();
					break;
				default:
					return;
			}

		}

		protected override bool assignObject(object obj)
		{
			if (obj == null || obj.GetType() != typeof(Notes_CheckListItem))
			{
				return false;
			}

			checkListItem = (Notes_CheckListItem)obj;

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
