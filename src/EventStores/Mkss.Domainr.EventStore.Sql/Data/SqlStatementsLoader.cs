using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Caching;

namespace Domainr.EventStore.Sql.Data
{
    public sealed class SqlStatementsLoader
        : ISqlStatementsLoader
    {
        private IReadOnlyDictionary<string, string> _sqlStatements;

        public void LoadScripts()
        {
            var currentDirPath = Directory.GetCurrentDirectory();

            var sqlFilesPath = Path.Combine(currentDirPath, "Data\\SqlScripts");

            var sqlFilesDir = new DirectoryInfo(sqlFilesPath);
            if (!sqlFilesDir.Exists)
            {
                return;
            }

            _sqlStatements = sqlFilesDir
                .GetFiles()
                .Where(file => file.Extension.Equals(".sql"))
                .ToDictionary(
                    key => key.Name.Split('.')[0],
                    value => File.ReadAllText(value.FullName));
        }

        public string this[string key] => key.ToLower().Contains("async")
                ? _sqlStatements[key.Substring(0, key.Length - 5)]
                : _sqlStatements[key];
    }
}