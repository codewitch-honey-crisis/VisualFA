using VisualFA;
namespace Scratch2
{
    internal class Program
   {
        static void Main(string[] args)
        {
            var commentBlock = FA.Parse(@"\/\*", 0);//, Symbol = "commentBlock", BlockEnd = @"\*\/")]
            var commentLine = FA.Parse(@"\/\/[^\n]*", 1);
            var whiteSpace = FA.Parse(@"[ \t\r\n]+", 2);
            var identifier = FA.Parse(@"[A-Za-z_][A-Za-z0-9_]*", 3);
            var number = FA.Parse(@"(0|([1-9][0-9]*))((\.[0-9]+[Ee]\-?[1-9][0-9]*)?|\.[0-9]+)", 3);
            var plus = FA.Parse(@"\+",4);
            var minus = FA.Parse(@"\-",5);
            var multiply = FA.Parse(@"\*",6);
            var divide = FA.Parse(@"\/",7);
            var modulo = FA.Parse(@"%", 8);

            Console.WriteLine("Hello, World!");
        }
    }
}
