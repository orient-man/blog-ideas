# Binary Search Extended

For example supports finding _first_ greater or equal than x, or _last_ less or equal.

Usage:

    public int TryFindLastLessOrEqual(int[] values, int x)
    {
        return BinarySearch.FirstOrDefault(
            values,
            v => v <= x ? BinarySearch.NextStep.FoundAndGoRight : BinarySearch.NextStep.GoLeft);
    }

    public int TryFindLastGreaterOrEqual(int[] values, int x)
    {
        return BinarySearch.FirstOrDefault(
            values,
            v => v >= x ? BinarySearch.NextStep.FoundAndGoLeft : BinarySearch.NextStep.GoRight);
    }

    public int TryFind(int[] values, int x)
    {
        return BinarySearch.FirstOrDefault(
            values,
            v =>
            {
                switch (v.CompareTo(x))
                {
                    case 0:
                        return BinarySearch.NextStep.FoundAndStop;
                    case -1:
                        return BinarySearch.NextStep.GoRight;
                    default:
                        return BinarySearch.NextStep.GoLeft;
                }
            });
    }