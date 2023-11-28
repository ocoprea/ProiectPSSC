using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using ProiectPSSC.Domain;
using ProiectPSSC.Domain.Commands;
using ProiectPSSC.Domain.Models;
using ProiectPSSC.Domain.Workflows;
using System.Threading.Tasks;
using ProiectPSSC.Data;
using ProiectPSSC.Data.Repositories;

namespace ProiectPSSC
{
    internal class Program
    {
        private static string ConnectionString = "Server=localhost\\SQLEXPRESS;Database=ProiectPSSC;Trusted_Connection=True";
        static async Task<int> Main(string[] args)
        {
            using ILoggerFactory loggerFactory = ConfigureLoggerFactory();
            ILogger<TakingTheOrderWorkflow> logger = loggerFactory.CreateLogger<TakingTheOrderWorkflow>();
            var dbContextBuilder = new DbContextOptionsBuilder<OrderingContext>()
                                                .UseSqlServer(ConnectionString)
                                                .UseLoggerFactory(loggerFactory);
            OrderingContext context = new OrderingContext(dbContextBuilder.Options);

            int option;
            while (true)
            {
                option = await readOption();
                Console.Clear();
                switch (option)
                {
                    case 1:
                        await PreluareComandaHandler(context);
                        break;
                    case 2:
                        Console.WriteLine("Inca nu e implementat :)");
                        Console.WriteLine();
                        break;
                    case 3:
                        Console.WriteLine("Inca nu e implementat :)");
                        Console.WriteLine();
                        break;
                    case 4:
                        Console.WriteLine("Inca nu e implementat :)");
                        Console.WriteLine();
                        break;
                    case 0:
                        return 0;
                    default:
                        Console.WriteLine("Optiune incorecta");
                        Console.WriteLine();
                        break;
                }
            }
        }
        static async Task<int> readOption()
        {
            int option = -1;
            Console.WriteLine("1.Preluare comanda");
            Console.WriteLine("2.Modificare comanda");
            Console.WriteLine("3.Finalizare comanda (generare factura)");
            Console.WriteLine("4.Expediere comanda");
            Console.WriteLine("0.Iesire");
            Console.Write("Optiunea dvs: ");
            if (int.TryParse(Console.ReadLine(), out option))
                return option;
            return -1;
        }
        static async Task PreluareComandaHandler(OrderingContext context)
        {
            ProductsRepository productsRepo = new(context);

            var listOfProducts = await readProducts();
            TakingTheOrderCommand command = new(listOfProducts.ToArray());
            TakingTheOrderWorkflow workflow = new(productsRepo);

            var result = await workflow.ExecuteAsync(command);
            result.Match(whenTakingTheOrderSuccededEvent: @event =>
                        {
                            Console.Clear();
                            Console.WriteLine("Comanda a fost preluata cu succes !");
                            Console.WriteLine();
                            return @event;
                        },
                         whenTakingTheOrderFailedEvent: @event =>
                         {
                             //Console.Clear();
                             Console.WriteLine();
                             Console.WriteLine("Comanda nu a fost preluata !");
                             Console.WriteLine(@event.Reason);
                             Console.WriteLine();
                             return @event;
                         });
        }
        private static ILoggerFactory ConfigureLoggerFactory()
        {
            return LoggerFactory.Create(builder =>
                                builder.AddSimpleConsole(options =>
                                {
                                    options.IncludeScopes = true;
                                    options.SingleLine = true;
                                    options.TimestampFormat = "hh:mm:ss ";
                                })
                                .AddProvider(new Microsoft.Extensions.Logging.Debug.DebugLoggerProvider()));
        }
        static async Task<List<UnvalidatedProduct>> readProducts()
        {
            List<UnvalidatedProduct> unvalidatedProducts_ = new ();
            Dictionary<string, string> products = new();
            do
            {
                string  ? id, quantity;
                string ? existingQuantity;

                Console.Write("Id produs: ");
                id = Console.ReadLine();
                if (string.IsNullOrEmpty(id))
                    break;

                Console.Write("Caltitate: ");
                quantity = Console.ReadLine();
                if (string.IsNullOrEmpty(quantity))
                    break;

                if (products.TryGetValue(id, out existingQuantity))
                    products[id] = (int.Parse(products[id]) + int.Parse(quantity)).ToString();
                else
                    products[id] = quantity;
            } while (true);

            foreach (var item in products)
                unvalidatedProducts_.Add(new UnvalidatedProduct(item.Key, item.Value));

            return unvalidatedProducts_;
        }
    }
}