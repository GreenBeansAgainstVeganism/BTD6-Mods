using BTD_Mod_Helper;
using MelonLoader;
using BTD_Mod_Helper.Api.Towers;
using BTD_Mod_Helper.Api.Display;
using Assets.Scripts.Models.Towers;
using BTD_Mod_Helper.Extensions;
using Assets.Scripts.Unity;
using Assets.Scripts.Models.Towers.Behaviors.Attack;
using Assets.Scripts.Models.Towers.Projectiles.Behaviors;
using Assets.Scripts.Unity.Display;
using Assets.Scripts.Models.GenericBehaviors;
using Assets.Scripts.Models.Towers.Behaviors.Emissions;
using System.Collections.Generic;
using Assets.Scripts.Models.Towers.Behaviors.Abilities;
using Assets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors;
using Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors;
using Assets.Scripts.Models.Powers;
using Assets.Scripts.Models.Towers.Projectiles;
using Assets.Scripts.Models.Towers.Weapons.Behaviors;
using Assets.Scripts.Models.Towers.Filters;
using Assets.Scripts.Models.Towers.Behaviors;
using Assets.Scripts.Simulation.Towers.Behaviors.Attack;
using Assets.Scripts.Simulation.Objects;
using Assets.Scripts.Models;
using System;
using Assets.Scripts.Simulation.Towers.Projectiles;
using Assets.Scripts.Simulation.Towers.Projectiles.Behaviors;
using Assets.Scripts.Models.Powers.Effects;
using Assets.Scripts.Models.Towers.Weapons;
using Assets.Scripts.Unity.UI_New.InGame;
using Assets.Scripts.Simulation.Towers;
using Assets.Scripts.Simulation;
using UnityEngine;
using HarmonyLib;
using Assets.Scripts.Models.Bloons;

[assembly: MelonInfo(typeof(PowerMonkey.Main), "Power Monkey", "1.0.0", "AttackDuck")]
[assembly: MelonGame("Ninja Kiwi", "BloonsTD6")]
namespace PowerMonkey
{
  public class Main : BloonsTD6Mod
  {
    // Github API URL used to check if this mod is up to date. For example:
    // public override string GithubReleaseURL => "https://api.github.com/repos/gurrenm3/BTD-Mod-Helper/releases";

    // As an alternative to a GithubReleaseURL, a direct link to a web-hosted version of the .cs file
    // that has the "MelonInfo" attribute with the version of your mod
    //public override string MelonInfoCsURL => "https://raw.githubusercontent.com/doombubbles/BTD6-Mods/main/MegaKnowledge/Main.cs";

    // The link to your normal GitHub Releases page if you're using those, or a direct link to your dll file
    // public override string LatestURL => "https://github.com/gurrenm3/BTD-Mod-Helper/releases/latest ";

    public static Il2CppSystem.Collections.Generic.List<TowerModel> InstaList = new Il2CppSystem.Collections.Generic.List<TowerModel>();
    public static bool InstasInitialized = false;
    public static System.Random InstaPicker = new System.Random();

    public override void OnMainMenu()
    {
      base.OnMainMenu();

    }

    public override void OnProjectileCreated(Projectile projectile, Entity entity, Model modelToUse)
    {
      base.OnProjectileCreated(projectile, entity, modelToUse);
      //MelonLogger.Msg(projectile.projectileModel.id);

      if(projectile.projectileModel.id == "InstaMonkeySentry")
      {
        projectile.GetProjectileBehavior<CreateTower>().createTowerModel.tower = InstaList[InstaPicker.Next(InstaList.Count)];
      }
    }

