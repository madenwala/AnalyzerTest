using System.Text.RegularExpressions;

namespace ConsoleApp1
{
    class R   
    {
        void M()
        {
            Regex.Match("my text", @"\pXXX");
        }
    }
}