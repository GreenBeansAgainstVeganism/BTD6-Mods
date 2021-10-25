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

[assembly: MelonInfo(typeof(WallFactory.Main), "Wall Factory", "1.0.0", "AttackDuck")]
[assembly: MelonGame("Ninja Kiwi", "BloonsTD6")]
namespace WallFactory
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


		public override void OnMainMenu()
		{
			base.OnMainMenu();
		}

		public override void OnApplicationStart()
        {
            MelonLogger.Msg("Wall Factory Loaded!");
        }
    }
		
		public class WallFactory : ModTower
  {
    public override string TowerSet => SUPPORT;
    public override string BaseTower => TowerType.SpikeFactory;
    public override int Cost => 1200;

    public override int TopPathUpgrades => 5;
    public override int MiddlePathUpgrades => 5;
    public override int BottomPathUpgrades => 5;
    public override string Description => "Produces high quality barricades which damage bloons on contact and knock them back. Cannot hold back heavier bloons.";

    public override void ModifyBaseTowerModel(TowerModel towerModel)
    {
      var attackModel = Game.instance.model.GetTowerFromId("SpikeFactory-205").GetAttackModel().Duplicate();
      towerModel.RemoveBehavior<AttackModel>();
      towerModel.AddBehavior(attackModel);
      towerModel.range = 30f;
      attackModel.range = towerModel.range;

      var knockback = Game.instance.model.GetTowerFromId("SuperMonkey-001").GetBehavior<AttackModel>().weapons[0].projectile.GetBehavior<KnockbackModel>().Duplicate();
      knockback.CreateMutator();
      /*MelonLogger.Msg(knockback.lifespan);
      MelonLogger.Msg(knockback.lightMultiplier);
      MelonLogger.Msg(knockback.heavyMultiplier);
      MelonLogger.Msg(knockback.moabMultiplier);*/
      var projectile = attackModel.weapons[0].projectile;
      projectile.AddBehavior(knockback);
      projectile.GetBehavior<ClearHitBloonsModel>().interval = 0.35f;
      projectile.GetBehavior<AgeModel>().rounds = 0;
      projectile.GetBehavior<AgeModel>().lifespan = 30;
      projectile.pierce = 30;
      //projectile.RemoveBehavior<SetSpriteFromPierceModel>();
      projectile.RemoveBehavior<RotateModel>();
      projectile.GetDamageModel().damage = 1;
      //projectile.ApplyDisplay<WallDisplay1>();
      //projectile.display = Game.instance.model.GetTowerFromId("SpikeFactory-205").GetAttackModel().weapons[0].projectile.GetBehavior<DisplayModel>().display;
      projectile.collisionPasses = new int[] { -1, 0 };

      towerModel.targetTypes = Game.instance.model.GetTowerFromId("SpikeFactory-205").targetTypes;
    }
    public override string Icon => "WallFactoryIcon";
    public override string Portrait => "WallFactoryIcon";
  }

  /*public class WallDisplay1 : ModDisplay
  {
    public override string BaseDisplay => Generic2dDisplay;
    public override void ModifyDisplayNode(UnityDisplayNode node)
    {
      Set2DTexture(node, "WallDisplay1");
    }
  }*/

  //  Path 1 Upgrades
  public class SpikyWalls : ModUpgrade<WallFactory>
  {
    public override string Name => "Spiky Walls";
    public override string DisplayName => "Spiky Walls";
    public override string Description => "Walls deal +1 damage on contact.";
    public override int Cost => 800;
    public override int Path => TOP;
    public override int Tier => 1;
    public override void ApplyUpgrade(TowerModel towerModel)
    {
      var attackModel = towerModel.GetAttackModel();
      var projectile = attackModel.weapons[0].projectile;
      projectile.GetDamageModel().damage += 1;
    }
    public override string Icon => "WallFactory100Icon";
    public override string Portrait => "WallFactory100";
  }
  public class SelfDestruct : ModUpgrade<WallFactory>
  {
    public override string Name => "Self Destruct";
    public override string DisplayName => "Self Destruct";
    public override string Description => "Walls erupt violently upon destruction.";
    public override int Cost => 1400;
    public override int Path => TOP;
    public override int Tier => 2;
    public override void ApplyUpgrade(TowerModel towerModel)
    {
      var attackModel = towerModel.GetAttackModel();
      var projectile = attackModel.weapons[0].projectile;
      /*var mineProj = Game.instance.model.GetTowerFromId("SpikeFactory-400").GetAttackModel().weapons[0].projectile.GetBehavior<CreateProjectileOnExpireModel>().projectile.Duplicate();
      var mineEm = Game.instance.model.GetTowerFromId("SpikeFactory-400").GetAttackModel().weapons[0].projectile.GetBehavior<CreateProjectileOnExpireModel>().emission.Duplicate();
      projectile.AddBehavior(new CreateProjectileOnExpireModel("FailureContingency",mineProj,mineEm,false));*/

      var bomb = Game.instance.model.GetTowerFromId("BombShooter-300").GetWeapon().projectile.Duplicate();
      var pb = bomb.GetBehavior<CreateProjectileOnContactModel>();
      var sound = bomb.GetBehavior<CreateSoundOnProjectileCollisionModel>();
      var effect = bomb.GetBehavior<CreateEffectOnContactModel>();
      pb.projectile.GetDamageModel().immuneBloonProperties = BloonProperties.None;
      pb.projectile.SetHitCamo(true);
      var behavior = new CreateProjectileOnExhaustFractionModel(
          "Bomb",
          pb.projectile, pb.emission, 1f, 1f, true);
      projectile.AddBehavior(behavior);

      var soundBehavior = new CreateSoundOnProjectileExhaustModel(
          "BombSound",
          sound.sound1, sound.sound2, sound.sound3, sound.sound4, sound.sound5);
      projectile.AddBehavior(soundBehavior);

      var eB = new CreateEffectOnExhaustedModel("BombEffect", "", 0f, false,
          false, effect.effectModel);
      projectile.AddBehavior(eB);
    }
    public override string Icon => "WallFactory200Icon";
    public override string Portrait => "WallFactory200";
  }
  public class FlameWard : ModUpgrade<WallFactory>
  {
    public override string Name => "Flame Ward";
    public override string DisplayName => "Flame Ward";
    public override string Description => "The best defense is a good offense! Walls are now equipped with pressure-sensitive flame throwers.";
    public override int Cost => 3800;
    public override int Path => TOP;
    public override int Tier => 3;
    public override void ApplyUpgrade(TowerModel towerModel)
    {
      var attackModel = towerModel.GetAttackModel();
      var projectile = attackModel.weapons[0].projectile;
      /*var mineProj = Game.instance.model.GetTowerFromId("SpikeFactory-400").GetAttackModel().weapons[0].projectile.GetBehavior<CreateProjectileOnExpireModel>().projectile.Duplicate();
      var mineEm = Game.instance.model.GetTowerFromId("SpikeFactory-400").GetAttackModel().weapons[0].projectile.GetBehavior<CreateProjectileOnExpireModel>().emission.Duplicate();
      projectile.AddBehavior(new CreateProjectileOnExpireModel("FailureContingency",mineProj,mineEm,false));*/

      var flame = Game.instance.model.GetTowerFromId("Gwendolin").GetAttackModel().weapons[0].projectile.Duplicate();
      //var pb = flame.GetBehavior<CreateProjectileOnContactModel>();
      //var sound = flame.GetBehavior<CreateSoundOnProjectileCollisionModel>();
      //var effect = flame.GetBehavior<CreateEffectOnContactModel>();
      flame.GetDamageModel().immuneBloonProperties = BloonProperties.None;
      flame.GetBehavior<TravelStraitModel>().Lifespan = 0.25f;
      flame.SetHitCamo(true);
      var behavior = new CreateProjectileOnContactModel(
          "Flame",
          flame, new RandomArcEmissionModel("FlameEmission",3,0f,0f,360f,0f,new UnhollowerBaseLib.Il2CppReferenceArray<EmissionBehaviorModel>(new EmissionBehaviorModel[0])),false,true,false);
      projectile.AddBehavior(behavior);

      /*var soundBehavior = new CreateSoundOnProjectileExhaustModel(
          "BombSound",
          sound.sound1, sound.sound2, sound.sound3, sound.sound4, sound.sound5);
      projectile.AddBehavior(soundBehavior);*/

      /*var eB = new CreateEffectOnExhaustedModel("BombEffect", "", 0f, false,
          false, effect.effectModel);
      projectile.AddBehavior(eB);*/
    }
    public override string Icon => "WallFactory300Icon";
    public override string Portrait => "WallFactory300";
  }

  public class ElasticDeformation : ModUpgrade<WallFactory>
  {
    public override string Name => "Elastic Deformation";
    public override string DisplayName => "Elastic Deformation";
    public override string Description => "Advanced impact control systems cause all bloon fortifications to shatter instantly on contact with these walls, and also increase damage against ceramics, leads, and DDTs.";
    public override int Cost => 7200;
    public override int Path => TOP;
    public override int Tier => 4;
    public override void ApplyUpgrade(TowerModel towerModel)
    {
      var attackModel = towerModel.GetAttackModel();
      var projectile = attackModel.weapons[0].projectile;
      projectile.AddBehavior(new RemoveBloonModifiersModel("EDStripFortified", false, false, false, true, false, new Il2CppSystem.Collections.Generic.List<string>()));

      projectile.AddBehavior(new DamageModifierForTagModel("EDCeramDamage", "Ceramic", 1, 5, false, false));
      projectile.AddBehavior(new DamageModifierForTagModel("EDLeadDamage", "Lead", 1, 2, false, false));
      projectile.AddBehavior(new DamageModifierForTagModel("EDDdtDamage", "Ddt", 1, 28, false, false));
    }
    public override string Icon => "WallFactory400Icon";
    public override string Portrait => "WallFactory400";
  }
  public class GatesOfJudgement : ModUpgrade<WallFactory>
  {
    public override string Name => "Gates of Judgement";
    public override string DisplayName => "Gates of Judgement";
    public override string Description => "The monkeys went a little overboard and now instead of fire, walls release a shower of deadly homing defense lasers whenever a bloon dares to tread near them.";
    public override int Cost => 51000;
    public override int Path => TOP;
    public override int Tier => 5;
    public override void ApplyUpgrade(TowerModel towerModel)
    {
      var attackModel = towerModel.GetAttackModel();
      var projectile = attackModel.weapons[0].projectile;
      /*var mineProj = Game.instance.model.GetTowerFromId("SpikeFactory-400").GetAttackModel().weapons[0].projectile.GetBehavior<CreateProjectileOnExpireModel>().projectile.Duplicate();
      var mineEm = Game.instance.model.GetTowerFromId("SpikeFactory-400").GetAttackModel().weapons[0].projectile.GetBehavior<CreateProjectileOnExpireModel>().emission.Duplicate();
      projectile.AddBehavior(new CreateProjectileOnExpireModel("FailureContingency",mineProj,mineEm,false));*/
      projectile.RemoveBehavior<CreateProjectileOnContactModel>();
      var laser = Game.instance.model.GetTowerFromId("DartlingGunner-300").GetAttackModel().weapons[0].projectile.Duplicate();
      var tracker = Game.instance.model.GetTowerFromId("Adora").GetAttackModel().weapons[0].projectile.GetBehavior<AdoraTrackTargetModel>().Duplicate();
      laser.GetDamageModel().immuneBloonProperties = BloonProperties.None;
      laser.RemoveBehavior<TravelStraitModel>();
      laser.AddBehavior(tracker);
      laser.GetDamageModel().damage = 3;
      laser.pierce = 20;
      laser.SetHitCamo(true);
      var behavior = new CreateProjectileOnContactModel(
          "Laser",
          laser, new RandomArcEmissionModel("LaserEmission", 4, 0f, 0f, 360f, 0f, new UnhollowerBaseLib.Il2CppReferenceArray<EmissionBehaviorModel>(new EmissionBehaviorModel[0])), false, true, false);
      projectile.AddBehavior(behavior);

      projectile.GetBehavior<CreateProjectileOnExhaustFractionModel>().projectile.GetDamageModel().damage = 50;
      projectile.GetDamageModel().damage += 5;

      foreach(DamageModifierForTagModel i in projectile.GetBehaviors<DamageModifierForTagModel>())
      {
        switch(i.name)
        {
          case "EDCeramDamage":
            i.damageAddative = 8;
            break;
          case "EDDdtDamage":
            i.damageAddative = 63;
            break;
        }
      }
      /*var soundBehavior = new CreateSoundOnProjectileExhaustModel(
          "BombSound",
          sound.sound1, sound.sound2, sound.sound3, sound.sound4, sound.sound5);
      projectile.AddBehavior(soundBehavior);*/

      /*var eB = new CreateEffectOnExhaustedModel("BombEffect", "", 0f, false,
          false, effect.effectModel);
      projectile.AddBehavior(eB);*/
    }
    public override string Icon => "WallFactory500Icon";
    public override string Portrait => "WallFactory500";
  }
  //  Path 2 Upgrades
  public class FasterConstruction : ModUpgrade<WallFactory>
  {
    public override string Name => "Faster Construction";
    public override string DisplayName => "Faster Construction";
    public override string Description => "Builds walls faster.";
    public override int Cost => 600;
    public override int Path => MIDDLE;
    public override int Tier => 1;
    public override void ApplyUpgrade(TowerModel towerModel)
    {
      var attackModel = towerModel.GetAttackModel();
      attackModel.weapons[0].rate /= 1.5f;
    }
    //public override string Icon => "";
  }

  public class Rebound : ModUpgrade<WallFactory>
  {
    public override string Name => "Rebound";
    public override string DisplayName => "Rebound";
    public override string Description => "Bloons are sent flying further away.";
    public override int Cost => 850;
    public override int Path => MIDDLE;
    public override int Tier => 2;
    public override void ApplyUpgrade(TowerModel towerModel)
    {
      var attackModel = towerModel.GetAttackModel();
      attackModel.weapons[0].projectile.GetBehavior<KnockbackModel>().lifespan *= 1.8f;
      attackModel.weapons[0].projectile.GetBehavior<KnockbackModel>().lightMultiplier = 1.6f;
      attackModel.weapons[0].projectile.GetBehavior<KnockbackModel>().mutator.lightMultiplier = 1.6f;
    }
    //public override string Icon => "";
  }

  public class DurableAlloys : ModUpgrade<WallFactory>
  {
    public override string Name => "Durable Alloys";
    public override string DisplayName => "Durable Alloys";
    public override string Description => "Walls last much longer and can carry over between rounds.";
    public override int Cost => 1100;
    public override int Path => MIDDLE;
    public override int Tier => 3;
    public override void ApplyUpgrade(TowerModel towerModel)
    {
      var attackModel = towerModel.GetAttackModel();
      attackModel.weapons[0].projectile.GetBehavior<AgeModel>().lifespan *= 2f;
      attackModel.weapons[0].projectile.GetBehavior<AgeModel>().rounds += 1;
    }
    //public override string Icon => "";
  }

  public class EnergyShield : ModUpgrade<WallFactory>
  {
    public override string Name => "Energy Shield";
    public override string DisplayName => "Energy Shield";
    public override string Description => "Ability: Erects a forcefield around the factory for a short while which repels bloons back along the track and slows heavier bloons.";
    public override int Cost => 4500;
    public override int Path => MIDDLE;
    public override int Tier => 4;
    public override void ApplyUpgrade(TowerModel towerModel)
    {
      var attackModel = towerModel.GetAttackModel();

      var ability = Game.instance.model.GetTowerFromId("DartlingGunner-040").GetBehavior<AbilityModel>().Duplicate();

      attackModel.weapons[0].projectile.GetBehavior<KnockbackModel>().moabMultiplier = 0.4f;
      attackModel.weapons[0].projectile.GetBehavior<KnockbackModel>().mutator.moabMultiplier = 0.4f;
      /*var abilityAttack = Game.instance.model.GetTowerFromId("MonkeySub-400").GetAttackModel();
      ability.GetBehavior<ActivateAttackModel>().attacks = new UnhollowerBaseLib.Il2CppReferenceArray<AttackModel>(1);
      ability.GetBehavior<ActivateAttackModel>().attacks[0] = abilityAttack;*/
      var abilityAttack = ability.GetBehavior<ActivateAttackModel>().attacks[0].weapons[0];
      var p = Game.instance.model.GetTowerFromId("MonkeySub-420").GetAttackModel(1).weapons[1].projectile.Duplicate();
      p.radius = towerModel.range+10;
      p.pierce = 300;
      p.AddBehavior(attackModel.weapons[0].projectile.GetBehavior<KnockbackModel>());
      p.RemoveBehavior<RemoveBloonModifiersModel>();
      abilityAttack.projectile = p;
      abilityAttack.emission = new SingleEmissionAtTowerModel("AbilityEmission", new UnhollowerBaseLib.Il2CppReferenceArray<EmissionBehaviorModel>(0));
      abilityAttack.rate = 0.35f;
      //ability.GetBehavior<ActivateAttackModel>().attacks[0].GetBehavior<CreateEffectWhileAttackingModel>().effectModel.scale *= 2f;
      ability.GetBehavior<ActivateAttackModel>().lifespan = 10f;
      ability.cooldown = 30f;
      towerModel.AddBehavior(ability);
    }
    //public override string Icon => "";
  }

  public class TheTrueAreaDenialSystem : ModUpgrade<WallFactory>
  {
    public override string Name => "The True Area Denial System";
    public override string DisplayName => "The True Area Denial System";
    public override string Description => "Vastly increased range of influence, wall construction speed, and energy shield potency. Energy shield stays up longer as well.";
    public override int Cost => 22000;
    public override int Path => MIDDLE;
    public override int Tier => 5;
    public override void ApplyUpgrade(TowerModel towerModel)
    {
      var attackModel = towerModel.GetAttackModel();

      var ability = towerModel.GetBehavior<AbilityModel>();

      attackModel.weapons[0].projectile.GetBehavior<KnockbackModel>().moabMultiplier = 0.7f;
      attackModel.weapons[0].projectile.GetBehavior<KnockbackModel>().mutator.moabMultiplier = 0.7f;
      attackModel.weapons[0].projectile.GetBehavior<KnockbackModel>().heavyMultiplier += 0.2f;
      attackModel.weapons[0].projectile.GetBehavior<KnockbackModel>().mutator.heavyMultiplier += 0.2f;
      attackModel.weapons[0].projectile.GetBehavior<KnockbackModel>().lightMultiplier += 0.2f;
      attackModel.weapons[0].projectile.GetBehavior<KnockbackModel>().mutator.lightMultiplier += 0.2f;
      towerModel.range += 30;
      attackModel.range = towerModel.range;
      attackModel.weapons[0].rate /= 3f;

      var abilityProj = ability.GetBehavior<ActivateAttackModel>().attacks[0].weapons[0].projectile;
      abilityProj.radius = towerModel.range + 10;
      abilityProj.pierce = 500;
      ability.GetBehavior<ActivateAttackModel>().lifespan = 15f;
    }
    //public override string Icon => "";
  }

  //  Path 3 Upgrades
  public class ThickerWalls : ModUpgrade<WallFactory>
  {
    public override string Name => "Thicker Walls";
    public override string DisplayName => "Thicker Walls";
    public override string Description => "Walls can endure more punishment before breaking.";
    public override int Cost => 650;
    public override int Path => BOTTOM;
    public override int Tier => 1;
    public override void ApplyUpgrade(TowerModel towerModel)
    {
      var attackModel = towerModel.GetAttackModel();
      attackModel.weapons[0].projectile.pierce += 20;
    }
    //public override string Icon => "";
  }
  public class HeavierWalls : ModUpgrade<WallFactory>
  {
    public override string Name => "Heavier Walls";
    public override string DisplayName => "Heavier Walls";
    public override string Description => "Walls can successfully hold back heavier bloons.";
    public override int Cost => 900;
    public override int Path => BOTTOM;
    public override int Tier => 2;
    public override void ApplyUpgrade(TowerModel towerModel)
    {
      var attackModel = towerModel.GetAttackModel();
      attackModel.weapons[0].projectile.GetBehavior<KnockbackModel>().heavyMultiplier = 1.25f;
      attackModel.weapons[0].projectile.GetBehavior<KnockbackModel>().mutator.heavyMultiplier = 1.25f;
    }
    //public override string Icon => "";
  }
  public class WiderWalls : ModUpgrade<WallFactory>
  {
    public override string Name => "Wider Walls";
    public override string DisplayName => "Wider Walls";
    public override string Description => "Walls have a greater deploy range can sustain more damage and are extra resistant to small bloons.";
    public override int Cost => 2700;
    public override int Path => BOTTOM;
    public override int Tier => 3;
    public override void ApplyUpgrade(TowerModel towerModel)
    {
      var attackModel = towerModel.GetAttackModel();
      var projectile = attackModel.weapons[0].projectile;
      towerModel.range += 10f;
      attackModel.range = towerModel.range;
      projectile.pierce += 70;
      projectile.AddBehavior(new CollideExtraPierceReductionModel("CeramicPierce", "Ceramic", 1, false));
      projectile.AddBehavior(new CollideExtraPierceReductionModel("MoabPierce", "Moab", 1, false));
      projectile.AddBehavior(new CollideExtraPierceReductionModel("BfbPierce", "Bfb", 1, false));
      projectile.AddBehavior(new CollideExtraPierceReductionModel("ZomgPierce", "Zomg", 1, false));
      projectile.AddBehavior(new CollideExtraPierceReductionModel("DdtPierce", "Ddt", 1, false));
      projectile.AddBehavior(new CollideExtraPierceReductionModel("BadPierce", "Bad", 1, false));
      projectile.AddBehavior(new IgnoreInsufficientPierceModel("IgnorePierceCap"));
      projectile.scale *= 1.2f;
    }
    //public override string Icon => "";
  }
  public class TallerWalls : ModUpgrade<WallFactory>
  {
    public override string Name => "Taller Walls";
    public override string DisplayName => "Taller Walls";
    public override string Description => "Walls are now capable of holding even high-altitude Moab-class bloons at bay.";
    public override int Cost => 12500;
    public override int Path => BOTTOM;
    public override int Tier => 4;
    public override void ApplyUpgrade(TowerModel towerModel)
    {
      var attackModel = towerModel.GetAttackModel();
      var projectile = attackModel.weapons[0].projectile;
      var pierceModels = projectile.GetBehaviors<CollideExtraPierceReductionModel>();
      foreach(CollideExtraPierceReductionModel i in pierceModels)
      {
        switch(i.name)
        {
          case "MoabPierce":
            i.extraAmount = 20;
            break;
          case "BfbPierce":
            i.extraAmount = 40;
            break;
          case "ZomgPierce":
            i.extraAmount = 80;
            break;
          case "DdtPierce":
            i.extraAmount = 20;
            break;
          case "BadPierce":
            i.extraAmount = 80;
            break;
        }
      }
      projectile.GetDamageModel().damage += 1;
      projectile.pierce += 40;
      projectile.AddBehavior(new DamageModifierForTagModel("MoabDamage", "Moabs", 1, 18, false, false));

      attackModel.weapons[0].projectile.GetBehavior<KnockbackModel>().moabMultiplier = 1.25f;
      attackModel.weapons[0].projectile.GetBehavior<KnockbackModel>().mutator.moabMultiplier = 1.25f;
    }
    //public override string Icon => "";
  }

  public class TheGreatWall : ModUpgrade<WallFactory>
  {
    public override string Name => "The Great Wall";
    public override string DisplayName => "The Great Wall";
    public override string Description => "Creates a nearly impenetrable barrier against Moab-class bloons.";
    public override int Cost => 32000;
    public override int Path => BOTTOM;
    public override int Tier => 5;
    public override void ApplyUpgrade(TowerModel towerModel)
    {
      var attackModel = towerModel.GetAttackModel();
      var projectile = attackModel.weapons[0].projectile;
      var pierceModels = projectile.GetBehaviors<CollideExtraPierceReductionModel>();
      foreach (CollideExtraPierceReductionModel i in pierceModels)
      {
        switch (i.name)
        {
          case "MoabPierce":
            i.extraAmount = 10;
            break;
          case "BfbPierce":
            i.extraAmount = 20;
            break;
          case "ZomgPierce":
            i.extraAmount = 40;
            break;
          case "DdtPierce":
            i.extraAmount = 10;
            break;
          case "BadPierce":
            i.extraAmount = 40;
            break;
        }
      }
      projectile.pierce += 40;
      projectile.GetDamageModel().damage += 1;
      projectile.GetBehavior<DamageModifierForTagModel>().damageAddative = 47;

      attackModel.weapons[0].projectile.GetBehavior<KnockbackModel>().moabMultiplier = 1.35f;
      attackModel.weapons[0].projectile.GetBehavior<KnockbackModel>().mutator.moabMultiplier = 1.35f;
    }
    //public override string Icon => "";
  }
}
