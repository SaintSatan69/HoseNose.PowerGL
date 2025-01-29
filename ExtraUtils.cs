
namespace HoseRenderer
{
    namespace ExtraUtils
    {
        class DebugMessages
        {
            public static void PrintDebugMSG(string message)
            {
                string finalmessage = "DEBUG:" + message;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(finalmessage);
                Console.ResetColor();
            }//apparently visual studio didn't like these on the powershell side 
            public static void PrintDebugMSG(string message,int ActuallyPrint)
            {
                if (ActuallyPrint == 1)
                {
                    string finalmessage = "DEBUG:" + message;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(finalmessage);
                    Console.ResetColor();
                }
                else
                {
                    return;
                }
            }
        }
    }
}
