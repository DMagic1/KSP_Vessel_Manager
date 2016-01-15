using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using BetterNotes.Framework;

namespace BetterNotes.NoteClasses
{
	public class Notes_VesselLog : Notes_Base
	{
		private Vector2d targetLocation;
		private FlightLog shipsLog;
		private bool archived;

		public Notes_VesselLog()
		{ }

		public Notes_VesselLog(Notes_Container n)
		{
			root = n;
			vessel = n.NotesVessel;
		}

		public Notes_VesselLog(Notes_Archive_Container n)
		{
			archive_Root = n;
			vessel = null;
			archived = true;
		}

		public Notes_VesselLog(Notes_VesselLog copy, Notes_Container n)
		{
			targetLocation = copy.targetLocation;
			root = n;
			vessel = n.NotesVessel;
		}

		public Notes_VesselLog(Notes_VesselLog copy, Notes_Archive_Container n)
		{
			targetLocation = copy.targetLocation;
			archive_Root = n;
			vessel = null;
			archived = true;
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

		public bool Archived
		{
			get { return archived; }
		}
	}
}
