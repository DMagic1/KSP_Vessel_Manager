using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using BetterNotes.Framework;

namespace BetterNotes.NoteClasses
{
	public class Notes_ExpContainer : Notes_PartBase
	{
		private Dictionary<uint, Notes_ExpPart> expParts = new Dictionary<uint, Notes_ExpPart>();

		public Notes_ExpContainer()
		{ }

		public Notes_ExpContainer(Notes_Container n)
		{
			root = n;
			vessel = n.NotesVessel;
		}

		public Notes_ExpPart getExpNotes(uint id)
		{
			if (expParts.ContainsKey(id))
				return expParts[id];

			return null;
		}

		protected override void scanVessel()
		{
			if (vessel == null)
				return;

			validParts.Clear();

			for (int i = 0; i < vessel.Parts.Count; i++)
			{
				Part p = vessel.Parts[i];

				if (p == null)
					continue;

				var exps = p.FindModulesImplementing<ModuleScienceExperiment>();

				if (exps.Count > 0)
					validParts.Add(p);
			}
		}

		protected override void updateValidParts()
		{
			if (validParts.Count <= 0)
				return;

			for (int i = 0; i < validParts.Count; i++)
			{
				Part p = validParts[i];

				if (p == null)
					continue;

				Notes_ExpPart n = getExpNotes(p.flightID);

				if (n == null)
					n = new Notes_ExpPart(p);

				n.clearExp();

				var exps = p.FindModulesImplementing<ModuleScienceExperiment>();

				for (int j = 0; j < exps.Count; j++)
				{
					ModuleScienceExperiment sciExp = exps[j];

					if (sciExp == null)
						continue;

					if (string.IsNullOrEmpty(sciExp.experimentID))
						continue;

					ScienceExperiment exp = ResearchAndDevelopment.GetExperiment(sciExp.experimentID);

					if (exp == null)
						continue;

					n.addPartExperiment(sciExp, exp);
				}

				if (n.ExpCount > 0)
				{
					if (!expParts.ContainsKey(p.flightID))
						expParts.Add(p.flightID, n);
				}
				else if (expParts.ContainsKey(p.flightID))
					expParts.Remove(p.flightID);
			}
		}
	}

	public class Notes_ExpPart
	{
		private List<Notes_Experiment> allExperiments = new List<Notes_Experiment>();
		private Part part;

		public Notes_ExpPart(Part p)
		{
			part = p;
		}

		public void clearExp()
		{
			allExperiments.Clear();
		}

		public void addPartExperiment(ModuleScienceExperiment m, ScienceExperiment e)
		{
			Notes_Experiment exp = new Notes_Experiment(this, m, e, 1);

			if (!allExperiments.Contains(exp))
				allExperiments.Add(exp);
		}

		public void addPartExperiment(Notes_Experiment e)
		{
			if (!allExperiments.Contains(e))
				allExperiments.Add(e);
		}

		public Part Part
		{
			get { return part; }
		}

		public int ExpCount
		{
			get { return allExperiments.Count; }
		}
	}

	public class Notes_Experiment
	{
		private Notes_ExpPart root;
		private ModuleScienceExperiment experimentModule;
		private ScienceExperiment experiment;
		private bool inactive = false;
		private bool dataCollected = false;
		private string name = "";
		private int dataLimit = 1;

		public Notes_Experiment(Notes_ExpPart r, ModuleScienceExperiment m, ScienceExperiment e, int limit = 1)
		{
			root = r;
			experimentModule = m;
			experiment = e;
			name = e.experimentTitle;
			dataLimit = limit;
		}

		public void updateExperiment(bool I, bool C, string N, int L)
		{
			inactive = I;
			dataCollected = C;
			name = N;
			dataLimit = L;
		}

		//This one is borrowed from Science Alert to deploy the experiment using the method from any derived modules
		// https://bitbucket.org/xEvilReeperx/ksp_sciencealert/src/687114a3dcdbb2cc64a0f280bac698ebb5c918c2/ScienceAlert/Experiments/Observers/ExperimentObserver.cs?at=1.8.9&fileviewer=file-view-default#ExperimentObserver.cs-426:439 
		public bool deployExperiment()
		{
			if (FlightGlobals.ActiveVessel == null)
			{
				return false;
			}

			if (FlightGlobals.ActiveVessel != root.Part.vessel)
			{
				return false;
			}

			if (experimentModule == null)
			{
				return false;
			}

			try
			{
				experimentModule.GetType().InvokeMember("DeployExperiment", BindingFlags.Public | BindingFlags.IgnoreReturn | BindingFlags.InvokeMethod, null, experimentModule, null);
			}
			catch (Exception e)
			{
				Debug.Log("[BetterNotes] Error in invoking derived Deploy Experiment Module; Running base method " + e);
				experimentModule.DeployExperiment();
			}

			return true;
		}

		public Notes_ExpPart Root
		{
			get { return root; }
		}

		public Part RootPart
		{
			get { return root.Part; }
		}

		public bool Inactive
		{
			get { return inactive; }
			set { inactive = value; }
		}

		public bool DataCollected
		{
			get { return dataCollected; }
			set { dataCollected = value; }
		}

		public string Name
		{
			get { return name; }
		}

		public int DataLimit
		{
			get { return dataLimit; }
		}
	}
}
