using ProiectPSSC.Domain;
using ProiectPSSC.Domain.Commands;
using ProiectPSSC.Domain.Models;
using ProiectPSSC.Domain.Workflows;

namespace ProiectPSSC
{
    internal class Program
    {
        static async Task<int> Main(string[] args)
        {
            int option;
            while (true)
            {
                option = await readOption();
                Console.Clear();
                switch (option)
                {
                    case 1:
                        await PreluareComandaHandler();
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
        static async Task PreluareComandaHandler()
        {
            var listOfProducts = await readProducts();
            TakingTheOrderCommand command = new(listOfProducts.ToArray());
            TakingTheOrderWorkflow workflow = new();
            var result = await workflow.ExecuteAsync(command);
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
                Console.Write("Caltitate: ");
                quantity = Console.ReadLine();
                if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(quantity))
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