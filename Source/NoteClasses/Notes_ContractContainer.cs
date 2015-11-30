using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Reflection;
using UnityEngine;
using BetterNotes.Framework;
using Contracts;
using Contracts.Agents;
using Contracts.Templates;
using FinePrint.Contracts;
using FinePrint.Contracts.Parameters;
using FinePrint.Utilities;

namespace BetterNotes.NoteClasses
{
	public class Notes_ContractContainer : Notes_Base
	{
		private Dictionary<Guid, Notes_ContractShell> allContracts = new Dictionary<Guid, Notes_ContractShell>();
		private List<Guid> contractIDs = new List<Guid>();

		public Notes_ContractContainer()
		{ }

		public Notes_ContractContainer(Notes_Container n)
		{
			root = n;
			vessel = n.NotesVessel;
		}

		public Notes_ContractContainer(Notes_ContractContainer copy, List<Guid> id, Notes_Container n)
		{
			allContracts = copy.allContracts;
			contractIDs = id;
			root = n;
			vessel = n.NotesVessel;
		}

		public void contractsRefresh()
		{
			for (int i = 0; i < contractIDs.Count; i++)
			{
				Guid g = contractIDs[i];

				if (g == null)
					continue;

				if (allContracts.ContainsKey(g))
					continue;

				Notes_ContractInfo n = Notes_Core.Instance.getContract(g);

				if (n == null)
					continue;

				Notes_ContractShell shell = new Notes_ContractShell(n, this);

				allContracts.Add(g, shell);
			}
		}

		public void addContract(Guid id)
		{
			if (!contractIDs.Contains(id))
				contractIDs.Add(id);

			contractsRefresh();
		}

		public void removeContract(Guid id)
		{
			if (allContracts.ContainsKey(id))
			{
				allContracts[id] = null;
				allContracts.Remove(id);
			}
			if (contractIDs.Contains(id))
				contractIDs.RemoveAll(a => a == id);

			contractsRefresh();
		}

		public int contractCount
		{
			get { return allContracts.Count; }
		}

		public Notes_ContractShell getContract(int index, bool warn = false)
		{
			if (allContracts.Count > index)
				return allContracts.ElementAt(index).Value;
			else if (warn)
				Debug.LogWarning("Notes Contracts dictionary index out of range; something went wrong here...");

			return null;
		}

		public Notes_ContractShell getContract(Guid id)
		{
			if (allContracts.ContainsKey(id))
				return allContracts[id];

			return null;
		}

		public IEnumerable<Guid> getAllContractIDs
		{
			get { return allContracts.Keys; }
		}
	}

	public class Notes_ContractShell
	{
		private Notes_ContractInfo contract;
		private Notes_ContractContainer root;

		public Notes_ContractShell(Notes_ContractInfo c, Notes_ContractContainer r)
		{
			contract = c;
			root = r;
		}

		public Notes_ContractInfo ContractInfo
		{
			get { return contract; }
		}

		public Notes_ContractContainer RootContainer
		{
			get { return root; }
		}
	}

	public class Notes_ContractInfo
	{
		private string title;
		private float totalFundsReward, totalRepReward, totalSciReward;
		private float totalFundsPenalty, totalRepPenalty;
		private double expire, deadline, completed;
		private string fundsRew, fundsAdv, fundsPen, repRew, repPen, sciRew;
		private float fundsRewStrat, fundsAdvStrat, fundsPenStrat, repRewStrat, repPenStrat, sciRewStrat;
		private string targetPlanet;
		private Contract root;
		private Guid id;
		private Agent agent;
		private List<Notes_ContractParameterInfo> parameters = new List<Notes_ContractParameterInfo>();
		private List<Notes_ContractParameterInfo> allParameters = new List<Notes_ContractParameterInfo>();

		public Notes_ContractInfo(Contract c)
		{
			root = c;
			try
			{
				id = root.ContractGuid;
			}
			catch (Exception e)
			{
				Debug.LogError("[BettorNotes] Contract Guid not set, skipping...: " + e);
				root = null;
				return;
			}

			try
			{
				title = root.Title;
			}
			catch (Exception e)
			{
				Debug.LogError("[BetterNotes] Contract Title not set, using type name..: " + e);
				title = root.GetType().Name;
			}

			if (root.Agent != null)
				agent = root.Agent;
			else
				agent = AgentList.Instance.GetAgentRandom();

			for (int i = 0; i < root.ParameterCount; i++)
			{
				ContractParameter p = c.GetParameter(i);
				if (p == null)
					continue;

				addContractParam(p);
			}

			updateTimeValues();

			contractRewards();
			contractAdvance();
			contractPenalties();

			totalFundsReward = rewards();
			totalFundsPenalty = penalties();
			totalRepReward = repRewards();
			totalSciReward = sciRewards();
			totalRepPenalty = repPenalties();

			CelestialBody t = getTargetBody();

			targetPlanet = t == null ? "" : t.name;
		}

