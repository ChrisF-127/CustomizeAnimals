﻿using RimWorld;
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
		public static List<AnimalSettings> Animals { get; private set; } = null;
		public static CustomizeAnimals_Settings Settings { get; private set; } = null;
		public static AnimalSettings SelectedAnimalSettings { get; private set; } = null;

		private List<Settings_Base> SettingsList { get; } = new List<Settings_Base>
		{
			new Settings_Trainability(),
			new Settings_RoamMtbDays(),
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

			Settings = GetSettings<CustomizeAnimals_Settings>();
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


			// All animals
			// Selection
			var allAnimalsSelectionRect = new Rect(0, offsetY, viewWidth, _listRowHeight);
			if (Widgets.ButtonInvisible(allAnimalsSelectionRect, true))
				SelectedAnimalSettings = null;
			Widgets.DrawOptionBackground(allAnimalsSelectionRect, SelectedAnimalSettings == null);
			Settings_Base.DrawTooltip(allAnimalsSelectionRect, "SY_CA.TooltipAllAnimalsSelection".Translate());

			// Icon
			Text.Font = GameFont.Medium;
			Text.Anchor = TextAnchor.MiddleCenter;
			Widgets.Label(new Rect(4, offsetY, _listIconSize, _listRowHeight), "∀");
			Text.Anchor = TextAnchor.MiddleLeft;
			Text.Font = GameFont.Small;

			// Label
			float labelWidth = viewWidth - _listIconSize - 12;
			Widgets.Label(new Rect(_listIconSize + 10, offsetY + 2, labelWidth, _listRowHeight - 2), "SY_CA.AllAnimalsSelection".Translate());
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
			Settings_Base.DrawTooltip(searchFieldRect, "SY_CA.TooltipFilter".Translate());
			var clearButtonRect = new Rect(width - 76, offsetY, 60, 20);
			if (Widgets.ButtonText(clearButtonRect, "SY_CA.Clear".Translate()))
				_searchTerm = "";
			Settings_Base.DrawTooltip(clearButtonRect, "SY_CA.TooltipClearFilter".Translate());
			offsetY += 26;

			// Scrollable area
			Widgets.BeginScrollView(
				new Rect(0, offsetY, width, height - offsetY),
				ref _listScrollPosition,
				new Rect(0, offsetY, viewWidth, _listViewHeight));
			_listScrollPosition.y = Mathf.Round(_listScrollPosition.y / _listViewHeight) * _listViewHeight;

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

			// Begin
			GUI.BeginGroup(new Rect(x, y, width, height));
			Text.Anchor = TextAnchor.MiddleLeft;

			// Animal settings
			var labelRect = new Rect(SettingsIconSize + 32, 0, width - SettingsIconSize + 32, _listRowHeight);
			if (animal != null)
			{
				// Header
				Text.Font = GameFont.Medium;
				Widgets.DefIcon(new Rect(8, 0, SettingsIconSize, SettingsIconSize), animal);
				Widgets.Label(labelRect, animal.label.CapitalizeFirst());
				Text.Font = GameFont.Small;
				// Mod name
				Widgets.Label(new Rect(SettingsIconSize + 32, _listRowHeight, width - SettingsIconSize + 32, SettingsIconSize - _listRowHeight), $"({animal.modContentPack.Name})");

				// Begin
				var viewWidth = width - 16;
				Widgets.BeginScrollView(
					new Rect(0, SettingsOffsetY, width, height - SettingsOffsetY),
					ref _settingsScrollPosition,
					new Rect(0, SettingsOffsetY, viewWidth, SettingsViewHeight));

				// Settings
				float totalHeight = SettingsOffsetY;
				foreach (var setting in SettingsList)
				{
					totalHeight += setting.CreateSetting(totalHeight, viewWidth, SelectedAnimalSettings);
				}

				// End
				Widgets.EndScrollView();

				// Remember settings view height for potential scrolling
				SettingsViewHeight = totalHeight;
			}
			// No animal selected
			else
			{
				// Header
				Text.Font = GameFont.Medium;
				Widgets.Label(labelRect, "SY_CA.AllAnimals".Translate());
				Text.Font = GameFont.Small;

				// No settings available
				SettingsViewHeight = 0;
			}

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
