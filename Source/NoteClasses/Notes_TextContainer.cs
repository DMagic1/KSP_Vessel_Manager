using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using BetterNotes.Framework;

namespace BetterNotes.NoteClasses
{
	public class Notes_TextContainer : Notes_Base
	{
		private Dictionary<Guid, Notes_TextItem> notes = new Dictionary<Guid, Notes_TextItem>();

		public Notes_TextContainer()
		{ }

		public Notes_TextContainer(Notes_Container n)
		{
			root = n;
			vessel = n.NotesVessel;
		}

		public Notes_TextContainer(Notes_TextContainer copy, Notes_Container n)
		{
			notes = copy.notes;
			root = n;
			vessel = n.NotesVessel;
		}

		public int noteCount
		{
			get { return notes.Count; }
		}

		public Notes_TextItem getNote(int index, bool warn = false)
		{
			if (notes.Count > index)
				return notes.ElementAt(index).Value;
			else if (warn)
				Debug.LogWarning("Text Notes dictionary index out of range; something went wrong here...");

			return null;
		}

		public Notes_TextItem getNote(Guid id)
		{
			if (notes.ContainsKey(id))
				return notes[id];

			return null;
		}

		public void addNote(Notes_TextItem note)
		{
			if (!notes.ContainsKey(note.ID))
				notes.Add(note.ID, note);
		}

	}

	public class Notes_TextItem
	{
		private string text;
		private string title;
		private Guid key;
		private DateTime createTime;
		private DateTime editTime;

		public Notes_TextItem(DateTime time)
		{
			createTime = time;
			editTime = time;
			key = Guid.NewGuid();
		}

		public Notes_TextItem(string note, string t, Guid id, DateTime create, DateTime edit)
		{
			text = note;
			title = t;
			key = id;
			createTime = create;
			editTime = edit;
		}

		public string Text
		{
			get { return text; }
		}

		public string Title
		{
			get { return title; }
		}

		public Guid ID
		{
			get { return key; }
		}

		public DateTime CreateTime
		{
			get { return createTime; }
		}

		public DateTime EditTime
		{
			get { return editTime; }
			set
			{
				if (value > createTime && value > editTime)
					editTime = value;
			}
		}
	}
}
