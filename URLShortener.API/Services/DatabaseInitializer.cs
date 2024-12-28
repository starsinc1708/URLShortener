using Npgsql;
using Microsoft.Data.SqlClient;
using System;
using System.Threading.Tasks;
using Dapper;

namespace URLShortener.API.Services
{
	public class PostgresDatabaseInitializer(
		NpgsqlDataSource dataSource,
		IConfiguration configuration,
		ILogger<PostgresDatabaseInitializer> logger) : BackgroundService
	{
		protected override async Task ExecuteAsync(CancellationToken cancellationToken)
		{
			try
			{
				await CreateDatabaseIfNotExists(cancellationToken);
				await InitializeSchema(cancellationToken);

				logger.LogInformation("Database initialization completed succesfully.");
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Error while initializing database.");
				throw;
			}
		}

		private async Task CreateDatabaseIfNotExists(CancellationToken cancellationToken)
		{
			var connectionString = configuration.GetConnectionString("url-shortener");
			var builder = new NpgsqlConnectionStringBuilder(connectionString);
			string? databaseName = builder.Database;
			builder.Database = "postgres";

			await using var connection = new NpgsqlConnection(builder.ConnectionString);
			await connection.OpenAsync(cancellationToken);

			bool dbExists = await connection.ExecuteScalarAsync<bool>(
				"SELECT EXISTS (SELECT 1 FROM pg_database WHERE datname = @databaseName)",
				new { databaseName });

			if (dbExists) return;
			
			logger.LogInformation($"Creating database {databaseName}");
			await connection.ExecuteAsync($"CREATE DATABASE \"{databaseName}\"");
		}

		private async Task InitializeSchema(CancellationToken cancellationToken)
		{
			const string createTablesSql =
				"""
				CREATE TABLE IF NOT EXISTS shortened_url (
					id SERIAL PRIMARY KEY,
					short_code VARCHAR(10) UNIQUE NOT NULL,
					original_url TEXT NOT NULL,
					created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
				);
				CREATE INDEX IF NOT EXISTS idx_short_code ON shortened_url(short_code);

				CREATE TABLE IF NOT EXISTS url_visits (
					id SERIAL PRIMARY KEY,
					short_code VARCHAR(10) NOT NULL,
					visited_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
					user_agent TEXT,
					referer TEXT,
					FOREIGN KEY (short_code) REFERENCES shortened_url(short_code)
				);
				CREATE INDEX IF NOT EXISTS idx_visits_short_code ON url_visits(short_code);
				""";

			await using var command = dataSource.CreateCommand(createTablesSql);
			await command.ExecuteNonQueryAsync(cancellationToken);
		}
	}
}
