﻿#pragma checksum "E:\XiYouFastPass\SocketClientDemo\SocketClientDemo\ViewPage\DiskShow.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "B7CBFBFC11ADFB2D30141055B0246FF7"
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
    partial class DiskShow : 
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
                    this.disklistview = (global::Windows.UI.Xaml.Controls.ListView)(target);
                    #line 33 "..\..\..\ViewPage\DiskShow.xaml"
                    ((global::Windows.UI.Xaml.Controls.ListView)this.disklistview).SelectionChanged += this.disklistview_SelectionChanged;
                    #line default
                }
                break;
            case 2:
                {
                    this.pathname = (global::Windows.UI.Xaml.Controls.TextBlock)(target);
                }
                break;
            case 3:
                {
                    this.btn = (global::Windows.UI.Xaml.Controls.Button)(target);
                    #line 20 "..\..\..\ViewPage\DiskShow.xaml"
                    ((global::Windows.UI.Xaml.Controls.Button)this.btn).Click += this.btn_Click;
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

