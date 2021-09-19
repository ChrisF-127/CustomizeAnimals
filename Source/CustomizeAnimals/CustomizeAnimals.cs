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
	public enum SettingsSubMenuEnum
	{
		General,
	}

	internal class CustomizeAnimals : Mod
	{
		#region PROPERTIES
		public static CustomizeAnimals Instance { get; private set; } = null;

		public static GlobalSettings Global { get; set; } = new GlobalSettings();
		public static List<AnimalSettings> Animals { get; private set; } = null;
		public static CustomizeAnimals_ModSettings Settings { get; private set; } = null;
		public static AnimalSettings SelectedAnimalSettings { get; private set; } = null;

		private GeneralSettingsControls GeneralSettings { get; } = new GeneralSettingsControls();
		private List<BaseSettingControl> ControlsList { get; } = new List<BaseSettingControl>
		{
			new ControlMarketValue(),
			new ControlMeatAmount(),
			new ControlLeatherAmount(),
			new ControlToxicSensitivity(),
			new ControlBodySize(),
			new ControlHealthScale(),
			new ControlMoveSpeed(),
			new ControlTrainability(),
			new ControlFilthRate(),
			new ControlCaravanRidingSpeed(),
			new ControlCarryingCapacity(),
			new ControlPackAnimal(),
			new ControlRoamMtbDays(),
			new ControlWildness(),
			new ControlLifeExpectancy(),
			new ControlMaxTemperature(),
			new ControlMinTemperature(),
			new ControlHungerRate(),
			new ControlFoodType(),
			new ControlWillNeverEat(),
			new ControlManhunterOnTameFail(),
			new ControlManhunterOnDamage(),
			new ControlPredator(), // Predator & MaxPreyBodySize
			new ControlNuzzleMtbHours(),

			new ControlArmorRating_Sharp(),
			new ControlArmorRating_Blunt(),
			new ControlArmorRating_Heat(),
		};
		private Dictionary<string, BaseSpecialSettingControl> SpecialControlsList { get; } = new Dictionary<string, BaseSpecialSettingControl>
		{
		};
		#endregion

		#region FIELDS
		private ThingDef _previousAnimal = null;

		private string _searchTerm = "";
		private float _listViewHeight = 0;

		private const float _listRowHeight = 32;
		private const float _listIconSize = _listRowHeight - 4;

		private Vector2 _listScrollPosition = new Vector2();
		private Vector2 _settingsScrollPosition = new Vector2();

		public static float SettingsViewHeight = 0;
		public const float SettingsRowHeight = 32;
		public const float SettingsDoubleRowHeight = 56;
		public const float SettingsIconSize = 64;
		public const float SettingsOffsetY = 64;

		public static GameFont OriTextFont;
		public static TextAnchor OriTextAnchor;
		public static Color OriColor;
		public static Color ModifiedColor = Color.cyan;
		public static Color SelectionColor = Color.green;

		public static SettingsSubMenuEnum SelectedSettingsSubMenu = SettingsSubMenuEnum.General;
		#endregion

		#region CONSTRUCTORS
		public CustomizeAnimals(ModContentPack content) : base(content)
		{
			Instance = this;
			LongEventHandler.ExecuteWhenFinished(Initialize);
		}
		#endregion

		#region PUBLIC METHODS
		public void Initialize()
		{
			GlobalSettings.Initialize();

			Animals = new List<AnimalSettings>();
			foreach (var thingDef in DefDatabase<ThingDef>.AllDefs)
			{
				if (AnimalSettings.IsValidAnimal(thingDef))
					Animals.Add(new AnimalSettings(thingDef));
			}
			Animals.SortBy((a) => a.Animal.label);

			Settings = GetSettings<CustomizeAnimals_ModSettings>();
		}
		
		public void ResetControls(AnimalSettings animal)
		{
			foreach (var control in ControlsList)
				control.Reset();
			foreach (var control in SpecialControlsList.Values)
				control.Reset();
		}

		public void Reset(AnimalSettings animal)
		{
			if (animal == null)
				return;
			animal.Reset();
			ResetControls(animal);
			Log.Message($"{nameof(CustomizeAnimals)}: '{animal.Animal?.label?.CapitalizeFirst()}' settings have been reset!");
		}
		public void ResetAll()
		{
			Global.Reset();
			foreach (var animal in Animals)
				animal.Reset();
			ResetControls(null);
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
			BaseControl.DrawTooltip(globalSettingsSelectionRect, "SY_CA.TooltipGlobalSettings".Translate());

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
			BaseControl.DrawTooltip(searchFieldRect, "SY_CA.TooltipFilter".Translate());
			var clearButtonRect = new Rect(width - 76, offsetY, 60, 20);
			if (Widgets.ButtonText(clearButtonRect, "SY_CA.Clear".Translate()))
				_searchTerm = "";
			BaseControl.DrawTooltip(clearButtonRect, "SY_CA.TooltipClearFilter".Translate());
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
#warning TODO filter
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
			var viewWidth = width - 16;
			float topRow = SettingsOffsetY;

			// Reset settings text buffers if the animal changes
			if (_previousAnimal != animal)
			{
				ResetControls(SelectedAnimalSettings);
				_previousAnimal = animal;
			}

			// Begin
			GUI.BeginGroup(new Rect(x, y, width, height));
			Text.Anchor = TextAnchor.MiddleLeft;

			// Header
			var labelRect = new Rect(SettingsIconSize + 32, 0, width - SettingsIconSize + 32, _listRowHeight);
			var subLabelRect = new Rect(SettingsIconSize + 32, _listRowHeight, width - SettingsIconSize + 32, SettingsIconSize - _listRowHeight);
			string title, subtitle;
			if (!isGlobal)
			{
				// Animal icon
				Widgets.DefIcon(new Rect(8, 0, SettingsIconSize, SettingsIconSize), animal);

				// Title
				title = animal.label.CapitalizeFirst();
				subtitle = $"({animal.modContentPack.Name})";

				// Reset button
				var resetAllRect = new Rect(width - 78, 0, 76, _listRowHeight);
				if (Widgets.ButtonText(resetAllRect, "SY_CA.Reset".Translate()))
					Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation(
						"SY_CA.DialogSettingsReset".Translate(animal?.label?.CapitalizeFirst()) + "\n\n" + "SY_CA.DialogConfirm".Translate(), 
						() => Reset(SelectedAnimalSettings)));
				BaseControl.DrawTooltip(resetAllRect, "SY_CA.TooltipSettingsReset".Translate(animal?.label?.CapitalizeFirst()));


				// Sub menu buttons
				float buttonWidth = viewWidth / 2f;
				float buttonHeight = SettingsRowHeight / 1.5f;
				float subMenButtonOffsetX = 8;
				CreateSubMenuSelector(new Rect(subMenButtonOffsetX + 2, topRow, buttonWidth - 4, buttonHeight), "SY_CA.SubMenuGeneral".Translate(), SettingsSubMenuEnum.General);
				subMenButtonOffsetX += buttonWidth;
				topRow += buttonHeight;
			}
			else
			{
				// Icon
				Widgets.DefIcon(new Rect(8, 0, SettingsIconSize, SettingsIconSize), DefDatabase<ThingDef>.AllDefs.First((t) => t.defName == "Plant_Grass"));

				// Title
				title = "SY_CA.GlobalSettings".Translate();
				subtitle = $"({"SY_CA.GlobalSettingsSubtitle".Translate()})";

				// Reset all button
				var resetAllRect = new Rect(width - 78, 0, 76, _listRowHeight);
				if (Widgets.ButtonText(resetAllRect, "SY_CA.GlobalSettingsResetAll".Translate()))
					Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation(
						"SY_CA.DialogGlobalSettingsResetAll".Translate() + "\n\n" + "SY_CA.DialogConfirm".Translate(), 
						() => ResetAll()));
				BaseControl.DrawTooltip(resetAllRect, "SY_CA.TooltipGlobalSettingsResetAll".Translate());
			}

			// Title
			Text.Font = GameFont.Medium;
			Widgets.Label(labelRect, title);
			Text.Font = GameFont.Small;
			// Subtitle
			Text.Anchor = TextAnchor.UpperLeft;
			Widgets.Label(subLabelRect, subtitle);
			Text.Anchor = TextAnchor.MiddleLeft;


			// Begin
			Widgets.BeginScrollView(
				new Rect(0, topRow, width, height - SettingsOffsetY),
				ref _settingsScrollPosition,
				new Rect(0, topRow, viewWidth, SettingsViewHeight));

			// Animal settings
			float totalHeight = topRow;
			if (animal != null)
			{
				switch (SelectedSettingsSubMenu)
				{
					case SettingsSubMenuEnum.General:
						CreateSubMenuGeneral(ref totalHeight, viewWidth);
						break;
				}

				// Apply animal settings
				SelectedAnimalSettings.ApplySettings();
			}
			// Global settings
			else
			{
				// General settings separator
				Widgets.ListSeparator(ref totalHeight, width, "SY_CA.SeparatorGeneralSettings".Translate());
				totalHeight += 2;
				Text.Anchor = TextAnchor.MiddleLeft;

				// General settings
				totalHeight += GeneralSettings.DrawTrainabilityLimitsControls(totalHeight, viewWidth);


				// Global animal settings separator
				Widgets.ListSeparator(ref totalHeight, width, "SY_CA.SeparatorAnimalSettings".Translate());
				totalHeight += 2;
				Text.Anchor = TextAnchor.MiddleLeft;

				// Global animal settings
				foreach (var control in ControlsList)
					totalHeight += control.CreateSettingGlobal(totalHeight, viewWidth);

				// Apply global settings
				foreach (var animalSetting in Animals)
					animalSetting.ApplySettings();
			}

			// End
			Widgets.EndScrollView();

			// Remember settings view height for potential scrolling
			SettingsViewHeight = totalHeight - topRow;

			// End
			GUI.EndGroup();

			// Reset text settings
			Text.Font = OriTextFont;
			Text.Anchor = OriTextAnchor;
		}

		public void CreateSubMenuSelector(Rect rect, string label, SettingsSubMenuEnum value)
		{
			// Colorize if selected
			if (SelectedSettingsSubMenu == value)
				GUI.color = SelectionColor;
			// Draw button
			if (Widgets.ButtonText(rect, label))
				SelectedSettingsSubMenu = value;
			// Reset color
			GUI.color = Color.white;
		}

		public void CreateSubMenuGeneral(ref float totalHeight, float viewWidth)
		{
			// Separator
			Widgets.ListSeparator(ref totalHeight, viewWidth - 16, "SY_CA.SeparatorAnimalSettings".Translate());
			totalHeight += 2;
			Text.Anchor = TextAnchor.MiddleLeft;

			// Draw animal settings - general
			foreach (var control in ControlsList)
				totalHeight += control.CreateSetting(totalHeight, viewWidth, SelectedAnimalSettings);
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
