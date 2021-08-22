using CustomizeAnimals.Controls;
using CustomizeAnimals.Settings;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace CustomizeAnimals
{
	public class CustomizeAnimals : Mod
	{
		#region PROPERTIES
		public static GlobalSettings Global { get; set; } = new GlobalSettings();
		public static List<AnimalSettings> Animals { get; private set; } = null;
		public static CustomizeAnimals_ModSettings Settings { get; private set; } = null;
		public static AnimalSettings SelectedAnimalSettings { get; private set; } = null;

		private List<BaseControls> SettingsList { get; } = new List<BaseControls>
		{
			new ControlsTrainability(),
			new ControlsRoamMtbDays(),
		};
		#endregion

		#region FIELDS
		private string _searchTerm = "";
		private float _listViewHeight = 0;

		private const float _listRowHeight = 32;
		private const float _listIconSize = _listRowHeight - 4;

		private Vector2 _listScrollPosition = new Vector2();
		private Vector2 _settingsScrollPosition = new Vector2();

		public static float SettingsViewHeight = 0;
		public const float SettingsRowHeight = 32;
		public const float SettingsIconSize = 64;
		public const float SettingsOffsetY = 64;

		public static GameFont OriTextFont;
		public static TextAnchor OriTextAnchor;
		public static Color OriColor;
		public static Color ModifiedColor = Color.cyan;
		#endregion

		#region CONSTRUCTORS
		public CustomizeAnimals(ModContentPack content) : base(content)
		{
			LongEventHandler.ExecuteWhenFinished(Initialize);
		}
		#endregion

		#region PUBLIC METHODS
		public void Initialize()
		{
			Animals = new List<AnimalSettings>();
			foreach (var thingDef in DefDatabase<ThingDef>.AllDefs)
			{
				if (AnimalSettings.IsValidAnimal(thingDef))
					Animals.Add(new AnimalSettings(thingDef));
			}
			Animals.SortBy((a) => a.Animal.label);

			Settings = GetSettings<CustomizeAnimals_ModSettings>();
		}

		public void ResetAll()
		{
			Global.Reset();
			foreach (var animal in Animals)
				animal.Reset();
			Log.Message($"{nameof(CustomizeAnimals)}: All settings have been reset!");
		}
		#endregion

		#region PRIVATE METHODS
		private void CreateAnimalList(float x, float y, float width, float height)
		{
			float offsetY = 0;
			float viewWidth = width - 16;

			// Begin
			GUI.BeginGroup(new Rect(x, y, width, height));
			Text.Anchor = TextAnchor.MiddleLeft;

			// Label
			Text.Font = GameFont.Medium;
			Widgets.Label(new Rect(0, offsetY, width, _listRowHeight), "SY_CA.SelectAnimalToCustomize".Translate());
			Text.Font = GameFont.Small;
			offsetY += 36;


			// AnimalSettings Global
			// Selection
			var globalSettingsSelectionRect = new Rect(0, offsetY, viewWidth, _listRowHeight);
			if (Widgets.ButtonInvisible(globalSettingsSelectionRect, true))
				SelectedAnimalSettings = null;
			Widgets.DrawOptionBackground(globalSettingsSelectionRect, SelectedAnimalSettings == null);
			BaseControls.DrawTooltip(globalSettingsSelectionRect, "SY_CA.TooltipGlobalSettings".Translate());

			// Icon
			Text.Font = GameFont.Medium;
			Text.Anchor = TextAnchor.MiddleCenter;
			Widgets.Label(new Rect(4, offsetY, _listIconSize, _listRowHeight), "∀");
			Text.Anchor = TextAnchor.MiddleLeft;
			Text.Font = GameFont.Small;

			// Label
			float labelWidth = viewWidth - _listIconSize - 12;
			if (Global.IsGlobalUsed())
				GUI.color = ModifiedColor;
			Widgets.Label(new Rect(_listIconSize + 10, offsetY + 2, labelWidth, _listRowHeight - 2), "SY_CA.GlobalSettings".Translate());
			GUI.color = OriColor;
			offsetY += 42;


			// Search field
			var searchFieldRect = new Rect(0, offsetY, width - 84, 20);
			_searchTerm = Widgets.TextArea(searchFieldRect, _searchTerm);
			if (string.IsNullOrEmpty(_searchTerm))
			{
				Text.Anchor = TextAnchor.MiddleCenter;
				GUI.color = Color.grey;
				Widgets.Label(new Rect(searchFieldRect.x, searchFieldRect.y + 2, searchFieldRect.width, searchFieldRect.height - 2), "SY_CA.Filter".Translate());
				GUI.color = OriColor;
				Text.Anchor = TextAnchor.MiddleLeft;
			}
			BaseControls.DrawTooltip(searchFieldRect, "SY_CA.TooltipFilter".Translate());
			var clearButtonRect = new Rect(width - 76, offsetY, 60, 20);
			if (Widgets.ButtonText(clearButtonRect, "SY_CA.Clear".Translate()))
				_searchTerm = "";
			BaseControls.DrawTooltip(clearButtonRect, "SY_CA.TooltipClearFilter".Translate());
			offsetY += 26;


			// Scrollable area
			Widgets.BeginScrollView(
				new Rect(0, offsetY, width, height - offsetY),
				ref _listScrollPosition,
				new Rect(0, offsetY, viewWidth, _listViewHeight));
			_listScrollPosition.y = Mathf.Round(_listScrollPosition.y / _listRowHeight) * _listRowHeight;

			// Make list
			int index = 0;
			string searchLower = _searchTerm?.ToLower();
			foreach (var animalSettings in Animals)
			{
				var animal = animalSettings.Animal;
				if (animal == null)
					continue;
				if (searchLower != "")
				{
					// Apply filter
					if (searchLower != "*")
					{
						if (!animal.label.ToLower().Contains(searchLower))
							continue;
					}
					// Show only modified animals
					else // (searchLower == "*")
					{
						if (!animalSettings.IsModified())
							continue;
					}
				}

				// Selection
				float listOffsetY = offsetY + _listRowHeight * index;
				if (Widgets.ButtonInvisible(new Rect(0, listOffsetY, viewWidth, _listRowHeight), true))
					SelectedAnimalSettings = animalSettings;
				Widgets.DrawOptionBackground(new Rect(0, listOffsetY, viewWidth, _listRowHeight), SelectedAnimalSettings == animalSettings);

				// Icon
				float listOffsetX = 2;
				Widgets.DefIcon(new Rect(listOffsetX, listOffsetY + 2, _listIconSize, _listIconSize), animal);
				listOffsetX += _listIconSize + 8;

				// Label
				labelWidth = viewWidth - _listIconSize - 12;
				if (animalSettings.IsModified())
					GUI.color = ModifiedColor;
				Widgets.Label(new Rect(listOffsetX, listOffsetY + 2, labelWidth, _listRowHeight - 2), animal.label.CapitalizeFirst());
				GUI.color = OriColor;

				index++;
			}
			_listViewHeight = _listRowHeight * index + 1;

			// End
			Widgets.EndScrollView();
			GUI.EndGroup();


			// Reset text settings
			Text.Font = OriTextFont;
			Text.Anchor = OriTextAnchor;
		}

		private void CreateSettings(float x, float y, float width, float height)
		{
			// Selected animal for easier access
			var animal = SelectedAnimalSettings?.Animal;
			var isGlobal = animal == null;

			// Begin
			GUI.BeginGroup(new Rect(x, y, width, height));
			Text.Anchor = TextAnchor.MiddleLeft;

			// Animal settings
			var labelRect = new Rect(SettingsIconSize + 32, 0, width - SettingsIconSize + 32, _listRowHeight);

			// Header
			if (!isGlobal)
			{
				// Animal icon
				Widgets.DefIcon(new Rect(8, 0, SettingsIconSize, SettingsIconSize), animal);

				// Animal name
				Text.Font = GameFont.Medium;
				Widgets.Label(labelRect, animal.label.CapitalizeFirst());
				Text.Font = GameFont.Small;

				// Mod name
				Widgets.Label(new Rect(SettingsIconSize + 32, _listRowHeight, width - SettingsIconSize + 32, SettingsIconSize - _listRowHeight), $"({animal.modContentPack.Name})");
			}
			else
			{
				// Header
				Text.Font = GameFont.Medium;
				Widgets.Label(labelRect, "SY_CA.GlobalSettingsHeader".Translate());
				Text.Font = GameFont.Small;

				// Reset all
				var resetAllRect = new Rect(width - 78, 0, 76, _listRowHeight);
				if (Widgets.ButtonText(resetAllRect, "SY_CA.GlobalSettingsResetAll".Translate()))
					Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation("SY_CA.DialogGlobalSettingsResetAll".Translate(), () => ResetAll()));
				BaseControls.DrawTooltip(resetAllRect, "SY_CA.TooltipGlobalSettingsResetAll".Translate());
			}

			// Begin
			var viewWidth = width - 16;
			Widgets.BeginScrollView(
				new Rect(0, SettingsOffsetY, width, height - SettingsOffsetY),
				ref _settingsScrollPosition,
				new Rect(0, SettingsOffsetY, viewWidth, SettingsViewHeight));

			// Settings
			float totalHeight = SettingsOffsetY;
			if (animal != null)
			{
				// Draw animal settings
				foreach (var setting in SettingsList)
					totalHeight += setting.CreateSetting(totalHeight, viewWidth, SelectedAnimalSettings);

				// Apply animal settings
				SelectedAnimalSettings.ApplySettings();
			}
			// No animal selected
			else
			{
				// Draw global settings
				foreach (var setting in SettingsList)
					totalHeight += setting.CreateSettingGlobal(totalHeight, viewWidth);

				// Apply global settings
				foreach (var animalSetting in Animals)
					animalSetting.ApplySettings();
			}

			// End
			Widgets.EndScrollView();

			// Remember settings view height for potential scrolling
			SettingsViewHeight = totalHeight;

			// End
			GUI.EndGroup();

			// Reset text settings
			Text.Font = OriTextFont;
			Text.Anchor = OriTextAnchor;
		}
		#endregion

		#region OVERRIDES
		public override string SettingsCategory() =>
			"Customize Animals";

		public override void DoSettingsWindowContents(Rect inRect)
		{
			var listWidth = inRect.width * 1 / 3 - 4;
			var optionsWidth = inRect.width * 2 / 3 - 4;

			// Save original settings
			OriTextFont = Text.Font;
			OriTextAnchor = Text.Anchor;
			OriColor = GUI.color;

			// Animal selection list
			CreateAnimalList(inRect.x, inRect.y, listWidth, inRect.height);

			// Animal settings
			CreateSettings(inRect.x + listWidth + 8, inRect.y, optionsWidth, inRect.height);

			base.DoSettingsWindowContents(inRect);
		}
		#endregion
	}
}