 /*   public override void OnCashAdded(double amount, Simulation.CashType from, int cashIndex, Simulation.CashSource source, Tower tower)
    {
      MelonLogger.Msg("Cashbefore: " + amount);
      double thrive = 1;
      MelonLogger.Msg("cash added");
      foreach(var t in InGame.instance.GetTowers())
      {
        MelonLogger.Msg("Checking tower: " + t.Id + " | " + t.towerModel.baseId + " | " + t.towerModel.tiers[2]);
        if (t.towerModel.baseId == "PowerMonkey-PowerMonkey" && t.towerModel.tiers[2] >= 5)
        {
          thrive = 2d;
          break;
        }
        if (t.towerModel.baseId == "PowerMonkey-PowerMonkey" && t.towerModel.tiers[2] >= 3)
        {
          thrive = 1.25d;
          MelonLogger.Msg("thrive detected");
        }
      }
      if (source != Simulation.CashSource.BankDeposit)
      {
        amount *= thrive;
        MelonLogger.Msg("Thrive applied");
        MelonLogger.Msg("Cashafter: " + amount);

      }
      base.OnCashAdded(amount, from, cashIndex, source, tower);
    }*/

    //Code to make thrive and double cash work
    [HarmonyPatch(typeof(Simulation), "AddCash")]
    public class PowerMonkeyThrive
    {
      [HarmonyPrefix]
      public static bool Prefix(ref double c, ref Simulation.CashSource source)
      {
        if (source != Simulation.CashSource.CoopTransferedCash && source != Simulation.CashSource.TowerSold && source != Simulation.CashSource.BankDeposit)
        {
          double thrive = 1;
          //MelonLogger.Msg("cash added");
          foreach (var t in InGame.instance.GetTowers())
          {
            //MelonLogger.Msg("Checking tower: " + t.Id + " | " + t.towerModel.baseId + " | " + t.towerModel.tiers[2]);
            if (t.towerModel.baseId == "PowerMonkey-PowerMonkey" && t.towerModel.tiers[2] >= 5)
            {
              thrive = 2d;
              break;
            }
            if (t.towerModel.baseId == "PowerMonkey-PowerMonkey" && t.towerModel.tiers[2] >= 3)
            {
              thrive = 1.25d;
              //MelonLogger.Msg("thrive detected");
            }
          }
          c *= thrive;
          //MelonLogger.Msg("Thrive applied");
          //MelonLogger.Msg("Cashafter: " + c);
        }
        return true;

      }
    }

    public override void OnApplicationStart()
    {
      MelonLogger.Msg("Power Monkey Loaded!");

    }


    public class PowerMonkey : ModTower
    {
      public override string TowerSet => MAGIC;
      public override string BaseTower => TowerType.SuperMonkey;
      public override int Cost => 3600;

      public override int TopPathUpgrades => 5;
      public override int MiddlePathUpgrades => 2;
      public override int BottomPathUpgrades => 5;
      public override string Description => "Super Monkey's delinquent cousin who throws pineapples and commits bank fraud";
      public override void ModifyBaseTowerModel(TowerModel towerModel)
      {
        var bomb = Game.instance.model.GetTowerWithName("BombShooter").GetWeapon().projectile.Duplicate();
        var t = Game.instance.model.GetTowerFromId("MonkeyAce-010").GetAttackModel(1).weapons[0].projectile.display;

        bomb.display = t;
        bomb.AddBehavior(new DamageModel("damage", 1f, 1f, true, false, true, BloonProperties.None));
        towerModel.GetAttackModel().weapons[0].projectile = bomb;
      }
    }

