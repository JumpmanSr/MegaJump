using Il2Cpp;
using Il2CppAssets.Scripts.Actors.Enemies;
using Il2CppRewired;
using MelonLoader;
using UnityEngine;
using UnityEngine.Splines;

[assembly: MelonInfo(typeof(MegaJump.Core), "MegaJump", "1.0.12", "JumpmanSr", null)]
[assembly: MelonGame("Ved", "Megabonk")]

namespace MegaJump
{
    public class Core : MelonMod
    {

        bool _ESP = false;
        float _ESP_Range = 25f;
        float _ESP_BoxWidth = 80f;
        float _ESP_BoxHeight = 100f;
        bool _DrawLineOnly = false;
        bool _SetFontSizeOnce = true;
        GUIStyle bigger = null;

        Il2CppAssets.Scripts.Managers.EnemyManager enemyManager = null;


        public override void OnInitializeMelon()
        {
            LoggerInstance.Msg("Initialized.");
            
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            LoggerInstance.Msg($"Scene Loaded: {sceneName} ({buildIndex})");
            enemyManager = Il2CppAssets.Scripts.Managers.EnemyManager.Instance;
            if (enemyManager != null)
                MelonLogger.Msg("EnemyManager initialized and ready.");
            else
            {
                _ESP = false;
            }
        }


        public override void OnGUI()
        {
            if (_SetFontSizeOnce)
            {
                bigger = new GUIStyle(GUI.skin.label);
                bigger.fontSize = 20;
                _SetFontSizeOnce = false;
            }
            if (_ESP)
            {
                foreach (var enemy in enemyManager.enemies.Values)
                {
                    if (enemy == null || enemy.transform == null)
                        continue;

                    var enemyPos = enemy.transform.position;
                    Vector3 screenPos = Camera.main.WorldToScreenPoint(enemyPos);
                    if (screenPos.z > 0) // && Vector3.Distance(Camera.main.transform.position, enemyPos) <= _ESP_Range) // Only draw if in front of the camera
                    {
                        float boxX = screenPos.x - (_ESP_BoxWidth / 2);
                        float boxY = Screen.height - screenPos.y - (_ESP_BoxHeight / 2); // Invert Y for GUI
                        GUI.Label(new Rect(boxX, boxY - 20, 200, 20), "HP " + Math.Round(enemy.hp,2).ToString() + "/" + Math.Round(enemy.maxHp,2).ToString());
                        if (_DrawLineOnly)
                            DrawLine(new Vector2(Camera.main.WorldToScreenPoint(Il2Cpp.GameManager.Instance.player.feet.transform.position).x, Camera.main.WorldToScreenPoint(Il2Cpp.GameManager.Instance.player.head.transform.position).y - Il2Cpp.GameManager.Instance.player.height*3), new Vector2(screenPos.x, Screen.height - screenPos.y), UnityEngine.Color.blue, 1f);
                        else
                            DrawBox(new Vector2(boxX, boxY), 80f, 100f, UnityEngine.Color.blue, 1f);
                    }
                }
                //foreach(var shrine in DataManager.Instance.maps[0].shrines) // doesn't update properly atm so commented out for now
                //{
                //    if (shrine == null || shrine.transform == null)
                //        continue;
                //    var shrinePos = shrine.transform.position;
                //    Vector3 screenPos = Camera.main.WorldToScreenPoint(shrinePos);
                //    if (screenPos.z > 0) // && Vector3.Distance(Camera.main.transform.position, enemyPos) <= _ESP_Range) // Only draw if in front of the camera
                //    {
                //        float boxX = screenPos.x - (_ESP_BoxWidth / 2);
                //        float boxY = Screen.height - screenPos.y - (_ESP_BoxHeight / 2); // Invert Y for GUI
                //        GUI.Label(new Rect(boxX, boxY - 20, 200, 20), "Shrine");
                //        if (_DrawLineOnly)
                //            DrawLine(new Vector2(Camera.main.WorldToScreenPoint(Il2Cpp.GameManager.Instance.player.feet.transform.position).x, Camera.main.WorldToScreenPoint(Il2Cpp.GameManager.Instance.player.head.transform.position).y - Il2Cpp.GameManager.Instance.player.height * 3), new Vector2(screenPos.x, Screen.height - screenPos.y), UnityEngine.Color.yellow, 1f);
                //        else
                //            DrawBox(new Vector2(boxX, boxY), 80f, 100f, UnityEngine.Color.yellow, 1f);
                //    }
                //}
            }
            // Simple toggle for ESP
            GUI.color = Color.red;
            GUI.Label(new Rect(10, Screen.height - 170, 300, 30), "MegaJump by JumpmanSr", bigger);
            if (_ESP)
            {
                GUI.color = Color.green;
                GUI.Label(new Rect(10, Screen.height - 150, 300, 30), "ESP: ON (F3 to toggle)");
            }
            else
            {
                GUI.Label(new Rect(10, Screen.height - 150, 300, 30), "ESP: OFF (F3 to toggle)");
            }
            GUI.Label(new Rect(10, Screen.height - 130, 300, 30), "F1 to Add an Extra Projectile");
            GUI.Label(new Rect(10, Screen.height - 110, 300, 30), "F2 To Increase Attack Speed + 100%");
            GUI.Label(new Rect(10, Screen.height - 90, 300, 30), "F4 to Toggle Boxes / Lines");
            GUI.Label(new Rect(10, Screen.height - 70, 300, 30), "F5 to Heal Player");
            GUI.Label(new Rect(10, Screen.height - 50, 300, 30), "F6 to Grab All XP");
            GUI.Label(new Rect(10, Screen.height - 30, 300, 30), "F7 to Add Extra Jumps");
        }

