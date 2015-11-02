using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using BetterNotes.NoteClasses;

namespace BetterNotes.NoteUIObjects
{
	public class Notes_TextButton : Notes_UIObjectBase
	{
		private Notes_TextItem noteObject;

		protected override bool assignObject(object obj)
		{
			if (obj == null || obj.GetType() != typeof(Notes_TextItem))
			{
				return false;
			}

			noteObject = (Notes_TextItem)obj;

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