		private void addContractParam(ContractParameter param)
		{
			Notes_ContractParameterInfo cc = new Notes_ContractParameterInfo(this, param, 0);
			parameters.Add(cc);
			allParameters.Add(cc);
		}

		private void contractRewards()
		{
			CurrencyModifierQuery currencyQuery = CurrencyModifierQuery.RunQuery(TransactionReasons.ContractReward, (float)root.FundsCompletion, root.ScienceCompletion, root.ReputationCompletion);

			fundsRew = "";
			if (root.FundsCompletion != 0)
				fundsRew = "+ " + root.FundsCompletion.ToString("N0");
			fundsRewStrat = currencyQuery.GetEffectDelta(Currency.Funds);
			if (fundsRewStrat != 0f)
			{
				fundsRew = string.Format("+ {0:N0} ({1:N0})", root.FundsCompletion + fundsRewStrat, fundsRewStrat);
			}

			repRew = "";
			if (root.ReputationCompletion != 0)
				repRew = "+ " + root.ReputationCompletion.ToString("N0");
			repRewStrat = currencyQuery.GetEffectDelta(Currency.Reputation);
			if (repRewStrat != 0f)
			{
				repRew = string.Format("+ {0:N0} ({1:N0})", root.ReputationCompletion + repRewStrat, repRewStrat);
			}

			sciRew = "";
			if (root.ScienceCompletion != 0)
				sciRew = "+ " + root.ScienceCompletion.ToString("N0");
			sciRewStrat = currencyQuery.GetEffectDelta(Currency.Science);
			if (sciRewStrat != 0f)
			{
				sciRew = string.Format("+ {0:N0} ({1:N0})", root.ScienceCompletion + sciRewStrat, sciRewStrat);
			}
		}

		private void contractAdvance()
		{
			CurrencyModifierQuery currencyQuery = CurrencyModifierQuery.RunQuery(TransactionReasons.ContractAdvance, (float)root.FundsAdvance, 0, 0);

			fundsAdv = "";
			if (root.FundsAdvance != 0)
				fundsAdv = "+ " + root.FundsAdvance.ToString("N0");
			fundsAdvStrat = currencyQuery.GetEffectDelta(Currency.Funds);
			if (fundsAdvStrat != 0f)
			{
				fundsAdv = string.Format("+ {0:N0} ({1:N0})", root.FundsAdvance + fundsAdvStrat, fundsAdvStrat);
			}
		}

		private void contractPenalties()
		{
			CurrencyModifierQuery currencyQuery = CurrencyModifierQuery.RunQuery(TransactionReasons.ContractPenalty, (float)root.FundsFailure, 0f, root.ReputationFailure);

			fundsPen = "";
			if (root.FundsFailure != 0)
				fundsPen = "- " + root.FundsFailure.ToString("N0");
			fundsPenStrat = currencyQuery.GetEffectDelta(Currency.Funds);
			if (fundsPenStrat != 0f)
			{
				fundsPen = string.Format("- {0:N0} ({1:N0})", root.FundsFailure + fundsPenStrat, fundsPenStrat);
			}

			repPen = "";
			if (root.ReputationFailure != 0)
				repPen = "- " + root.ReputationFailure.ToString("N0");
			repPenStrat = currencyQuery.GetEffectDelta(Currency.Reputation);
			if (repPenStrat != 0f)
			{
				repPen = string.Format("- {0:N0} ({1:N0})", root.ReputationFailure + repPenStrat, repPenStrat);
			}
		}

		private float rewards()
		{
			float f = 0;
			f += (float)root.FundsCompletion + fundsRewStrat;
			f += (float)root.FundsAdvance + fundsAdvStrat;
			foreach (Notes_ContractParameterInfo p in allParameters)
				f += (float)p.Param.FundsCompletion + p.FundsRewStrat;
			return f;
		}

		private float penalties()
		{
			float f = 0;
			f += (float)root.FundsFailure + fundsPenStrat;
			foreach (Notes_ContractParameterInfo p in allParameters)
				f += (float)p.Param.FundsFailure + p.FundsPenStrat;
			return f;
		}

		private float repRewards()
		{
			float f = 0;
			f += root.ReputationCompletion + repRewStrat;
			foreach (Notes_ContractParameterInfo p in allParameters)
				f += p.Param.ReputationCompletion + p.RepRewStrat;
			return f;
		}

		private float sciRewards()
		{
			float f = 0;
			f += root.ScienceCompletion + sciRewStrat;
			foreach (Notes_ContractParameterInfo p in allParameters)
				f += p.Param.ScienceCompletion + p.SciRewStrat;
			return f;
		}

