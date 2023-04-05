using System;
using System.Data.SqlClient;
using api.logging;


namespace Nippon.CommonLib.Models
{
    public class NipponConnectionString
    {
        const string SEPARATOR = "<>";
        public string ConnectionString { get; set; } = string.Empty;

        public NipponConnectionString(string initialCatalog, string registryConnection)
        {
            LogEvent log = LogEvent.Start();
            try
            {
                this.init(registryConnection);
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
                builder.IntegratedSecurity = this.authType.Equals("1");
                builder.UserID = this.userName;
                builder.Password = this.password;
                builder.Pooling = this.isPooling;
                builder.MaxPoolSize = this.maxPool;
                builder.MinPoolSize = this.minPool;
                builder.DataSource = string.IsNullOrEmpty(this.instance) ? this.server : string.Format(@"{0}\{1}", this.server, this.instance);
                builder.DataSource = this.port > 0 ? string.Format("{0},{1}", builder.DataSource, this.port) : builder.DataSource;
                builder.Encrypt = true;
                builder.TrustServerCertificate = true;
                builder.InitialCatalog = initialCatalog;

                if (builder.IntegratedSecurity)
                {
                    this.ConnectionString = builder.ConnectionString;
                    this.ConnectionString = this.ConnectionString.Replace("Integrated Security=True", "Integrated Security=SSPI");
                    this.ConnectionString = this.ConnectionString.Replace("User ID=;Password=;", "");
                }
                else
                {
                    this.ConnectionString = builder.ConnectionString;
                }

                this.ConnectionString = string.Format("{0};Connection Lifetime={1}", this.ConnectionString, this.timeoutSeconds);
            }
            catch (Exception ex)
            {
                log.LogError(ex);
            }
            finally
            {
                log.Exit();
            }
        }
        
        public string authType { get; set; }
        public string password { get; set; }
        public string userName { get; set; }
        public string server { get; set; }
        public string instance { get; set; }
        public int port { get; set; }
        public bool isPooling { get; set; }
        public int timeoutSeconds { get; set; }
        public int maxPool { get; set; }
        public int minPool { get; set; }

        private void init(string connStr)
        {
            string[] parts = connStr.Split(new string[] { SEPARATOR }, StringSplitOptions.None);
            this.authType = parts.getValue<string>(0);
            this.password = parts.getValue<string>(3);
            this.userName = parts.getValue<string>(4);
            this.server = parts.getValue<string>(5);
            this.instance = parts.getValue<string>(6);
            this.port = parts.getValue<int>(7);
            this.isPooling = parts.getValue<bool>(8);
            this.timeoutSeconds = parts.getValue<int>(9);
            this.maxPool = parts.getValue<int>(10);
            this.minPool = parts.getValue<int>(11);
        }
    }
}
