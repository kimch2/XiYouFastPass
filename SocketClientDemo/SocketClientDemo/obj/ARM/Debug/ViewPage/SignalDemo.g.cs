﻿#pragma checksum "E:\比赛\XiYouFastPass\SocketClientDemo\SocketClientDemo\ViewPage\SignalDemo.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "369F1335285662C2C2F252391888ABF0"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SocketClientDemo
{
    partial class Signal : 
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
                    #line 33 "..\..\..\ViewPage\SignalDemo.xaml"
                    ((global::Windows.UI.Xaml.Controls.Grid)this.grid).ManipulationDelta += this.Grid_ManipulationDelta;
                    #line default
                }
                break;
            case 2:
                {
                    this.Left = (global::Windows.UI.Xaml.Controls.Button)(target);
                    #line 44 "..\..\..\ViewPage\SignalDemo.xaml"
                    ((global::Windows.UI.Xaml.Controls.Button)this.Left).Click += this.Left_Click;
                    #line default
                }
                break;
            case 3:
                {
                    this.Right = (global::Windows.UI.Xaml.Controls.Button)(target);
                    #line 46 "..\..\..\ViewPage\SignalDemo.xaml"
                    ((global::Windows.UI.Xaml.Controls.Button)this.Right).Click += this.Right_Click;
                    #line default
                }
                break;
            case 4:
                {
                    this.img = (global::Windows.UI.Xaml.Controls.Image)(target);
                    #line 19 "..\..\..\ViewPage\SignalDemo.xaml"
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

