﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BetterNotes.Framework;
using BetterNotes.NoteClasses;
using UnityEngine;

namespace BetterNotes
{
	public class Notes_ScienceTransfer : Notes_MBE
	{
		private Part host;
		private List<IScienceDataContainer> containers;
		private int dataCount;
		private ScreenMessage instructionMessage;
		private ScreenMessage transferMessage;
		private List<PartSelector> parts;
		private Callback<CrewTransfer.DismissAction> onDismiss;
		private const string lockID = "Notes_ScienceTransfer_Lock";

		private static string scienceTransferInstructions = "Select a science container to transfer {0} data to\n[Esc]: Cancel";
		private static string scienceTransferFailFullContainer = "This container is full";
		private static string scienceTransferFailSourceContainer = "The data is already in this container";
		private static string scienceTransferSuccess = "Transfered Science Data to this container";
		private static string scienceTransferInterrupted = "Science transfer was interrupted...";
		private static bool stringsLoaded = false;

		public static Notes_ScienceTransfer Create(Part p, IScienceDataContainer src, Callback<CrewTransfer.DismissAction> call)
		{
			if (!stringsLoaded)
				loadStrings();

			Notes_ScienceTransfer transfer = new GameObject("Notes_ScienceTransfer").AddComponent<Notes_ScienceTransfer>();
			transfer.host = p;
			transfer.onDismiss = call;
			transfer.dataCount = src.GetScienceCount();
			transfer.containers = new List<IScienceDataContainer>(1) { src };
			transfer.transferSetup();
			return transfer;
		}

		public void Dismiss(CrewTransfer.DismissAction action)
		{
			if (action == CrewTransfer.DismissAction.Interrupted)
				ScreenMessages.PostScreenMessage(scienceTransferInterrupted, transferMessage);
			onDismiss(action);
			ScreenMessages.RemoveMessage(instructionMessage);
			Destroy(this);
		}

		private static void loadStrings()
		{
			stringsLoaded = true;

			if (Notes_MainMenu.Active_Localization_Pack == null)
				return;

			scienceTransferInstructions = Notes_MainMenu.Active_Localization_Pack.ScienceTransferInstructions;
			scienceTransferFailFullContainer = Notes_MainMenu.Active_Localization_Pack.ScienceTransferFailFullContainer;
			scienceTransferFailSourceContainer = Notes_MainMenu.Active_Localization_Pack.ScienceTransferFailSourceContainer;
			scienceTransferSuccess = Notes_MainMenu.Active_Localization_Pack.ScienceTransferSuccess;
			scienceTransferInterrupted = Notes_MainMenu.Active_Localization_Pack.ScienceTransferInterrupted;
		}

		private void transferSetup()
		{
			InputLockManager.SetControlLock(ControlTypes.ALLBUTCAMERAS, lockID);

			instructionMessage = new ScreenMessage("", 15f, ScreenMessageStyle.UPPER_CENTER);
			transferMessage = new ScreenMessage("", 3f, ScreenMessageStyle.LOWER_CENTER);

			GameEvents.onVesselWasModified.Add(onVesselModified);
			GameEvents.onVesselSituationChange.Add(onSituationChange);
			GameEvents.OnExperimentDeployed.Add(onExperimentDeployed);

			ScreenMessages.PostScreenMessage(string.Format(scienceTransferInstructions, dataCount), instructionMessage);

			parts = new List<PartSelector>();

			parts.Add(PartSelector.Create(host, onSourceContainerSelect, XKCDColors.DarkOrange, XKCDColors.KSPNotSoGoodOrange));

			IEnumerable<Part> containerParts = host.vessel.Parts.Where(c => c.FindModulesImplementing<ModuleScienceContainer>().Count > 0 && c != host);

			IEnumerable<Part> availableList = containerParts.Where(c => c.FindModulesImplementing<ModuleScienceContainer>().Any(m => m.capacity > m.GetScienceCount()));
			IEnumerable<Part> fullList = containerParts.Except(availableList);

			for (int i = 0; i < availableList.Count(); i++)
			{
				Part c = availableList.ElementAt(i);

				if (c == null)
					continue;

				parts.Add(PartSelector.Create(c, onContainerSelect, XKCDColors.LightAqua, XKCDColors.BrightAqua));
			}

			for (int i = 0; i < fullList.Count(); i++)
			{
				Part c = fullList.ElementAt(i);

				if (c == null)
					continue;

				parts.Add(PartSelector.Create(c, onFullContainerSelect, XKCDColors.DarkOrange, XKCDColors.KSPNotSoGoodOrange));
			}
		}

		private void transferScience(ModuleScienceContainer container)
		{
			container.StoreData(containers, false);
			ScreenMessages.PostScreenMessage(scienceTransferSuccess, transferMessage);
			Dismiss(CrewTransfer.DismissAction.CrewMoved);
		}

		private void onContainerSelect(Part p)
		{
			ModuleScienceContainer m = p.FindModulesImplementing<ModuleScienceContainer>().FirstOrDefault(c => c.capacity > c.GetScienceCount());

			if (m == null)
			{
				ScreenMessages.PostScreenMessage(scienceTransferFailFullContainer, transferMessage);
				return;
			}

			transferScience(m);
		}

		private void onFullContainerSelect(Part p)
		{
			ScreenMessages.PostScreenMessage(scienceTransferFailFullContainer, transferMessage);
		}

		private void onSourceContainerSelect(Part p)
		{
			ScreenMessages.PostScreenMessage(scienceTransferFailSourceContainer, transferMessage);
		}

		private void onVesselModified(Vessel v)
		{
			Dismiss(CrewTransfer.DismissAction.Interrupted);
		}

		private void onSituationChange(GameEvents.HostedFromToAction<Vessel, Vessel.Situations> vs)
		{
			Dismiss(CrewTransfer.DismissAction.Interrupted);
		}

		private void onExperimentDeployed(ScienceData data)
		{
			Dismiss(CrewTransfer.DismissAction.Interrupted);
		}

		protected override void OnDestroy()
		{
			InputLockManager.RemoveControlLock(lockID);
			GameEvents.onVesselWasModified.Remove(onVesselModified);
			GameEvents.onVesselSituationChange.Remove(onSituationChange);
			GameEvents.OnExperimentDeployed.Remove(onExperimentDeployed);
		}

		protected override void LateUpdate()
		{
			if (Input.GetKeyUp(KeyCode.Escape))
				Dismiss(CrewTransfer.DismissAction.Cancelled);
		}


	}
}
