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
using ContractParser;

namespace BetterNotes.NoteClasses
{
	public class Notes_ContractContainer : Notes_Base
	{
		private Dictionary<Guid, Notes_ContractShell> activeContracts = new Dictionary<Guid, Notes_ContractShell>();
		private Dictionary<Guid, Notes_ContractShell> completedContracts = new Dictionary<Guid, Notes_ContractShell>();
		private List<Guid> activeContractIDs = new List<Guid>();
		private List<Guid> completedContractIDs = new List<Guid>();
		private bool archived;

		public Notes_ContractContainer()
		{ }

		public Notes_ContractContainer(Notes_Container n)
		{
			root = n;
			vessel = n.NotesVessel;
		}

		public Notes_ContractContainer(Notes_Archive_Container n)
		{
			archive_Root = n;
			vessel = null;
			archived = true;
		}

		public Notes_ContractContainer(Notes_ContractContainer copy, List<Guid> id, Notes_Container n)
		{
			activeContracts = copy.activeContracts;
			completedContracts = copy.completedContracts;
			activeContractIDs = id;
			completedContractIDs = id;
			root = n;
			vessel = n.NotesVessel;
		}

		public Notes_ContractContainer(Notes_ContractContainer copy, List<Guid> id, Notes_Archive_Container n)
		{
			activeContracts = copy.activeContracts;
			completedContracts = copy.completedContracts;
			activeContractIDs = id;
			completedContractIDs = id;
			archive_Root = n;
			vessel = null;
			archived = true;
		}

		public void contractsRefresh()
		{
			for (int i = 0; i < activeContractIDs.Count; i++)
			{
				Guid g = activeContractIDs[i];

				if (g == null)
					continue;

				if (activeContracts.ContainsKey(g))
					continue;

				contractContainer c = contractParser.getActiveContract(g);

				if (c == null)
					continue;

				Notes_ContractShell shell = new Notes_ContractShell(c, this);

				activeContracts.Add(g, shell);
			}

			for (int i = 0; i < completedContractIDs.Count; i++)
			{
				Guid g = completedContractIDs[i];

				if (g == null)
					continue;

				if (completedContracts.ContainsKey(g))
					continue;

				contractContainer c = contractParser.getCompletedContract(g);

				if (c == null)
					c = contractParser.getFailedContract(g);

				if (c == null)
					continue;

				Notes_ContractShell shell = new Notes_ContractShell(c, this);

				completedContracts.Add(g, shell);
			}
		}

		public void addActiveContract(Guid id)
		{
			if (archived)
				return;

			if (!activeContractIDs.Contains(id))
				activeContractIDs.Add(id);

			contractsRefresh();
		}

		public void addCompletedContract(Guid id)
		{
			if (archived)
				return;

			if (!completedContractIDs.Contains(id))
				completedContractIDs.Add(id);

			contractsRefresh();
		}

		public void removeActiveContract(Guid id)
		{
			if (activeContracts.ContainsKey(id))
			{
				activeContracts[id] = null;
				activeContracts.Remove(id);
			}
			if (activeContractIDs.Contains(id))
				activeContractIDs.RemoveAll(a => a == id);

			contractsRefresh();
		}

		public void removeCompletedContract(Guid id)
		{
			if (completedContracts.ContainsKey(id))
			{
				completedContracts[id] = null;
				completedContracts.Remove(id);
			}
			if (completedContractIDs.Contains(id))
				completedContractIDs.RemoveAll(a => a == id);

			contractsRefresh();
		}

		public int activeContractCount
		{
			get { return activeContracts.Count; }
		}

		public int completedContractCount
		{
			get { return completedContracts.Count; }
		}

		public Notes_ContractShell getActiveContract(int index, bool warn = false)
		{
			if (activeContracts.Count > index)
				return activeContracts.ElementAt(index).Value;
			else if (warn)
				Debug.LogWarning("Notes Contracts dictionary index out of range; something went wrong here...");

			return null;
		}

		public Notes_ContractShell getActiveContract(Guid id)
		{
			if (activeContracts.ContainsKey(id))
				return activeContracts[id];

			return null;
		}

		public Notes_ContractShell getCompletedContract(int index, bool warn = false)
		{
			if (completedContracts.Count > index)
				return completedContracts.ElementAt(index).Value;
			else if (warn)
				Debug.LogWarning("Notes Contracts dictionary index out of range; something went wrong here...");

			return null;
		}

		public Notes_ContractShell getCompletedContract(Guid id)
		{
			if (completedContracts.ContainsKey(id))
				return completedContracts[id];

			return null;
		}

		public IEnumerable<Guid> getAllActiveContractIDs
		{
			get { return activeContracts.Keys; }
		}

		public IEnumerable<Guid> getAllCompletedContractIDs
		{
			get { return completedContracts.Keys; }
		}

		public bool Archived
		{
			get { return archived; }
		}
	}

	public class Notes_ContractShell
	{
		private contractContainer contract;
		private Notes_ContractContainer root;

		public Notes_ContractShell(contractContainer c, Notes_ContractContainer r)
		{
			contract = c;
			root = r;
		}

		public contractContainer ContractInfo
		{
			get { return contract; }
		}

		public Notes_ContractContainer RootContainer
		{
			get { return root; }
		}
	}


}
