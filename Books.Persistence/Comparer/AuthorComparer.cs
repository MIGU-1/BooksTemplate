using Books.Core.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Books.Persistence.Comparer
{
    public class AuthorComparer : IEqualityComparer<Author>
    {
        public bool Equals([AllowNull] Author x, [AllowNull] Author y)
        {
            //Check whether the compared objects reference the same data.
            if (ReferenceEquals(x, y)) return true;

            //Check whether any of the compared objects is null.
            if (x is null || y is null)
                return false;

            //Check whether the products' properties are equal.
            return x.Name == y.Name;
        }

        public int GetHashCode([DisallowNull] Author author)
        {
            //Check whether the object is null
            if (author is null) return 0;

            //Get hash code for the Name field if it is not null.
            int hashProductName = author.Name == null ? 0 : author.Name.GetHashCode();

            //Calculate the hash code for the product.
            return hashProductName;
        }
    }
}
