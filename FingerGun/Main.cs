using Assets.Scripts.Models.Towers.Behaviors.Emissions;
using Assets.Scripts.Models.Towers.Projectiles;
using Assets.Scripts.Models.Towers.Projectiles.Behaviors;
using Assets.Scripts.Models.Towers.Weapons;
using Assets.Scripts.Simulation.Bloons;
using Assets.Scripts.Simulation.Towers;
using Assets.Scripts.Simulation.Towers.Projectiles;
using Assets.Scripts.Unity;
using Assets.Scripts.Unity.UI_New.InGame;
using BTD_Mod_Helper;
using BTD_Mod_Helper.Extensions;
using MelonLoader;
using UnityEngine;

[assembly: MelonInfo(typeof(FingerGun.Main), "Finger Gun", "1.0.0", "AttackDuck")]
[assembly: MelonGame("Ninja Kiwi", "BloonsTD6")]
namespace FingerGun
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
    //public static ProjectileModel gunmodel;
    public static int tickTimer = 0;

    public override void OnMainMenu()
    {
      base.OnMainMenu();
    }

    public override void OnApplicationStart()
    {
      MelonLogger.Msg("Finger Gun Loaded!");
      //Game.instance.model.GetTowerFromId("BombTower").GetAttackModel().weapons[0].projectile.GetBehavior<CreateProjectileOnContactModel>().projectile.Duplicate();
    }

    public override void OnUpdate()
    {
      base.OnUpdate();
      if (tickTimer>0) tickTimer--;
      try
      {
        bool inAGame = InGame.instance != null && InGame.instance.bridge != null;

        if (inAGame)
        {
          if (Input.GetMouseButtonDown(0) && tickTimer==0)
          {
            var v3 = UnityEngine.Input.mousePosition;
            v3.z = 0;
            v3 = InGame.instance.sceneCamera.ScreenToWorldPoint(v3);

            float x = v3.x;
            float y = v3.y * (-2.3f);
            foreach (Bloon b in InGameExt.GetBloons(InGame.instance))
            {
              if (b.Position.ToVector2().Distance(new Assets.Scripts.Simulation.SMath.Vector2(x, y)) < 8f)
              {
                //b.Damage(1.0f, null, false, false, false, null, BloonProperties.None, false, false, false, true);
                //b.RecieveDamage(1.0f, null, false, false, false, null, false);
                if(!b.IsImmune(BloonProperties.Lead))
                {
                  b.ExecuteDamageTask(1.0f, null, false, false, true, null, false);
                }
                tickTimer = 3;
                break;
              }
            }
          }
        }
      }
      catch (System.NullReferenceException)
      {

      }
    }
  }


}
