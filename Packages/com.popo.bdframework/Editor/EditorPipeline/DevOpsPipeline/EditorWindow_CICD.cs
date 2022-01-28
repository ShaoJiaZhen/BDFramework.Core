﻿using System;
using BDFramework.Editor.DevOps;
using BDFramework.Editor.EditorPipeline.DevOps;
using BDFramework.Editor.Unity3dEx;
using UnityEditor;
using UnityEngine;

namespace BDFramework.Editor.DevOps
{
    /// <summary>
    /// 一键构建资源
    /// </summary>
    public class EditorWindow_CICD : EditorWindow
    {
        [MenuItem("BDFrameWork工具箱/DevOps/CI", false, (int) BDEditorGlobalMenuItemOrderEnum.DevOps)]
        public static void Open()
        {
            var window = EditorWindow.GetWindow<EditorWindow_CICD>(false, "CI");
            window.maxSize = window.minSize = new Vector2(800, 800);
            window.Show();
            window.Focus();
        }


        private void OnGUI()
        {
            OnGUI_BuildpipelineCI();
        }

        private void OnDisable()
        {
            //保存
            BDEditorApplication.BDFrameWorkFrameEditorSetting.Save();
        }

        /// <summary>
        /// CI 相关
        /// </summary>
        public void OnGUI_BuildpipelineCI()
        {
            GUILayout.BeginVertical();
            {
                GUILayout.Label("CI相关测试:");
                var devops_setting = BDEditorApplication.BDFrameWorkFrameEditorSetting.DevOpsSetting;
                devops_setting.AssetBundleSVNUrl = EditorGUILayout.TextField("SVN地址", devops_setting.AssetBundleSVNUrl, GUILayout.Width(350));
                devops_setting.AssetBundleSVNAccount = EditorGUILayout.TextField("SVN账号", devops_setting.AssetBundleSVNAccount, GUILayout.Width(350));
                devops_setting.AssetBundleSVNPsw = EditorGUILayout.TextField("SVN密码", devops_setting.AssetBundleSVNPsw, GUILayout.Width(350));

                GUILayout.Space(20);
                
                GUILayout.Label("支持CI列表:");

                //获取所有ciapi
                var ciMethods = DevOpsTools.GetCIApis();

                foreach (var cim in ciMethods)
                {
                    var attrs = cim.GetCustomAttributes(false);
                    var ciAttr = attrs[0] as CIAttribute;
                    GUILayout.BeginHorizontal();
                    {
                        //描述
                        GUILayout.Label(ciAttr.Des+":", GUILayout.Width(150));
                        
                        //函数
                        var ciName = cim.ReflectedType.FullName + "." + cim.Name;
                        GUILayout.Label(ciName,GUILayout.Width(580));
                        //按钮
                        if (GUILayout.Button("复制",GUILayout.Width(50)))
                        {
                            GUIUtility.systemCopyBuffer = ciName;
                            EditorUtility.DisplayDialog("提示", "复制成功!", "OK");
                        }
                    }
                    GUILayout.EndHorizontal();
                }
                
                
                GUILayout.Space(10);
                EditorGUILayoutEx.Layout_DrawLineH(Color.white);
                
                GUILayout.Label(@"服务器CI流程:
一般Git管理代码，SVN或P4管理美术资产。
Git master分支作为稳定发布版本分支，工作都在子分支，测试通过后会合并到主分支。
SVN资产也会用hook实现同步到Git assets分支，供程序使用. 程序也会将测试通过的资产随着code提交到主分支.
CI一般监听Git Master分支，定时一键构建所有资产:AB包、脚本、Sql

1.资源流程: 每次美术提交=>更新老资产=>AB性能测试=>WebHook通知到内部=>提交到SVN
2.母包流程: 更新美术SVN，更新Git=>构建母包=>自动测试=>通知测试结果
3.资源更新: 直接将SVN资源=>转hash=>发布到资源服务器，客户端会自行下载
");
            }
            GUILayout.EndVertical();
        }
    }
}