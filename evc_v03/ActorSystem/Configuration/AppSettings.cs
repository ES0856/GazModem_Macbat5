using Akka.Routing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json.Serialization;

namespace evc_v03.ActorSystem.Configuration
{
    internal class AppSettings
    {
        public ActorSystemSettings ActorSystem { get; set; } = new();
        public LoggingSettings Logging { get; set; } = new();
        public ServerSettings Server { get; set; } = new();
        public DeviceSettings Device { get; set; } = new();
        public ProtocolSettings Protocol { get; set; } = new();
        public MessageBrokers MessageBroker { get; set; } = new();

        public CacheSettings Cache { get; set; } = new();
        public DatabaseSettings Database { get; set; } = new();


    }
    public class ActorSystemSettings
    {
        public string Name { get; set; } = "EvcListenerSystem";
        public int MaxMessagesPerActor { get; set; } = 1000;
        public int MaxActors { get; set; } = 10000;
        public TimeSpan ActorIdleTimeout { get; set; } = TimeSpan.FromMinutes(30);
        public TimeSpan ActorShutdownTimeout { get; set; } = TimeSpan.FromSeconds(30);
        public TimeSpan ActorRestartDelay { get; set; } = TimeSpan.FromSeconds(5);
        public int MaxRetries { get; set; } = 3;
        public TimeSpan RetryDelay { get; set; } = TimeSpan.FromSeconds(2);
    }
    public class CacheSettings
    {
        public string ConnectionString { get; set; } = string.Empty;
        public int Database { get; set; } = 0;
        public string KeyPrefix { get; set; } = "evc_listener:";
        public int ExpiryMinutes { get; set; } = 60;
    }

    public class DatabaseSettings
    {

    }
    public class LoggingSettings
    {
        public bool LogEnabled { get; set; } = true;
        public string LogLevel { get; set; } = "info";
        public bool FileLoggingEnabled { get; set; } = true;
        public string LogFilePath { get; set; } = "./logs";

        public bool ConsoleLoggingEnabled { get; set; } = true;

        public ConsoleLogging Console { get; set; } = new();

        public FileLogging File { get; set; } = new();
    }

    public class ConsoleLogging
    {
        public bool Enabled { get; set; } = true;
        public string MinLevel { get; set; } = "Info";
        public bool UseColors { get; set; } = true;
        public string OutputTemplate { get; set; } =
            "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}";
    }
    public class FileLogging
    {
        public bool Enabled { get; set; } = true;
        public string MinLevel { get; set; } = "Info";
        public string BasePath { get; set; } = "./logs";
        public string FileNameFormat { get; set; } = "evc-listener-{Date}.log";
        public string OutputTemplate { get; set; } =
            "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] [{Level:u3}] {Message:lj}{NewLine}{Exception}";
        public string RollingInterval { get; set; } = "Day";
        public int RetainedFileCountLimit { get; set; } = 14;
        public bool RollOnFileSizeLimit { get; set; } = true;
        public long FileSizeLimitBytes { get; set; } = 10 * 1024 * 1024; // 10 MB
        public int BufferSize { get; set; } = 0;                 // 0 = auto-flush
    }
    public class ServerSettings
    {
        public TcpServerConfig TcpServerConfig { get; set; } = new();

    }
    /// <summary>
    /// Selects which IP stack the TCP listener binds to.
    /// Deserialised from JSON as a string ("IPv4", "IPv6", "DualStack")    
    /// <see cref="System.Text.Json.Serialization.JsonStringEnumConverter"/>
    /// </summary>

    public enum IpMode
    {
        IPv4,
        IPv6,
        DualStack


    }
    public class TcpServerConfig
    {
        public bool Enabled { get; set; } = true;
        public int Port { get; set; } = 9000;
        public IpMode Mode { get; set; } = IpMode.IPv4;
        public int Backlog { get; set; } = 100;
        public int ReceiveBufferSize { get; set; } = 8192;
        public int SendBufferSize { get; set; } = 8192;
        public bool NoDelay { get; set; } = true;
        public bool KeepAlive { get; set; } = true;

