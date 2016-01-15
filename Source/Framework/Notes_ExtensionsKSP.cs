using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using BetterNotes.NoteClasses;
using BetterNotes.NoteClasses.CheckListHandler;

namespace BetterNotes.Framework
{
	/// <summary>
	/// Class containing some extensions for KSP Classes
	/// </summary>
	public static class KSPExtensions
	{
		/// <summary>
		/// Returns the Stage number at which this part will be separated from the vehicle.
		/// </summary>
		/// <param name="p">Part to Check</param>
		/// <returns>Stage at which part will be decoupled. Returns -1 if the part will never be decoupled from the vessel</returns>
		internal static Int32 DecoupledAt(this Part p)
		{
			return CalcDecoupleStage(p);
		}

		/// <summary>
		/// Worker to find the decoupled at value
		/// </summary>
		/// <returns>Stage at which part will be decoupled. Returns -1 if the part will never be decoupled from the vessel</returns>
		private static Int32 CalcDecoupleStage(Part pTest)
		{
			Int32 stageOut = -1;

			//Is this part a decoupler
			if (pTest.Modules.OfType<ModuleDecouple>().Count() > 0 || pTest.Modules.OfType<ModuleAnchoredDecoupler>().Count() > 0)
			{
				stageOut = pTest.inverseStage;
			}
			//if not look further up the vessel tree
			else if (pTest.parent != null)
			{
				stageOut = CalcDecoupleStage(pTest.parent);
			}
			return stageOut;
		}

		public static string parse(this ConfigNode node, string name, string original)
		{
			if (node.HasValue(name))
				return node.GetValue(name);

			return original;
		}

		public static int parse(this ConfigNode node, string name, int original)
		{
			if (!node.HasValue(name))
				return original;

			int i = original;

			if (int.TryParse(node.GetValue(name), out i))
				return i;

			return original;
		}

		public static uint parse(this ConfigNode node, string name, uint original)
		{
			if (!node.HasValue(name))
				return original;

			uint i = original;

			if (uint.TryParse(node.GetValue(name), out i))
				return i;

			return original;
		}

		public static float parse(this ConfigNode node, string name, float original)
		{
			if (!node.HasValue(name))
				return original;

			float f = original;

			if (float.TryParse(node.GetValue(name), out f))
				return f;

			return original;
		}

		public static float? parse(this ConfigNode node, string name, float? original)
		{
			if (!node.HasValue(name))
				return original;

			float f = original == null ? 0 : (float)original;

			if (float.TryParse(node.GetValue(name), out f))
				return (float?)f;

			return original;
		}

		public static double parse(this ConfigNode node, string name, double original)
		{
			if (!node.HasValue(name))
				return original;

			double d = original;

			if (double.TryParse(node.GetValue(name), out d))
				return d;

			return original;
		}

		public static bool parse(this ConfigNode node, string name, bool original)
		{
			if (!node.HasValue(name))
				return original;

			bool b = original;

			if (bool.TryParse(node.GetValue(name), out b))
				return b;

			return original;
		}

		public static Vector2d parse(this ConfigNode node, string name, Vector2d original)
		{
			if (!node.HasValue(name))
				return original;

			Vector2d v = original;

			string[] values = node.GetValue(name).Split('|');

			if (values.Length != 2)
				return v;

			double first = original.x;
			double second = original.y;

			if (!double.TryParse(values[0], out first))
				first = original.x;

			if (!double.TryParse(values[1], out second))
				second = original.y;

			v.x = first;
			v.y = second;

			return v;
		}

		public static Guid parse(this ConfigNode node, string name, Guid original)
		{
			if (!node.HasValue(name))
				return original;

			Guid g = original;

			try
			{
				g = new Guid(node.GetValue(name));
			}
			catch (Exception e)
			{
				Debug.LogError("BetterNotes; Error while reading Guid value; creating new value...\n" + e);
			}

			return g;
		}

		public static DateTime parse(this ConfigNode node, string name, DateTime original)
		{
			if (!node.HasValue(name))
				return original;

			DateTime d = original;

			if (DateTime.TryParse(node.GetValue(name), out d))
				return d;

			return original;
		}

