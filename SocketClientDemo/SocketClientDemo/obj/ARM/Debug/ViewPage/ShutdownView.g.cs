﻿#pragma checksum "E:\比赛\XiYouFastPass\SocketClientDemo\SocketClientDemo\ViewPage\ShutdownView.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "3C45C73B3A405C87DA4439FA085E48B7"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SocketClientDemo.ViewPage
{
    partial class ShutdownView : 
        global::Windows.UI.Xaml.Controls.Page, 
        global::Windows.UI.Xaml.Markup.IComponentConnector,
        global::Windows.UI.Xaml.Markup.IComponentConnector2
    {
        /// <summary>
        /// Connect()
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 14.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 1:
                {
                    this.grid = (global::Windows.UI.Xaml.Controls.Grid)(target);
                }
                break;
            case 2:
                {
                    this.loadanimation = (global::Windows.UI.Xaml.Media.Animation.Storyboard)(target);
                }
                break;
            case 3:
                {
                    this.canelanimation = (global::Windows.UI.Xaml.Media.Animation.Storyboard)(target);
                }
                break;
            case 4:
                {
                    this.immediately = (global::Windows.UI.Xaml.Controls.Button)(target);
                    #line 80 "..\..\..\ViewPage\ShutdownView.xaml"
                    ((global::Windows.UI.Xaml.Controls.Button)this.immediately).Click += this.immediately_Click;
                    #line default
                }
                break;
            case 5:
                {
                    this.afterhalfhour = (global::Windows.UI.Xaml.Controls.Button)(target);
                    #line 82 "..\..\..\ViewPage\ShutdownView.xaml"
                    ((global::Windows.UI.Xaml.Controls.Button)this.afterhalfhour).Click += this.afterhalfhour_Click;
                    #line default
                }
                break;
            case 6:
                {
                    this.afteronehour = (global::Windows.UI.Xaml.Controls.Button)(target);
                    #line 83 "..\..\..\ViewPage\ShutdownView.xaml"
                    ((global::Windows.UI.Xaml.Controls.Button)this.afteronehour).Click += this.afteronehour_Click;
                    #line default
                }
                break;
            case 7:
                {
                    this.canelClose = (global::Windows.UI.Xaml.Controls.Button)(target);
                    #line 84 "..\..\..\ViewPage\ShutdownView.xaml"
                    ((global::Windows.UI.Xaml.Controls.Button)this.canelClose).Click += this.canelClose_Click;
                    #line default
                }
                break;
            case 8:
                {
                    this.canel = (global::Windows.UI.Xaml.Controls.Button)(target);
                    #line 86 "..\..\..\ViewPage\ShutdownView.xaml"
                    ((global::Windows.UI.Xaml.Controls.Button)this.canel).Click += this.canel_Click;
                    #line default
                }
                break;
            case 9:
                {
                    this.shutdown = (global::Windows.UI.Xaml.Controls.Button)(target);
                    #line 36 "..\..\..\ViewPage\ShutdownView.xaml"
                    ((global::Windows.UI.Xaml.Controls.Button)this.shutdown).Tapped += this.shutdown_Tapped;
                    #line default
                }
                break;
            case 10:
                {
                    this.logoff = (global::Windows.UI.Xaml.Controls.Button)(target);
                    #line 42 "..\..\..\ViewPage\ShutdownView.xaml"
                    ((global::Windows.UI.Xaml.Controls.Button)this.logoff).Tapped += this.logoff_Tapped;
                    #line default
                }
                break;
            case 11:
                {
                    this.img = (global::Windows.UI.Xaml.Controls.Image)(target);
                    #line 18 "..\..\..\ViewPage\ShutdownView.xaml"
                    ((global::Windows.UI.Xaml.Controls.Image)this.img).Tapped += this.img_Tapped;
                    #line default
                }
                break;
            default:
                break;
            }
            this._contentLoaded = true;
        }

        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 14.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::Windows.UI.Xaml.Markup.IComponentConnector GetBindingConnector(int connectionId, object target)
        {
            global::Windows.UI.Xaml.Markup.IComponentConnector returnValue = null;
            return returnValue;
        }
    }
}

