﻿#pragma checksum "E:\XiYouFastPass\SocketClientDemo\SocketClientDemo\ViewPage\ScreenShow.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "90203E09632B8D32622EEB253CC05E0F"
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
    partial class ScreenShow : 
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
                    this.screenimg = (global::Windows.UI.Xaml.Controls.Image)(target);
                }
                break;
            case 2:
                {
                    this.img = (global::Windows.UI.Xaml.Controls.Image)(target);
                    #line 18 "..\..\..\ViewPage\ScreenShow.xaml"
                    ((global::Windows.UI.Xaml.Controls.Image)this.img).Tapped += this.img_Tapped;
                    #line default
                }
                break;
            case 3:
                {
                    this.txt = (global::Windows.UI.Xaml.Controls.TextBlock)(target);
                }
                break;
            case 4:
                {
                    this.saveimg = (global::Windows.UI.Xaml.Controls.Image)(target);
                    #line 20 "..\..\..\ViewPage\ScreenShow.xaml"
                    ((global::Windows.UI.Xaml.Controls.Image)this.saveimg).Tapped += this.save_Tapped;
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

