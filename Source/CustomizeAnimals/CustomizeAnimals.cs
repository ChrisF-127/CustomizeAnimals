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
		public static List<AnimalSettings> Animals { get; private set; } = null;
		public CustomizeAnimals_Settings Settings { get; private set; } = null;
		public AnimalSettings SelectedAnimalSettings { get; private set; } = null;
		#endregion

		#region FIELDS
		private string _searchTerm = "";
		private float _listViewHeight = 0;
		private Vector2 _listScrollPosition = new Vector2();
		private const float _listRowHeight = 32;
		private const float _listIconSize = _listRowHeight - 4;
		private const float _listOffsetY = 64;

		private float _settingsViewHeight = 0;
		private Vector2 _settingsScrollPosition = new Vector2();
		private const float _settingsRowHeight = 32;
		private const float _settingsIconSize = 64;
		private const float _settingsOffsetY = 64;

		private GameFont _oriTextFont;
		private TextAnchor _oriTextAnchor;
		private Color _oriColor;
		private Color _modifiedColor = Color.cyan;
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
			Log.Message($"{nameof(CustomizeAnimals)}.{nameof(Initialize)}");

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

		#region OVERRIDES
		public override string SettingsCategory() => 
			"Customize Animals";

		public override void DoSettingsWindowContents(Rect inRect)
		{
			var listWidth = inRect.width * 1 / 3 - 4;
			var optionsWidth = inRect.width * 2 / 3 - 4;

			// Save original settings
			_oriTextFont = Text.Font;
			_oriTextAnchor = Text.Anchor;
			_oriColor = GUI.color;

			// Animal selection list
			CreateAnimalList(inRect.x, inRect.y, listWidth, inRect.height);

			// Animal settings
			CreateSettings(inRect.x + listWidth + 8, inRect.y, optionsWidth, inRect.height);

			base.DoSettingsWindowContents(inRect);
		}
		#endregion

		#region PRIVATE METHODS
		#region MOD SETTINGS
		private void CreateAnimalList(float x, float y, float width, float height)
		{
			// Begin
			GUI.BeginGroup(new Rect(x, y, width, height));
			Text.Anchor = TextAnchor.MiddleLeft;

			// Label
			Text.Font = GameFont.Medium;
			Widgets.Label(new Rect(0, 0, width, _listRowHeight), "SY_CA.SelectAnimalToCustomize".Translate());
			Text.Font = GameFont.Small;

			// Search field
			var searchFieldRect = new Rect(0, 36, width - 84, 20);
			_searchTerm = Widgets.TextArea(searchFieldRect, _searchTerm);
			if (string.IsNullOrEmpty(_searchTerm))
			{
				Text.Anchor = TextAnchor.MiddleCenter;
				GUI.color = Color.grey;
				Widgets.Label(new Rect(searchFieldRect.x, searchFieldRect.y + 2, searchFieldRect.width, searchFieldRect.height - 2), "SY_CA.Filter".Translate());
				GUI.color = _oriColor;
				Text.Anchor = TextAnchor.MiddleLeft;
			}
			DrawTooltip(searchFieldRect, "SY_CA.TooltipFilter".Translate());
			var clearButtonRect = new Rect(width - 76, 36, 60, 20);
			if (Widgets.ButtonText(clearButtonRect, "SY_CA.Clear".Translate()))
				_searchTerm = "";
			DrawTooltip(clearButtonRect, "SY_CA.TooltipClearFilter".Translate());

			// Scrollable area
			var viewWidth = width - 16;
			Widgets.BeginScrollView(
				new Rect(0, _listOffsetY, width, height - _listOffsetY),
				ref _listScrollPosition,
				new Rect(0, _listOffsetY, viewWidth, _listViewHeight));

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
				float offsetY = _listOffsetY + _listRowHeight * index;
				if (Widgets.ButtonInvisible(new Rect(0, offsetY, viewWidth, _listRowHeight), true))
					SelectedAnimalSettings = animalSettings;
				Widgets.DrawOptionBackground(new Rect(0, offsetY, viewWidth, _listRowHeight), SelectedAnimalSettings == animalSettings);

				// Icon
				float offsetX = 2;
				Widgets.DefIcon(new Rect(offsetX, offsetY + 2, _listIconSize, _listIconSize), animal);
				offsetX += _listIconSize + 8;

				// Label
				float labelWidth = viewWidth - _listIconSize - 12;
				if (animalSettings.IsModified())
					GUI.color = _modifiedColor;
				Widgets.Label(new Rect(offsetX, offsetY, labelWidth, _listRowHeight), animal.label.CapitalizeFirst());
				GUI.color = _oriColor;

				index++;
			}
			_listViewHeight = _listRowHeight * index + 1;

			// End
			Widgets.EndScrollView();
			GUI.EndGroup();

			// Reset text settings
			Text.Font = _oriTextFont;
			Text.Anchor = _oriTextAnchor;
		}

		private void CreateSettings(float x, float y, float width, float height)
		{
			// Selected animal for easier access
			var animal = SelectedAnimalSettings?.Animal;

			// Begin
			GUI.BeginGroup(new Rect(x, y, width, height));
			Text.Anchor = TextAnchor.MiddleLeft;

			// Animal settings
			var labelRect = new Rect(_settingsIconSize + 32, 0, width - _settingsIconSize + 32, _listRowHeight);
			if (animal != null)
			{
				// Header
				Text.Font = GameFont.Medium;
				Widgets.DefIcon(new Rect(8, 0, _settingsIconSize, _settingsIconSize), animal);
				Widgets.Label(labelRect, animal.label.CapitalizeFirst());
				Text.Font = GameFont.Small;
				// Mod name
				Widgets.Label(new Rect(_settingsIconSize + 32, _listRowHeight, width - _settingsIconSize + 32, _settingsIconSize - _listRowHeight), $"({animal.modContentPack.Name})");

				// Begin
				var viewWidth = width - 16;
				Widgets.BeginScrollView(
					new Rect(0, _settingsOffsetY, width, height - _settingsOffsetY),
					ref _settingsScrollPosition,
					new Rect(0, _settingsOffsetY, viewWidth, _settingsViewHeight));

				// Settings
				int index = 0;
				// Trainability
				SettingTrainability(_settingsOffsetY + _settingsRowHeight * index++, viewWidth);
				// RoamMtbDays
				SettingRoamMtbDays(_settingsOffsetY + _settingsRowHeight * index++, viewWidth);

				// End
				Widgets.EndScrollView();

				// Remember settings view height for potential scrolling
				_settingsViewHeight = _settingsRowHeight * index;
			}
			// No animal selected
			else
			{
				// Header
				Text.Font = GameFont.Medium;
				Widgets.Label(labelRect, "SY_CA.NoAnimalSelected".Translate());
				Text.Font = GameFont.Small;

				// No settings available
				_settingsViewHeight = 0;
			}

			// End
			GUI.EndGroup();

			// Reset text settings
			Text.Font = _oriTextFont;
			Text.Anchor = _oriTextAnchor;
		}
		#endregion

		#region SETTING CONTROLS
		private void SettingTrainability(float offsetY, float viewWidth)
		{
			float controlWidth = GetControlWidth(viewWidth);
			var trainabilitySetting = SelectedAnimalSettings.Trainability;
			var roamMtbDaysSetting = SelectedAnimalSettings.RoamMtbDays;

			// Switch color if modified
			var labelRect = new Rect(0, offsetY, controlWidth, _settingsRowHeight);
			if (trainabilitySetting.Value != TrainabilityDefOf.None && roamMtbDaysSetting.Value != null)
			{
				GUI.color = Color.red;
				DrawTooltip(labelRect, "SY_CA.TrainabilityRoamingWarning".Translate());
			}
			else if (trainabilitySetting.IsModified())
				GUI.color = _modifiedColor;

			// Label
			Widgets.Label(labelRect, "SY_CA.Trainability".Translate());
			GUI.color = _oriColor;

			var trainability = trainabilitySetting.Value;
			var trainabilityOptionWidth = controlWidth / 3;
			var trainabilityOffsetY = offsetY + (_settingsRowHeight - 24) / 2;

			// None
			var trainabilityOffsetX = controlWidth + trainabilityOptionWidth * 0;
			if (Widgets.RadioButton(trainabilityOffsetX, trainabilityOffsetY, trainability == TrainabilityDefOf.None))
				trainability = TrainabilityDefOf.None;
			Widgets.Label(new Rect(trainabilityOffsetX + 30, offsetY, trainabilityOptionWidth, _settingsRowHeight), "SY_CA.TrainabilityNone".Translate());
			DrawTooltip(new Rect(trainabilityOffsetX, offsetY, trainabilityOptionWidth, _settingsRowHeight), "SY_CA.TooltipTrainabilityNone".Translate());

			// Intermediate
			trainabilityOffsetX = controlWidth + trainabilityOptionWidth * 1;
			if (Widgets.RadioButton(trainabilityOffsetX, trainabilityOffsetY, trainability == TrainabilityDefOf.Intermediate))
				trainability = TrainabilityDefOf.Intermediate;
			Widgets.Label(new Rect(trainabilityOffsetX + 30, offsetY, trainabilityOptionWidth, _settingsRowHeight), "SY_CA.TrainabilityIntermediate".Translate());
			DrawTooltip(new Rect(trainabilityOffsetX, offsetY, trainabilityOptionWidth, _settingsRowHeight), "SY_CA.TooltipTrainabilityIntermediate".Translate());

			// Advanced
			trainabilityOffsetX = controlWidth + trainabilityOptionWidth * 2;
			if (Widgets.RadioButton(trainabilityOffsetX, trainabilityOffsetY, trainability == TrainabilityDefOf.Advanced))
				trainability = TrainabilityDefOf.Advanced;
			Widgets.Label(new Rect(trainabilityOffsetX + 30, offsetY, trainabilityOptionWidth, _settingsRowHeight), "SY_CA.TrainabilityAdvanced".Translate());
			DrawTooltip(new Rect(trainabilityOffsetX, offsetY, trainabilityOptionWidth, _settingsRowHeight), "SY_CA.TooltipTrainabilityAdvanced".Translate());

			// Reset button
			if (trainabilitySetting.IsModified() && DrawResetButton(offsetY, viewWidth, trainabilitySetting.DefaultValue.ToString()))
				trainabilitySetting.Reset();
			// Set Trainability
			else
				trainabilitySetting.Value = trainability;
		}

		private void SettingRoamMtbDays(float offsetY, float viewWidth)
		{
			float controlWidth = GetControlWidth(viewWidth);
			var roamMtbDaysSetting = SelectedAnimalSettings.RoamMtbDays;

			// Label
			// Switch color if modified
			if (roamMtbDaysSetting.IsModified())
				GUI.color = _modifiedColor;
			Widgets.Label(new Rect(0, offsetY, controlWidth, _settingsRowHeight), "SY_CA.RoamMtbDays".Translate());
			GUI.color = _oriColor;

			// RoamMtbDays Settings
			float? roamMtbDays; 
			var roamSelected = roamMtbDaysSetting.Value != null;
			var checkboxSize = _settingsRowHeight - 8;
			Widgets.Checkbox(controlWidth, offsetY + (_settingsRowHeight - checkboxSize) / 2, ref roamSelected, checkboxSize);
			DrawTooltip(new Rect(controlWidth, offsetY, checkboxSize, checkboxSize), "SY_CA.TooltipRoamMtbDaysChk".Translate());
			// RoamMtbDays active = roamer (requires pen!)
			if (roamSelected)
			{
				var roamValue = roamMtbDaysSetting.Value ?? roamMtbDaysSetting.DefaultValue ?? 2;
				var roamBuffer = roamValue.ToString();
				var textFieldRect = new Rect(controlWidth + checkboxSize, offsetY + (_settingsRowHeight - 20) / 2, controlWidth - checkboxSize, 20);
				Widgets.TextFieldNumeric(textFieldRect, ref roamValue, ref roamBuffer, 1);
				DrawTooltip(textFieldRect, "SY_CA.TooltipRoamMtbDays".Translate());
				roamMtbDays = roamValue;
			}
			else
				roamMtbDays = null;

			// Reset button
			if (roamMtbDaysSetting.IsModified() && DrawResetButton(offsetY, viewWidth, (roamMtbDaysSetting.DefaultValue ?? 0).ToString()))
				roamMtbDaysSetting.Reset();
			// Set RoamMtbDays
			else
				roamMtbDaysSetting.Value = roamMtbDays;
		}
		#endregion

		#region CONTROLS HELPER
		private bool DrawResetButton(float offsetY, float viewWidth, string tooltip)
		{
			var buttonRect = new Rect(viewWidth - _settingsRowHeight * 2 + 2, offsetY + 2, _settingsRowHeight * 2 - 4, _settingsRowHeight - 4);
			var output = Widgets.ButtonText(buttonRect, "SY_CA.Reset".Translate());

			DrawTooltip(buttonRect, "SY_CA.TooltipDefaultValue".Translate() + " " + tooltip);
			return output;
		}

		private void DrawTooltip(Rect rect, string tooltip)
		{
			if (Mouse.IsOver(rect))
			{
				ActiveTip activeTip = new ActiveTip(tooltip);
				activeTip.DrawTooltip(GenUI.GetMouseAttachedWindowPos(activeTip.TipRect.width, activeTip.TipRect.height) + (UI.MousePositionOnUIInverted - Event.current.mousePosition));
			}
		}

		private float GetControlWidth(float viewWidth) => 
			viewWidth / 2 - _settingsRowHeight - 4;
		#endregion
		#endregion
	}
}
