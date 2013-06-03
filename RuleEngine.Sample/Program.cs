#region Usings

using System;

#endregion

namespace Yea.RuleEngine.Sample
{
    public class Program
    {
        [STAThread]
        public static void Main()
        {
            var account = new Account(1);
            account.Charge(100);
            account.Consume(50);
            account.Charge(1000);
            account.Consume(100);
            account.Charge(10000);
            account.Consume(1000);
            account.Charge(100000);
            account.Consume(10000);

            Console.WriteLine("Money:{0} ,Discount:{1}", account.Money, account.Discount);
            Console.ReadKey();
        }
    }
}