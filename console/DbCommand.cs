using System;
using System.Data.SqlClient;
using ManyConsole;
using model;
using NDesk.Options;

namespace console {
	public abstract class DbCommand : ConsoleCommand {

		protected string Server { get; set; }
        protected string FailoverPartner { get; set; }
		protected string DbName { get; set; }
		protected string User { get; set; }
		protected string Pass { get; set; }
		protected string ScriptDir { get; set; }
		protected bool Overwrite { get; set; }

		protected DbCommand(string command, string oneLineDescription) {
			IsCommand(command, oneLineDescription);
			Options = new OptionSet();
			SkipsCommandSummaryBeforeRunning();
			HasRequiredOption("s|server=", "server", o => Server = o);
            HasOption("f|failoverpartner=", "failover partner", o => FailoverPartner = o);
			HasRequiredOption("b|database=", "database", o => DbName = o);
			HasOption("u|user=", "user", o => User = o);
			HasOption("p|pass=", "pass", o => Pass = o);
			HasRequiredOption(
				"d|scriptDir=",
				"Path to database script directory.",
				o => ScriptDir = o);
			HasOption(
				"o|overwrite=",
				"Overwrite existing target without prompt.",
				o => Overwrite = o != null);
		}

		protected Database CreateDatabase() {
			var builder = new SqlConnectionStringBuilder() {
				DataSource = Server,
				InitialCatalog = DbName,
				IntegratedSecurity = String.IsNullOrEmpty(User)
			};
            if (!string.IsNullOrEmpty(this.FailoverPartner))
                builder.FailoverPartner = FailoverPartner;

			if (!builder.IntegratedSecurity){
				builder.UserID = User;
				builder.Password = Pass;
			}
			return new Database() {
				Connection = builder.ToString(),
				Dir = ScriptDir
			};
			
		}
    }
}
