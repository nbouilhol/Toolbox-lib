namespace Utilities
{
    public static class ArrayExtension
    {
        //public static T[,] ToMultidimensional<T>(this T[][] jaggedArray)
        //{
        //    Contract.Requires(jaggedArray != null);

        //    int rows = jaggedArray.Length;
        //    int cols = jaggedArray.Max(subArray => subArray.Length);
        //    T[,] array = new T[rows, cols];

        //    for (int i = 0; i < rows; i++)
        //        for (int j = 0; j < cols; j++)
        //            array[i, j] = jaggedArray[i][j];

        //    return array;
        //}

        //public static T[,] ToMultidimensional<T>(this IEnumerable<IEnumerable<T>> jaggedArray)
        //{
        //    Contract.Requires(jaggedArray != null);

        //    int rows = jaggedArray.Count();
        //    int cols = jaggedArray.Max(subArray => subArray.Count());
        //    T[,] array = new T[rows, cols];

        //    for (int i = 0; i < rows; i++)
        //    {
        //        for (int j = 0; j < cols; j++)
        //        {
        //            IEnumerable<T> element = jaggedArray.ElementAt(i);
        //            if (element != null)
        //                array[i, j] = element.ElementAt(j);
        //        }
        //    }

        //    return array;
        //}

        //public static T[,] ToMultidimensional2<T>(this IList<IList<T>> jaggedArray)
        //{
        //    Contract.Requires(jaggedArray != null);

        //    int rows = jaggedArray.Count;
        //    int cols = jaggedArray.Max(subArray => subArray.Count);
        //    T[,] array = new T[rows, cols];

        //    for (int i = 0; i < rows; i++)
        //    {
        //        for (int j = 0; j < cols; j++)
        //        {
        //            array[i, j] = jaggedArray[i][j];
        //        }
        //    }

        //    return array;
        //}
    }
}