        public int MaxConnections { get; set; } = 1000;
        public string ListenIP { get; set; } = string.Empty;

        public string IPAddress
        {
            get => string.IsNullOrWhiteSpace(ListenIP) ? GetDefaultBindAddress() : ListenIP;
            set => ListenIP = value;
        }
        private string GetDefaultBindAddress()
        {
            return Mode == IpMode.IPv4 ? "0.0.0.0" : "::";
        }
    }

    public class DeviceSettings
    {
        public string Timezone { get; set; } = "UTC";
        public string SharedDeviceKey { get; set; } = "default_shared_key";
        public string SharedDpVersion { get; set; } = "5.0";
    }


    public class ProtocolSettings
    {
        public DeviceProtocol DeviceProtocol { get; set; } = new();
        public PushModeSettings PushMode { get; set; } = new();
        public PullModeSettings PullMode { get; set; } = new();
    }

    public class DeviceProtocol
    {
        public string Name { get; set; } = "GAZ-MODEM";
        public string Version { get; set; } = "3.0";
        public byte FrameDelimiter { get; set; } = 126;
        public int MaxPayloadSize { get; set; } = 4096;
        public bool EnableCrcValidation { get; set; } = true;
        public bool StrictModeEnabled { get; set; } = true;
    }

    public class PushModeSettings
    {
        public bool Enabled { get; set; } = true;
        public int Timeout { get; set; } = 180_000;   // ms

        // BACKWARD COMPATIBILITY: Alias for Timeout
        [JsonIgnore]
        public int WaitForPushEndTimeout
        {
            get => Timeout;
            set => Timeout = value;
        }
    }

    public class PullModeSettings
    {
        public bool Enabled { get; set; } = true;
        public int DelayBeforePull { get; set; } = 15_000;   // ms
        public int Timeout { get; set; } = 600_000;  // ms

    }

    // -------------------------------------------------------------------------
    // Message brokers
    // -------------------------------------------------------------------------

    public class MessageBrokers
    {
        public RabbitMQSettings RabbitMQ { get; set; } = new();
    }

    public class RabbitMQSettings
    {
        public bool Enabled { get; set; } = false;
        public string Host { get; set; } = "localhost";
        public int Port { get; set; } = 5672;
        public string VirtualHost { get; set; } = "/";
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
        public string ExchangeName { get; set; } = "";
        public string ExchangeType { get; set; } = "topic";
        public int DeliveryMode { get; set; } = 2;  // 2 = persistent

        public RabbitMQQueues Queues { get; set; } = new();
    }

    public class RabbitMQQueues
    {
        public QueueConfig Schedule { get; set; } = new(); // Current/Instantaneous data
        public QueueConfig Alarms { get; set; } = new(); // The grouped Alarm data that includes log+limits
        public QueueConfig RegisteredData { get; set; } = new(); //Successive Periodic Archives
        public QueueConfig Events { get; set; } = new();

        // ===== NEW: PULL REQUEST QUEUE (incoming from MDM) =====
        public QueueConfig PullRequest { get; set; } = new();

        // ===== NEW: ARCHIVE RESPONSE QUEUES (outgoing to MDM) =====
        public QueueConfig PeriodicArchivie { get; set; } = new();
        public QueueConfig HourlyArchive { get; set; } = new();
        public QueueConfig DailyArchive { get; set; } = new();
        public QueueConfig MonthlyArchive { get; set; } = new();
        public QueueConfig MomentaryArchive { get; set; } = new();
        public QueueConfig BillingArchive { get; set; } = new();
        public QueueConfig SetupLog { get; set; } = new();
        public QueueConfig TimeLog { get; set; } = new();
        public QueueConfig GasLog { get; set; } = new();

        public QueueConfig EventsResponse { get; set; } = new();
    }

    public class QueueConfig
    {
        public string QueueName { get; set; } = string.Empty;
        public string RoutingKey { get; set; } = string.Empty;
    }
}