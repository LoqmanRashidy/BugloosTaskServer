
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using NuGet.Packaging.Signing;
using CommonLayer;

namespace DataLayer.Context
{
    
    public class DbSchemaInitializer : IDisposable
    {
        private DbContext _dbContext = null;

        public DbSchemaInitializer(DbContext dbContext)
        {
            if (dbContext == null)
                throw new ArgumentNullException("dbContext:DbContext cannot be null!");
            else
                this._dbContext = dbContext;
        }


        /// Update the descriptions of Table and its columns from Code first model's DescriptionAttribute
        public void UpdateTableColDescriptions()
        {
            var context = this._dbContext;

            var dbsetProps = context.GetDbSetProperties();
            dbsetProps.ForEach(prop =>
            {
                #region Get DAO type
                //Get DbSet's model. For example, DbSet<MyModel> => MyModel
                Type typeArgument = prop.PropertyType.GetGenericArguments()[0];
                #endregion

                #region Get Table description
                string tableName = string.Empty, desc = string.Empty, schemaName = string.Empty;
                Object tableNameAttr = this.getTableAttribute(typeArgument, "TableAttribute");
                Object descAttr = this.getTableAttribute(typeArgument, "CustomDescriptionAttribute");
                if (tableNameAttr != null)
                {
                    tableName = (tableNameAttr as System.ComponentModel.DataAnnotations.Schema.TableAttribute).Name;
                    schemaName = GetSchema(context, tableName);

                }

                if (descAttr != null)
                    desc = (descAttr as System.ComponentModel.DescriptionAttribute).Description;

                if (!string.IsNullOrEmpty(tableName) && !string.IsNullOrEmpty(desc))
                {
                    //Sync to database
                    this.syncTableDescription(context, schemaName, tableName, desc);
                }

                #endregion

                #region Get Columns description
                if (!string.IsNullOrEmpty(tableName))
                {
                    List<string> cols = this.getColsFromTable(context, tableName);
                    if (cols != null)
                    {
                        
                        var methodProp = typeof(AttributeUtility).GetMethod("GetPropertyAttributes");
                        cols.ForEach(col =>
                        {
                            List<Object> propDescAttrs = this.getPropAttribute(typeArgument, col, "CustomDescriptionAttribute");
                            if (col == "Code")
                            {
                                this.syncColIdentity(context, schemaName, tableName, col,99);
                            }
                            if (propDescAttrs != null && propDescAttrs.Count > 0)
                            {
                                string colDesc = (propDescAttrs[0] as System.ComponentModel.DescriptionAttribute).Description;
                                Debug.WriteLine($"{tableName}.{col} = {colDesc}");

                                //add Identity to database

                               

                                //Sync to database
                                this.syncColDescription(context, schemaName, tableName, col, colDesc);
                            }
                        });
                    }

                }
                #endregion

            });
            context.SaveChanges();
        }

        private string GetSchema(DbContext context, string tableName)
        {
            var test = context.GetDbSetProperties();
            string schema = null;
            List<string> result = new List<string>();
            var entityTypes = context.Model.GetEntityTypes();

            foreach (var item in entityTypes)
            {
                if (item.ClrType.Name == tableName)
                {
                    var name = item.Name.Split('.');
                    //var prop = name[name.Length - 1].GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                    schema = name[name.Length - 2];
                }
            }
            return schema;
        }

        /// Get attribute of class
        private object getTableAttribute(Type typeArgument, string attribute)
        {
            var method = typeof(AttributeUtility).GetMethod("GetClassAttributes");
            var generic = method.MakeGenericMethod(typeArgument);
            var result = generic.Invoke(null, new object[] { false });
            var dics = (result as Dictionary<string, object>);

            Object value = null;
            if (dics.TryGetValue(attribute, out value))
                return value;
            else
                return null;
        }


        /// Get attribute of property
        private List<object> getPropAttribute(Type typeArgument, string propName, string attribute)
        {
            var method = typeof(AttributeUtility).GetMethod("GetPropertyAttributes");
            var generic = method.MakeGenericMethod(typeArgument);
            var result = generic.Invoke(null, new object[] { propName, true });
            var dics = (result as Dictionary<string, List<object>>);

            List<Object> values = null;
            if (dics.TryGetValue(attribute, out values))
                return values;
            else
                return null;
        }

        /// Get all Column names of a Table
        private List<string> getColsFromTable(DbContext context, string tableName)
        {
            var test = context.GetDbSetProperties();
            List<string> result = new List<string>();
            var entityTypes = context.Model.GetEntityTypes();

            foreach (var item in entityTypes)
            {
                if (item.ClrType.Name == tableName)
                {
                   
                    //var prop = name[name.Length - 1].GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                    var prop= item.ClrType.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                    foreach (var item2 in prop)
                    {
                        var temp = item2.Name.Split('>');
                        if (temp[0].Contains("Persian"))
                        {
                            continue;
                        }
                        else
                        {
                            result.Add(temp[0].Substring(1).ToString());
                        }

                    }

                }
            }
            
            return result;

        }

