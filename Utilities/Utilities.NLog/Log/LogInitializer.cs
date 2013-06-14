using System;
using System.Threading.Tasks;
using NLog;
using NLog.Config;
using NLog.Layouts;
using NLog.Targets;
using NLog.Targets.Wrappers;

namespace Utilities
{
    public class LogInitializer
    {
        public const string Separator = "--------------------------------------------------------------------------------";

        public LogInitializer(string logPath, string filenameInfo, string filenameError)
        {
            Task.Factory.StartNew(() => Init(logPath, filenameInfo, filenameError));
        }

        public static void Init(string logPath, string filenameInfo, string filenameError)
        {
            LogManager.Configuration = BuildConfiguration(logPath, filenameInfo, filenameError);
        }

        private static LoggingConfiguration BuildConfiguration(string logPath, string filenameInfo, string filenameError)
        {
            LoggingConfiguration config = new LoggingConfiguration();

#if DEBUG
            Target infoFileTarget = BuildInfoFileTarget(logPath, filenameInfo);
            config.AddTarget("infoFileTarget", infoFileTarget);
            config.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, infoFileTarget));
#else
			Target infoFileTarget = BuildInfoFileTarget(logPath, filenameInfo);
			config.AddTarget("infoFileTarget", infoFileTarget);
			config.LoggingRules.Add(new LoggingRule("*", LogLevel.Info, infoFileTarget));
#endif

            Target instantFileTarget = BuildInstantFileTarget(logPath, filenameError);
            config.AddTarget("instantFileTarget", instantFileTarget);
            config.LoggingRules.Add(new LoggingRule("*", LogLevel.Error, instantFileTarget));

            return config;
        }

        private static Target BuildInfoFileTarget(string logPath, string filename)
        {
            FileTarget target = new FileTarget();

            target.FileName = string.Concat(logPath, filename);
            target.AutoFlush = false;
            target.Layout = BuildLayout();
            target.ArchiveFileName = string.Concat(logPath, @"archives\log.{#####}.", filename);
            target.ArchiveEvery = FileArchivePeriod.Month;
            target.ArchiveNumbering = ArchiveNumberingMode.Sequence;
            target.NetworkWrites = true;

            BufferingTargetWrapper wrapper = new BufferingTargetWrapper(target);

            return wrapper;
        }

        private static Target BuildInstantFileTarget(string logPath, string filename)
        {
            FileTarget fileTarget = new FileTarget();

            fileTarget.FileName = string.Concat(logPath, filename);
            fileTarget.AutoFlush = true;
            fileTarget.Layout = BuildLayout();

            return fileTarget;
        }

        private static LayoutWithHeaderAndFooter BuildLayout()
        {
            LayoutWithHeaderAndFooter layout = new LayoutWithHeaderAndFooter();
            Guid guid = Guid.NewGuid();

            layout.Header = string.Concat(Separator, "${newline}Guid: ", guid.ToString(), "${newline}${date:format=dddd dd MMMM yyyy}${newline}${date:format=T}${newline}Login: ${windows-identity}${newline}Machine: ${machinename}${newline}Version: ${gdc:item=version}${newline}", Separator);
            //layout.Layout = "${level:uppercase=true} ${logger}: ${message}${onexception:inner=${newline}${exception:format=tostring}}";
            layout.Layout = "${message}${onexception:inner=${newline}${exception:format=tostring}}";
            layout.Footer = string.Concat(Separator, "${newline}", guid.ToString(), " - ${date:format=T} - ${qpc}s${newline}", Separator);

            return layout;
        }
    }
}