    //Path 1 Upgrades
    public class CamoTrapper : ModUpgrade<PowerMonkey>
    {
      public override string Name => "CamoTrapper";
      public override string DisplayName => "Camo Trapper";
      public override string Description => "Occasionally places camo traps around. (These ones can decamo DDTs, like they always should have)";
      public override int Cost => 1200;
      public override int Path => TOP;
      public override int Tier => 1;
      public override void ApplyUpgrade(TowerModel towerModel)
      {
        var p = Game.instance.model.GetPowerWithName("CamoTrap").GetBehavior<CamoTrapModel>().projectileModel.Duplicate();
        var a = Game.instance.model.GetTowerFromId("NinjaMonkey-002").GetAttackModel(1).Duplicate();

        p.AddBehavior(a.weapons[0].projectile.GetBehavior<ArriveAtTargetModel>().Duplicate());
        p.AddBehavior(new IgnoreInsufficientPierceModel("ignoreinsufficientpierce"));
        p.pierce = 80;
        /*foreach(var item in p.GetBehavior<ProjectileFilterModel>().filters)
        {
          MelonLogger.Msg("b: " + item.name);
        }*/
        p.GetBehavior<ProjectileFilterModel>().filters = p.GetBehavior<ProjectileFilterModel>().filters.RemoveItemOfType<FilterModel, FilterWithTagModel>();
        a.weapons[0].projectile = p;
        a.weapons[0].AddBehavior(new LimitProjectileModel("limitcamotrap", p.id, 4, 15, false, false));
        a.range = towerModel.range;


        towerModel.AddBehavior(a);
      }
    }
    public class InstaMonkeys : ModUpgrade<PowerMonkey>
    {
      public override string Name => "InstaMonkeys";
      public override string DisplayName => "InstaMonkeys";
      public override string Description => "Employs highly calculated little sibling strats by occasionally placing random, temporary tier 0-2 towers in random locations.";
      public override int Cost => 2600;
      public override int Path => TOP;
      public override int Tier => 2;
      public override void ApplyUpgrade(TowerModel towerModel)
      {
        if (!InstasInitialized)
        {
          foreach (string t in new string[] {"Alchemist", "BananaFarm", "BombShooter", "BoomerangMonkey", "DartlingGunner", "DartMonkey",
            "Druid", "EngineerMonkey", "GlueGunner", "HeliPilot", "IceMonkey", "MonkeyAce", "MonkeyBuccaneer", "MonkeySub", "MonkeyVillage",
            "MortarMonkey", "NinjaMonkey", "SniperMonkey", "SpikeFactory", "SuperMonkey", "TackShooter"})
          {
            AddInsta(Game.instance.model.GetTowerWithName(t));
            foreach (string u in new string[] { "100", "200", "010", "020", "001", "002", "110", "120", "210", "220", "101", "102", "201", "202", "011", "012", "021", "022" })
            {
              AddInsta(Game.instance.model.GetTowerFromId(t + "-" + u));
            }
          }
          InstasInitialized = true;
        }
        var a = Game.instance.model.GetTowerFromId("EngineerMonkey-100").GetAttackModel().Duplicate();
        a.name = "InstaMonkeyEngineer";
        a.weapons[0].projectile.name = "InstaMonkeySentry";
        a.weapons[0].projectile.id = "InstaMonkeySentry";
        a.weapons[0].rate = 5.5f;
        towerModel.AddBehavior(a);
        /*foreach(var item in InstaList)
        {
          MelonLogger.Msg("list: " + item.name);
        }
        MelonLogger.Msg("InstaPicker: " + InstaPicker.Next(InstaList.Count) + "\tCount: " + InstaList.Count);*/

        var spawn = a.weapons[0].projectile.GetBehavior<CreateTowerModel>();
        spawn.tower = InstaList[InstaPicker.Next(InstaList.Count)];
        spawn.useParentTargetPriority = false;
        spawn.useProjectileRotation = false;
        //a.weapons[0].projectile.GetBehavior<CreateTowerModel>().tower = InstaList.ToArray()[InstaPicker.Next(InstaList.ToArray().Length)];

      }
    }

