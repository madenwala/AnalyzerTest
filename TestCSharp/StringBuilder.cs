using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class SB
    {
        public string M()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Hello");
            sb.Append(" W");
            sb.Append("o"); // Show analyzer
            sb.Append('r');
            string s = "l";
            sb.Append(s); // Show analyzer
            char c = 'd';
            sb.Append(c);
            int i = 1;
            sb.Append(i);
            sb.Append(i.ToString()); // Show analyzer
            double d = 2.0;
            sb.Append(d);
            sb.Append(d.ToString()); // Show analyzer

            System.Diagnostics.Debug.WriteLine(sb);
            return sb.ToString();
        }
    }
}