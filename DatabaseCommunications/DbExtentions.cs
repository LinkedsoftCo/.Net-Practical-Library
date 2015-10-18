using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseCommunications
{
    public static class DbExtentions
    {
        // adds parameters to a DbCommand object
        public static void AddParameters(DbCommand command, object[] parms)
        {
            if (parms != null && parms.Length > 0)
            {
                for (var i = 0; i < parms.Length; i += 2)
                {
                    var name = parms[i].ToString();

                    // no empty strings to the database

                    if (parms[i + 1] is string && (string)parms[i + 1] == "")
                        parms[i + 1] = null;

                    // if null, set to DbNull

                    var value = parms[i + 1] ?? DBNull.Value;

                    var dbParameter = command.CreateParameter();
                    dbParameter.ParameterName = name;
                    dbParameter.Value = value;

                    command.Parameters.Add(dbParameter);
                }
            }
        }

        // concatenates SQL and ORDER BY clauses into a single string
        public static string OrderBy(this string sql, string sortExpression)
        {
            if (string.IsNullOrEmpty(sortExpression))
                return sql;

            return sql + " ORDER BY " + sortExpression;
        }


        // takes an enumerable source and returns a comma separate string.
        // handy for building SQL Statements
        //(for example with IN () statements) from object collections
        public static string CommaSeparate<T, TU>(this IEnumerable<T> source, Func<T, TU> func)
        {
            return string.Join(",", source.Select(str => func(str).ToString()).ToArray());
        }
    }
}