    public static TowerModel AddInsta(TowerModel Base)
    {
      var m = Base.Duplicate();
      var sentry = Game.instance.model.GetTowerWithName("Sentry");
      m.isSubTower = true;
      m.dontDisplayUpgrades = true;
      m.name = "insta_" + m.name;
      m.cost = 0;
      m.footprint = sentry.footprint.Duplicate();
      m.AddBehavior(sentry.GetBehavior<TowerExpireModel>());
      m.GetBehavior<TowerExpireModel>().lifespan = 30f;
      m.AddBehavior(sentry.GetBehavior<Assets.Scripts.Models.Towers.Behaviors.CreateEffectOnExpireModel>());
      m.AddBehavior(new CreditPopsToParentTowerModel("creditpopstoparenttowermodel"));
      //MelonLogger.Msg(m.name);
      InstaList.Add(m);
      return m;
    }
    public class GlueTrapper : ModUpgrade<PowerMonkey>
    {
      public override string Name => "GlueTrapper";
      public override string DisplayName => "Glue Trapper";
      public override string Description => "Occasionally places glue traps around. (The good kind, which can handle Moabs, of course)";
      public override int Cost => 2800;
      public override int Path => TOP;
      public override int Tier => 3;
      public override void ApplyUpgrade(TowerModel towerModel)
      {
        var p = Game.instance.model.GetPowerWithName("GlueTrap").GetBehavior<GlueTrapModel>().projectileModel.Duplicate();
        var a = Game.instance.model.GetTowerFromId("NinjaMonkey-002").GetAttackModel(1).Duplicate();

        /*foreach (var item in a.weapons[0].projectile.behaviors)
        {
          MelonLogger.Msg("a: " + item.name);
        }
        foreach (var item in p.behaviors)
        {
          MelonLogger.Msg("p: " + item.name);
        }
        foreach (var item in p.GetBehavior<ProjectileFilterModel>().filters)
        {
          MelonLogger.Msg("p: " + item.name);
        }
        foreach (var item in p.GetBehaviors<CollideExtraPierceReductionModel>())
        {
          MelonLogger.Msg("p: " + item.bloonTag +": " + item.extraAmount);
        }
        MelonLogger.Msg("lifespan: " + p.GetBehavior<AgeModel>().lifespan);
        MelonLogger.Msg("rounds: " + p.GetBehavior<AgeModel>().rounds);
        MelonLogger.Msg("pierce: " + p.pierce);
        MelonLogger.Msg("CappedPierce: " + p.CappedPierce);
        MelonLogger.Msg("maxPierce: " + p.maxPierce);*/

        p.AddBehavior(a.weapons[0].projectile.GetBehavior<ArriveAtTargetModel>().Duplicate());
        p.AddBehavior(new IgnoreInsufficientPierceModel("ignoreinsufficientpierce"));
        p.pierce = 60;
        p.GetBehavior<ProjectileFilterModel>().filters = p.GetBehavior<ProjectileFilterModel>().filters.RemoveItemOfType<FilterModel, FilterOutTagModel>();
        a.weapons[0].projectile = p;
        a.weapons[0].AddBehavior(new LimitProjectileModel("limitgluetrap", p.id, 4, 15, false, false));
        a.range = towerModel.range;

        towerModel.AddBehavior(a);
      }
    }
    
    public class Minecrafter : ModUpgrade<PowerMonkey>
    {
      public override string Name => "Minecrafter";
      public override string DisplayName => "Minecrafter";
      public override string Description => "Occasionally places Moab mines on the track.";
      public override int Cost => 7600;
      public override int Path => TOP;
      public override int Tier => 4;
      public override void ApplyUpgrade(TowerModel towerModel)
      {
        var p = Game.instance.model.GetPowerWithName("MoabMine").GetBehavior<MoabMineModel>().projectileModel.Duplicate();
        var a = Game.instance.model.GetTowerFromId("NinjaMonkey-002").GetAttackModel(1).Duplicate();

        p.AddBehavior(a.weapons[0].projectile.GetBehavior<ArriveAtTargetModel>().Duplicate());
        p.AddBehavior(new IgnoreInsufficientPierceModel("ignoreinsufficientpierce"));
        //p.pierce = 60;
        //p.GetBehavior<ProjectileFilterModel>().filters = p.GetBehavior<ProjectileFilterModel>().filters.RemoveItemOfType<FilterModel, FilterOutTagModel>();
        a.weapons[0].projectile = p;
        a.weapons[0].AddBehavior(new LimitProjectileModel("limitmoabmine", p.id, 4, 15, false, false));
        a.range = towerModel.range;

        towerModel.AddBehavior(a);
      }
    }

