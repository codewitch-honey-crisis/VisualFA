using Tests;
namespace Scratch
{
    internal class Program
    {
        static void Main(string[] args)
        {
            foreach(var match in TestSource.Calc(new StringReader("Fuck you")))
            {
                Console.WriteLine(match);
            }
            Console.WriteLine("-----------------------------------");
            foreach (var match in TestSource2.Calc("Fuck you"))
            {
                Console.WriteLine(match);
            }
        }
    }
}
