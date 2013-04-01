using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StringFormat;

namespace Example
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var cmd = new StringFormater(@"www.g.cn?{\{\[pv=\@@pv$}&c=2[&a=@a$][&b=@b$321]&d=abc"))
            {
                cmd.ClearParameter();
                cmd.AddParameter("pv", 1);
                cmd.AddParameter("a", 2);
                cmd.AddParameter("b", 3);
                var strTmp = cmd.Format();
                Console.WriteLine(strTmp);

                cmd.ClearParameter();
                cmd.AddParameter("pv", 1);
                cmd.AddParameter("a", 2);

                strTmp = cmd.Format();
                Console.WriteLine(strTmp);
            }
            Console.WriteLine("press any key continue.");
            Console.ReadKey();
        }
    }
}
