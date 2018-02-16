using System.Collections.Generic;
using System.Linq;
using Sitecore;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.Diagnostics;
using Sitecore.Shell.Applications.ContentEditor.Gutters;

namespace Menon.Me.Sitecore.LanguageWarnings.Renderers
{
	public class LanguageWarnings : GutterRenderer
	{
		private static List<string> MissingLanguages { get; set; }

		protected override GutterIconDescriptor GetIconDescriptor(Item item)
		{
			Assert.ArgumentNotNull(item, "item");

			return !IsValidItem(item) ? GetGutterIconDescriptor() : null;
		}

		private static GutterIconDescriptor GetGutterIconDescriptor()
		{
			var descriptor = new GutterIconDescriptor
			{
				Icon = "Applications/16x16/help_earth.png",
				Tooltip =
					$"Followng Language version{(MissingLanguages.Count > 1 ? "'s" : string.Empty)} are missing: \n {string.Join("\n", MissingLanguages.Select(x => x))}"
			};

			return descriptor;
		}

		private bool IsValidItem(Item item)
		{
			if (item.Fields[FieldIDs.LayoutField] != null && string.IsNullOrEmpty(item.Fields[FieldIDs.LayoutField].Value)) return true;

			var languageVersions = ItemManager.GetContentLanguages(item).Select(x => new
			{
				x.Name,
				Versions = ItemManager.GetVersions(item, x).Count
			}).ToList();

			if (languageVersions == null || !languageVersions.Any()) return true;

			MissingLanguages = languageVersions.Where(x => x.Versions == 0).Select(x => x.Name).ToList();

			return languageVersions.All(x => x.Versions != 0);
		}
	}
}
