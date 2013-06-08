#region Usings

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

#endregion

namespace Yea.DataTypes.ExtensionMethods
{
    /// <summary>
    ///     MatchCollection extensions
    /// </summary>
    public static class MatchCollectionExtensions
    {
        #region Functions

        #region Where

        /// <summary>
        ///     Gets a list of items that satisfy the predicate from the collection
        /// </summary>
        /// <param name="collection">Collection to search through</param>
        /// <param name="predicate">Predicate that the items must satisfy</param>
        /// <returns>The matches that satisfy the predicate</returns>
        public static IEnumerable<Match> Where(this MatchCollection collection, Predicate<Match> predicate)
        {
            if (collection.IsNull())
                return null;
            Guard.NotNull(predicate, "predicate");
            var matches = new List<Match>();
            foreach (Match item in collection)
                if (predicate(item))
                    matches.Add(item);
            return matches;
        }

        #endregion

        #endregion
    }
}