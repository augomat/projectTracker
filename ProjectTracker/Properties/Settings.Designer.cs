﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Dieser Code wurde von einem Tool generiert.
//     Laufzeitversion:4.0.30319.42000
//
//     Änderungen an dieser Datei können falsches Verhalten verursachen und gehen verloren, wenn
//     der Code erneut generiert wird.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ProjectTracker.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "16.5.0.0")]
    public sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0")]
        public int WorktimebreakLeftSecs {
            get {
                return ((int)(this["WorktimebreakLeftSecs"]));
            }
            set {
                this["WorktimebreakLeftSecs"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("10")]
        public int countAsWorktimebreakMins {
            get {
                return ((int)(this["countAsWorktimebreakMins"]));
            }
            set {
                this["countAsWorktimebreakMins"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("2")]
        public int carryOverWorktimeCountHours {
            get {
                return ((int)(this["carryOverWorktimeCountHours"]));
            }
            set {
                this["carryOverWorktimeCountHours"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::System.DateTime lastAppExit {
            get {
                return ((global::System.DateTime)(this["lastAppExit"]));
            }
            set {
                this["lastAppExit"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool flagFinishWTDay {
            get {
                return ((bool)(this["flagFinishWTDay"]));
            }
            set {
                this["flagFinishWTDay"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool flagAutoFinishWT {
            get {
                return ((bool)(this["flagAutoFinishWT"]));
            }
            set {
                this["flagAutoFinishWT"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("10:00")]
        public string maxWorktime {
            get {
                return ((string)(this["maxWorktime"]));
            }
            set {
                this["maxWorktime"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool flagConsiderOvertime {
            get {
                return ((bool)(this["flagConsiderOvertime"]));
            }
            set {
                this["flagConsiderOvertime"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string timeularAPIkey {
            get {
                return ((string)(this["timeularAPIkey"]));
            }
            set {
                this["timeularAPIkey"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string timeularAPIsecret {
            get {
                return ((string)(this["timeularAPIsecret"]));
            }
            set {
                this["timeularAPIsecret"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("C:\\Users\\kapeller\\Documents\\DesktopTimes.csv")]
        public string OutputCsvFilePath {
            get {
                return ((string)(this["OutputCsvFilePath"]));
            }
            set {
                this["OutputCsvFilePath"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("C:\\Users\\Georg\\AppData\\Roaming\\Dexpot\\dexpot.log")]
        public string DexbotLogFilePath {
            get {
                return ((string)(this["DexbotLogFilePath"]));
            }
            set {
                this["DexbotLogFilePath"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(@"<?xml version=""1.0"" encoding=""utf-16""?>
<ArrayOfString xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
  <string>AL-Orga</string>
  <string>Customer</string>
  <string>KVP</string>
  <string>Micromanagement</string>
  <string>Other</string>
</ArrayOfString>")]
        public global::System.Collections.Specialized.StringCollection AvailableProjects {
            get {
                return ((global::System.Collections.Specialized.StringCollection)(this["AvailableProjects"]));
            }
            set {
                this["AvailableProjects"] = value;
            }
        }
    }
}
