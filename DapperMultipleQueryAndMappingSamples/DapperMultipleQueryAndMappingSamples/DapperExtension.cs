using System;
using System.Collections.Generic;
using System.Data;
using Dapper;
using DapperMultipleQueryAndMappingSamples.Model.Data;

namespace DapperMultipleQueryAndMappingSamples
{
    public static class DapperExtension
    {
        public static IEnumerable<T> PolymorphicQuery<T>(
            this IDbConnection cnn,
            string sql,
            string discriminator,
            object param = null)
        {
            var result = new List<T>();

            using (var reader = cnn.ExecuteReader(sql, param))
            {
                var parsers = GenerateParsers<T>(reader);

                var typeColumnIndex = reader.GetOrdinal(discriminator);

                while (reader.Read())
                {
                    result.Add(parsers[reader.GetString(typeColumnIndex)](reader));
                }
            }

            return result;
        }

        private static Dictionary<string, Func<IDataReader, T>> GenerateParsers<T>(IDataReader reader)
        {
            if (typeof(T) == typeof(Product))
            {
                return new Dictionary<string, Func<IDataReader, T>>
                           {
                               ["CellPhone"] = reader.GetRowParser<T>(typeof(CellPhone)),
                               ["Book"] = reader.GetRowParser<T>(typeof(Book))
                           };
            }

            throw new ArgumentOutOfRangeException();
        }
    }
}