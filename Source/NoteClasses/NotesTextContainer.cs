using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using BetterNotes.Framework;

namespace BetterNotes.NoteClasses
{
	public class NotesTextContainer : NotesBase
	{
		private Dictionary<Guid, TextNotes> notes = new Dictionary<Guid, TextNotes>();

		public NotesTextContainer()
		{ }

		public NotesTextContainer(NotesContainer n)
		{
			root = n;
			vessel = n.NotesVessel;
		}

		public NotesTextContainer(NotesTextContainer copy, NotesContainer n)
		{
			notes = copy.notes;
			root = n;
			vessel = n.NotesVessel;
		}

		public int noteCount
		{
			get { return notes.Count; }
		}

		public TextNotes getNote(int index, bool warn = false)
		{
			if (notes.Count > index)
				return notes.ElementAt(index).Value;
			else if (warn)
				Debug.LogWarning("Text Notes dictionary index out of range; something went wrong here...");

			return null;
		}

		public TextNotes getNote(Guid id)
		{
			if (notes.ContainsKey(id))
				return notes[id];

			return null;
		}

		public void addNote(TextNotes note)
		{
			if (!notes.ContainsKey(note.ID))
				notes.Add(note.ID, note);
		}

	}

	public class TextNotes
	{
		private string text;
		private string title;
		private Guid key;
		private DateTime createTime;
		private DateTime editTime;

		public TextNotes(DateTime time)
		{
			createTime = time;
			editTime = time;
			key = Guid.NewGuid();
		}

		public TextNotes(string note, string t, Guid id, DateTime create, DateTime edit)
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