    public class SuperStorm : ModUpgrade<PowerMonkey>
    {
      public override string Name => "SuperStorm";
      public override string DisplayName => "Super Storm";
      public override string Description => "Periodically summons Super Monkey's other relatives to deal heavy flat damage to all bloons on screen. (Not to be confused with 'Superstorm,' the overpriced druid upgrade.)";
      public override int Cost => 41000;
      public override int Path => TOP;
      public override int Tier => 5;
      public override void ApplyUpgrade(TowerModel towerModel)
      {
        var a = Game.instance.model.GetTowerFromId("BombShooter-005").GetAbility(0).GetBehavior<ActivateAttackModel>().attacks[0].Duplicate();
        
        var w = a.weapons[0];
        a.weapons = new UnhollowerBaseLib.Il2CppReferenceArray<WeaponModel>(1);
        a.weapons[0] = w;
        w.projectile.GetDamageModel().damage = 2000f;
        w.projectile.RemoveBehavior<ProjectileFilterModel>();
        w.projectile.RemoveBehavior<DistributeToChildrenSetModel>();
        /*foreach (var item in w.projectile.behaviors)
        {
          MelonLogger.Msg("gz: " + item.name);
        }*/

        w.rate = 12f;
        var e = Game.instance.model.GetPowerWithName("SuperMonkeyStorm").GetBehavior<CreateEffectOnPowerModel>().effectModel;
        w.AddBehavior(new EjectEffectModel("ejecteffectmodel", e.assetId, e, e.lifespan, e.fullscreen, false, true, false, false));
        var s = Game.instance.model.GetPowerWithName("SuperMonkeyStorm").GetBehavior<CreateSoundOnPowerModel>().sound;
        w.AddBehavior(new CreateSoundOnProjectileCreatedModel("createsoundonprojectilecreatedmodel", s,s,s,s,s,"yes"));

        towerModel.AddBehavior(a);
      }
    }

    //Path 2 Upgrades
    public class GrilledPineapples : ModUpgrade<PowerMonkey>
    {
      public override string Name => "GrilledPineapples";
      public override string DisplayName => "Grilled Pineapples";
      public override string Description => "Pineapples can pop black bloons and also set fire to bloons.";
      public override int Cost => 1500;
      public override int Path => MIDDLE;
      public override int Tier => 1;
      public override void ApplyUpgrade(TowerModel towerModel)
      {
        var p = towerModel.GetAttackModel().weapons[0].projectile.GetBehavior<CreateProjectileOnContactModel>().projectile;
        p.GetDamageModel().immuneBloonProperties = BloonProperties.None;

        var fire = Game.instance.model.GetTowerFromId("MortarMonkey-002").GetAttackModel().weapons[0].projectile.
          GetBehavior<CreateProjectileOnExhaustFractionModel>().projectile.GetBehavior<AddBehaviorToBloonModel>().Duplicate();

        p.AddBehavior(fire);
        p.collisionPasses = new int[] { -1, 0 };
      }
    }
    public class EnergizingMonkey : ModUpgrade<PowerMonkey>
    {
      public override string Name => "EnergizingMonkey";
      public override string DisplayName => "Energizing Monkey";
      public override string Description => "Acts as an infinite energizing totem to monkeys in range, boosting their attack speed by 25%.";
      public override int Cost => 2700;
      public override int Path => MIDDLE;
      public override int Tier => 2;
      public override void ApplyUpgrade(TowerModel towerModel)
      {
        var t = Game.instance.model.GetTowerFromId("EnergisingTotem").GetBehavior<RateSupportModel>().Duplicate();
        t.appliesToOwningTower = true;
        towerModel.AddBehavior(t);

      }
    }

