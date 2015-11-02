using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BetterNotes.Framework;
using UnityEngine;

namespace BetterNotes.NoteClasses.CheckListHandler
{
	public class Notes_CheckListMonoBehaviour : Notes_MBE
	{
		private static Notes_CheckListMonoBehaviour instance;

		public static Notes_CheckListMonoBehaviour Instance
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

		public void startBlastOffWatcher(Vessel v, Notes_CheckListItem n)
		{
			StartCoroutine(blastOffWatcher(v, n));
		}

		private IEnumerator blastOffWatcher(Vessel v, Notes_CheckListItem n)
		{
			float timer = 0;
			double targetAlt = 0;

			if (v.mainBody.atmosphere)
				targetAlt = v.mainBody.atmosphereDepth / 20;
			else
				targetAlt = v.mainBody.Radius / 200;

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

						timer += TimeWarp.deltaTime;

						n.Text = string.Format("Take off from {0}\n(Achieve {1:F0}m within {2:F0}sec)", n.TargetBody.theName, targetAlt, timer);

						yield return null;
						break;
				}
			}

			n.Text = string.Format("Take off from {0}", n.TargetBody.theName);
		}


	}
}