        /// Use sp_addextendedproperty/sp_updateextendedproperty to update the Description of a Table
        private void syncTableDescription(DbContext context, string SchemaName, string tableName, string description)
        {
         
            string sql = string.Empty;
            var connection = context.Database.GetDbConnection();
            try
            {
                sql = $@"
    EXEC sp_updateextendedproperty   
     @name = N'MS_Description',  
     @value = N'{description}',  
     @level0type = N'Schema', @level0name = {SchemaName},  
     @level1type = N'Table',  @level1name = {tableName}";
                //rslt = context.Database.ExecuteSqlCommand(sql);
                //             string conString = Microsoft
                //.Extensions
                //.Configuration
                //.ConfigurationExtensions
                //.GetConnectionString(this.Configuration, "DefaultConnection");
                
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = sql;
                    var result = command.ExecuteNonQuery();
                }
                connection.Close();

            }
            catch
            {
                connection.Close();
                sql = $@"
    EXEC sp_addextendedproperty   
     @name = N'MS_Description',  
     @value = N'{description}',  
     @level0type = N'Schema', @level0name = {SchemaName},  
     @level1type = N'Table',  @level1name = {tableName}";
                //context.Database.ExecuteSqlCommand(sql);

                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = sql;
                    var result = command.ExecuteNonQuery();
                }
                connection.Close();
            }

        }


        /// Use sp_addextendedproperty/sp_updateextendedproperty to update the Description of a Column
        private void syncColDescription(DbContext context, string SchemaName, string tableName, string colName, string description)
        {
            var connection = context.Database.GetDbConnection();
            string sql = string.Empty;
            try
            {
                sql = $@"
EXEC sp_updateextendedproperty   
    @name = N'MS_Description'  
    ,@value = N'{description}'  
    ,@level0type = N'Schema', @level0name = {SchemaName}
    ,@level1type = N'Table',  @level1name = {tableName}
    ,@level2type = N'Column', @level2name = {colName}";

                //using (var connection = context.Database.GetDbConnection())
                //{
                //    connection.Open();
                //    using (var command = connection.CreateCommand())
                //    {
                //        command.CommandText = sql;
                //        var result = command.ExecuteNonQuery();
                //    }
                //}

              

                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = sql;
                    var result = command.ExecuteNonQuery();
                }
                connection.Close();

            }
            catch
            {
                connection.Close();
                sql = $@"
EXEC sp_addextendedproperty   
    @name = N'MS_Description'  
    ,@value = N'{description}'  
    ,@level0type = N'Schema', @level0name = {SchemaName} 
    ,@level1type = N'Table',  @level1name = {tableName}
    ,@level2type = N'Column', @level2name = {colName}";

                //using (var connection = context.Database.GetDbConnection())
                //{
                //    connection.Open();
                //    using (var command = connection.CreateCommand())
                //    {
                //        command.CommandText = sql;
                //        var result = command.ExecuteNonQuery();
                //    }
                //}
                

                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = sql;
                    var result = command.ExecuteNonQuery();
                }
                connection.Close();
            }
        }

        private void syncColIdentity(DbContext context, string SchemaName, string tableName, string colName, int seed)
        {
            var connection = context.Database.GetDbConnection();
            string sql = string.Empty;
            try
            {
                sql = $@"
                     CREATE  TRIGGER {SchemaName}.{tableName}_InsertCode  
                    ON {SchemaName}.{tableName} 
                    For INSERT   
                    AS 
                    DECLARE  @code bigint ; 
                     set @code= (SELECT TOP(1) Code FROM {SchemaName}.{tableName} ORDER BY 1 DESC);
                       if(@code=0)
                           set @code = {seed}
                        update {SchemaName}.{tableName} set Code=@code +1
                       FROM {SchemaName}.{tableName}
                       JOIN inserted
                       ON {SchemaName}.{tableName}.Id = inserted.Id

                      ";
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = sql;
                    var result = command.ExecuteNonQuery();
                }
                connection.Close();

            }
            catch
            {
                connection.Close();


                sql = $@"
                     ALTER  TRIGGER {SchemaName}.{tableName}_InsertCode  
                    ON {SchemaName}.{tableName} 
                    For INSERT   
                    AS 
                    DECLARE  @code bigint ; 
                     set @code= (SELECT TOP(1) Code FROM {SchemaName}.{tableName} ORDER BY 1 DESC);
                       if(@code=0)
                            set @code = {seed}
                        update {SchemaName}.{tableName} set Code=@code +1
                       FROM {SchemaName}.{tableName}
                       JOIN inserted
                       ON {SchemaName}.{tableName}.Id = inserted.Id

                      ";

                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = sql;
                    var result = command.ExecuteNonQuery();
                }
                connection.Close();
            }
        }
        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            if (this._dbContext != null)
            {
                this._dbContext.Dispose();
            }
        }
    }
}
