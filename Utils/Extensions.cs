namespace AOC.Utils;

public static class Extensions
{
    // given a list-of-lists where each list item has one or more elements, return the cartesian product (all combinations) of picking one element in each list,
    // as another list-of-lists.
    // example:  [[1,2], [3], [4,5,6]] => [[1,3,4], [1,3,5], [1,3,6], [2,3,4], [2,3,5], [2,3,6]]
    public static IEnumerable<IEnumerable<T>> CartesianProduct<T>(this IEnumerable<IEnumerable<T>> sequences)
    {
        IEnumerable<IEnumerable<T>> emptyProduct = [[]];
        return sequences.Aggregate(
            emptyProduct,
            (accumulator, sequence) =>
                from accseq in accumulator
                from item in sequence
                select accseq.Concat([item]));
    }
}