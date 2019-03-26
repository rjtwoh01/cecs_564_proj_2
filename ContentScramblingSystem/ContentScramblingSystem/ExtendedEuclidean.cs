using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContentScramblingSystem
{
    class ExtendedEuclidean
    {
        private int a, b;

        public ExtendedEuclidean(int a, int b)
        {
            this.a = a;
            this.b = b;
        }

        public ExtendedEuclideanSolution calculate()
        {
            int x0 = 1, xn = 1;
            int y0 = 0, yn = 0;
            int x1 = 0;
            int y1 = 1;
            int q;
            int r = a % b;

            while (r > 0)
            {
                q = a / b;
                xn = x0 - q * x1;
                yn = y0 - q * y1;

                x0 = x1;
                y0 = y1;
                x1 = xn;
                y1 = yn;
                a = b;
                b = r;
                r = a % b;
            }

            return new ExtendedEuclideanSolution(xn, yn, b);
        }

    }
}
