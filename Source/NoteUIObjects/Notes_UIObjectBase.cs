using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using BetterNotes.Framework;
using BetterNotes.NoteClasses;

namespace BetterNotes.NoteUIObjects
{
	public abstract class Notes_UIObjectBase : MonoBehaviour
	{
		protected abstract void OnLeftClick();
		protected abstract void OnRightClick();
		protected abstract void ToolTip();
		protected abstract void OnMouseIn();
		protected abstract void OnMouseOut();
		protected abstract bool assignObject(object obj);
	}
}
