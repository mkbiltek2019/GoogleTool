﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace GoogleTool.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "15.0.1.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("My GPlus")]
        public string APP_NAME {
            get {
                return ((string)(this["APP_NAME"]));
            }
            set {
                this["APP_NAME"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("224026233474-aoekci2lv3beb3fu9in4u1iqs9i32qi3.apps.googleusercontent.com")]
        public string CLIENT_ID {
            get {
                return ((string)(this["CLIENT_ID"]));
            }
            set {
                this["CLIENT_ID"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("VS2420KZJl2-3BXkhwwLf5nM")]
        public string CLIENT_SECRET {
            get {
                return ((string)(this["CLIENT_SECRET"]));
            }
            set {
                this["CLIENT_SECRET"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("224026233474-ne7tju6tut6om80j7e07d16kpvouvugk.apps.googleusercontent.com")]
        public string CLIENT_WEB_ID {
            get {
                return ((string)(this["CLIENT_WEB_ID"]));
            }
            set {
                this["CLIENT_WEB_ID"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("LjddkvK0s2CPdRlPayZKIYjN")]
        public string CLIENT_WEB_SECRET {
            get {
                return ((string)(this["CLIENT_WEB_SECRET"]));
            }
            set {
                this["CLIENT_WEB_SECRET"] = value;
            }
        }
    }
}