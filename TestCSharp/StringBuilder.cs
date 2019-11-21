using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class SB
    {
        public void M()
        {
            //StringBuilder sb = new StringBuilder();
            //sb.Append("Hello");
            //sb.Append(" W");
            //sb.Append("o"); // Show analyzer
            //sb.Append('r');
            //string s = "l";
            //sb.Append(s); // Show analyzer

            //char c = 'a';
            //for (int i = 0; i < 3; i++)
            //    c += 'b';
            
            //sb.Append('a' + 'b');
            //sb.Append('a' + "b");
            //sb.Append("a" + 'a' + "c");


            //char testChar = 'd';
            //sb.Append(testChar);
            //int number = 1;
            //sb.Append(number);
            //sb.Append(number.ToString()); // Show analyzer
            //double d = 2.0;
            //sb.Append(d);
            //sb.Append(d.ToString()); // Show analyzer

            //System.Diagnostics.Debug.WriteLine(sb);

            StringBuilder testSB = null;
            //testSB = new StringBuilder("Hello");
            //testSB = new StringBuilder("H"); // Show analyzer
            testSB = new StringBuilder('H'); // Show analyzer
            //testSB = new StringBuilder("Hello", 1);
            //testSB = new StringBuilder("Hello");
        }
    }
}