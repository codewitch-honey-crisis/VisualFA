using Tests;
namespace Scratch
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var exp = "the 10 quick brown foxes jumped over 1.5 lazy dogs";
            foreach (var match in TestSource.Calc(new StringReader(exp)))
            {
                Console.WriteLine(match);
            }
            Console.WriteLine("-----------------------------------");
            foreach (var match in TestSource2.Calc(exp))
            {
                Console.WriteLine(match);
            }
        }
    }
}
