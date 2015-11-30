using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using BetterNotes.NoteClasses;

namespace BetterNotes.NoteUIObjects
{
	public class Notes_ContractButton : Notes_UIObjectBase
	{
		private Notes_ContractShell contract;

		protected override bool assignObject(object obj)
		{
			if (obj == null || obj.GetType() != typeof(Notes_ContractShell))
			{
				return false;
			}

			contract = (Notes_ContractShell)obj;

			return true;
		}

		protected override void OnLeftClick()
		{
			//Expand contract parameters
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