		public static List<Guid> parse(this ConfigNode node, string name, List<Guid> original)
		{
			if (!node.HasValue(name))
				return original;

			string source = node.GetValue(name);

			if (string.IsNullOrEmpty(source))
				return original;
			else
			{
				List<Guid> ids = new List<Guid>();

				string[] sA = source.Split('|');
				for (int i = 0; i < sA.Length; i++)
				{
					try
					{
						Guid g = new Guid(sA[i]);

						if (g == null)
							continue;

						if (!ids.Contains(g))
							ids.Add(g);
					}
					catch (Exception e)
					{
						Notes_MBE.LogFormatted("[Better Notes] Guid invalid: {0}", e);
						continue;
					}
				}

				return ids;
			}
		}

		public static Notes_CheckListType parse(this ConfigNode node, string name, Notes_CheckListType original)
		{
			if (!node.HasValue(name))
				return original;

			Notes_CheckListType n = original;

			string s = node.GetValue(name);

			if (string.IsNullOrEmpty(s))
				return original;
			else
			{
				try
				{
					n = (Notes_CheckListType)Enum.Parse(typeof(Notes_CheckListType), s);
				}
				catch (Exception e)
				{
					Notes_MBE.LogFormatted("[Better Notes] Check List Type invalid: {0}", e);
					return original;
				}
			}

			return n;
		}

		public static Vessel parse(this ConfigNode node, string name, Vessel original)
		{
			if (!node.HasValue(name))
				return original;

			Vessel v= original;

			string s = node.GetValue(name);

			if (string.IsNullOrEmpty(s))
				return original;
			else
			{
				try
				{
					Guid id = new Guid(s);

					v = FlightGlobals.Vessels.FirstOrDefault(a => a.id == id);

					if (v == null)
						return original;
				}
				catch (Exception e)
				{
					Notes_MBE.LogFormatted("[Better Notes] Check List Target Vessel invalid: {0}", e);
					return original;
				}
			}

			return v;
		}

		public static VesselType parse(this ConfigNode node, string name, VesselType original)
		{
			if (!node.HasValue(name))
				return original;

			VesselType t = original;

			int i = (int)original;

			if (!int.TryParse(node.GetValue(name), out i))
				return original;

			if (i > 10 || i < 0)
				return original;

			return (VesselType)i;
		}

		public static Vessel.Situations parse(this ConfigNode node, string name, Vessel.Situations original)
		{
			if (!node.HasValue(name))
				return original;

			Vessel.Situations sit = original;

			string s = node.GetValue(name);

			if (string.IsNullOrEmpty(s))
				return original;
			else
			{
				try
				{
					sit = (Vessel.Situations)Enum.Parse(typeof(Vessel.Situations), s);
				}
				catch (Exception e)
				{
					Notes_MBE.LogFormatted("[Better Notes] Check List Target Situation invalid: {0}", e);
					return original;
				}
			}

			return sit;
		}

		public static CelestialBody parse(this ConfigNode node, string name, CelestialBody original)
		{
			if (!node.HasValue(name))
				return original;

			CelestialBody c = original;

			string s = node.GetValue(name);

			if (string.IsNullOrEmpty(s))
				return original;
			else
			{
				try
				{
					c = FlightGlobals.Bodies.FirstOrDefault(a => a.name == s);

					if (c == null)
						return original;
				}
				catch (Exception e)
				{
					Notes_MBE.LogFormatted("[Better Notes] Check List Target Body invalid: {0}", e);
					return original;
				}
			}

			return c;
		}

		public static ScienceSubject parse(this ConfigNode node, string name, ScienceSubject original)
		{
			if (!node.HasValue(name))
				return original;

			ScienceSubject subject = original;

			string id = node.GetValue(name);

			subject = ResearchAndDevelopment.GetSubjectByID(id);

			if (subject != null)
				return subject;

			return original;
		}

		public static string vector2ToString(this Vector2d v)
		{
			return v.x.ToString("F6") + "|" + v.y.ToString("F6");
		}

	}
}