        public override void OnUpdate()
        {

            if (UnityEngine.Input.GetKeyDown(KeyCode.F1))
            {
                var player = Il2Cpp.GameManager.Instance.GetPlayerInventory();
                if (player != null)
                {
                    player.playerStats.stats[Il2CppAssets.Scripts.Menu.Shop.EStat.Projectiles] += 1f;

                }
                else
                    MelonLogger.Warning("Player is NULL");
            }
            if (UnityEngine.Input.GetKeyDown(KeyCode.F2))
            {
                var player = Il2Cpp.GameManager.Instance.GetPlayerInventory();
                if (player != null)
                {
                    player.playerStats.stats[Il2CppAssets.Scripts.Menu.Shop.EStat.AttackSpeed] += 1f;
                    
                }
                else
                    MelonLogger.Warning("Player is NULL");
            }
            if (UnityEngine.Input.GetKeyDown(KeyCode.F3))
            {
                _ESP = !_ESP;
            }
            if (UnityEngine.Input.GetKeyDown(KeyCode.F4))
            {
                _DrawLineOnly = !_DrawLineOnly;
            }
            if(UnityEngine.Input.GetKeyDown(KeyCode.F5))
            {
                var player = Il2Cpp.GameManager.Instance.GetPlayerInventory();
                if (player != null)
                {
                    player.playerHealth.Heal(1000f);
                }
                else
                    MelonLogger.Warning("Player is NULL");
            }
            if(UnityEngine.Input.GetKeyDown(KeyCode.F6))
            {
                Il2Cpp.PickupManager.Instance.PickupAllXp();
            }
            if (UnityEngine.Input.GetKeyDown(KeyCode.F7))
            {
                var player = Il2Cpp.GameManager.Instance.GetPlayerInventory();
                if (player != null)
                {
                    player.playerStats.stats[Il2CppAssets.Scripts.Menu.Shop.EStat.ExtraJumps] += 1f;
                }
                else
                    MelonLogger.Warning("Player is NULL");
            }
        }

        //ESP Helpers
        public static void DrawLine(Vector2 start, Vector2 end, Color color, float width)
        {
            Color oldColour = GUI.color;

            var rad2deg = 360 / (Math.PI * 2);

            Vector2 d = end - start;

            float a = (float)rad2deg * Mathf.Atan(d.y / d.x);

            if (d.x < 0)
                a += 180;

            int width2 = (int)Mathf.Ceil(width / 2);

            GUIUtility.RotateAroundPivot(a, start);

            GUI.color = color;

            GUI.DrawTexture(new Rect(start.x, start.y - width2, d.magnitude, width), Texture2D.whiteTexture, ScaleMode.StretchToFill);

            GUIUtility.RotateAroundPivot(-a, start);

            GUI.color = oldColour;
        }
        public static void DrawBox(Vector2 topLeft, float width, float height, Color color, float lineWidth)
        {
            // Define the four corners of the box
            Vector2 topRight = new Vector2(topLeft.x + width, topLeft.y);
            Vector2 bottomLeft = new Vector2(topLeft.x, topLeft.y + height);
            Vector2 bottomRight = new Vector2(topLeft.x + width, topLeft.y + height);

            // Draw the four sides of the box (top, right, bottom, left)
            DrawLine(topLeft, topRight, color, lineWidth);      // Top
            DrawLine(topRight, bottomRight, color, lineWidth);  // Right
            DrawLine(bottomRight, bottomLeft, color, lineWidth); // Bottom
            DrawLine(bottomLeft, topLeft, color, lineWidth);    // Left
        }

    }
}