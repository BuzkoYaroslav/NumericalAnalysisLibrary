using NumericalAnalysisLibrary.Functions;

namespace NumericalAnalysisLibrary.IntegralApproximation
{
    public enum RectangularMethodType { Left = 1, Right, Central};
    public class RectangularMethod : IntegralMethod
    {
        private RectangularMethodType type;

        public RectangularMethod(RectangularMethodType type)
        {
            this.type = type;
        }

        public double Solve(MathFunction func, double a, double b, int n)
        {
            if (a == b) return 0;
            if (a > b)
            {
                double tmp = a;
                a = b; b = tmp;
            }

            double result = 0;
            double difference = (b - a) / n;

            for (int i = 0; i < n; i++)
            {
                result += func.Calculate(SuitableXForIndex(a + difference * i, a + difference * (i + 1)));
            }
            result *= difference;

            return result;
        }

        private double SuitableXForIndex(double left, double right)
        {
            switch (type)
            {
                case RectangularMethodType.Left:
                    return left;
                case RectangularMethodType.Right:
                    return right;
                case RectangularMethodType.Central:
                    return (left + right) / 2;
                default:
                    return 0;
            }
        }
    }
}
