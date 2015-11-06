using System;
using System.Collections.Generic;
using System.Linq;
using BetterNotes.Framework;

namespace BetterNotes
{
	public class Notes_Localization : Notes_ConfigNodeStorage
	{
		[Persistent]
		private List<Notes_LanguagePack> Language_Packs = new List<Notes_LanguagePack>();

		private Notes_LanguagePack activePack;

		public Notes_Localization(string path, string node)
		{
			FilePath = path;
			NodeName = path + "/" + node;

			if (!Load())
			{
				activePack = new Notes_LanguagePack();
				Language_Packs.Add(activePack);
				Save();
				LoadSavedCopy();
			}
		}

		public override void OnDecodeFromConfigNode()
		{
			activePack = Language_Packs.FirstOrDefault(l => l.ActivePack);

			if (activePack == null)
				activePack = new Notes_LanguagePack();
		}

		public Notes_LanguagePack ActivePack
		{
			get { return activePack; }
		}
	}
}
