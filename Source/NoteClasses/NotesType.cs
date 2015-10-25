﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using BetterNotes.Framework;

namespace BetterNotes.NoteClasses
{
	public enum NotesType
	{
		Vessels = 1,
		HomePage = 2,
		Notes = 3,
		Contracts = 4,
		Science = 5,
	}

	public enum HomePageSubType
	{
		VitalStats = 1,
		VesselLog = 2,
		Crew = 3,
	}

	public enum TextSubType
	{
		Text = 1,
		CheckList = 2,
	}

	public enum ScienceSubType
	{
		Experiments = 1,
		Data = 2,
	}

	public enum VesselSubType
	{
		ActiveVessels = 1,
		AllVessels = 2,
	}

	public enum ContractsSubType
	{
		ActiveContracts = 1,
		AllContracts = 2,
		ContractsPlusMissions = 3,
	}
}