    public class TimeStop : ModUpgrade<PowerMonkey>
    {
      public override string Name => "TimeStop";
      public override string DisplayName => "Time Stop";
      public override string Description => "Ability: Slow down all monkeys and bloons on screen for 10 seconds, slowing smaller bloons slightly more.";
      public override int Cost => 2850;
      public override int Path => MIDDLE;
      public override int Tier => 3;
      public override void ApplyUpgrade(TowerModel towerModel)
      {
        var a = Game.instance.model.GetTowerFromId("BombShooter-005").GetAbility().GetBehavior<ActivateAttackModel>().attacks[0].Duplicate();
        var ability = Game.instance.model.GetTowerFromId("IceMonkey-040").GetAbility().Duplicate();
        var dt = Game.instance.model.GetPowerWithName("DartTime").GetBehavior<DartTimeModel>();
        /*foreach(var item in Game.instance.model.powers)
        {
          MelonLogger.Msg("powers: " + item.name);
        }*/

        var w = a.weapons[0];
        a.weapons = new UnhollowerBaseLib.Il2CppReferenceArray<WeaponModel>(1);
        a.weapons[0] = w;
        w.projectile.RemoveBehavior<DamageModel>();
        w.projectile.RemoveBehavior<ProjectileFilterModel>();
        w.projectile.RemoveBehavior<DistributeToChildrenSetModel>();
        var bm = new UnhollowerBaseLib.Il2CppReferenceArray<BloonBehaviorModel>(new BloonBehaviorModel[] { dt.dartTimeBloonBehaviorModel });
        /*foreach(var item in Game.instance.model.GetTowerWithName("Alchemist").GetAttackModel().weapons[0].projectile.behaviors)
        {
          MelonLogger.Msg("alch: " + item.name);
        }*/
        var b = Game.instance.model.GetTowerWithName("Alchemist").GetAttackModel().weapons[0].projectile.GetBehavior<CreateProjectileOnExhaustFractionModel>().projectile.GetBehavior<AddBehaviorToBloonModel>().Duplicate();
        b.behaviors = bm;
        b.lifespan = 1f;
        MelonLogger.Msg(b.mutationId);
        MelonLogger.Msg(dt.BloonTimeSlowMutator.id);
        //b.mutationId = dt.bloonTimeSlowMutator.id;
        //new AddBehaviorToBloonModel("DartTime", dt.bloonTimeSlowMutator.id, 1f, 99, new FilterModel("emptyfilter"), new UnhollowerBaseLib.Il2CppReferenceArray<FilterModel>(0), bm, new Il2CppSystem.Collections.Generic.Dictionary<string, Assets.Scripts.Models.Effects.AssetPathModel>(), 0, true, false, true, false, -1, false, 1);
        w.projectile.AddBehavior(b);
        foreach (var item in w.projectile.behaviors)
        {
          MelonLogger.Msg("darttimeweapon: " + item.name);
        }

        w.rate = 0.5f;

        ability.GetBehavior<ActivateAttackModel>().attacks[0] = a;
        ability.GetBehavior<ActivateAttackModel>().lifespan = 10f;
        ability.cooldown = 30f;
        ability.name = "TimeStopPowerMonkey";

        towerModel.AddBehavior(ability);
      }
    }

    //Path 3 Upgrades

    public class RoadSpikes : ModUpgrade<PowerMonkey>
    {
      public override string Name => "RoadSpikes";
      public override string DisplayName => "Road Spikes";
      public override string Description => "Adds a completely original attack that no other tower in the game has already.";
      public override int Cost => 1350;
      public override int Path => BOTTOM;
      public override int Tier => 1;
      public override void ApplyUpgrade(TowerModel towerModel)
      {
        var p = Game.instance.model.GetPowerWithName("RoadSpikes").GetBehavior<RoadSpikesModel>().projectileModel.Duplicate();
        var a = Game.instance.model.GetTowerFromId("NinjaMonkey-002").GetAttackModel(1).Duplicate();

        p.AddBehavior(a.weapons[0].projectile.GetBehavior<ArriveAtTargetModel>().Duplicate());
        //p.GetBehavior<ProjectileFilterModel>().filters = p.GetBehavior<ProjectileFilterModel>().filters.RemoveItemOfType<FilterModel, FilterOutTagModel>();
        a.weapons[0].projectile = p;
        a.weapons[0].rate = 1.2f;
        a.weapons[0].AddBehavior(new LimitProjectileModel("limitroadspikes", p.id, 36, 15, true, false));
        a.range = towerModel.range;


        towerModel.AddBehavior(a);
      }
    }

