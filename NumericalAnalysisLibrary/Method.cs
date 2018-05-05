using System;

namespace NumericalAnalysisLibrary
{
    public class Method
    {
        protected static Random rnd = new Random();

        protected delegate bool Condition(double func);

        protected static double RandomNumber(double a, double b)
        {
            double epsilan = Constants.epsilan;
            return rnd.Next(Convert.ToInt32(a / epsilan), Convert.ToInt32(b / epsilan)) * epsilan;
        }
        protected static double SuitableRandomArgumnent(Condition cond, double a, double b)
        {
            double arg;

            do
            {
                arg = RandomNumber(a, b);
            } while (!cond(arg));

            return arg;
        }
    }
}
