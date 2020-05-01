using System;
using System.IO;
using System.Linq;
using System.Runtime.Caching;

namespace Domainr.EventStore.Data
{
    public sealed class SqlStatementsLoader
        : ISqlStatementsLoader
    {
        private readonly ObjectCache _cache = MemoryCache.Default;

        ////private readonly IReadOnlyDictionary<string, string> _sqlStatements;

        public SqlStatementsLoader()
        {
            ////var currentDirPath = Directory.GetCurrentDirectory();

            ////var sqlFilesPath = Path.Combine(currentDirPath, "Data\\SqlScripts");

            ////var sqlFilesDir = new DirectoryInfo(sqlFilesPath);
            ////if (!sqlFilesDir.Exists)
            ////{
            ////    return;
            ////}

            ////_sqlStatements = sqlFilesDir
            ////    .GetFiles()
            ////    .ToDictionary(key => key.Name.Split('.')[0], value => File.ReadAllText(value.FullName));
        }

        ////public string this[string key] => key.Contains("Async")
        ////        ? _sqlStatements[key.Substring(0, key.Length - 5)]
        ////        : _sqlStatements[key];

        public string this[string key]
        {
            get
            {
                const string ASYNC_SUFFIX = "async";

                var fileName = key.ToLower().EndsWith(ASYNC_SUFFIX)
                    ? key.Substring(0, key.Length - ASYNC_SUFFIX.Length)
                    : key;

                var fileContent = _cache[fileName];
                if (fileContent != null)
                {
                    return fileContent.ToString();
                }

                var currentDirPath = Directory.GetCurrentDirectory();

                var sqlFilesPath = Path.Combine(currentDirPath, @"Data\SqlScripts");

                var sqlFilesDir = new DirectoryInfo(sqlFilesPath);
                if (!sqlFilesDir.Exists)
                {
                    return null;
                }

                var file = sqlFilesDir
                    .GetFiles()
                    .SingleOrDefault(sqlFile => sqlFile.Name.Split('.')[0] == fileName);

                if (file == null)
                {
                    return null;
                }

                fileContent = File.ReadAllText(file.FullName);

                _cache.Set(key, fileContent, DateTimeOffset.UtcNow.AddMinutes(60)); // TODO: Add expiration to configuration

                return fileContent.ToString();
            }
        }
    }
}