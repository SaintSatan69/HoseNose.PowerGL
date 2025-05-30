
using System.Diagnostics;

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
        class ArrayBuffs
        {
            public static T[] ResizeArray<T>(T[] Array, int Size)
            {
                if (Array.Length == Size)
                {
                    return Array;
                }
                T[] temp_array = new T[Size];
                if (Array.Length < Size)
                {
                    int diff_size = Size - Array.Length;
                    Debugger.Log(1, "Array", $"New Array Size is smaller then current array size data loss will occur if there is anything in the end of the array. Diff:{diff_size}{Environment.NewLine}");
                    Console.WriteLine("New Array Size is smaller then current array");
                    for (int i = 0; i < Size; i++)
                    {
                        temp_array[i] = Array[i];
                    }
                }
                else
                {
                    for (int i = 0; i < Array.Length; i++)
                    {
                        temp_array[i] = Array[i];
                    }
                }
                return temp_array;
            }
        }
    }
}