		private float repPenalties()
		{
			float f = 0;
			f += root.ReputationFailure + repPenStrat;
			foreach (Notes_ContractParameterInfo p in allParameters)
				f += p.Param.ReputationFailure + p.RepPenStrat;
			return f;
		}

		public void updateTimeValues()
		{
			expire = root.DateExpire;
			if (expire <= 0)
				expire = double.MaxValue;

			deadline = root.DateDeadline;
			if (deadline <= 0)
				deadline = double.MaxValue;

			completed = root.DateFinished;
		}

		private CelestialBody getTargetBody()
		{
			if (root == null)
				return null;

			bool checkTitle = false;

			Type t = root.GetType();

			if (t == typeof(CollectScience))
				return ((CollectScience)root).TargetBody;
			else if (t == typeof(ExploreBody))
				return ((ExploreBody)root).TargetBody;
			else if (t == typeof(PartTest))
			{
				var fields = typeof(PartTest).GetFields(BindingFlags.NonPublic | BindingFlags.Instance);

				return fields[1].GetValue((PartTest)root) as CelestialBody;
			}
			else if (t == typeof(PlantFlag))
				return ((PlantFlag)root).TargetBody;
			else if (t == typeof(RecoverAsset))
			{
				var fields = typeof(RecoverAsset).GetFields(BindingFlags.NonPublic | BindingFlags.Instance);

				return fields[0].GetValue((RecoverAsset)root) as CelestialBody;
			}
			else if (t == typeof(GrandTour))
				return ((GrandTour)root).TargetBodies.LastOrDefault();
			else if (t == typeof(ARMContract))
			{
				var fields = typeof(ARMContract).GetFields(BindingFlags.NonPublic | BindingFlags.Instance);

				return fields[0].GetValue((ARMContract)root) as CelestialBody;
			}
			else if (t == typeof(BaseContract))
				return ((BaseContract)root).targetBody;
			else if (t == typeof(ISRUContract))
				return ((ISRUContract)root).targetBody;
			else if (t == typeof(SatelliteContract))
			{
				SpecificOrbitParameter p = root.GetParameter<SpecificOrbitParameter>();

				if (p == null)
					return null;

				return p.TargetBody;
			}
			else if (t == typeof(StationContract))
				return ((StationContract)root).targetBody;
			else if (t == typeof(SurveyContract))
				return ((SurveyContract)root).targetBody;
			else if (t == typeof(TourismContract))
				return null;
			else if (t == typeof(WorldFirstContract))
			{
				ProgressTrackingParameter p = root.GetParameter<ProgressTrackingParameter>();

				if (p == null)
					return null;

				var fields = typeof(ProgressTrackingParameter).GetFields(BindingFlags.NonPublic | BindingFlags.Instance);

				var milestone = fields[0].GetValue(p) as ProgressMilestone;

				if (milestone == null)
					return null;

				return milestone.body;
			}
			else
				checkTitle = true;

			if (checkTitle)
			{
				foreach (CelestialBody b in FlightGlobals.Bodies)
				{
					string n = b.name;

					Regex r = new Regex(string.Format(@"\b{0}\b", n));

					if (r.IsMatch(title))
						return b;
				}
			}

			return null;
		}

		public void addToParams(Notes_ContractParameterInfo p)
		{
			if (!allParameters.Contains(p))
				allParameters.Add(p);
			else
				Notes_MBE.LogFormatted("BetterNotes Parameter Object: [{0}] Already Present In Contract Container", p.Title);
		}

		public Notes_ContractParameterInfo getParameter(int i)
		{
			if (parameters.Count >= i)
				return parameters[i];
			else
				Notes_MBE.LogFormatted("BetterNotesParameter List Index Out Of Range; Something Went Wrong Here...");

			return null;
		}

		public float TotalReward
		{
			get { return totalFundsReward; }
		}

		public float TotalPenalty
		{
			get { return totalFundsPenalty; }
		}

		public float TotalRepReward
		{
			get { return totalRepReward; }
		}

		public float TotalRepPenalty
		{
			get { return totalRepPenalty; }
		}

		public float TotalSciReward
		{
			get { return totalSciReward; }
		}

		public Contract Root
		{
			get { return root; }
		}

		public Guid ID
		{
			get { return id; }
		}

		public Agent Agent
		{
			get { return agent; }
		}
	}

	public class Notes_ContractParameterInfo
	{
		private string title;
		private Notes_ContractInfo root;
		private ContractParameter param;
		private int level;
		private string fundsRew, fundsPen, repRew, repPen, sciRew;
		private float fundsRewStrat, fundsPenStrat, repRewStrat, repPenStrat, sciRewStrat;
		private List<Notes_ContractParameterInfo> parameters = new List<Notes_ContractParameterInfo>();

