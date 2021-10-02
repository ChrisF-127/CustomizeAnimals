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
		Reproduction,
		Productivity,
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
		private List<BaseControl> GeneralControlsList { get; } = new List<BaseControl>
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

			new ControlAttackModifier()
		};
		private List<BaseControl> ReproductionControlsList { get; } = new List<BaseControl>
		{
			new ControlMateMtbHours(),
			new ControlGestationPeriodDays(),
			new ControlLitterSizeCurve(),
			new SpecialControlEggLayer(),
			new SpecialControlLifeStageAges(),
		};
		private List<BaseControl> ProductivityControlsList { get; } = new List<BaseControl>
		{
			new SpecialControlMilkable(),
			//new SpecialControlShearable(),
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
		public const float SettingsThinRowHeight = 26;
		public const float SettingsDoubleRowHeight = 58;
		public const float SettingsTripleRowHeight = 84;
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

			Animals.Insert(0, new AnimalSettings(ThingDefOf.Human));

			Settings = GetSettings<CustomizeAnimals_ModSettings>();
		}
		
		public void ResetControls(AnimalSettings animal)
		{
			foreach (var control in GeneralControlsList)
				control.Reset();
			foreach (var control in ReproductionControlsList)
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
				var listOffsetY = offsetY + _listRowHeight * index;
				var rect = new Rect(0, listOffsetY, viewWidth, _listRowHeight);
				if (Widgets.ButtonInvisible(rect, true))
					SelectedAnimalSettings = animalSettings;
				Widgets.DrawOptionBackground(rect, SelectedAnimalSettings == animalSettings);

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
			try
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

				try
				{
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
						title = animal.label?.CapitalizeFirst();
						subtitle = $"({animal.modContentPack?.Name ?? "[null]"})";

						// Reset button
						var resetAllRect = new Rect(width - 78, 0, 76, _listRowHeight);
						if (Widgets.ButtonText(resetAllRect, "SY_CA.Reset".Translate()))
							Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation(
								"SY_CA.DialogSettingsReset".Translate(animal.label?.CapitalizeFirst()) + "\n\n" + "SY_CA.DialogConfirm".Translate(),
								() => Reset(SelectedAnimalSettings)));
						BaseControl.DrawTooltip(resetAllRect, "SY_CA.TooltipSettingsReset".Translate(animal.label?.CapitalizeFirst()));


						// Sub menu buttons
						float buttonWidth = viewWidth / 3f;
						float buttonHeight = SettingsRowHeight / 4f * 3f;
						float subMenButtonOffsetX = 0;
						// General
						CreateSubMenuSelector(
							new Rect(subMenButtonOffsetX + 2, topRow, buttonWidth - 4, buttonHeight), 
							"SY_CA.SubMenuGeneral".Translate(), 
							SettingsSubMenuEnum.General,
							SettingsListIsModified(SelectedAnimalSettings.GeneralSettings.Values));
						subMenButtonOffsetX += buttonWidth;
						// Reproduction
						CreateSubMenuSelector(
							new Rect(subMenButtonOffsetX + 2, topRow, buttonWidth - 4, buttonHeight), 
							"SY_CA.SubMenuReproduction".Translate(), 
							SettingsSubMenuEnum.Reproduction,
							SettingsListIsModified(SelectedAnimalSettings.ReproductionSettings.Values));
						subMenButtonOffsetX += buttonWidth;
						// Productivity
						CreateSubMenuSelector(
							new Rect(subMenButtonOffsetX + 2, topRow, buttonWidth - 4, buttonHeight), 
							"SY_CA.SubMenuProductivity".Translate(), 
							SettingsSubMenuEnum.Productivity,
							SettingsListIsModified(SelectedAnimalSettings.ProductivitySettings.Values));
						subMenButtonOffsetX += buttonWidth;

						topRow += buttonHeight + 4;
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

					try
					{
						// Begin
						Widgets.BeginScrollView(
							new Rect(0, topRow, width, height - topRow),
							ref _settingsScrollPosition,
							new Rect(0, topRow, viewWidth, SettingsViewHeight));

						// Animal settings
						float totalHeight = topRow;
						if (!isGlobal)
						{
							switch (SelectedSettingsSubMenu)
							{
								case SettingsSubMenuEnum.General:
									CreateSubMenuGeneral(ref totalHeight, viewWidth);
									break;
								case SettingsSubMenuEnum.Reproduction:
									CreateSubMenuReproduction(ref totalHeight, viewWidth);
									break;
								case SettingsSubMenuEnum.Productivity:
									CreateSubMenuProductivity(ref totalHeight, viewWidth);
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
							totalHeight += GeneralSettings.CreateTrainabilityLimitsControls(totalHeight, viewWidth);
							totalHeight += GeneralSettings.CreateEggSettings(totalHeight, viewWidth);


							// Global animal settings separator
							Widgets.ListSeparator(ref totalHeight, width, "SY_CA.SeparatorAnimalSettings".Translate());
							totalHeight += 2;
							Text.Anchor = TextAnchor.MiddleLeft;

							// Global animal settings
							foreach (var control in GeneralControlsList)
								totalHeight += control.CreateSettingGlobal(totalHeight, viewWidth);

							// Apply global settings
							foreach (var animalSetting in Animals)
								animalSetting.ApplySettings();
						}

						// Remember settings view height for potential scrolling
						SettingsViewHeight = totalHeight - topRow;
					}
					finally
					{
						// End
						Widgets.EndScrollView();
					}
				}
				finally
				{
					// End
					GUI.EndGroup();
				}
			}
			catch (Exception exc)
			{
				// Show error
				Log.Error(exc.ToString());
				Log.TryOpenLogWindow();

				// Unselect the animal which caused the exception
				SelectedAnimalSettings = null;
			}
			finally
			{
				// Reset text settings
				Text.Font = OriTextFont;
				Text.Anchor = OriTextAnchor;
			}
		}

		public void CreateSubMenuSelector(Rect rect, string label, SettingsSubMenuEnum value, bool isModified)
		{
			// Colorize if selected
			if (SelectedSettingsSubMenu == value)
				GUI.color = SelectionColor;
			// Colorize if modified
			else if (isModified)
				GUI.color = ModifiedColor;
			// Draw button
			if (Widgets.ButtonText(rect, label))
				SelectedSettingsSubMenu = value;
			// Reset color
			GUI.color = Color.white;
		}

		public bool SettingsListIsModified(IEnumerable<ISetting> list)
		{
			foreach (var item in list)
				if (item.IsModified())
					return true;
			return false;
		}
		
		public void CreateSubMenuGeneral(ref float totalHeight, float viewWidth)
		{
			// Separator
			Widgets.ListSeparator(ref totalHeight, viewWidth - 16, "SY_CA.SeparatorGeneralSettings".Translate());
			totalHeight += 2;
			Text.Anchor = TextAnchor.MiddleLeft;

			// Draw controls
			foreach (var control in GeneralControlsList)
				totalHeight += control.CreateSetting(totalHeight, viewWidth, SelectedAnimalSettings);
		}
		public void CreateSubMenuReproduction(ref float totalHeight, float viewWidth)
		{
			// Separator
			Widgets.ListSeparator(ref totalHeight, viewWidth - 16, "SY_CA.SeparatorReproduction".Translate());
			totalHeight += 2;
			Text.Anchor = TextAnchor.MiddleLeft;

			// Draw controls
			foreach (var control in ReproductionControlsList)
				totalHeight += control.CreateSetting(totalHeight, viewWidth, SelectedAnimalSettings);
		}
		public void CreateSubMenuProductivity(ref float totalHeight, float viewWidth)
		{
			// Separator
			Widgets.ListSeparator(ref totalHeight, viewWidth - 16, "SY_CA.SeparatorProductivity".Translate());
			totalHeight += 2;
			Text.Anchor = TextAnchor.MiddleLeft;

			// Settings are only applicable for non-human animals
			if (SelectedAnimalSettings?.IsHuman == true)
			{
				Text.Anchor = TextAnchor.MiddleCenter;
				Widgets.Label(new Rect(2, totalHeight + 2, viewWidth - 4, SettingsRowHeight - 4), $"({"SY_CA.NotApplicable".Translate()})");
				Text.Anchor = TextAnchor.MiddleLeft;
				totalHeight += SettingsRowHeight;
				return;
			}

			// Draw controls
			foreach (var control in ProductivityControlsList)
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
