﻿#pragma checksum "E:\XiYouFastPass\SocketClientDemo\SocketClientDemo\ViewPage\PPTView.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "92350AA497F872CE126CA299D829F94E"
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
    partial class PPTView : 
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
                    this.content1 = (global::Windows.UI.Xaml.Controls.RelativePanel)(target);
                }
                break;
            case 3:
                {
                    this.previous = (global::Windows.UI.Xaml.Controls.Button)(target);
                    #line 23 "..\..\..\ViewPage\PPTView.xaml"
                    ((global::Windows.UI.Xaml.Controls.Button)this.previous).Tapped += this.previous_Tapped;
                    #line default
                }
                break;
            case 4:
                {
                    this.next = (global::Windows.UI.Xaml.Controls.Button)(target);
                    #line 28 "..\..\..\ViewPage\PPTView.xaml"
                    ((global::Windows.UI.Xaml.Controls.Button)this.next).Tapped += this.next_Tapped;
                    #line default
                }
                break;
            case 5:
                {
                    this.first = (global::Windows.UI.Xaml.Controls.Button)(target);
                    #line 39 "..\..\..\ViewPage\PPTView.xaml"
                    ((global::Windows.UI.Xaml.Controls.Button)this.first).Tapped += this.first_Tapped;
                    #line default
                }
                break;
            case 6:
                {
                    this.last = (global::Windows.UI.Xaml.Controls.Button)(target);
                    #line 42 "..\..\..\ViewPage\PPTView.xaml"
                    ((global::Windows.UI.Xaml.Controls.Button)this.last).Tapped += this.last_Tapped;
                    #line default
                }
                break;
            case 7:
                {
                    this.close = (global::Windows.UI.Xaml.Controls.Button)(target);
                    #line 45 "..\..\..\ViewPage\PPTView.xaml"
                    ((global::Windows.UI.Xaml.Controls.Button)this.close).Tapped += this.shutdown_Tapped;
                    #line default
                }
                break;
            case 8:
                {
                    this.img = (global::Windows.UI.Xaml.Controls.Image)(target);
                    #line 18 "..\..\..\ViewPage\PPTView.xaml"
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

