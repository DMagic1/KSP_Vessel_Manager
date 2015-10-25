using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using BetterNotes.Framework;

namespace BetterNotes.NoteClasses
{
	public class NotesVesselLog : NotesBase
	{
		private Vector2d targetLocation;
		private FlightLog shipsLog;

		public NotesVesselLog()
		{ }

		public NotesVesselLog(NotesContainer n)
		{
			root = n;
			vessel = n.NotesVessel;
		}

		public NotesVesselLog(NotesVesselLog copy, NotesContainer n)
		{
			targetLocation = copy.targetLocation;
			root = n;
			vessel = n.NotesVessel;
		}

		private void loadVesselLog()
		{
			if (vessel.loaded)
			{
				shipsLog = VesselTripLog.FromVessel(vessel).Log;
			}
			else
			{
				shipsLog = VesselTripLog.FromProtoVessel(vessel.protoVessel).Log;
			}
		}

		public void setTarget(Vector2d t)
		{
			targetLocation = t;
		}

		public int targetCount
		{
			get { return 0; }
		}

		public Vector2d TargetLocation
		{
			get { return targetLocation; }
		}
	}
}
