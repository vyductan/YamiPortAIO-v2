using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;

using EloBuddy; 
using LeagueSharp.Common; 
namespace TheCassiopeia.Commons
{
    /*
        Note: This class is from the xSalice Religion AIO!
     */

    class DamageIndicator
    {
        public delegate float DamageToUnitDelegate(AIHeroClient hero);

        private const int XOffset = 10;
        private const int YOffset = 20;
        private const int Width = 103;
        private const int Height = 8;

        public static Color Color = Color.Goldenrod;
        public static Color FillColor = Color.FromArgb(100, Color.Goldenrod);
        public static bool Fill = true;

        public static bool Enabled = true;
        private static DamageToUnitDelegate _damageToUnit;

        private static readonly Render.Text Text = new Render.Text(
            0, 0, "", 11, new ColorBGRA(255, 0, 0, 255), "monospace");

        public static DamageToUnitDelegate DamageToUnit
        {
            get { return _damageToUnit; }

            set
            {
                if (_damageToUnit == null)
                {
                    Drawing.OnDraw += Drawing_OnDraw;
                }
                _damageToUnit = value;
            }
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (!Enabled || _damageToUnit == null)
            {
                return;
            }

            foreach (var unit in ObjectManager.Get<AIHeroClient>().Where(h => h.IsValid && h.IsHPBarRendered && h.IsEnemy))
            {
                var barPos = unit.HPBarPosition;
                var damage = _damageToUnit(unit);
                var percentHealthAfterDamage = Math.Max(0, unit.Health - damage) / unit.MaxHealth;
                var yPos = barPos.Y + YOffset;
                var xPosDamage = barPos.X + XOffset + Width * percentHealthAfterDamage;
                var xPosCurrentHp = barPos.X + XOffset + Width * unit.Health / unit.MaxHealth;

                if (damage > unit.Health)
                {
                    Text.X = (int)barPos.X + XOffset;
                    Text.Y = (int)barPos.Y + YOffset - 13;
                    Text.text = ((int)(unit.Health - damage)).ToString();
                    Text.OnEndScene();
                }

                Drawing.DrawLine(xPosDamage, yPos, xPosDamage, yPos + Height, 2, Color);

                if (Fill)
                {
                    float differenceInHP = xPosCurrentHp - xPosDamage;
                    var pos1 = barPos.X + 9 + (107 * percentHealthAfterDamage);

                    for (int i = 0; i < differenceInHP; i++)
                    {
                        Drawing.DrawLine(pos1 + i, yPos, pos1 + i, yPos + Height, 1, FillColor);
                    }
                }
            }
        }
    }
}
