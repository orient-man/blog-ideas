using System;
using System.Collections.Generic;

namespace App
{
    public static class BinarySearch
    {
        public static T FirstOrDefault<T>(
            IReadOnlyList<T> array,
            Func<T, NextStep> pred)
        {
            var min = 0;
            var max = array.Count - 1;
            var candidate = default(T);
            while (min <= max)
            {
                var mid = (min + max) / 2;
                var value = array[mid];

                switch (pred(value))
                {
                    case NextStep.FoundAndStop:
                        return value;

                    case NextStep.FoundAndGoLeft:
                        candidate = value;
                        max = mid - 1;
                        break;
                    case NextStep.GoLeft:
                        max = mid - 1;
                        break;
                    case NextStep.FoundAndGoRight:
                        candidate = value;
                        min = mid + 1;
                        break;
                    case NextStep.GoRight:
                        min = mid + 1;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return candidate;
        }

        public enum NextStep
        {
            FoundAndStop,
            FoundAndGoLeft,
            FoundAndGoRight,
            GoLeft,
            GoRight
        }
    }
}