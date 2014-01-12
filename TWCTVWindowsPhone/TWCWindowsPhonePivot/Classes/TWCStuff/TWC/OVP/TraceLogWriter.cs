namespace TWC.OVP
{
    using Microsoft.SilverlightMediaFramework.Plugins;
    using Microsoft.SilverlightMediaFramework.Plugins.Metadata;
    using Microsoft.SilverlightMediaFramework.Plugins.Primitives;
    using System;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using TWC.OVP.Framework;

    [ExportLogWriter(PluginName="DebugLogWriter", PluginDescription="Writes logging output to the debug window.", PluginVersion="2.2012.0214.0", LogWriterId="Debug")]
    public class TraceLogWriter : ILogWriter, IPlugin
    {
        private const string LogWriterId = "Debug";
        private const string PluginDescription = "Writes logging output to the debug window.";
        private const string PluginName = "DebugLogWriter";
        private const string PluginVersion = "2.2012.0214.0";

        public event System.Action<IPlugin, LogEntry> LogReady;

        public event System.Action<ILogWriter, LogEntry, Exception> LogWriteFailed;

        public event System.Action<ILogWriter, LogEntry> LogWriteSuccessful;

        public event Action<IPlugin> PluginLoaded;

        public event System.Action<IPlugin, Exception> PluginLoadFailed;

        public event Action<IPlugin> PluginUnloaded;

        public event System.Action<IPlugin, Exception> PluginUnloadFailed;

        public void Load()
        {
            this.IsLoaded = true;
            if (this.PluginLoaded != null)
            {
                this.PluginLoaded(this);
            }
        }

        public void Unload()
        {
            this.IsLoaded = false;
            if (this.PluginUnloaded != null)
            {
                this.PluginUnloaded(this);
            }
        }

        public void WriteLog(LogEntry logEntry)
        {
            try
            {
                Trace.WriteLine(string.Format("Severity: \"{0}\" Sender: \"{1}\" Message: \"{2}\" Timestamp: \"{3}\"", new object[] { logEntry.Severity, logEntry.SenderName, logEntry.Message, logEntry.Timestamp }));
                if (this.LogWriteSuccessful != null)
                {
                    this.LogWriteSuccessful(this, logEntry);
                }
            }
            catch (Exception exception)
            {
                if (this.LogWriteFailed != null)
                {
                    this.LogWriteFailed(this, logEntry, exception);
                }
            }
        }

        public bool IsLoaded { get; private set; }
    }
}

