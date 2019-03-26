using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContentScramblingSystem
{
    class ExtendedEuclideanSolution
    {
        private int x, y, d;

        public int X
        {
            get
            {
                return this.x;
            }
        }

        public int Y
        {
            get
            {
                return this.y;
            }
        }

        public int D
        {
            get
            {
                return this.d;
            }
        }

        public ExtendedEuclideanSolution(int x, int y, int d)
        {
            this.x = x;
            this.y = y;
            this.d = d;
        }
    }
}