using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DiscontMD.BusinessLogic;
using DiscontMD.BusinessLogic.Bus;
using DiscontMD.BusinessLogic.Bus.Commands;
using DiscontMD.BusinessLogic.DomainModel;

namespace DiscontMD.Console
{
    class Program
    {
        static void  Main(string[] args)
        {
            Registry.Init(new ConsoleCommonInfrastructureProvider());

            AsyncHelpers.RunSync(Print);
            //ProcessCommands();
        }

        private static async Task Print()
        {
            var printPagesPackCommandHandler = new PrintPagesPackCommandHandler(Guid.Empty);
            await printPagesPackCommandHandler.Process(new PrintPagesPackCommand(new Guid("F4273903-9798-4B28-BD54-001772498FBB")));
        }
        private static void ProcessCommands()
        {
            do
            {
                var result = new CommandExecutor(System.Console.WriteLine).TakeOneAndExecute().Result;
                // if nothing processed
                if (!result) Thread.Sleep(2000); // then sleep 2 sec
            } while (true);
        }

        private static void Do3()
        {
            Registry.Current.Services.Card.AssignPackToStore(1, new Guid("BF1A81D8-BA91-46AA-82EB-973586485A44"));
        }
        private static void Do2()
        {
            List<int> list = new List<int>();
            for (int i = 1000; i < 10000; i++)
            {
                list.Add(i);
            }
            var MyArray = list.ToArray();
            Random rnd = new Random();
            int[] MyRandomArray = MyArray.OrderBy(x => rnd.Next()).ToArray();
            int[] MyRandomArray2 = MyRandomArray.OrderBy(x => rnd.Next()).ToArray();
            foreach (var i in MyRandomArray2)
            {
                Registry.Current.Data.CardPacks.Save(new CardPack {NumBase = i,Printed = false}).Wait();
            }
        }
        private static void Do1()
        {
            Store store = new Store
            {
                Domain= "avtomoika"
            };

            store.Settings.AccumulativeRules = new List<PriceRule>
            {
                new PriceRule {From = 0, Discount = 2},
                new PriceRule {From = 1000, Discount = 3},
                new PriceRule {From = 2000, Discount = 4},
                new PriceRule {From = 5000, Discount = 5},
                new PriceRule {From = 10000, Discount = 7},
            };
            store.Settings.Type = DiscountType.Accumulative;
            store.Name= "Автомойка";
            Registry.Current.Data.Stores.Save(store).Wait();
            Registry.Current.Services.Card.ActivateCard(111111, "Николай", "niklegaloff@gmail.com").Wait();
        }
    }
}
