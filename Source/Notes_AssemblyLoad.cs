using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace BetterNotes
{
	public class Notes_AssemblyLoad
	{
		private static bool cPlusListNamesLoaded = false;
		private static bool cPlusListCountLoaded = false;
		private static bool cPlusListLoaded = false;
		private static bool cPlusListStatusLoaded = false;

		private const string cPlusTypeName = "ContractsWindow.contractUtils";
		private const string cPlusMissionListName = "GetMissionNames";
		private const string cPlusMissionCount = "GetListCount";
		private const string cPlusMissionList = "GetMissionList";
		private const string cPlusMissionListStatus = "GetMissionListLoadedStatus";

		private static Type cPlusType;

		private delegate IEnumerable<string> CPlusMissionListNames();
		private delegate int CPlusListCount(string list);
		private delegate IEnumerable<Guid> CPlusMissionList(string list);

		private static CPlusMissionListNames _MissionListNames;
		private static CPlusListCount _MissionListCount;
		private static CPlusMissionList _CPlusMissionList;

		private static PropertyInfo _CPlusMissionListStatus;

		public static bool loadMethods()
		{
			cPlusListNamesLoaded = loadCPlusMissionListNames();
			cPlusListCountLoaded = loadCPlusMissionListCount();
			cPlusListLoaded = loadCPlusMissionList();
			cPlusListStatusLoaded = loadCPlusMissionListLoadedStatus();

			return cPlusListNamesLoaded && cPlusListCountLoaded && cPlusListLoaded && cPlusListStatusLoaded;
		}

		public static IEnumerable<string> GetContractMissionNames()
		{
			if (_MissionListNames == null)
				return null;

			return _MissionListNames();
		}

		public static int GetContractMissonCount(string name)
		{
			if (_MissionListCount == null)
				return 0;

			if (string.IsNullOrEmpty(name))
				return 0;

			return _MissionListCount(name);
		}

		public static IEnumerable<Guid> GetContractMission(string name)
		{
			if (_CPlusMissionList == null)
				return null;

			if (string.IsNullOrEmpty(name))
				return null;

			return _CPlusMissionList(name);
		}

		public static bool ContractMissionListsLoaded()
		{
			return (bool)_CPlusMissionListStatus.GetValue(null, null);
		}

		private static bool loadCPlusMissionListNames()
		{
			if (_MissionListNames != null)
				return true;

			try
			{
				if (cPlusType == null)
				{
					cPlusType = AssemblyLoader.loadedAssemblies.SelectMany(a => a.assembly.GetExportedTypes())
						.SingleOrDefault(t => t.FullName == cPlusTypeName);

					if (cPlusType == null)
					{
						Debug.Log("[BetterNotes] Contracts Window + Type Not Found");
						return false;
					}
				}

				MethodInfo CPlusMissionListNameMethod = cPlusType.GetMethod(cPlusMissionListName, BindingFlags.Static | BindingFlags.Public, null, new Type[] {}, null);

				if (CPlusMissionListNameMethod == null)
				{
					Debug.Log("[BetterNotes] Contracts Window + Mission List Name Method Not Found");
					return false;
				}

				_MissionListNames = (CPlusMissionListNames)Delegate.CreateDelegate(typeof(CPlusMissionListNames), CPlusMissionListNameMethod);

				Debug.Log("[BetterNotes] Contracts Window + Mission List Name Method Assigned");

				return _MissionListNames != null;
			}
			catch (Exception e)
			{
				Debug.Log("[BetterNotes] Error While Loading Contracts Window Plus Contract Reflection Methods: " + e);
				return false;
			}
		}

		private static bool loadCPlusMissionListCount()
		{
			if (_MissionListCount != null)
				return true;

			try
			{
				if (cPlusType == null)
				{
					cPlusType = AssemblyLoader.loadedAssemblies.SelectMany(a => a.assembly.GetExportedTypes())
						.SingleOrDefault(t => t.FullName == cPlusTypeName);

					if (cPlusType == null)
					{
						Debug.Log("[BetterNotes] Contracts Window + Type Not Found");
						return false;
					}
				}

				MethodInfo CPlusMissionCountMethod = cPlusType.GetMethod(cPlusMissionCount, BindingFlags.Static | BindingFlags.Public, null, new Type[] { typeof(string) }, null);

				if (CPlusMissionCountMethod == null)
				{
					Debug.Log("[BetterNotes] Contracts Window + Mission List Count Method Not Found");
					return false;
				}

				_MissionListCount = (CPlusListCount)Delegate.CreateDelegate(typeof(CPlusListCount), CPlusMissionCountMethod);

				Debug.Log("[BetterNotes] Contracts Window + Mission List Count Method Assigned");

				return _MissionListCount != null;
			}
			catch (Exception e)
			{
				Debug.Log("[BetterNotes] Error While Loading Contracts Window Plus Contract Reflection Methods: " + e);
				return false;
			}
		}

		private static bool loadCPlusMissionList()
		{
			if (_CPlusMissionList != null)
				return true;

			try
			{
				if (cPlusType == null)
				{
					cPlusType = AssemblyLoader.loadedAssemblies.SelectMany(a => a.assembly.GetExportedTypes())
						.SingleOrDefault(t => t.FullName == cPlusTypeName);

					if (cPlusType == null)
					{
						Debug.Log("[BetterNotes] Contracts Window + Type Not Found");
						return false;
					}
				}

				MethodInfo CPlusMissionListMethod = cPlusType.GetMethod(cPlusMissionList, BindingFlags.Static | BindingFlags.Public, null, new Type[] { typeof(string) }, null);

				if (CPlusMissionListMethod == null)
				{
					Debug.Log("[BetterNotes] Contracts Window + Mission List Method Not Found");
					return false;
				}

				_CPlusMissionList = (CPlusMissionList)Delegate.CreateDelegate(typeof(CPlusMissionList), CPlusMissionListMethod);

				Debug.Log("[BetterNotes] Contracts Window + Mission List Method Assigned");

				return _CPlusMissionList != null;
			}
			catch (Exception e)
			{
				Debug.Log("[BetterNotes] Error While Loading Contracts Window Plus Contract Reflection Methods: " + e);
				return false;
			}
		}

		private static bool loadCPlusMissionListLoadedStatus()
		{
			if (_CPlusMissionListStatus != null)
				return true;

			try
			{
				if (cPlusType == null)
				{
					cPlusType = AssemblyLoader.loadedAssemblies.SelectMany(a => a.assembly.GetExportedTypes())
						.SingleOrDefault(t => t.FullName == cPlusTypeName);

					if (cPlusType == null)
					{
						Debug.Log("[BetterNotes] Contracts Window + Type Not Found");
						return false;
					}
				}

				_CPlusMissionListStatus = cPlusType.GetProperty(cPlusMissionListStatus, BindingFlags.Static | BindingFlags.Public, null, typeof(bool), new Type[] { }, null);

				return _CPlusMissionListStatus != null;
			}
			catch (Exception e)
			{
				Debug.Log("[BetterNotes] Error While Loading Contracts Window Plus Mission List Status Reflection Methods: " + e);
				return false;
			}
		}
	}
}
