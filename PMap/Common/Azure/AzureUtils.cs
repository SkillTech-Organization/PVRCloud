using Microsoft.WindowsAzure.Storage.Table;
using PMapCore.Common.Attrib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PMapCore.Common.Azure
{
    public static class ContainsExtension
    {
        public static Expression<Func<TEntity, bool>> Contains<TEntity,
            TProperty>(this IEnumerable<object> values,
            Expression<Func<TEntity, TProperty>> expression)
        {
            // Get the property name
            var propertyName = ((PropertyInfo)((MemberExpression)expression.Body).Member).Name;

            // Create the parameter expression
            var parameterExpression = Expression.Parameter(typeof(TEntity), "e");

            // Init the body
            Expression mainBody = Expression.Constant(false);

            foreach (var value in values)
            {
                // Create the equality expression
                var equalityExpression = Expression.Equal(
                    Expression.PropertyOrField(parameterExpression, propertyName),
                    Expression.Constant(value));

                // Add to the main body
                mainBody = Expression.OrElse(mainBody, equalityExpression);
            }

            return Expression.Lambda<Func<TEntity, bool>>(mainBody, parameterExpression);
        }
    }

    public static class CloudExtensions
    {

        public static IEnumerable<TElement> StartsWith<TElement>
        (this CloudTable table, string partitionKey, string searchStr,
        string columnName = "RowKey") where TElement : ITableEntity, new()
        {
            if (string.IsNullOrEmpty(searchStr)) return null;

            char lastChar = searchStr[searchStr.Length - 1];
            char nextLastChar = (char)((int)lastChar + 1);
            string nextSearchStr = searchStr.Substring(0, searchStr.Length - 1) + nextLastChar;
            string prefixCondition = TableQuery.CombineFilters(
                TableQuery.GenerateFilterCondition(columnName, QueryComparisons.GreaterThanOrEqual, searchStr),
                TableOperators.And,
                TableQuery.GenerateFilterCondition(columnName, QueryComparisons.LessThan, nextSearchStr)
                );

            string filterString = TableQuery.CombineFilters(
                TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionKey),
                TableOperators.And,
                prefixCondition
                );
            var query = new TableQuery<TElement>().Where(filterString);
            return table.ExecuteQuery<TElement>(query);
        }
    }
    public static class TableQueryExtensions
    {
        public static TableQuery<TElement> AndWhere<TElement>(this TableQuery<TElement> @this, string filter)
        {
            @this.FilterString = TableQuery.CombineFilters(@this.FilterString, TableOperators.And, filter);
            return @this;
        }

        public static TableQuery<TElement> OrWhere<TElement>(this TableQuery<TElement> @this, string filter)
        {
            @this.FilterString = TableQuery.CombineFilters(@this.FilterString, TableOperators.Or, filter);
            return @this;
        }

        public static TableQuery<TElement> NotWhere<TElement>(this TableQuery<TElement> @this, string filter)
        {
            @this.FilterString = TableQuery.CombineFilters(@this.FilterString, TableOperators.Not, filter);
            return @this;
        }
    }

}
