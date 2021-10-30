using Assets.Scripts.Simulation.Towers;
using Assets.Scripts.Unity;
using Assets.Scripts.Unity.Bridge;
using Assets.Scripts.Unity.UI_New.InGame;
using BTD_Mod_Helper;
using MelonLoader;
using UnityEngine;

[assembly: MelonInfo(typeof(MovableTowers.Main), "Movable Towers", "1.0.0", "AttackDuck")]
[assembly: MelonGame("Ninja Kiwi", "BloonsTD6")]
namespace MovableTowers
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

    public static Tower moveTarget = null;
    public static bool moveActive = false;

    public override void OnMainMenu()
    {
      base.OnMainMenu();
    }

    public override void OnApplicationStart()
    {
      MelonLogger.Msg("Movable Towers Loaded!");
      MelonLogger.Msg("Press [Shift] with a tower selected to move it. Press [Left Button] to place it again.");
    }

    public override void OnTowerSelected(Tower tower)
    {
      base.OnTowerSelected(tower);
      moveTarget = tower;
    }

    public override void OnUpdate()
    {
      base.OnUpdate();
      try
      {
        bool inAGame = InGame.instance != null && InGame.instance.bridge != null;

        if (inAGame)
        {
          if (Input.GetKey(KeyCode.LeftShift))
          {
            moveActive = true;
          }

          if (moveActive && Input.GetMouseButtonDown(0))
          {
            moveActive = false;
            //MelonLogger.Msg("x: " + moveTarget.Position.X + "\ny: " + moveTarget.Position.Y);
            if(moveTarget != null)
            {
              moveTarget.Position.X = Mathf.Clamp(moveTarget.Position.X, -146, 146);
              moveTarget.Position.Y = Mathf.Clamp(moveTarget.Position.Y, -112, 116);
            }
          }

          if (moveActive && moveTarget != null)
          {
            var v3 = UnityEngine.Input.mousePosition;
            v3.z = 0;
            v3 = InGame.instance.sceneCamera.ScreenToWorldPoint(v3);

            float x = v3.x;
            float y = v3.y * (-2.3f);
            moveTarget.PositionTower(new Assets.Scripts.Simulation.SMath.Vector2(x, y));
          }
        }
      }
      catch (System.NullReferenceException)
      {

      }
    }
  }
}
