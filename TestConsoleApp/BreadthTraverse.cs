using System.Collections.Generic;

namespace TestConsoleApp
{
    public class BreadthTraverse
    {
        public (bool, List<int>) Escape(List<List<int>> galaxy, int fuel)
        {
            var escaped = false;

            var result = FindMinSumPath(galaxy);

            if (result.Item1 != null && result.Item2 != int.MaxValue)
            {
                if (result.Item2 <= fuel)
                {
                    escaped = true;
                }
            }
            
            return (escaped, result.Item1);
        }

        private (List<int>, int) FindMinSumPath(List<List<int>> levels)
        {
            if (levels.Count == 0 || levels[0].Count == 0)
                return (new List<int>(), 0);

            var queue = new Queue<(int level, int index, int sum, List<int> path)>();
            queue.Enqueue((0, 0, levels[0][0], new List<int> { levels[0][0] }));

            int minSum = int.MaxValue;
            List<int> minPath = null;

            while (queue.Count > 0)
            {
                var (level, index, sum, path) = queue.Dequeue();

                // If it's the last level, consider for min
                if (level == levels.Count - 1)
                {
                    if (sum < minSum)
                    {
                        minSum = sum;
                        minPath = new List<int>(path);
                    }
                    continue;
                }

                // Add left child (same index)
                int nextLevel = level + 1;
                if (index < levels[nextLevel].Count)
                {
                    int leftVal = levels[nextLevel][index];
                    var newPath = new List<int>(path) { leftVal };
                    queue.Enqueue((nextLevel, index, sum + leftVal, newPath));
                }

                // Add right child (index + 1)
                if (index + 1 < levels[nextLevel].Count)
                {
                    int rightVal = levels[nextLevel][index + 1];
                    var newPath = new List<int>(path) { rightVal };
                    queue.Enqueue((nextLevel, index + 1, sum + rightVal, newPath));
                }
            }

            return (minPath, minSum);
        }
    }
}
