using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace CustomizeAnimals.Settings
{
	internal class SpecialSettingEggLayer : BaseSpecialSetting
	{
		#region PROPERTIES
		public CompProperties_EggLayer DefaultEggLayer { get; private set; }
		public CompProperties_EggLayer EggLayer { get; private set; }

		public ThingDef DefaultFertilizedDef { get; private set; }
		public ThingDef FertilizedDef { get; private set; }
		public ThingDef DefaultUnfertilizedDef { get; private set; }
		public ThingDef UnfertilizedDef { get; private set; }

		public bool DefaultIsEggLayer { get; private set; }
		public bool IsEggLayer { get; set; }

		public bool DefaultFemaleOnly { get; private set; }
		public bool FemaleOnly { get; set; }
		public float DefaultIntervalDays { get; private set; }
		public float IntervalDays { get; set; }
		public int DefaultCountRangeMin { get; private set; }
		public int CountRangeMin { get; set; }
		public int DefaultCountRangeMax { get; private set; }
		public int CountRangeMax { get; set; }
		public int DefaultFertilizationCountMax { get; private set; }
		public int FertilizationCountMax { get; set; }
		public float DefaultProgressUnfertilizedMax { get; private set; }
		public float ProgressUnfertilizedMax { get; set; }

		public CompProperties_Hatcher Hatcher { get; private set; }

		public float DefaultDaysToHatch { get; private set; }
		public float DaysToHatch { get; set; }
		#endregion

		#region CONSTRUCTORS
		public SpecialSettingEggLayer(ThingDef animal) : base(animal)
		{
			GetValue();

			DefaultIsEggLayer = IsEggLayer;

			DefaultFertilizedDef = FertilizedDef;
			DefaultUnfertilizedDef = UnfertilizedDef;

			DefaultFemaleOnly = FemaleOnly;
			DefaultIntervalDays = IntervalDays;
			DefaultCountRangeMin = CountRangeMin;
			DefaultCountRangeMax = CountRangeMax;
			DefaultFertilizationCountMax = FertilizationCountMax;
			DefaultProgressUnfertilizedMax = ProgressUnfertilizedMax;

			DefaultDaysToHatch = DaysToHatch;
		}
		#endregion

		#region OVERRIDES
		public override void GetValue()
		{
			EggLayer = Animal?.comps?.Find((comp) => comp is CompProperties_EggLayer) as CompProperties_EggLayer;
			IsEggLayer = EggLayer != null;

			if (IsEggLayer)
			{
				FemaleOnly = EggLayer.eggLayFemaleOnly;
				IntervalDays = EggLayer.eggLayIntervalDays;
				CountRangeMin = EggLayer.eggCountRange.min;
				CountRangeMax = EggLayer.eggCountRange.max;
				FertilizationCountMax = EggLayer.eggFertilizationCountMax;
				ProgressUnfertilizedMax = EggLayer.eggProgressUnfertilizedMax;

				FertilizedDef = EggLayer.eggFertilizedDef;
				UnfertilizedDef = EggLayer.eggUnfertilizedDef;

				Hatcher = FertilizedDef?.comps?.Find((comp) => comp is CompProperties_Hatcher) as CompProperties_Hatcher;
				if (Hatcher != null) // should never be null
					DaysToHatch = Hatcher.hatcherDaystoHatch;
			}
			else
			{
				FemaleOnly = true;
				IntervalDays = 6f;
				CountRangeMin = 1;
				CountRangeMax = 1;
				FertilizationCountMax = 1;
				ProgressUnfertilizedMax = 0.5f;

				FertilizedDef = null;
				UnfertilizedDef = null;

				Hatcher = null;
				DaysToHatch = 3.5f;
			}
		}
		public override void SetValue()
		{
			if (IsEggLayer)
			{
				if (EggLayer == null)
				{
					if (DefaultEggLayer != null)
						EggLayer = DefaultEggLayer;
					else
					{
						if (FertilizedDef == null)
							FertilizedDef = DefaultFertilizedDef ?? GenerateEggDef(true);

						EggLayer = new CompProperties_EggLayer
						{
							eggFertilizedDef = FertilizedDef,
							eggUnfertilizedDef = UnfertilizedDef,
							eggLayFemaleOnly = FemaleOnly,
							eggLayIntervalDays = IntervalDays,
							eggCountRange = new IntRange(CountRangeMin, CountRangeMax),
							eggFertilizationCountMax = FertilizationCountMax,
							eggProgressUnfertilizedMax = ProgressUnfertilizedMax,
						};
					}

					if (Animal.comps == null)
						Animal.comps = new List<CompProperties>();
					if (!Animal.comps.Contains(EggLayer))
						Animal.comps.Add(EggLayer);
				}
			}
			else
			{
				if (EggLayer != null)
				{
					Animal.comps?.Remove(EggLayer);
					EggLayer = null;
				}
				return;
			}

			EggLayer.eggLayFemaleOnly = FemaleOnly;
			EggLayer.eggLayIntervalDays = IntervalDays;
			EggLayer.eggCountRange.min = CountRangeMin;
			EggLayer.eggCountRange.max = CountRangeMax;
			EggLayer.eggFertilizationCountMax = FertilizationCountMax;
			EggLayer.eggProgressUnfertilizedMax = ProgressUnfertilizedMax;

			if (ProgressUnfertilizedMax >= 1f)
			{
				if (EggLayer.eggUnfertilizedDef == null)
					EggLayer.eggUnfertilizedDef = DefaultUnfertilizedDef ?? GenerateEggDef(false);
			}
			else if (ProgressUnfertilizedMax < 1f)
			{
				if (DefaultUnfertilizedDef == null && EggLayer.eggUnfertilizedDef != null)
					EggLayer.eggUnfertilizedDef = null;
			}

			if (Hatcher != null)
				Hatcher.hatcherDaystoHatch = DaysToHatch;
		}

		public override void Reset()
		{
			IsEggLayer = DefaultIsEggLayer;

			FemaleOnly = DefaultFemaleOnly;
			IntervalDays = DefaultIntervalDays;
			CountRangeMin = DefaultCountRangeMin;
			CountRangeMax = DefaultCountRangeMax;
			FertilizationCountMax = DefaultFertilizationCountMax;
			ProgressUnfertilizedMax = DefaultProgressUnfertilizedMax;

			DaysToHatch = DefaultDaysToHatch;
		}
		public override bool IsModified()
		{
			if (IsEggLayer != DefaultIsEggLayer
				|| FemaleOnly != DefaultFemaleOnly
				|| IntervalDays != DefaultIntervalDays
				|| CountRangeMin != DefaultCountRangeMin
				|| CountRangeMax != DefaultCountRangeMax
				|| FertilizationCountMax != DefaultFertilizationCountMax
				|| ProgressUnfertilizedMax != DefaultProgressUnfertilizedMax
				|| DaysToHatch != DefaultDaysToHatch)
				return true;
			return false;
		}

		public override void ExposeData()
		{
			bool boolValue = IsEggLayer;
			Scribe_Values.Look(ref boolValue, nameof(IsEggLayer), DefaultIsEggLayer);
			IsEggLayer = boolValue;

			boolValue = FemaleOnly;
			Scribe_Values.Look(ref boolValue, nameof(FemaleOnly), DefaultFemaleOnly);
			FemaleOnly = boolValue;

			float floatValue = IntervalDays;
			Scribe_Values.Look(ref floatValue, nameof(IntervalDays), DefaultIntervalDays);
			IntervalDays = floatValue;

			int intValue = CountRangeMin;
			Scribe_Values.Look(ref intValue, nameof(CountRangeMin), DefaultCountRangeMin);
			CountRangeMin = intValue;

			intValue = CountRangeMax;
			Scribe_Values.Look(ref intValue, nameof(CountRangeMax), DefaultCountRangeMax);
			CountRangeMax = intValue;

			intValue = FertilizationCountMax;
			Scribe_Values.Look(ref intValue, nameof(FertilizationCountMax), DefaultFertilizationCountMax);
			FertilizationCountMax = intValue;

			floatValue = ProgressUnfertilizedMax;
			Scribe_Values.Look(ref floatValue, nameof(ProgressUnfertilizedMax), DefaultProgressUnfertilizedMax);
			ProgressUnfertilizedMax = floatValue;

			floatValue = DaysToHatch;
			Scribe_Values.Look(ref floatValue, nameof(DaysToHatch), DefaultDaysToHatch);
			DaysToHatch = floatValue;
		}
		#endregion

		#region PRIVATE METHODS
		private ThingDef GenerateEggDef(bool fertilized)
		{
			var fert = fertilized ? "fert" : "unfert";
			var name = $"Egg{Animal.defName}{fert.CapitalizeFirst()}ilized";
			if (DefDatabase<ThingDef>.AllDefs.FirstOrDefault((def) => def.defName == name) is ThingDef thingDef)
			{
				Log.Message($"{nameof(GenerateEggDef)}: found: " + thingDef.defName);
				return thingDef;
			}

			var label = Animal.label;
			var eggDef = new ThingDef
			{
				// This
				modContentPack = CustomizeAnimals.Instance.Content,
				defName = name,
				label = $"{label} egg ({fert}.)",
				description = $"A {fert}ilized {label} egg.\n\n(This item was auto-generated via Customize Animals.)",
				comps = new List<CompProperties>(),
				thingCategories = new List<ThingCategoryDef>(),
				// ResourceBase
				thingClass = typeof(ThingWithComps),
				category = ThingCategory.Item,
				drawerType = DrawerType.MapMeshOnly,
				resourceReadoutPriority = ResourceCountPriority.Middle,
				useHitPoints = true,
				selectable = true,
				altitudeLayer = AltitudeLayer.Item,
				stackLimit = 75,
				alwaysHaulable = true,
				drawGUIOverlay = true,
				rotatable = false,
				pathCost = 14,
				allowedArchonexusCount = -1,
				// OrganicProductBase
				graphicData = new GraphicData
				{
					texPath = "Things/Item/Resource/AnimalProductRaw/EggBirdSmall",
					color = Color.white,
					graphicClass = typeof(Graphic_StackCount),
				},
				tickerType = TickerType.Rare,
				healthAffectsPrice = false,
				// EggBase
				socialPropernessMatters = true,
			};

			// ResourceBase
			eggDef.SetStatBaseValue(StatDefOf.Beauty, -4);
			eggDef.comps.Add(new CompProperties_Forbiddable());
			// OrganicProductBase
			// MaxHitPoints, Flammability, DeteriorationRate & Mass overwritten by EggBase
			// EggBase
			eggDef.ingestible = new IngestibleProperties
			{
				parent = eggDef,
				foodType = FoodTypeFlags.AnimalProduct,
				ingestEffect = EffecterDefOf.EatMeat,
				ingestSound = SoundDefOf.RawMeat_Eat,
				tasteThought = ThoughtDefOf.AteRawFood,
			};
			eggDef.SetStatBaseValue(StatDefOf.Mass, 0.15f);
			eggDef.SetStatBaseValue(StatDefOf.MaxHitPoints, 20);
			eggDef.SetStatBaseValue(StatDefOf.DeteriorationRate, 2);
			eggDef.SetStatBaseValue(StatDefOf.Flammability, 0.7f);
			eggDef.SetStatBaseValue(StatDefOf.Nutrition, 0.25f);
			eggDef.SetStatBaseValue(StatDefOf.FoodPoisonChance, 0.02f);
			eggDef.comps.Add(new CompProperties_Rottable
			{
				daysToRotStart = 15,
				rotDestroys = true,
				disableIfHatcher = true,
			});
			// EggFertBase
			if (fertilized)
			{
				eggDef.tickerType = TickerType.Normal;
				eggDef.ingestible.preferability = FoodPreferability.DesperateOnly;
				eggDef.thingCategories.Add(ThingCategoryDefOf.EggsFertilized);

				var pawnKindDefs = DefDatabase<PawnKindDef>.AllDefs.Where((def) => def.race == Animal);
				if (pawnKindDefs.Count() < 1)
					Log.Warning($"{nameof(CustomizeAnimals)}.{nameof(GenerateEggDef)}: found no {nameof(PawnKindDef)} for {Animal.label}");
				else
				{
					if (pawnKindDefs.Count() > 1)
						Log.Warning($"{nameof(CustomizeAnimals)}.{nameof(GenerateEggDef)}: found multiple {nameof(PawnKindDef)}s for {Animal.label} {pawnKindDefs.Count()}");

					eggDef.comps.Add(new CompProperties_Hatcher
					{
						hatcherDaystoHatch = 3.5f,
						hatcherPawn = pawnKindDefs.First(),
					});
				}
			}
			// EggUnfertBase
			else
			{
				eggDef.ingestible.preferability = FoodPreferability.RawBad;
				eggDef.thingCategories.Add(ThingCategoryDefOf.EggsUnfertilized);
				eggDef.comps.Add(new CompProperties_TemperatureRuinable
				{
					minSafeTemperature = 0,
					maxSafeTemperature = 50,
					progressPerDegreePerTick = 0.00003f,
				});
			}

			// Add def
			Helper.GiveShortHashDelegate(eggDef, eggDef.GetType());
			DefGenerator.AddImpliedDef(eggDef);
			//Log.Message("Egg generated: " + eggDef.defName);
			return eggDef;
		}
		#endregion
	}
}
