using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jenzabar.JX.Core.Interfaces;
using Jenzabar.JX.Core;

namespace MyConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WindowWidth = 179;
            Console.WindowHeight = 52;

            Setup.Mode mode = Setup.Mode.Dev;

            foreach (string arg in args)
            {
                if (arg.ToLower() == "live")
                {
                    mode = Setup.Mode.Live;
                }
            }

            Setup setup = new Setup(new[] { typeof(Program).Assembly.FullName }, mode);

            if(!setup.IsValid())
            {
                Console.WriteLine("Setup Not Valid");
                Console.WriteLine("press key");
                Console.ReadKey();
                return;
            }

            IStructureMapWrapper profile = setup.ObjectFactory.GetProfile("ARM");
            AttributeLoader updater = profile.GetInstance<AttributeLoader>();
            updater.Run();

            Console.WriteLine("Process complete, press any key to close.");
            Console.ReadKey();
        }
    }
}
