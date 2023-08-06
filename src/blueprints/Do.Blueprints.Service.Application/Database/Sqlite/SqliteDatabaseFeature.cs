﻿using Do.Architecture;
using Do.Configuration;
using FluentNHibernate.Cfg.Db;
using Microsoft.Extensions.DependencyInjection;

namespace Do.Database.Sqlite;

public class SqliteDatabaseFeature : IFeature
{
    readonly Setting<string> _fileName;

    public SqliteDatabaseFeature(Setting<string> fileName) =>
        _fileName = fileName;

    string FullFilePath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), _fileName);

    public void Configure(LayerConfigurator configurator)
    {
        configurator.ConfigureServiceCollection(services =>
        {
            services.AddSingleton<ITransaction, FlatTransaction>();
        });

        configurator.ConfigurePersistence(persistence =>
        {
            var sqlite = SQLiteConfiguration.Standard.UsingFile(FullFilePath);

            sqlite.ShowSql();
            sqlite.MaxFetchDepth(1);

            persistence.Configurer = sqlite;
            persistence.AutoUpdateSchema = true;
        });

        configurator.ConfigureMiddlewareCollection(middlewares =>
        {
            middlewares.Add<FlatTransactionMiddleware>();
        });
    }
}