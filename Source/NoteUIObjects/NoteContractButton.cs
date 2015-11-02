using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using BetterNotes.NoteClasses;

namespace BetterNotes.NoteUIObjects
{
	public class NoteContractButton : NoteUIObjectBase
	{
		private NotesContractInfo contract;

		protected override bool assignObject(object obj)
		{
			if (obj == null || obj.GetType() != typeof(NotesContractInfo))
			{
				return false;
			}

			contract = (NotesContractInfo)obj;

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
