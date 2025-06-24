using TestConsoleApp.Interfaces;

namespace TestConsoleApp
{
    public class FactorialRecursive : IFactorial
    {
        public long Calculate(int n)
        {
            if (n <= 1)
            {
                return 1;
            }

            return n * Calculate(n - 1);
        }
    }

    public class FactorialIterative : IFactorial
    {
        public long Calculate(int n)
        {
            long result = 1;
            for (var i = n; i > 0; i--)
            {
                result *= i;
            }
            return result;
        }
    }
}
