using System;
using System.Text;
using Sandbox;
using Sandbox.Graphics.GUI;
using Shared.Plugin;
using VRage;
using VRage.Utils;
using VRageMath;

namespace ClientPlugin.GUI
{

    public class PluginConfigDialog : MyGuiScreenBase
    {
        private const string Caption = "Bugfixes Configuration";
        public override string GetFriendlyName() => "PluginConfigDialog";

        private MyLayoutTable layoutTable;

        private MyGuiControlLabel enabledLabel;
        private MyGuiControlCheckbox enabledCheckbox;

        private MyGuiControlLabel turretNanLabel;
        private MyGuiControlCheckbox turretNanCheckbox;

        private MyGuiControlLabel aiCrashLabel;
        private MyGuiControlCheckbox aiCrashCheckbox;

        /*BOOL_OPTION
        private MyGuiControlLabel optionNameLabel;
        private MyGuiControlCheckbox optionNameCheckbox;

        BOOL_OPTION*/
        private MyGuiControlMultilineText infoText;
        private MyGuiControlButton closeButton;

        public PluginConfigDialog() : base(new Vector2(0.5f, 0.5f), MyGuiConstants.SCREEN_BACKGROUND_COLOR, new Vector2(0.5f, 0.7f), false, null, MySandboxGame.Config.UIBkOpacity, MySandboxGame.Config.UIOpacity)
        {
            EnabledBackgroundFade = true;
            m_closeOnEsc = true;
            m_drawEvenWithoutFocus = true;
            CanHideOthers = true;
            CanBeHidden = true;
            CloseButtonEnabled = true;
        }

        public override void LoadContent()
        {
            base.LoadContent();
            RecreateControls(true);
        }

        public override void RecreateControls(bool constructor)
        {
            base.RecreateControls(constructor);

            CreateControls();
            LayoutControls();
        }

        private void CreateControls()
        {
            AddCaption(Caption);

            var config = Common.Config;
            CreateCheckbox(out enabledLabel, out enabledCheckbox, config.Enabled, value => config.Enabled = value, "Enabled", "Enables the plugin");
            CreateCheckbox(out turretNanLabel, out turretNanCheckbox, config.TurretNan, value => config.TurretNan = value, "Fix NaN crash in TurretControlBlock", "Fixes crash due to NaN value in TurretControlBlock");
CreateCheckbox(out aiCrashLabel, out aiCrashCheckbox, config.AiCrash, value => config.AiCrash = value, "Fix crash in AI blocks", "Fix crash in AI blocks (Automaton)");
//BOOL_OPTION CreateCheckbox(out optionNameLabel, out optionNameCheckbox, config.OptionName, value => config.OptionName = value, "Option label", "Option tooltip");

            EnableDisableFixes();

            enabledCheckbox.IsCheckedChanged += EnableDisableFixes;

            infoText = new MyGuiControlMultilineText
            {
                Name = "InfoText",
                OriginAlign = MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_TOP,
                TextAlign = MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_TOP,
                TextBoxAlign = MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_TOP,
                Text = new StringBuilder("\r\nIt is safe to change these options during the game.\r\nPlease send me feedback on the SE Mods Discord\r\nwhether they worked out. Thanks!")
            };

            closeButton = new MyGuiControlButton(originAlign: MyGuiDrawAlignEnum.HORISONTAL_RIGHT_AND_VERTICAL_CENTER, text: MyTexts.Get(MyCommonTexts.Ok), onButtonClick: OnOk);
        }

        private void OnOk(MyGuiControlButton _) => CloseScreen();

        private void CreateCheckbox(out MyGuiControlLabel labelControl, out MyGuiControlCheckbox checkboxControl, bool value, Action<bool> store, string label, string tooltip)
        {
            labelControl = new MyGuiControlLabel
            {
                Text = label,
                OriginAlign = MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_TOP
            };

            checkboxControl = new MyGuiControlCheckbox(toolTip: tooltip)
            {
                OriginAlign = MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_TOP,
                Enabled = true,
                IsChecked = value
            };
            checkboxControl.IsCheckedChanged += cb => store(cb.IsChecked);
        }

        private void EnableDisableFixes(MyGuiControlCheckbox _ = null)
        {
            var enabled = enabledCheckbox.IsChecked;

            turretNanCheckbox.Enabled = enabled;
aiCrashCheckbox.Enabled = enabled;
//BOOL_OPTION optionNameCheckbox.Enabled = enabled;
        }

        private void LayoutControls()
        {
            var size = Size ?? Vector2.One;
            layoutTable = new MyLayoutTable(this, new Vector2(-0.3f * size.X, -0.375f * size.Y), new Vector2(0.6f * size.X, 0.8f * size.Y));
            layoutTable.SetColumnWidths(400f, 100f);
            layoutTable.SetRowHeights(80f, 50f, 50f,50f,/*BOOL_OPTION 50f,BOOL_OPTION*/ 150f, 50f);

            var row = 0;

            layoutTable.Add(enabledLabel, MyAlignH.Left, MyAlignV.Center, row, 0);
            layoutTable.Add(enabledCheckbox, MyAlignH.Left, MyAlignV.Center, row, 1);
            row++;

            layoutTable.Add(turretNanLabel, MyAlignH.Left, MyAlignV.Center, row, 0);
            layoutTable.Add(turretNanCheckbox, MyAlignH.Left, MyAlignV.Center, row, 1);
            row++;

            layoutTable.Add(aiCrashLabel, MyAlignH.Left, MyAlignV.Center, row, 0);
            layoutTable.Add(aiCrashCheckbox, MyAlignH.Left, MyAlignV.Center, row, 1);
            row++;

            /*BOOL_OPTION
            layoutTable.Add(optionNameLabel, MyAlignH.Left, MyAlignV.Center, row, 0);
            layoutTable.Add(optionNameCheckbox, MyAlignH.Left, MyAlignV.Center, row, 1);
            row++;

            BOOL_OPTION*/
            layoutTable.Add(infoText, MyAlignH.Left, MyAlignV.Top, row, 0, colSpan: 2);
            row++;

            layoutTable.Add(closeButton, MyAlignH.Center, MyAlignV.Center, row, 0, colSpan: 2);
            // row++;
        }
    }
}