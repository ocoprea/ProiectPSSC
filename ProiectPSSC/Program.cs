using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using ProiectPSSC.Domain;
using ProiectPSSC.Domain.Commands;
using ProiectPSSC.Domain.Models;
using ProiectPSSC.Domain.Workflows;
using System.Threading.Tasks;
using ProiectPSSC.Data;
using ProiectPSSC.Data.Repositories;
using ProiectPSSC.Data.Models;
using System.Security.Cryptography;
using LanguageExt;
using LanguageExt.ClassInstances;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.CodeAnalysis.CSharp.Syntax;

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
            var dbContextBuilder2 = new DbContextOptionsBuilder<InvoiceContext>()
                                                .UseSqlServer(ConnectionString)
                                                .UseLoggerFactory(loggerFactory);

            int option;
            while (true)
            {
                option = await readOption();
                Console.Clear();
                switch (option)
                {
                    case 1:
                        await PreluareComandaHandler(dbContextBuilder);
                        break;
                    case 2:
                        Console.WriteLine("Inca nu e implementat :)");
                        Console.WriteLine();
                        break;
                    case 3:
                        await FinalizareComandaHandler(dbContextBuilder2);
                        Console.WriteLine();
                        break;
                    case 4:
                        await ShippingHandler();
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

        static async Task FinalizareComandaHandler(DbContextOptionsBuilder<InvoiceContext> dbContextBuilder)
        {
            InvoiceContext context = new InvoiceContext(dbContextBuilder.Options);
            
            InvoiceRepository invoiceRepo = new(context);

            var invoice = await readInfo();

            GenerateTheInvoiceCommand command = new(invoice);
            FinishingTheOrderWorkflow workflow = new(invoiceRepo);

            var result = await workflow.ExecuteAsync(command);
            result.Match(whenGenerateTheInvoiceSuccededEvent: @event =>
            {
                Console.Clear();
                Console.WriteLine("Factura cu id-ul:");
                Console.WriteLine(@event.InvoiceId);
                Console.WriteLine(" a fost generata cu succes");
                return @event;
            },
            whenGenerateTheInvoiceFailedEvent: @event =>
             {
                 Console.WriteLine("Factura nu a fost generata.");
                 return @event;
             });
        }

        static async Task PreluareComandaHandler(DbContextOptionsBuilder<OrderingContext> dbContextBuilder)
        {
            OrderingContext context = new OrderingContext(dbContextBuilder.Options);
            ProductsRepository productsRepo = new(context);
            OrdersRepository ordersRepo = new(context);
            OrderedProductRepository orderedProductsRepo = new(context);

            var listOfProducts = await readProducts();
            if (listOfProducts.Count() == 0)
            {
                Console.Clear();
                return;
            }

            TakingTheOrderCommand command = new(listOfProducts.ToArray());
            TakingTheOrderWorkflow workflow = new(productsRepo, ordersRepo, orderedProductsRepo);
            
            var result = await workflow.ExecuteAsync(command);
            result.Match(whenTakingTheOrderSuccededEvent: @event =>
                        {
                            Console.Clear();
                            Console.Write("Comanda cu ID: ");
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.Write(@event.OrderId);
                            Console.ResetColor();
                            Console.WriteLine(" a fost generata cu succes !");
                            Console.WriteLine();
                            return @event;
                        },
                         whenTakingTheOrderFailedEvent: @event =>
                         {
                             Console.Clear();
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
        static async Task<List<UnvalidateInvoice>> readInfo()
        {
            List < UnvalidateInvoice > unvalidatedInvoice_ = new ();
            string ? id, adress, payMetod;
            Console.Write("Id comanda: ");
            id = Console.ReadLine();
            Console.Write("Adressa de livrare: ");
            adress = Console.ReadLine();
            Console.Write("Metoda de plata: ");
            payMetod = Console.ReadLine();
            unvalidatedInvoice_.Add(new UnvalidateInvoice(id, adress, payMetod));
            return unvalidatedInvoice_;

        }

        static async Task ShippingHandler()
        {
            var listOfOrders = await readOrders();
            if (listOfOrders.Count == 0)
            {
                Console.Clear();
                return;
            }

            ShippingWorkflow workflow = new ShippingWorkflow();
            var result = workflow.Execute(listOfOrders, order => ValidateOrder(order));

            // Handle the result
            result.SuccessEvents.ForEach(successEvent =>
            {
                Console.WriteLine($"Expedierea pentru comanda {successEvent.OrderId} a fost generata cu succes.");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"AWB: {successEvent.Awb}");
                Console.ResetColor();
            });

            result.FailureEvents.ForEach(failureEvent =>
            {
                Console.WriteLine($"Expedierea pentru comanda {failureEvent.OrderId} nu a fost facuta.");
                Console.WriteLine($"Motiv: {failureEvent.Reason}");
                Console.WriteLine();
            });
        }

        static async Task<List<Order>> readOrders()
        {
            List<Order> listOfOrders = new();
            do
            {
                var orderIdString = ReadValue("Id-ul comenzii: ");
                if (string.IsNullOrEmpty(orderIdString))
                {
                    break;
                }

                if (OrderId.TryParse(orderIdString, out var orderId))
                {
                    listOfOrders.Add(new Order(orderId));
                }
                else
                {
                    Console.WriteLine("Invalid order ID. Please enter a valid Order ID.");
                }
            } while (true);
            return listOfOrders;
        }

        private static bool ValidateOrder(Order order)
        {
            // Check if the order ID exists in the database
            return true;
        }

        private static string? ReadValue(string prompt)
        {
            Console.Write(prompt);
            return Console.ReadLine();
        }
    }
}