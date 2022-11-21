using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace LetConst.MasterData
{
    public sealed class HomeEditorPage : EditorPageBase<HomeEditorPage>
    {
        public override void OnGUI(EditorWindow _)
        {
            // ヘッダー表示
            EditorGUILayout.LabelField("Master Data Editor", GUIStyles.HomeTitleStyle);

            IReadOnlyList<HomeGridButton> buttons      = EditorConstants.Home.GridButtons;
            int                           passedButton = 0;

            GUILayout.FlexibleSpace();
            EditorGUILayout.BeginVertical();

            // 各種マスターデータの編集画面に移動するボタンを表示
            for (int y = 0; y < EditorConstants.Home.MaxVerticalGrid; y++)
            {
                if (buttons.Count == 0) break;

                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();

                for (int x = 0; x < EditorConstants.Home.MaxHorizontalGrid; x++)
                {
                    HomeGridButton button = buttons[passedButton];
                    GUIStyle       style  = GUIStyles.HomeButtonStyle(button);

                    // ボタンクリックで対応するページにセット
                    if (GUILayout.Button(button.label, style, GUILayout.Width(button.buttonSize.x),
                                         GUILayout.Height(button.buttonSize.y)))
                    {
                        MasterDataEditor.Broker.Publish(MasterDataEvent.SetPage.Get((RenderPage) button.dataType));
                    }

                    passedButton++;

                    if (passedButton == buttons.Count)
                        break;
                }

                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();

                if (passedButton == buttons.Count)
                    break;
            }

            EditorGUILayout.EndVertical();

            GUILayout.FlexibleSpace();
            EditorGUILayout.LabelField(EditorConstants.ToolVersion, GUIStyles.VersionStyle);
        }
    }
}
