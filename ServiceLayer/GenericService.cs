using Datalayer.Models;
using DataLayer.Context;
using DataLayer.ViewModels;
using EntityFramework.Audit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using ServiceStack.DataAnnotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer
{
    public interface IAsyncService<T> where T : BaseEntity
    {
        Task<T> FindByIdAsync(long id, ApplicationDbContext context= null);
        T FindById(long id, ApplicationDbContext context = null);
        List<T> FindCondition(Expression<Func<T, bool>> predicate, ApplicationDbContext context = null);
        Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, ApplicationDbContext context = null);
        Task<T> LastOrDefaultAsync(Expression<Func<T, bool>> predicate = null, ApplicationDbContext context = null);
        Task<T> AddAsync(T entity, ApplicationDbContext context = null);
        Task<T> AddOrUpdateAsync(T entity, ApplicationDbContext context = null);
        T AddOrUpdate(T entity, ApplicationDbContext context = null);
        Task<T> UpdateAsync(T entity, ApplicationDbContext context = null);
        Task<(bool IsSuccess, string childs)> RemoveAsync(long id, bool physically = false, ApplicationDbContext context = null);
        (bool IsSuccess, string childs) Remove(long id, bool physically = false, ApplicationDbContext context = null);
        Task<List<T>> FindAllAsync(ApplicationDbContext context = null);
        Task<List<T>> FindConditionAsync(Expression<Func<T, bool>> predicate, ApplicationDbContext context = null);
        Task<int> CountAllAsync(ApplicationDbContext context = null);
        Task<int> CountWhereAsync(Expression<Func<T, bool>> predicate, ApplicationDbContext context = null);
        Task<dynamic> FindPagingAsync(State state, ApplicationDbContext context = null);
        RawQuery RawSqlPagingQuery<T>(State state, ApplicationDbContext context = null);

    }

    public class GenericService<T> : IAsyncService<T> where T : BaseEntity
    {
 
        protected ApplicationDbContext _context;

        public GenericService(ApplicationDbContext context)
        {
            _context = context;
        }

        #region Methods

        public virtual async Task<List<T>> FindAllAsync(ApplicationDbContext context = null)
        {
            if (context == null) context = _context;
            //IEnumerable<string> list = context.GetIncludePaths(typeof(T));
            //Type type = typeof(T);
            //var query = context.Set<T>().Where(x => x.IsDelete == false).Include(list, null);
            IQueryable<T> query = context.Set<T>();
            return await query.ToListAsync();
        }

        public virtual async Task<dynamic> FindPagingAsync(State state, ApplicationDbContext context = null)
        {
            if (context == null) context = _context;
            RawQuery raw = RawSqlPagingQuery<T>(state);
            IQueryable<T> _lst = context.Set<T>().FromSqlRaw(raw.rawfilterpaging);

            return await _lst.ToListAsync();
        }
        public virtual async Task<T> FindByIdAsync(long id, ApplicationDbContext context = null)
        {
            if (context == null) context = _context;
            //var query = context.Set<T>().Where(x => x.IsDelete == false).Include(list, null);
            IQueryable<T> query = context.Set<T>();
            return await query.FirstOrDefaultAsync(x => x.Id == id);
        }

        public virtual T FindById(long id, ApplicationDbContext context = null)
        {
            if (context == null) context = _context;
            //var query = context.Set<T>().Where(x => x.IsDelete == false).Include(list, null);
            IQueryable<T> query = context.Set<T>();
            return query.FirstOrDefault(x => x.Id == id);
        }

        public virtual async Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, ApplicationDbContext context = null)
        {
            if (context == null) context = _context;
            var query=  await context.Set<T>().FirstOrDefaultAsync(predicate);
            return query;
        }

        public virtual async Task<T> LastOrDefaultAsync(Expression<Func<T, bool>> predicate = null, ApplicationDbContext context = null)
        {
            if (context == null) context = _context;
            var query=await context.Set<T>().OrderByDescending(x => x.Id).FirstOrDefaultAsync(predicate != null ? predicate : x => x.Id != 0);
            return query;
        }

        public virtual async Task<List<T>> FindConditionAsync(Expression<Func<T, bool>> predicate, ApplicationDbContext context = null)
        {
            if(context == null) context = _context;
            IQueryable<T> query = context.Set<T>();
            return await query.Where(predicate).ToListAsync();
        }

        public virtual List<T> FindCondition(Expression<Func<T, bool>> predicate, ApplicationDbContext context = null)
        {
            if(context == null) context = _context;
            IQueryable<T> query = context.Set<T>();
            return query.Where(predicate).ToList();

        }

        public virtual Task<int> CountAllAsync(ApplicationDbContext context = null)
        {
            if(context == null) context = _context;
            var query=context.Set<T>().CountAsync();
            return query;
        }

        public virtual Task<int> CountWhereAsync(Expression<Func<T, bool>> predicate, ApplicationDbContext context = null)
        {
            if(context == null) context = _context;
            var query= context.Set<T>().CountAsync(predicate);
            return query;
        }
        public virtual async Task<T> AddAsync(T entity, ApplicationDbContext context = null)
        {
            try
            {
                if(context == null) context = _context;
                await context.Set<T>().AddAsync(entity);
                await context.SaveChangesAsync();
                return entity;
            }
            catch
            {

                return null;
            }

        }

        public virtual async Task<T> AddOrUpdateAsync(T entity, ApplicationDbContext context = null)
        {
            try
            {
                if(context == null) context = _context;
                if(entity.Id == 0)
                {
                    await context.Set<T>().AddAsync(entity);
                }
                else if (entity.Id > 0)
                {
                    T newentity = context.Set<T>().First(x => x.Id == entity.Id);
                    context.Entry(newentity).CurrentValues.SetValues(entity);

                }
                await context.SaveChangesAsync();
                return entity;
            }
            catch(Exception ex)
            {
                throw;
            }
        }
        public virtual  T AddOrUpdate(T entity, ApplicationDbContext context = null)
        {
            try
            {
                if(context == null) context = _context;
                if (entity.Id == 0)
                {
                    context.Set<T>().AddAsync(entity);
                }
                else if (entity.Id > 0)
                {
                    T newentity = context.Set<T>().First(x => x.Id == entity.Id);
                    context.Entry(newentity).CurrentValues.SetValues(entity);

                }
                 context.SaveChanges();
                return entity;
            }
            catch(Exception ex)
            {

                return entity;
            }
        }
        public virtual async Task<T> UpdateAsync(T entity, ApplicationDbContext context = null)
        {
            try
            {
                if(context == null) context = _context;
                T loc = context.Set<T>().Local.SingleOrDefault(x => x.Id == entity.Id);
                if (loc != null)
                    context.Entry(loc).State = EntityState.Detached;
                context.Entry(entity).State = EntityState.Modified;
                await context.SaveChangesAsync();
            }
            catch
            {


            }
            return entity;
        }

       
        public virtual async Task<(bool IsSuccess, string childs)> RemoveAsync(long id, bool physically = false, ApplicationDbContext context = null)
        {
            try
            {
                if(context == null) context = _context;
                IEnumerable<string> list = context.GetIncludePaths(typeof(T));
                T entity =await context.Set<T>().Where(x => x.Id == id).Include(list, null).FirstOrDefaultAsync();
                List<string> childs = HasChildProperty(entity);
                if (childs.Count() == 0 || (childs.ToArray().Distinct().Count() == 1) && childs[0].Contains("فایل"))
                {
                    if (entity != null && !physically)
                    {
                        entity.IsDelete = true;
                        
                    }
                    else
                    {
                        context.Remove(entity);
                    }
                    if (await context.SaveChangesAsync() > 0)
                    {
                        return (true, "");
                    }
                    else
                    {
                        return (false, string.Join("، ", childs));
                    }
                }
                else
                    return (false, string.Join("، ", childs));
            }
            catch
            {
                return (false, "");

            }

        }
        public virtual (bool IsSuccess, string childs) Remove(long id, bool physically = false, ApplicationDbContext context = null)
        {
            try
            {
                if (context == null) context = _context;
                IEnumerable<string> list = context.GetIncludePaths(typeof(T));
                T entity =  context.Set<T>().Where(x => x.Id == id).Include(list, null).FirstOrDefault();
                List<string> childs = HasChildProperty(entity);
                if (childs.Count() == 0 || (childs.ToArray().Distinct().Count() == 1) && childs[0].Contains("فایل"))
                {
                    if (entity != null && !physically)
                    {
                        entity.IsDelete = true;

                    }
                    if (context.SaveChanges() > 0)
                    {
                        return (true, "");
                    }
                    else
                    {
                        return (false, string.Join("، ", childs));
                    }
                }
                else
                    return (false, string.Join("، ", childs));
            }
            catch
            {
                return (false, "");

            }
        }


        public virtual List<string> HasChildProperty(object data)
        {
            List<object> childs = new List<object>();
            foreach (PropertyInfo propertyInfo in data.GetType().GetProperties())
            {
                Type type = propertyInfo.PropertyType;
                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
                {
                    IList list = (IList)propertyInfo.GetValue(data);
                    if (list!=null && list.Count > 0)
                    {
                        foreach (object item in list)

                        {
                            childs.Add(item);
                        }
                    }
                }

            }

            return childs?.Select(q =>
            {
                return q.GetType().GetTypeInfo().GetCustomAttribute<DescriptionAttribute>()?.Description ?? q.GetType().Name;
            }).Distinct().ToList() ?? new List<string>();



        }

        public RawQuery RawSqlPagingQuery<T>(State state, ApplicationDbContext context = null)
        {
            if (context == null) context = _context;
            try
            {
                RawQuery rawQuery = new RawQuery();
                TableAttribute tableAttr = null;
                List<T> result = null;
                PrimaryKeyAttribute primAttr = null;

                #region VaribleForFilter

                string tableName = null;
                string schemaName = null;
                string relationColumnName = null;
                string currentColumnName = null;
                string currentTableColumnName = null;

                string connectorSchemaName = null;
                string connectorTableName = null;
                string connectorColumnName = null;

                string columnName = null;
                string sortClause = null;
                string whereClause = null;
                string where = null;
                string valueitem = null;

                string Schema1 = null;
                string Schema2 = null;
                string Schema3 = null;

                string table1 = null;
                string table2 = null;
                string table3 = null;

                string column1 = null;
                string column2 = null;
                string column3 = null;

                string relationcolumn1 = null;
                string relationcolumn2 = null;
                string relationcolumn3 = null;
                #endregion VaribleForFilter


                Attribute[] attrs = Attribute.GetCustomAttributes(typeof(T));

                foreach (Attribute attr in attrs)
                {
                    if (attr is TableAttribute)
                    {
                        tableAttr = (TableAttribute)attr;
                    }
                    if (attr is PrimaryKeyAttribute)
                    {
                        primAttr = (PrimaryKeyAttribute)attr;
                    }
                }

                if (tableAttr != null)
                {
                    if (state != null && !string.IsNullOrEmpty(tableAttr.Schema) && !string.IsNullOrEmpty(tableAttr.Name))
                    {
                        var operators = new string[] { "eq", "neq", "contains", "dosenotcontain", "startswith", "endswith", "isnull", "isnotnull", "isempty", "isnotempty" };
                        if (state.sort.Length > 0)
                        {
                            var sortField = state.sort[0].field.Split('.');
                            if (sortField.Length >= 10)
                            {
                                Schema1 = sortField[0];
                                table1 = sortField[1];
                                column1 = sortField[2];
                                relationcolumn1 = sortField[3];

                                Schema2 = sortField[4];
                                table2 = sortField[5];
                                column2 = sortField[6];
                                relationcolumn2 = sortField[7];

                                schemaName = sortField[8];
                                tableName = sortField[9];
                                relationColumnName = sortField[10];
                                columnName = sortField[11];

                                sortClause = (state.sort.Count() > 0 ? $@"{tableName}.{columnName} {state.sort[0].dir}" : null);

                                //if (state.filter != null)
                                //{

                                //    whereClause = $" inner join { Schema1 + "." + table1 } on { table1 + "." + column1} = { tableAttr.Name + "." + relationcolumn1} " +
                                //         $" inner join { Schema2 + "." + table2 } on { table2 + "." + column2} = { table1 + "." + relationcolumn2} " +
                                //         $" inner join { schemaName + "." + tableName } on { table2 + "." + relationColumnName }={tableName}.Id ";
                                //}
                                //else
                                //{
                                //    whereClause = $" left join { Schema1 + "." + table1 } on { table1 + "." + column1} = { tableAttr.Name + "." + relationcolumn1} " +
                                //      $" inner join { Schema2 + "." + table2 } on { table2 + "." + column2} = { table1 + "." + relationcolumn2} " +
                                //      $" inner join { schemaName + "." + tableName } on { table2 + "." + relationColumnName }={tableName}.Id ";
                                //}

                            }
                            else if (sortField.Length > 5 && sortField.Length < 10)
                            {

                                connectorSchemaName = sortField[0];
                                connectorTableName = sortField[1];
                                connectorColumnName = sortField[2];
                                currentTableColumnName = sortField[3];
                                schemaName = sortField[4];
                                tableName = sortField[5];
                                relationColumnName = sortField[6];
                                columnName = sortField[7];

                                sortClause = (state.sort.Count() > 0 ? $@"{tableName}.{columnName} {state.sort[0].dir}" : null);

                                if (state.filter != null)
                                {
                                    whereClause = $"inner join { connectorSchemaName + "." + connectorTableName } on { tableAttr.Name}.{currentTableColumnName} = { connectorTableName + "." + connectorColumnName} " +
                                     $"inner join { schemaName + "." + tableName} on { connectorTableName + "." + relationColumnName }={tableName}.Id ";
                                }
                                else
                                {
                                    whereClause = $"left join { connectorSchemaName + "." + connectorTableName } on { tableAttr.Name}.{currentTableColumnName} = { connectorTableName + "." + connectorColumnName} " +
                                      $"inner join { schemaName + "." + tableName} on { connectorTableName + "." + relationColumnName }={tableName}.Id ";
                                }

                            }
                            else if (sortField.Length == 5)
                            {
                                schemaName = sortField[0];
                                tableName = sortField[1];
                                relationColumnName = sortField[2];
                                currentTableColumnName = sortField[3];
                                columnName = sortField[4];

                                sortClause = (state.sort.Count() > 0 ? $@"{tableName}.{columnName} {state.sort[0].dir}" : null);



                                if (state.filter != null)
                                {
                                    whereClause = $"inner join { schemaName + "." + tableName } on { tableName + "." + relationColumnName }={tableAttr.Name}.{currentTableColumnName}";
                                }
                                else
                                {
                                    whereClause = $"left join { schemaName + "." + tableName } on { tableName + "." + relationColumnName }={tableAttr.Name}.{currentTableColumnName}";
                                }
                            }

                            else
                            {
                                sortClause = (state.sort.Count() > 0 ? $@"{ tableAttr.Name}.{state.sort[0].field} {state.sort[0].dir}" : $"Id asc");

                            }
                        }
                        else
                        {
                            sortClause = (state.sort.Count() > 0 ? $@"{ tableAttr.Name}.{state.sort[0].field} {state.sort[0].dir}" : $"Id asc");
                        }


                        if (state.filter != null && state.filter.filters.Count > 0)
                        {
                            if (state.filter.logic == "and")
                            {
                                state.filter.filters.ToList().ForEach((filter) =>
                                {
                                    valueitem = filter.value.ToString().Trim();
                                    var field = filter.field.ToString().Split('.');
                                    if (field != null && field.Length > 1)
                                    {
                                        if (field.Length >= 10)
                                        {
                                            Schema1 = field[0];
                                            table1 = field[1];
                                            column1 = field[2];
                                            relationcolumn1 = field[3];

                                            Schema2 = field[4];
                                            table2 = field[5];
                                            column2 = field[6];
                                            relationcolumn2 = field[7];

                                            schemaName = field[8];
                                            tableName = field[9];
                                            relationColumnName = field[10];
                                            columnName = field[11];

                                            valueitem = filter.value.ToString().Trim();
                                        }
                                        else if (field.Length > 5 && field.Length < 10)
                                        {

                                            connectorSchemaName = field[0];
                                            connectorTableName = field[1];
                                            connectorColumnName = field[2];
                                            currentTableColumnName = field[3];
                                            schemaName = field[4];
                                            tableName = field[5];
                                            relationColumnName = field[6];
                                            columnName = field[7];
                                            valueitem = filter.value.ToString().Trim();
                                        }
                                        else
                                        {
                                            schemaName = field[0];
                                            tableName = field[1];
                                            relationColumnName = field[2];
                                            currentTableColumnName = field[3];
                                            columnName = field[4];
                                            valueitem = filter.value.ToString().Trim();
                                        }
                                    }

                                    if (filter.@operator.Equals("eq"))
                                    {
                                        if (field != null && field.Length > 1)
                                        {
                                            if (field.Length >= 10)
                                            {


                                                if (whereClause != null)
                                                {
                                                    if (whereClause.Contains($"{Schema1 + "." + table1 }")
                                                    && whereClause.Contains($"{ Schema2 + "." + table2 }")
                                                    && whereClause.Contains($"{ schemaName + "." + tableName }"))
                                                    {

                                                        whereClause += $" and { tableName + "." + columnName} = N'{ (filter.value.ToString().Trim()) }'";

                                                    }
                                                    else if (whereClause.Contains($"{Schema1 + "." + table1 }")
                                                    && whereClause.Contains($"{ Schema2 + "." + table2 }"))
                                                    {
                                                        if (filter.ignoreCase == true)
                                                        {
                                                            whereClause += $" left join { schemaName + "." + tableName } on { table2 + "." + relationColumnName }={tableName}.Id and { tableName + "." + columnName} = N'{ (filter.value.ToString().Trim()) }'";
                                                        }
                                                        else
                                                        {
                                                            whereClause += $" inner join { schemaName + "." + tableName } on { table2 + "." + relationColumnName }={tableName}.Id and { tableName + "." + columnName} = N'{ (filter.value.ToString().Trim()) }'";
                                                        }

                                                    }
                                                    else if (whereClause.Contains($"{Schema1 + "." + table1 }"))
                                                    {
                                                        if (filter.ignoreCase == true)
                                                        {
                                                            whereClause += $" inner join { Schema2 + "." + table2 } on { table2 + "." + column2} = { table1 + "." + relationcolumn2} " +
                                                            $" left join { schemaName + "." + tableName } on { table2 + "." + relationColumnName }={tableName}.Id and { tableName + "." + columnName} = N'{ (filter.value.ToString().Trim()) }'";
                                                        }
                                                        else
                                                        {
                                                            whereClause += $" inner join { Schema2 + "." + table2 } on { table2 + "." + column2} = { table1 + "." + relationcolumn2} " +
                                                           $" inner join { schemaName + "." + tableName } on { table2 + "." + relationColumnName }={tableName}.Id and { tableName + "." + columnName} = N'{ (filter.value.ToString().Trim()) }'";
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (filter.ignoreCase == true)
                                                        {
                                                            whereClause += $" inner join { Schema1 + "." + table1 } on { table1 + "." + column1} = { tableAttr.Name + "." + relationcolumn1} " +
                                                            $" inner join { Schema2 + "." + table2 } on { table2 + "." + column2} = { table1 + "." + relationcolumn2} " +
                                                            $" left join { schemaName + "." + tableName } on { table2 + "." + relationColumnName }={tableName}.Id and { tableName + "." + columnName} = N'{ (filter.value.ToString().Trim()) }'";
                                                        }
                                                        else
                                                        {
                                                            whereClause += $" inner join { Schema1 + "." + table1 } on { table1 + "." + column1} = { tableAttr.Name + "." + relationcolumn1} " +
                                                            $" inner join { Schema2 + "." + table2 } on { table2 + "." + column2} = { table1 + "." + relationcolumn2} " +
                                                            $" inner join { schemaName + "." + tableName } on { table2 + "." + relationColumnName }={tableName}.Id and { tableName + "." + columnName} = N'{ (filter.value.ToString().Trim()) }'";
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    if (filter.ignoreCase == true)
                                                    {
                                                        whereClause = $" inner join { Schema1 + "." + table1 } on { table1 + "." + column1} = { tableAttr.Name + "." + relationcolumn1} " +
                                                            $" inner join { Schema2 + "." + table2 } on { table2 + "." + column2} = { table1 + "." + relationcolumn2} " +
                                                            $" left join { schemaName + "." + tableName } on { table2 + "." + relationColumnName }={tableName}.Id and { tableName + "." + columnName} = N'{ (filter.value.ToString().Trim()) }'";
                                                    }
                                                    else
                                                    {
                                                        whereClause = $" inner join { Schema1 + "." + table1 } on { table1 + "." + column1} = { tableAttr.Name + "." + relationcolumn1} " +
                                                            $" inner join { Schema2 + "." + table2 } on { table2 + "." + column2} = { table1 + "." + relationcolumn2} " +
                                                            $" inner join { schemaName + "." + tableName } on { table2 + "." + relationColumnName }={tableName}.Id and { tableName + "." + columnName} = N'{ (filter.value.ToString().Trim()) }'";
                                                    }

                                                }


                                            }
                                            else if (field.Length > 5 && field.Length < 10)
                                            {


                                                if (whereClause != null)
                                                {
                                                    if (whereClause.Contains($"{ connectorSchemaName + "." + connectorTableName }") && whereClause.Contains($"{ schemaName + "." + tableName }"))
                                                    {
                                                        whereClause += $" and { tableName + "." + columnName} = N'{ (filter.value.ToString().Trim()) }'";

                                                    }
                                                    else if (whereClause.Contains($"{ connectorSchemaName + "." + connectorTableName }"))
                                                    {
                                                        if (filter.ignoreCase == true)
                                                        {
                                                            whereClause += $" left join { schemaName + "." + tableName } on { connectorTableName + "." + relationColumnName }={tableName}.Id and { tableName + "." + columnName} = N'{ (filter.value.ToString().Trim()) }'";
                                                        }
                                                        else
                                                        {
                                                            whereClause += $" inner join { schemaName + "." + tableName } on { connectorTableName + "." + relationColumnName }={tableName}.Id and { tableName + "." + columnName} = N'{ (filter.value.ToString().Trim()) }'";
                                                        }

                                                    }
                                                    else
                                                    {
                                                        if (filter.ignoreCase == true)
                                                        {
                                                            whereClause += $" inner join { connectorSchemaName + "." + connectorTableName } on { connectorTableName + "." + connectorColumnName} = { tableAttr.Name + "." + currentTableColumnName} " +
                                                            $" left join { schemaName + "." + tableName } on { connectorTableName + "." + relationColumnName }={tableName}.Id and { tableName + "." + columnName} = N'{ (filter.value.ToString().Trim()) }'";
                                                        }
                                                        else
                                                        {
                                                            whereClause += $" inner join { connectorSchemaName + "." + connectorTableName } on { connectorTableName + "." + connectorColumnName} = { tableAttr.Name + "." + currentTableColumnName} " +
                                                            $" inner join { schemaName + "." + tableName } on { connectorTableName + "." + relationColumnName }={tableName}.Id and { tableName + "." + columnName} = N'{ (filter.value.ToString().Trim()) }'";
                                                        }

                                                    }
                                                }
                                                else
                                                {
                                                    if (filter.ignoreCase == true)
                                                    {
                                                        whereClause = $" inner join { connectorSchemaName + "." + connectorTableName } on { connectorTableName + "." + connectorColumnName} = { tableAttr.Name + "." + currentTableColumnName} " +
                                                        $" left join { schemaName + "." + tableName } on { connectorTableName + "." + relationColumnName }={tableName}.Id and { tableName + "." + columnName}= N'{ (filter.value.ToString().Trim()) }'";
                                                    }
                                                    else
                                                    {
                                                        whereClause = $" inner join { connectorSchemaName + "." + connectorTableName } on { connectorTableName + "." + connectorColumnName} = { tableAttr.Name + "." + currentTableColumnName} " +
                                                        $" inner join { schemaName + "." + tableName } on { connectorTableName + "." + relationColumnName }={tableName}.Id and { tableName + "." + columnName}= N'{ (filter.value.ToString().Trim()) }'";
                                                    }

                                                }


                                            }
                                            else
                                            {

                                                if (whereClause != null)
                                                {
                                                    if (whereClause.Contains($"{ schemaName + "." + tableName }"))
                                                    {
                                                        whereClause += $" and { tableName + "." + columnName} = N'{ (filter.value.ToString().Trim()) }'";
                                                    }
                                                    else
                                                    {
                                                        if (filter.ignoreCase == true)
                                                        {
                                                            if (tableName == tableAttr.Name)
                                                                whereClause += $" left join { schemaName + "." + tableName } as temp on { "temp." + relationColumnName }={tableAttr.Name}.{currentTableColumnName} and { "temp." + columnName} = N'{ (filter.value.ToString().Trim()) }'";
                                                            else
                                                                whereClause += $" left join { schemaName + "." + tableName } on { tableName + "." + relationColumnName }={tableAttr.Name}.{currentTableColumnName} and { tableName + "." + columnName} = N'{ (filter.value.ToString().Trim()) }'";
                                                        }
                                                        else
                                                        {
                                                            if (tableName == tableAttr.Name)
                                                                whereClause += $" inner join { schemaName + "." + tableName } as temp on { "temp." + relationColumnName }={tableAttr.Name}.{currentTableColumnName} and { "temp." + columnName} = N'{ (filter.value.ToString().Trim()) }'";
                                                            else
                                                                whereClause += $" inner join { schemaName + "." + tableName } on { tableName + "." + relationColumnName }={tableAttr.Name}.{currentTableColumnName} and { tableName + "." + columnName} = N'{ (filter.value.ToString().Trim()) }'";
                                                        }

                                                    }
                                                }
                                                else
                                                {
                                                    if (filter.ignoreCase == true)
                                                    {
                                                        if (tableName == tableAttr.Name)
                                                            whereClause = $" left join { schemaName + "." + tableName } as temp on { "temp." + relationColumnName }={tableAttr.Name}.{currentTableColumnName} and { "temp." + columnName} = N'{ (filter.value.ToString().Trim()) }'";
                                                        else
                                                            whereClause = $" left join { schemaName + "." + tableName } on { tableName + "." + relationColumnName }={tableAttr.Name}.{currentTableColumnName} and { tableName + "." + columnName} = N'{ (filter.value.ToString().Trim()) }'";
                                                    }
                                                    else
                                                    {
                                                        if (tableName == tableAttr.Name)
                                                            whereClause = $" inner join { schemaName + "." + tableName } as temp on { "temp." + relationColumnName }={tableAttr.Name}.{currentTableColumnName} and { "temp." + columnName} = N'{ (filter.value.ToString().Trim()) }'";
                                                        else
                                                            whereClause = $" inner join { schemaName + "." + tableName } on { tableName + "." + relationColumnName }={tableAttr.Name}.{currentTableColumnName} and { tableName + "." + columnName} = N'{ (filter.value.ToString().Trim()) }'";
                                                    }
                                                }



                                            }
                                            ////if (where == null)
                                            ////{
                                            ////    where = $" where {tableAttr.Name}.IsDeleted = 0 and 1 = 1 ";
                                            ////}

                                        }
                                        else
                                        {
                                            if (where == null)
                                            {
                                                where += $"{tableAttr.Name}.{ filter.field } = N'{ (filter.value.ToString().Trim()) }'";
                                            }
                                            else
                                            {
                                                ////where = $" where {tableAttr.Name}.IsDeleted = 0 and 1 = 1 ";
                                                where = $" and {tableAttr.Name}.{ filter.field } = N'{ (filter.value.ToString().Trim()) }'";
                                            }

                                        }
                                    }
                                    else if (filter.@operator.Equals("neq"))
                                    {
                                        whereClause += $"and { filter.field } <> N'{ (filter.value.ToString().Trim()) }'";
                                    }
                                    else if (filter.@operator.Equals("contains"))
                                    {

                                        if (field != null && field.Length > 1)
                                        {

                                            if (field.Length >= 10)
                                            {


                                                if (whereClause != null)
                                                {
                                                    if (whereClause.Contains($"{Schema1 + "." + table1 }")
                                                    && whereClause.Contains($"{ Schema2 + "." + table2 }")
                                                    && whereClause.Contains($"{ schemaName + "." + tableName }"))
                                                    {
                                                        whereClause += $" and { tableName + "." + columnName} = N'{ (filter.value.ToString().Trim()) }'";

                                                    }
                                                    else if (whereClause.Contains($"{Schema1 + "." + table1 }")
                                                    && whereClause.Contains($"{ Schema2 + "." + table2 }"))
                                                    {
                                                        if (filter.ignoreCase == true)
                                                        {
                                                            whereClause += $" left join { schemaName + "." + tableName } on { table2 + "." + relationColumnName }={tableName}.Id and { tableName + "." + columnName} Like N'%{ (filter.value.ToString().Trim()) }%'";
                                                        }
                                                        else
                                                        {
                                                            whereClause += $" inner join { schemaName + "." + tableName } on { table2 + "." + relationColumnName }={tableName}.Id and { tableName + "." + columnName} Like N'%{ (filter.value.ToString().Trim()) }%'";
                                                        }

                                                    }
                                                    else if (whereClause.Contains($"{Schema1 + "." + table1 }"))
                                                    {
                                                        if (filter.ignoreCase == true)
                                                        {
                                                            whereClause += $" inner join { Schema2 + "." + table2 } on { table2 + "." + column2} = { table1 + "." + relationcolumn2} " +
                                                             $" left join { schemaName + "." + tableName } on { table2 + "." + relationColumnName }={tableName}.Id and { tableName + "." + columnName} Like N'%{ (filter.value.ToString().Trim()) }%'";
                                                        }
                                                        else
                                                        {
                                                            whereClause += $" inner join { Schema2 + "." + table2 } on { table2 + "." + column2} = { table1 + "." + relationcolumn2} " +
                                                             $" inner join { schemaName + "." + tableName } on { table2 + "." + relationColumnName }={tableName}.Id and { tableName + "." + columnName} Like N'%{ (filter.value.ToString().Trim()) }%'";
                                                        }

                                                    }
                                                    else
                                                    {
                                                        if (filter.ignoreCase == true)
                                                        {
                                                            whereClause += $" inner join { Schema1 + "." + table1 } on { table1 + "." + column1} = { tableAttr.Name + "." + relationcolumn1} " +
                                                            $" inner join { Schema2 + "." + table2 } on { table2 + "." + column2} = { table1 + "." + relationcolumn2} " +
                                                            $" left join { schemaName + "." + tableName } on { table2 + "." + relationColumnName }={tableName}.Id and { tableName + "." + columnName} Like N'%{ (filter.value.ToString().Trim()) }%'";
                                                        }
                                                        else
                                                        {
                                                            whereClause += $" inner join { Schema1 + "." + table1 } on { table1 + "." + column1} = { tableAttr.Name + "." + relationcolumn1} " +
                                                            $" inner join { Schema2 + "." + table2 } on { table2 + "." + column2} = { table1 + "." + relationcolumn2} " +
                                                            $" inner join { schemaName + "." + tableName } on { table2 + "." + relationColumnName }={tableName}.Id and { tableName + "." + columnName} Like N'%{ (filter.value.ToString().Trim()) }%'";
                                                        }

                                                    }
                                                }
                                                else
                                                {
                                                    if (filter.ignoreCase == true)
                                                    {
                                                        whereClause = $" inner join { Schema1 + "." + table1 } on { table1 + "." + column1} = { tableAttr.Name + "." + relationcolumn1} " +
                                                             $" inner join { Schema2 + "." + table2 } on { table2 + "." + column2} = { table1 + "." + relationcolumn2} " +
                                                             $" left join { schemaName + "." + tableName } on { table2 + "." + relationColumnName }={tableName}.Id and { tableName + "." + columnName} Like N'%{ (filter.value.ToString().Trim()) }%'";
                                                    }
                                                    else
                                                    {
                                                        whereClause = $" inner join { Schema1 + "." + table1 } on { table1 + "." + column1} = { tableAttr.Name + "." + relationcolumn1} " +
                                                             $" inner join { Schema2 + "." + table2 } on { table2 + "." + column2} = { table1 + "." + relationcolumn2} " +
                                                             $" inner join { schemaName + "." + tableName } on { table2 + "." + relationColumnName }={tableName}.Id and { tableName + "." + columnName} Like N'%{ (filter.value.ToString().Trim()) }%'";
                                                    }

                                                }


                                            }
                                            else if (field.Length > 5 && field.Length < 10)
                                            {


                                                if (whereClause != null)
                                                {
                                                    if (whereClause.Contains($"{ connectorSchemaName + "." + connectorTableName }") && whereClause.Contains($"{ schemaName + "." + tableName }"))
                                                    {
                                                        whereClause += $" and { tableName + "." + columnName}  like  N'%{ (filter.value.ToString().Trim()) }%'";

                                                    }
                                                    else if (whereClause.Contains($"{ connectorSchemaName + "." + connectorTableName }"))
                                                    {
                                                        if (filter.ignoreCase == true)
                                                        {
                                                            whereClause += $" left join { schemaName + "." + tableName } on { connectorTableName + "." + relationColumnName }={tableName}.Id and { tableName + "." + columnName}  like  N'%{ (filter.value.ToString().Trim()) }%'";
                                                        }
                                                        else
                                                        {
                                                            whereClause += $" inner join { schemaName + "." + tableName } on { connectorTableName + "." + relationColumnName }={tableName}.Id and { tableName + "." + columnName}  like  N'%{ (filter.value.ToString().Trim()) }%'";
                                                        }

                                                    }
                                                    else
                                                    {
                                                        if (filter.ignoreCase == true)
                                                        {
                                                            whereClause += $" inner join { connectorSchemaName + "." + connectorTableName } on { connectorTableName + "." + connectorColumnName} = { tableAttr.Name + "." + currentTableColumnName} " +
                                                            $" left join { schemaName + "." + tableName } on { connectorTableName + "." + relationColumnName }={tableName}.Id and { tableName + "." + columnName}  like  N'%{ (filter.value.ToString().Trim()) }%'";
                                                        }
                                                        else
                                                        {
                                                            whereClause += $" inner join { connectorSchemaName + "." + connectorTableName } on { connectorTableName + "." + connectorColumnName} = { tableAttr.Name + "." + currentTableColumnName} " +
                                                            $" inner join { schemaName + "." + tableName } on { connectorTableName + "." + relationColumnName }={tableName}.Id and { tableName + "." + columnName}  like  N'%{ (filter.value.ToString().Trim()) }%'";
                                                        }

                                                    }
                                                }
                                                else
                                                {
                                                    if (filter.ignoreCase == true)
                                                    {
                                                        whereClause = $" inner join { connectorSchemaName + "." + connectorTableName } on { connectorTableName + "." + connectorColumnName} = { tableAttr.Name + "." + currentTableColumnName} " +
                                                        $" left join { schemaName + "." + tableName } on { connectorTableName + "." + relationColumnName }={tableName}.Id and { tableName + "." + columnName}  like  N'%{ (filter.value.ToString().Trim()) }%'";
                                                    }
                                                    else
                                                    {
                                                        whereClause = $" inner join { connectorSchemaName + "." + connectorTableName } on { connectorTableName + "." + connectorColumnName} = { tableAttr.Name + "." + currentTableColumnName} " +
                                                        $" inner join { schemaName + "." + tableName } on { connectorTableName + "." + relationColumnName }={tableName}.Id and { tableName + "." + columnName}  like  N'%{ (filter.value.ToString().Trim()) }%'";
                                                    }

                                                }


                                                //whereClause = $"inner join { connectorSchemaName + "." + connectorTableName } on { connectorTableName +"."+ connectorColumnName} = { tableAttr.Name + "." + currentTableColumnName} " +
                                                //$"inner join { schemaName + "." + tableName } on { connectorTableName + "." + relationColumnName }={tableName}.Id and { tableName + "." + columnName}  like  N'%{ (filter.value.ToString().Trim()) }%'";

                                                //whereClause += $" where {tableAttr.Name}.IsDeleted = 0 and 1 = 1 ";

                                            }
                                            else
                                            {

                                                if (whereClause != null)
                                                {
                                                    if (whereClause.Contains($"{ schemaName + "." + tableName }"))
                                                    {
                                                        whereClause += $" and { tableName + "." + columnName}  like  N'%{ (filter.value.ToString().Trim()) }%'";
                                                    }
                                                    else
                                                    {
                                                        if (filter.ignoreCase == true)
                                                        {
                                                            if (tableName == tableAttr.Name)
                                                                whereClause += $" left join { schemaName + "." + tableName } as temp on { "temp." + relationColumnName }={tableAttr.Name}.{currentTableColumnName} and { "temp." + columnName}  like  N'%{ (filter.value.ToString().Trim()) }%'";
                                                            else
                                                                whereClause += $" left join { schemaName + "." + tableName } on { tableName + "." + relationColumnName }={tableAttr.Name}.{currentTableColumnName} and { tableName + "." + columnName}  like  N'%{ (filter.value.ToString().Trim()) }%'";
                                                        }
                                                        else
                                                        {
                                                            if (tableName == tableAttr.Name)
                                                                whereClause += $" inner join { schemaName + "." + tableName } as temp on { "temp." + relationColumnName }={tableAttr.Name}.{currentTableColumnName} and { "temp." + columnName}  like  N'%{ (filter.value.ToString().Trim()) }%'";
                                                            else
                                                                whereClause += $" inner join { schemaName + "." + tableName } on { tableName + "." + relationColumnName }={tableAttr.Name}.{currentTableColumnName} and { tableName + "." + columnName}  like  N'%{ (filter.value.ToString().Trim()) }%'";
                                                        }

                                                    }
                                                }
                                                else
                                                {
                                                    if (filter.ignoreCase == true)
                                                    {
                                                        if (tableName == tableAttr.Name)
                                                            whereClause += $" left join { schemaName + "." + tableName } as temp on { "temp." + relationColumnName }={tableAttr.Name}.{currentTableColumnName} and { "temp." + columnName}  like  N'%{ (filter.value.ToString().Trim()) }%'";
                                                        else
                                                            whereClause = $" left join { schemaName + "." + tableName } on { tableName + "." + relationColumnName }={tableAttr.Name}.{currentTableColumnName} and { tableName + "." + columnName}  like  N'%{ (filter.value.ToString().Trim()) }%'";
                                                    }
                                                    else
                                                    {
                                                        if (tableName == tableAttr.Name)
                                                            whereClause += $" inner join { schemaName + "." + tableName } as temp on { "temp." + relationColumnName }={tableAttr.Name}.{currentTableColumnName} and { "temp." + columnName}  like  N'%{ (filter.value.ToString().Trim()) }%'";
                                                        else
                                                            whereClause = $" inner join { schemaName + "." + tableName } on { tableName + "." + relationColumnName }={tableAttr.Name}.{currentTableColumnName} and { tableName + "." + columnName}  like  N'%{ (filter.value.ToString().Trim()) }%'";
                                                    }

                                                }

                                                //whereClause = $"and { tableName + "." + columnName } like N'%{ (filter.value.ToString().Trim()) }%'";

                                            }
                                            ////if (where == null)
                                            ////{
                                            ////    where = $" where {tableAttr.Name}.IsDeleted = 0 and 1 = 1 ";
                                            ////}

                                        }
                                        else
                                        {
                                            if (where == null)
                                            {
                                                where += $" {tableAttr.Name}.{ filter.field } like N'%{ (filter.value.ToString().Trim()) }%'";
                                            }
                                            else
                                            {
                                                ////where = $" where {tableAttr.Name}.IsDeleted = 0 and 1 = 1 ";
                                                where += $" and {tableAttr.Name}.{ filter.field } like N'%{ (filter.value.ToString().Trim()) }%'";
                                            }

                                        }
                                    }
                                    else if (filter.@operator.Equals("dosenotcontain"))
                                    {
                                        whereClause += $" and { filter.field } not like N'%{ (filter.value.ToString().Trim()) }%'";
                                    }
                                    else if (filter.@operator.Equals("startswith"))
                                    {
                                        whereClause += $" and { filter.field } like N'%{ (filter.value.ToString().Trim()) }'";
                                    }
                                    else if (filter.@operator.Equals("endswith"))
                                    {
                                        whereClause += $" and { filter.field } like N'{ (filter.value.ToString().Trim()) }%'";
                                    }
                                    else if (filter.@operator.Equals("isnull"))
                                    {
                                        whereClause += $" and { filter.field } is null";
                                    }
                                    else if (filter.@operator.Equals("isnotnull"))
                                    {
                                        whereClause += $" and { filter.field } is not null";
                                    }
                                    else if (filter.@operator.Equals("isempty"))
                                    {
                                        whereClause += $" and { filter.field } = N''";
                                    }
                                    else if (filter.@operator.Equals("isnotempty"))
                                    {
                                        whereClause += $" and { filter.field }  <> N''";
                                    }
                                });
                            }
                            else if (state.filter.logic == "or")
                            {
                                state.filter.filters.ToList().ForEach((filter) =>
                                {
                                    valueitem = filter.value.ToString().Trim();
                                    var field = filter.field.ToString().Split('.');
                                    if (field != null && field.Length > 1)
                                    {
                                        if (field.Length >= 10)
                                        {
                                            Schema1 = field[0];
                                            table1 = field[1];
                                            column1 = field[2];
                                            relationcolumn1 = field[3];

                                            Schema2 = field[4];
                                            table2 = field[5];
                                            column2 = field[6];
                                            relationcolumn2 = field[7];

                                            schemaName = field[8];
                                            tableName = field[9];
                                            relationColumnName = field[10];
                                            columnName = field[11];

                                            valueitem = filter.value.ToString().Trim();
                                        }
                                        else if (field.Length > 5 && field.Length < 10)
                                        {

                                            connectorSchemaName = field[0];
                                            connectorTableName = field[1];
                                            connectorColumnName = field[2];
                                            currentTableColumnName = field[3];
                                            schemaName = field[4];
                                            tableName = field[5];
                                            currentColumnName = field[6];
                                            relationColumnName = field[7];
                                            columnName = field[8];
                                            valueitem = filter.value.ToString().Trim();
                                        }
                                        else
                                        {
                                            schemaName = field[0];
                                            tableName = field[1];
                                            relationColumnName = field[2];
                                            currentTableColumnName = field[3];
                                            columnName = field[4];
                                            valueitem = filter.value.ToString().Trim();
                                        }
                                    }

                                    if (filter.@operator.Equals("eq"))
                                    {
                                        if (field != null && field.Length > 1)
                                        {
                                            if (field.Length >= 10)
                                            {


                                                if (whereClause != null)
                                                {
                                                    if (whereClause.Contains($"{Schema1 + "." + table1 }")
                                                    && whereClause.Contains($"{ Schema2 + "." + table2 }")
                                                    && whereClause.Contains($"{ schemaName + "." + tableName }"))
                                                    {


                                                    }
                                                    else if (whereClause.Contains($"{Schema1 + "." + table1 }")
                                                    && whereClause.Contains($"{ Schema2 + "." + table2 }"))
                                                    {
                                                        if (filter.ignoreCase == true)
                                                        {
                                                            whereClause += $" left join { schemaName + "." + tableName } on { table2 + "." + relationColumnName }={tableName}.Id";
                                                        }
                                                        else
                                                        {
                                                            whereClause += $" inner join { schemaName + "." + tableName } on { table2 + "." + relationColumnName }={tableName}.Id";
                                                        }

                                                    }
                                                    else if (whereClause.Contains($"{Schema1 + "." + table1 }"))
                                                    {
                                                        if (filter.ignoreCase == true)
                                                        {
                                                            whereClause += $" inner join { Schema2 + "." + table2 } on { table2 + "." + column2} = { table1 + "." + relationcolumn2} " +
                                                            $" left join { schemaName + "." + tableName } on { table2 + "." + relationColumnName }={tableName}.Id";
                                                        }
                                                        else
                                                        {
                                                            whereClause += $" inner join { Schema2 + "." + table2 } on { table2 + "." + column2} = { table1 + "." + relationcolumn2} " +
                                                             $" inner join { schemaName + "." + tableName } on { table2 + "." + relationColumnName }={tableName}.Id";
                                                        }

                                                    }
                                                    else
                                                    {
                                                        if (filter.ignoreCase == true)
                                                        {
                                                            whereClause += $" inner join { Schema1 + "." + table1 } on { table1 + "." + column1} = { tableAttr.Name + "." + relationcolumn1} " +
                                                            $" inner join { Schema2 + "." + table2 } on { table2 + "." + column2} = { table1 + "." + relationcolumn2} " +
                                                            $" left join { schemaName + "." + tableName } on { table2 + "." + relationColumnName }={tableName}.Id";
                                                        }
                                                        else
                                                        {
                                                            whereClause += $" inner join { Schema1 + "." + table1 } on { table1 + "." + column1} = { tableAttr.Name + "." + relationcolumn1} " +
                                                            $" inner join { Schema2 + "." + table2 } on { table2 + "." + column2} = { table1 + "." + relationcolumn2} " +
                                                            $" inner join { schemaName + "." + tableName } on { table2 + "." + relationColumnName }={tableName}.Id";
                                                        }

                                                    }
                                                }
                                                else
                                                {
                                                    if (filter.ignoreCase == true)
                                                    {
                                                        whereClause = $" inner join { Schema1 + "." + table1 } on { table1 + "." + column1} = { tableAttr.Name + "." + relationcolumn1} " +
                                                            $" inner join { Schema2 + "." + table2 } on { table2 + "." + column2} = { table1 + "." + relationcolumn2} " +
                                                            $" left join { schemaName + "." + tableName } on { table2 + "." + relationColumnName }={tableName}.Id";
                                                    }
                                                    else
                                                    {
                                                        whereClause = $" inner join { Schema1 + "." + table1 } on { table1 + "." + column1} = { tableAttr.Name + "." + relationcolumn1} " +
                                                            $" inner join { Schema2 + "." + table2 } on { table2 + "." + column2} = { table1 + "." + relationcolumn2} " +
                                                            $" inner join { schemaName + "." + tableName } on { table2 + "." + relationColumnName }={tableName}.Id";
                                                    }

                                                }



                                            }
                                            else if (field.Length > 5 && field.Length < 10)
                                            {


                                                if (whereClause != null)
                                                {
                                                    if (whereClause.Contains($"{ connectorSchemaName + "." + connectorTableName }") && whereClause.Contains($"{ schemaName + "." + tableName }"))
                                                    {


                                                    }
                                                    else if (whereClause.Contains($"{ connectorSchemaName + "." + connectorTableName }"))
                                                    {
                                                        if (filter.ignoreCase == true)
                                                        {
                                                            whereClause += $" left join { schemaName + "." + tableName } on { connectorTableName + "." + relationColumnName }={tableName}.{currentColumnName}";
                                                        }
                                                        else
                                                        {
                                                            whereClause += $" inner join { schemaName + "." + tableName } on { connectorTableName + "." + relationColumnName }={tableName}.{currentColumnName}";
                                                        }

                                                    }
                                                    else
                                                    {
                                                        if (filter.ignoreCase == true)
                                                        {
                                                            whereClause += $" inner join { connectorSchemaName + "." + connectorTableName } on { connectorTableName + "." + connectorColumnName} = { tableAttr.Name + "." + currentTableColumnName} " +
                                                            $" left join { schemaName + "." + tableName } on { connectorTableName + "." + relationColumnName }={tableName}.{currentColumnName}";
                                                        }
                                                        else
                                                        {
                                                            whereClause += $" inner join { connectorSchemaName + "." + connectorTableName } on { connectorTableName + "." + connectorColumnName} = { tableAttr.Name + "." + currentTableColumnName} " +
                                                            $" inner join { schemaName + "." + tableName } on { connectorTableName + "." + relationColumnName }={tableName}.{currentColumnName}";
                                                        }

                                                    }
                                                }
                                                else
                                                {
                                                    if (filter.ignoreCase == true)
                                                    {
                                                        whereClause = $" inner join { connectorSchemaName + "." + connectorTableName } on { connectorTableName + "." + connectorColumnName} = { tableAttr.Name + "." + currentTableColumnName} " +
                                                        $" left join { schemaName + "." + tableName } on { connectorTableName + "." + relationColumnName }={tableName}.{currentColumnName}";
                                                    }
                                                    else
                                                    {
                                                        whereClause = $" inner join { connectorSchemaName + "." + connectorTableName } on { connectorTableName + "." + connectorColumnName} = { tableAttr.Name + "." + currentTableColumnName} " +
                                                        $" inner join { schemaName + "." + tableName } on { connectorTableName + "." + relationColumnName }={tableName}.{currentColumnName}";
                                                    }

                                                }


                                            }
                                            else
                                            {

                                                if (whereClause != null)
                                                {
                                                    if (whereClause.Contains($"{ schemaName + "." + tableName }"))
                                                    {

                                                    }
                                                    else
                                                    {
                                                        if (filter.ignoreCase == true)
                                                        {
                                                            if (tableName == tableAttr.Name)
                                                                whereClause += $" left join { schemaName + "." + tableName } as temp on { "temp." + relationColumnName }={tableAttr.Name}.{currentTableColumnName}";
                                                            else
                                                                whereClause += $" left join { schemaName + "." + tableName } on { tableName + "." + relationColumnName }={tableAttr.Name}.{currentTableColumnName}";
                                                        }
                                                        else
                                                        {
                                                            if (tableName == tableAttr.Name)
                                                                whereClause += $" inner join { schemaName + "." + tableName } as temp on { "temp." + relationColumnName }={tableAttr.Name}.{currentTableColumnName}";
                                                            else
                                                                whereClause += $" inner join { schemaName + "." + tableName } on { tableName + "." + relationColumnName }={tableAttr.Name}.{currentTableColumnName}";
                                                        }

                                                    }
                                                }
                                                else
                                                {
                                                    if (filter.ignoreCase == true)
                                                    {
                                                        if (tableName == tableAttr.Name)
                                                            whereClause = $" left join { schemaName + "." + tableName } as temp on { "temp." + relationColumnName }={tableAttr.Name}.{currentTableColumnName}";
                                                        else
                                                            whereClause = $" left join { schemaName + "." + tableName } on { tableName + "." + relationColumnName }={tableAttr.Name}.{currentTableColumnName}";
                                                    }
                                                    else
                                                    {
                                                        if (tableName == tableAttr.Name)
                                                            whereClause = $" inner join { schemaName + "." + tableName } as temp on { "temp." + relationColumnName }={tableAttr.Name}.{currentTableColumnName}";
                                                        else
                                                            whereClause = $" inner join { schemaName + "." + tableName } on { tableName + "." + relationColumnName }={tableAttr.Name}.{currentTableColumnName}";
                                                    }
                                                }



                                            }

                                            if (where == null)
                                            {
                                                where += $"{ tableName + "." + columnName} = N'%{ (filter.value.ToString().Trim()) }%'";
                                            }
                                            else
                                            {
                                                where += $" or { tableName + "." + columnName} = N'%{ (filter.value.ToString().Trim()) }%'";
                                            }

                                        }
                                        else
                                        {

                                            if (where == null)
                                            {
                                                where += $"{tableAttr.Name}.{ filter.field } = N'{ (filter.value.ToString().Trim()) }'";
                                            }
                                            else
                                            {
                                                where += $" or {tableAttr.Name}.{ filter.field } = N'{ (filter.value.ToString().Trim()) }'";
                                            }



                                        }
                                    }
                                    else if (filter.@operator.Equals("neq"))
                                    {
                                        whereClause += $"or { filter.field } <> N'{ (filter.value.ToString().Trim()) }'";
                                    }
                                    else if (filter.@operator.Equals("contains"))
                                    {

                                        if (field != null && field.Length > 1)
                                        {

                                            if (field.Length >= 10)
                                            {


                                                if (whereClause != null)
                                                {
                                                    if (whereClause.Contains($"{Schema1 + "." + table1 }")
                                                    && whereClause.Contains($"{ Schema2 + "." + table2 }")
                                                    && whereClause.Contains($"{ schemaName + "." + tableName }"))
                                                    {


                                                    }
                                                    else if (whereClause.Contains($"{Schema1 + "." + table1 }")
                                                    && whereClause.Contains($"{ Schema2 + "." + table2 }"))
                                                    {
                                                        if (filter.ignoreCase == true)
                                                        {
                                                            whereClause += $" left join { schemaName + "." + tableName } on { table2 + "." + relationColumnName }={tableName}.Id";
                                                        }
                                                        else
                                                        {
                                                            whereClause += $" inner join { schemaName + "." + tableName } on { table2 + "." + relationColumnName }={tableName}.Id";
                                                        }

                                                    }
                                                    else if (whereClause.Contains($"{Schema1 + "." + table1 }"))
                                                    {
                                                        if (filter.ignoreCase == true)
                                                        {
                                                            whereClause += $" inner join { Schema2 + "." + table2 } on { table2 + "." + column2} = { table1 + "." + relationcolumn2} " +
                                                             $" left join { schemaName + "." + tableName } on { table2 + "." + relationColumnName }={tableName}.Id";
                                                        }
                                                        else
                                                        {
                                                            whereClause += $" inner join { Schema2 + "." + table2 } on { table2 + "." + column2} = { table1 + "." + relationcolumn2} " +
                                                             $" inner join { schemaName + "." + tableName } on { table2 + "." + relationColumnName }={tableName}.Id";
                                                        }

                                                    }
                                                    else
                                                    {
                                                        if (filter.ignoreCase == true)
                                                        {
                                                            whereClause += $" inner join { Schema1 + "." + table1 } on { table1 + "." + column1} = { tableAttr.Name + "." + relationcolumn1} " +
                                                            $" inner join { Schema2 + "." + table2 } on { table2 + "." + column2} = { table1 + "." + relationcolumn2} " +
                                                            $" left join { schemaName + "." + tableName } on { table2 + "." + relationColumnName }={tableName}.Id";
                                                        }
                                                        else
                                                        {
                                                            whereClause += $" inner join { Schema1 + "." + table1 } on { table1 + "." + column1} = { tableAttr.Name + "." + relationcolumn1} " +
                                                            $" inner join { Schema2 + "." + table2 } on { table2 + "." + column2} = { table1 + "." + relationcolumn2} " +
                                                            $" inner join { schemaName + "." + tableName } on { table2 + "." + relationColumnName }={tableName}.Id";
                                                        }

                                                    }
                                                }
                                                else
                                                {
                                                    if (filter.ignoreCase == true)
                                                    {
                                                        whereClause = $" inner join { Schema1 + "." + table1 } on { table1 + "." + column1} = { tableAttr.Name + "." + relationcolumn1} " +
                                                           $" inner join { Schema2 + "." + table2 } on { table2 + "." + column2} = { table1 + "." + relationcolumn2} " +
                                                           $" left join { schemaName + "." + tableName } on { table2 + "." + relationColumnName }={tableName}.Id";
                                                    }
                                                    else
                                                    {
                                                        whereClause = $" inner join { Schema1 + "." + table1 } on { table1 + "." + column1} = { tableAttr.Name + "." + relationcolumn1} " +
                                                           $" inner join { Schema2 + "." + table2 } on { table2 + "." + column2} = { table1 + "." + relationcolumn2} " +
                                                           $" inner join { schemaName + "." + tableName } on { table2 + "." + relationColumnName }={tableName}.Id";
                                                    }

                                                }


                                            }
                                            else if (field.Length > 5 && field.Length < 10)
                                            {


                                                if (whereClause != null)
                                                {
                                                    if (whereClause.Contains($"{ connectorSchemaName + "." + connectorTableName }") && whereClause.Contains($"{ schemaName + "." + tableName }"))
                                                    {


                                                    }
                                                    else if (whereClause.Contains($"{ connectorSchemaName + "." + connectorTableName }"))
                                                    {
                                                        if (filter.ignoreCase == true)
                                                        {
                                                            whereClause += $" left join { schemaName + "." + tableName } on { connectorTableName + "." + relationColumnName }={tableName}.{currentColumnName}";
                                                        }
                                                        else
                                                        {
                                                            whereClause += $" inner join { schemaName + "." + tableName } on { connectorTableName + "." + relationColumnName }={tableName}.{currentColumnName}";
                                                        }

                                                    }
                                                    else
                                                    {
                                                        if (filter.ignoreCase == true)
                                                        {
                                                            whereClause += $" inner join { connectorSchemaName + "." + connectorTableName } on { connectorTableName + "." + connectorColumnName} = { tableAttr.Name + "." + currentTableColumnName} " +
                                                            $"left join { schemaName + "." + tableName } on { connectorTableName + "." + relationColumnName }={tableName}.{currentColumnName}";
                                                        }
                                                        else
                                                        {
                                                            whereClause += $" inner join { connectorSchemaName + "." + connectorTableName } on { connectorTableName + "." + connectorColumnName} = { tableAttr.Name + "." + currentTableColumnName} " +
                                                            $"inner join { schemaName + "." + tableName } on { connectorTableName + "." + relationColumnName }={tableName}.{currentColumnName}";
                                                        }

                                                    }
                                                }
                                                else
                                                {
                                                    if (filter.ignoreCase == true)
                                                    {
                                                        whereClause = $" inner join { connectorSchemaName + "." + connectorTableName } on { connectorTableName + "." + connectorColumnName} = { tableAttr.Name + "." + currentTableColumnName} " +
                                                         $" left join { schemaName + "." + tableName } on { connectorTableName + "." + relationColumnName }={tableName}.{currentColumnName}";
                                                    }
                                                    else
                                                    {
                                                        whereClause = $" inner join { connectorSchemaName + "." + connectorTableName } on { connectorTableName + "." + connectorColumnName} = { tableAttr.Name + "." + currentTableColumnName} " +
                                                         $" inner join { schemaName + "." + tableName } on { connectorTableName + "." + relationColumnName }={tableName}.{currentColumnName}";
                                                    }

                                                }


                                                //whereClause = $"inner join { connectorSchemaName + "." + connectorTableName } on { connectorTableName +"."+ connectorColumnName} = { tableAttr.Name + "." + currentTableColumnName} " +
                                                //$"inner join { schemaName + "." + tableName } on { connectorTableName + "." + relationColumnName }={tableName}.Id and { tableName + "." + columnName}  like  N'%{ (filter.value.ToString().Trim()) }%'";

                                                //whereClause += $" where {tableAttr.Name}.IsDeleted = 0 and 1 = 1 ";

                                            }
                                            else
                                            {

                                                if (whereClause != null)
                                                {
                                                    if (whereClause.Contains($"{ schemaName + "." + tableName }"))
                                                    {

                                                    }
                                                    else
                                                    {
                                                        if (filter.ignoreCase == true)
                                                        {
                                                            if (tableName == tableAttr.Name)
                                                                whereClause += $" left join { schemaName + "." + tableName } as temp on { "temp." + relationColumnName }={tableAttr.Name}.{currentTableColumnName}";
                                                            else
                                                                whereClause += $" left join { schemaName + "." + tableName } on { tableName + "." + relationColumnName }={tableAttr.Name}.{currentTableColumnName}";
                                                        }
                                                        else
                                                        {
                                                            if (tableName == tableAttr.Name)
                                                                whereClause += $" inner join { schemaName + "." + tableName } as temp on { "temp." + relationColumnName }={tableAttr.Name}.{currentTableColumnName}";
                                                            else
                                                                whereClause += $" inner join { schemaName + "." + tableName } on { tableName + "." + relationColumnName }={tableAttr.Name}.{currentTableColumnName}";
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    if (filter.ignoreCase == true)
                                                    {
                                                        if (tableName == tableAttr.Name)
                                                            whereClause += $" left join { schemaName + "." + tableName } as temp on { "temp." + relationColumnName }={tableAttr.Name}.{currentTableColumnName}";
                                                        else
                                                            whereClause = $" left join { schemaName + "." + tableName } on { tableName + "." + relationColumnName }={tableAttr.Name}.{currentTableColumnName}";
                                                    }
                                                    else
                                                    {
                                                        if (tableName == tableAttr.Name)
                                                            whereClause += $" inner join { schemaName + "." + tableName } as temp on { "temp." + relationColumnName }={tableAttr.Name}.{currentTableColumnName}";
                                                        else
                                                            whereClause = $" inner join { schemaName + "." + tableName } on { tableName + "." + relationColumnName }={tableAttr.Name}.{currentTableColumnName}";
                                                    }

                                                }


                                                //whereClause = $"and { tableName + "." + columnName } like N'%{ (filter.value.ToString().Trim()) }%'";

                                            }
                                            ////if (where == null)
                                            ////{
                                            ////    where = $" where {tableAttr.Name}.IsDeleted = 0 and 1 = 1 ";
                                            ////}

                                            if (where == null)
                                            {
                                                where += $"{ tableName + "." + columnName} like N'%{ (filter.value.ToString().Trim()) }%'";
                                            }
                                            else
                                            {
                                                where += $" or { tableName + "." + columnName} like N'%{ (filter.value.ToString().Trim()) }%'";
                                            }

                                        }
                                        else
                                        {
                                            if (where == null)
                                            {
                                                where += $"{tableAttr.Name}.{ filter.field } like N'%{ (filter.value.ToString().Trim()) }%'";
                                            }
                                            else
                                            {
                                                where += $" or {tableAttr.Name}.{ filter.field } like N'%{ (filter.value.ToString().Trim()) }%'";
                                            }

                                        }
                                    }
                                    else if (filter.@operator.Equals("dosenotcontain"))
                                    {
                                        whereClause += $" and { filter.field } not like N'%{ (filter.value.ToString().Trim()) }%'";
                                    }
                                    else if (filter.@operator.Equals("startswith"))
                                    {
                                        whereClause += $" and { filter.field } like N'%{ (filter.value.ToString().Trim()) }'";
                                    }
                                    else if (filter.@operator.Equals("endswith"))
                                    {
                                        whereClause += $" and { filter.field } like N'{ (filter.value.ToString().Trim()) }%'";
                                    }
                                    else if (filter.@operator.Equals("isnull"))
                                    {
                                        whereClause += $" and { filter.field } is null";
                                    }
                                    else if (filter.@operator.Equals("isnotnull"))
                                    {
                                        whereClause += $" and { filter.field } is not null";
                                    }
                                    else if (filter.@operator.Equals("isempty"))
                                    {
                                        whereClause += $" and { filter.field } = N''";
                                    }
                                    else if (filter.@operator.Equals("isnotempty"))
                                    {
                                        whereClause += $" and { filter.field }  <> N''";
                                    }
                                });
                            }
                        }

                        if (where == null)
                        {
                           
                            where = $" where {tableAttr.Name}.IsDelete=0";
                        }
                        else
                        {
                           
                            where = $" where {tableAttr.Name}.IsDelete=0 and ({where})";
                        }

                        //if(state.startDate != null && state.startDate != "" && state.endDate != null && state.endDate != "")
                        //    where = $" and {tableAttr.Name}.StationId={state.stationId} and {tableAttr.Name}.IsDelete=0 and ({where})";


                        if (whereClause != null && where != null)
                        {

                            whereClause = whereClause + where;
                        }
                        else if (where != null)
                        {

                            whereClause = where;
                        }
                        ////else if (whereClause == null && state.sort.Length <= 0)
                        ////{
                        ////    whereClause = $"where {tableAttr.Name}.IsDeleted = 0 and 1 = 1 ";
                        ////}
                        //var resultCount = context.CountByRawSql($@"SELECT Count(id) FROM [{tableAttr.Schema}].[{tableAttr.Name}] { whereClause } ORDER BY 1 DESC", new KeyValuePair<string, object>("@X", state.take));


                        //raw = new RawSqlString($@"SELECT [{tableAttr.Name}].* FROM [{tableAttr.Schema}].[{tableAttr.Name}] { whereClause }
                        //      ORDER BY { sortClause }
                        //      OFFSET {state.take * ((state.skip > 0 ? state.skip - 1 : 0))} ROWS
                        //      FETCH NEXT { state.take } ROWS ONLY");

                        //if(whereClause== null || whereClause=="")  whereClause += $" where {tableAttr.Name}.IsDelete=0";
                        //else  whereClause += $" and {tableAttr.Name}.IsDelete=0";

                        string rawfilteronly = $@"SELECT distinct [{tableAttr.Name}].* FROM [{tableAttr.Schema}].[{tableAttr.Name}] { whereClause }";

                        string raw = $@"SELECT distinct [{tableAttr.Name}].* FROM [{tableAttr.Schema}].[{tableAttr.Name}] { whereClause }
                                  ORDER BY [{tableAttr.Schema}].[{tableAttr.Name}].Id DESC
                                 OFFSET {state.take * ((state.skip > 0 ? (state.skip / state.take) : 0))} ROWS
                                 FETCH NEXT { state.take } ROWS ONLY";

                        rawQuery.rawfilterpaging = raw;
                        rawQuery.rawfilteronly = rawfilteronly;
                        rawQuery.sortlause = sortClause;
                        return rawQuery;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        #endregion Methods

        #region Logging

        private bool IsEntityAudit(object entity)
        {
            bool restult = entity.GetType().IsDefined(typeof(AuditAttribute), false);
            return restult;
        }


        private byte[] FillCheckSum(object entity)
        {
            byte[] hash = null;
            MD5 hashAlgorithm = MD5.Create();
            string value;

            value = GetObjectStringValue(entity);
            hash = hashAlgorithm.ComputeHash(Encoding.Unicode.GetBytes(value));

            return hash;
        }

        private string GetObjectStringValue(object entity)
        {
            string value = null;

            foreach (PropertyInfo item in entity.GetType().GetProperties())
            {
                string columnName = item.Name;
                if (item.GetValue(entity, null) != null && !string.IsNullOrWhiteSpace(item.GetValue(entity, null).ToString()))
                {
                    value += item.GetValue(entity, null).ToString();
                }
            }

            return value;

        }

        #endregion Logging
    }

}


namespace Microsoft.EntityFrameworkCore
{
    public static partial class CustomExtensions
    {

        public static IQueryable<T> Include<T>(this IQueryable<T> source, IEnumerable<string> navigationPropertyPaths, Task<T> task)
            where T : class
        {
            return navigationPropertyPaths.Aggregate(source, (query, path) => query.Include(path));
        }

        public static IEnumerable<string> GetIncludePaths(this DbContext context, Type clrEntityType)
        {
            IEntityType entityType = context.Model.FindEntityType(clrEntityType);
            HashSet<INavigation> includedNavigations = new HashSet<INavigation>();
            Stack<IEnumerator<INavigation>> stack = new Stack<IEnumerator<INavigation>>();

            List<INavigation> entityNavigations = new List<INavigation>();
            foreach (var navigation in entityType.GetNavigations())
            {
                if (includedNavigations.Add(navigation))
                    entityNavigations.Add(navigation);
            }

            return entityNavigations.Select(q => q.Name);

        }

    }
}
