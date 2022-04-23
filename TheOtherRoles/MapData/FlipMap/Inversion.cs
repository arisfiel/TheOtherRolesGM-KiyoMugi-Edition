using HarmonyLib;
using UnityEngine;

namespace TheOtherRoles.Mode.InversionMode
{
    
    [HarmonyPatch(typeof(ShipStatus), nameof(GameStartManager.Start))]
    public class inversion
    {

        public static GameObject skeld;
        public static GameObject mira;
        public static GameObject polus;
        public static GameObject airship;
        public static void Prefix()
        {

            if (PlayerControl.GameOptions.MapId == 0 && CustomOptionHolder.inversionSkeld.getBool())
            {
                skeld = GameObject.Find("SkeldShip(Clone)");
                skeld.transform.localScale = new Vector3(-1.2f, 1.2f, 1.2f);
            }
            else if (PlayerControl.GameOptions.MapId == 1 && CustomOptionHolder.inversionMira.getBool())
            {
                mira = GameObject.Find("MiraShip(Clone)");
                mira.transform.localScale = new Vector3(-1f, 1f, 1f);
                MiraShipStatus.Instance.InitialSpawnCenter = new Vector2(4.4f, 2.2f);
                MiraShipStatus.Instance.MeetingSpawnCenter = new Vector2(-25.3921f, 2.5626f);
                MiraShipStatus.Instance.MeetingSpawnCenter2 = new Vector2(-25.3921f, 2.5626f);
            }
            else if(PlayerControl.GameOptions.MapId == 2 && CustomOptionHolder.inversionPolus.getBool())
            {
                polus = GameObject.Find("PolusShip(Clone)");
                polus.transform.localScale = new Vector3(-1f, 1f, 1f);
                PolusShipStatus.Instance.InitialSpawnCenter = new Vector2(-16.7f, -2.1f);
                PolusShipStatus.Instance.MeetingSpawnCenter = new Vector2(-19.5f, -17f);
                PolusShipStatus.Instance.MeetingSpawnCenter2 = new Vector2(-19.5f, -17f);
            }
            /*else if(PlayerControl.GameOptions.MapId == 4 && CustomOptionHolder.InversionAShip.getBool())
            {
                airship = GameObject.Find("Airship(Clone)");
                airship.transform.localScale = new Vector3(-0.7f, 0.7f, 1f);
                airshipの選択スポーンシステムの対応ができてないため非表示 
            }*/

        }

    }

}