    public class TaskMaster : ModUpgrade<PowerMonkey>
    {
      public override string Name => "TaskMaster";
      public override string DisplayName => "Task Master";
      public override string Description => "Assumes the abilities of both a banana farmer and a tech bot, while also increasing the rate of all of Power Monkey's other actions by 20%.";
      public override int Cost => 1800;
      public override int Path => BOTTOM;
      public override int Tier => 2;
      public override int Priority => base.Priority-10;
      public override void ApplyUpgrade(TowerModel towerModel)
      {
        /*foreach(var item in Game.instance.model.GetTowerFromId("BananaFarmer").behaviors)
        {
          MelonLogger.Msg("farmer: " + item.name);
        }*/
        var f = Game.instance.model.GetTowerFromId("BananaFarmer").GetBehavior<CollectCashZoneModel>().Duplicate();

        towerModel.AddBehavior(f);

        var t = Game.instance.model.GetTowerFromId("TechBot").GetAbilites().Duplicate();

        foreach(var item in t)
        {
          towerModel.AddBehavior(item);
        }

        foreach(var a in towerModel.GetAttackModels())
        {
          foreach(var w in a.weapons)
          {
            w.rate *= 0.8f;
          }
        }
      }
    }


    public class Thrive : ModUpgrade<PowerMonkey>
    {
      public override string Name => "Thrive";
      public override string DisplayName => "Thrive";
      public override string Description => "Uses advanced stock manipulation to increase all cash earned by 25%.";
      public override int Cost => 12500;
      public override int Path => BOTTOM;
      public override int Tier => 3;
      public override int Priority => base.Priority - 10;
      public override void ApplyUpgrade(TowerModel towerModel)
      {
        
        /*var a = Game.instance.model.GetTowerFromId("MonkeyVillage-003");
        
        var b = a.GetBehavior<AddBehaviorToTowerSupportModel>().Duplicate();
        b.isGlobal = true;
        b.name = "powermonkey_thrive";
        b.isCustomRadius = true;
        b.customRadius = 69420f;
        b.appliesToOwningTower = true;
        b.mutationId = "powermonkey_thrive";
        foreach (CashIncreaseModel item in b.behaviors.GetItemsOfType<TowerBehaviorModel, CashIncreaseModel>())
        {
          MelonLogger.Msg("town: " + item.name);
          item.multiplier = 1.25f;
          item.name = "powermonkey_thrive";

        }
        towerModel.AddBehavior(b);*/
      }
    }
    public class CashDrop : ModUpgrade<PowerMonkey>
    {
      public override string Name => "CashDrop";
      public override string DisplayName => "Cash Drop";
      public override string Description => "Occasionally makes it rain.";
      public override int Cost => 32000;
      public override int Path => BOTTOM;
      public override int Tier => 4;
      public override int Priority => base.Priority - 10;
      public override void ApplyUpgrade(TowerModel towerModel)
      {
        var a = Game.instance.model.GetTowerFromId("SniperMonkey-040").GetAbility().GetBehavior<ActivateAttackModel>().attacks[0].Duplicate();
        var p = Game.instance.model.GetPowerWithName("CashDrop").GetBehavior<CashDropModel>().projectileModel.Duplicate();
        var e = Game.instance.model.GetTowerWithName("BananaFarm").GetAttackModel().weapons[0].GetBehavior<EmissionsPerRoundFilterModel>().Duplicate();
        /*foreach (var item in a.weapons[0].projectile.behaviors)
        {
          MelonLogger.Msg("cashdrop: " + item.name);
        }*/
        a.weapons[0].projectile.GetBehavior<CreateProjectileOnExhaustFractionModel>().projectile = p;
        e.count = 3;
        a.weapons[0].AddBehavior(e);
        a.weapons[0].AddBehavior(Game.instance.model.GetTowerWithName("BananaFarm").GetAttackModel().weapons[0].GetBehavior<WeaponRateMinModel>().Duplicate());
        towerModel.AddBehavior(a);
      }
    }
    public class DoubleCash : ModUpgrade<PowerMonkey>
    {
      public override string Name => "DoubleCash";
      public override string DisplayName => "Double Cash";
      public override string Description => "Power Monkey's little brother was complaining that BTD6 is too pay-to-win so he backdoored his way into the game code to put an end to this disgusting corporate greed once and for all.";
      public override int Cost => 69420;
      public override int Path => BOTTOM;
      public override int Tier => 5;
      public override int Priority => base.Priority - 10;
      public override void ApplyUpgrade(TowerModel towerModel)
      {

      }
    }


  }  
}
