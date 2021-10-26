using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Reflection;

namespace CommonLayer
{
    public static class ExtentionSqlMethod
    {

        public static List<PropertyInfo> GetDbSetProperties(this DbContext context)
        {
            List<PropertyInfo> dbSetProperties = new List<PropertyInfo>();
            PropertyInfo[] properties = context.GetType().GetProperties();

            foreach (PropertyInfo property in properties)
            {
                Type setType = property.PropertyType;
                bool isDbSet = setType.IsGenericType && (typeof(DbSet<>).IsAssignableFrom(setType.GetGenericTypeDefinition()) || setType.GetInterface(typeof(DbSet<>).FullName) != null);
                if (isDbSet)
                {
                    dbSetProperties.Add(property);
                }
            }

            return dbSetProperties;
        }

        public static List<T> ExecSQL<T>(this DbContext context, string query, ILogger logger = null)
        {
            return ExecSQL<T>(context, query, logger, null);
        }

        public static List<T> ExecSQL<T>(this DbContext context, string query, params SqlParameter[] sqlParams)
        {
            return ExecSQL<T>(context, query, null, sqlParams);
        }

        public static List<T> ExecSQL<T>(this DbContext context, string query, ILogger logger, params SqlParameter[] sqlParams)
        {

            context.Database.CloseConnection();
            var command = context.Database.GetDbConnection().CreateCommand();
            command.CommandText = query;
            command.CommandType = CommandType.Text;
            if (sqlParams != null) command.Parameters.AddRange(sqlParams);
            context.Database.OpenConnection();

            var result = command.ExecuteReader();
            var list = new List<T>();

            if (result != null)
            {
                var sw = new Stopwatch();
                sw.Start();

                var mapper = new DataReaderMapper<T>(result);
                while (result.Read())
                {
                    list.Add(mapper.MapFrom(result));
                }

                sw.Stop();
                logger?.LogInformation($"Executed ({sw.ElapsedMilliseconds}ms)");
                logger?.LogInformation($"{query}");

            }
            context.Database.CloseConnection();
            return list;



        }

    }
}