		public Notes_ContractParameterInfo(Notes_ContractInfo Root, ContractParameter c, int l)
		{
			root = Root;
			param = c;
			try
			{
				title = param.Title;
			}
			catch (Exception e)
			{
				Debug.LogError("BetterNotes] Parameter Title not set, using type name..." + e);
				title = param.GetType().Name;
			}

			level = l;

			if (level < 4)
			{
				for (int i = 0; i < param.ParameterCount; i++)
				{
					ContractParameter cp = param.GetParameter(i);
					if (cp == null)
						continue;

					addToParams(cp, level + 1);
				}
			}

			paramRewards();
			paramPenalties();
		}

		private void addToParams(ContractParameter p, int Level)
		{
			Notes_ContractParameterInfo cc = new Notes_ContractParameterInfo(root, p, Level);
			parameters.Add(cc);
			root.addToParams(cc);
		}

		private void paramRewards()
		{
			CurrencyModifierQuery currencyQuery = CurrencyModifierQuery.RunQuery(TransactionReasons.ContractReward, (float)param.FundsCompletion, param.ScienceCompletion, param.ReputationCompletion);
			fundsRew = "";
			if (param.FundsCompletion != 0)
				fundsRew = "+ " + param.FundsCompletion.ToString("N0");
			fundsRewStrat = currencyQuery.GetEffectDelta(Currency.Funds);
			if (fundsRewStrat != 0f)
			{
				fundsRew = string.Format("+ {0:N0} ({1:N0})", param.FundsCompletion + fundsRewStrat, fundsRewStrat);
			}

			repRew = "";
			if (param.ReputationCompletion != 0)
				repRew = "+ " + param.ReputationCompletion.ToString("N0");
			repRewStrat = currencyQuery.GetEffectDelta(Currency.Reputation);
			if (repRewStrat != 0f)
			{
				repRew = string.Format("+ {0:N0} ({1:N0})", param.ReputationCompletion + repRewStrat, repRewStrat);
			}

			sciRew = "";
			if (param.ScienceCompletion != 0)
				sciRew = "+ " + param.ScienceCompletion.ToString("N0");
			sciRewStrat = currencyQuery.GetEffectDelta(Currency.Science);
			if (sciRewStrat != 0f)
			{
				sciRew = string.Format("+ {0:N0} ({1:N0})", param.ScienceCompletion + sciRewStrat, sciRewStrat);
			}
		}

		private void paramPenalties()
		{
			CurrencyModifierQuery currencyQuery = CurrencyModifierQuery.RunQuery(TransactionReasons.ContractPenalty, (float)param.FundsFailure, 0f, param.ReputationFailure);

			fundsPen = "";
			if (param.FundsFailure != 0)
				fundsPen = "- " + param.FundsFailure.ToString("N0");
			fundsPenStrat = currencyQuery.GetEffectDelta(Currency.Funds);
			if (fundsPenStrat != 0f)
			{
				fundsPen = string.Format("- {0:N0} ({1:N0})", param.FundsFailure + fundsPenStrat, fundsPenStrat);
			}

			repPen = "";
			if (param.ReputationFailure != 0)
				repPen = "- " + param.ReputationFailure.ToString("N0");
			repPenStrat = currencyQuery.GetEffectDelta(Currency.Reputation);
			if (repPenStrat != 0f)
			{
				repPen = string.Format("- {0:N0} ({1:N0})", param.ReputationFailure + repPenStrat, repPenStrat);
			}
		}

		public Notes_ContractParameterInfo getParameter(int i)
		{
			if (parameters.Count >= i)
				return parameters[i];
			else
				Notes_MBE.LogFormatted("BetterNotes Sub Parameter List Index Out Of Range; Something Went Wrong Here...");

			return null;
		}

		public string Title
		{
			get { return title; }
		}

		public int Level
		{
			get { return level; }
		}

		public ContractParameter Param
		{
			get { return param; }
		}

		public int ParameterCount
		{
			get { return parameters.Count; }
		}

		public string FundsRew
		{
			get { return fundsRew; }
		}

		public string FundsPen
		{
			get { return fundsPen; }
		}

		public string RepRew
		{
			get { return repRew; }
		}

		public string RepPen
		{
			get { return repPen; }
		}

		public string SciRew
		{
			get { return sciRew; }
		}

		public float FundsRewStrat
		{
			get { return fundsRewStrat; }
		}

		public float FundsPenStrat
		{
			get { return fundsPenStrat; }
		}

		public float RepRewStrat
		{
			get { return repRewStrat; }
		}

		public float RepPenStrat
		{
			get { return repPenStrat; }
		}

		public float SciRewStrat
		{
			get { return sciRewStrat; }
		}

		public Notes_ContractInfo Root
		{
			get { return root; }
		}
	}
}
