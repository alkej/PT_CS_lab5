using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PT_lab5
{
    class BinomialCoef
    {
        public int N { get; set; }
        public int K { get; set; }
        public BinomialCoef(int N, int K)
        {
            this.N = N;
            this.K = K;
        }

        private int Numerator()
        {
            int output = 1;

            for (int i = N; i >= N - K + 1; i--)
            {
                output *= i;
            }

            return output;

        }

        private int Denumerator()
        {
            int sum = 1;
            for (int i = 2; i <= K; i++)
            {
                sum *= i;
            }

            return sum;
        }

        public int CalculateByTask()
        {
            Task<int> numTask = Task.Factory.StartNew<int>(
                (obj) => Numerator(),
                100
                );

            Task<int> denumTask = Task.Factory.StartNew<int>(
                (obj) => Denumerator(),
                100 
                );

            numTask.Wait();
            denumTask.Wait();

            return numTask.Result/denumTask.Result;
        }

        public int CalculateByDelegate()
        {

            Func<int> deg1 = Numerator;
            Func<int> deg2 = Denumerator;

            IAsyncResult num = deg1.BeginInvoke(null, null);
            IAsyncResult denum = deg2.BeginInvoke(null, null);

            while (num.IsCompleted == false || denum.IsCompleted == false)
            {
                // tu można coś zrobić
            }

            int output = deg1.EndInvoke(num) / deg2.EndInvoke(denum);

            return output;
        }

        public async Task<int> CalculateAsync()
        {

            int num = await Task.Run(Numerator);
            int denum = await Task.Run(Denumerator);

            return num / denum;
        }


    }
}
