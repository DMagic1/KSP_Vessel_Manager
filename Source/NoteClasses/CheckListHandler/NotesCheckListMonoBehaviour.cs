using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BetterNotes.Framework;
using UnityEngine;

namespace BetterNotes.NoteClasses.CheckListHandler
{
	public class NotesCheckListMonoBehaviour : Notes_MBE
	{
		private static NotesCheckListMonoBehaviour instance;

		public static NotesCheckListMonoBehaviour Instance
		{
			get { return instance; }
		}

		protected override void Start()
		{
			instance = this;
		}

		protected override void OnDestroy()
		{

		}

		public void startBlastOffWatcher(Vessel v, NotesCheckListItem n)
		{
			StartCoroutine(blastOffWatcher(v, n));
		}

		private IEnumerator blastOffWatcher(Vessel v, NotesCheckListItem n)
		{
			int timer = 0;
			double targetAlt = 0;

			if (v.mainBody.atmosphere)
				targetAlt = v.mainBody.atmosphereDepth / 10;
			else
				targetAlt = v.mainBody.Radius / 100;

			while (timer < 300)
			{
				switch (v.situation)
				{
					case Vessel.Situations.LANDED:
					case Vessel.Situations.SPLASHED:
						yield break;
					default:
						if (v.altitude >= targetAlt)
						{
							n.setComplete();
							yield break;
						}

						timer++;

						yield return null;
						break;
				}
			}
		}


	}
}
