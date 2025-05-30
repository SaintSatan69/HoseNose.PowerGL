using System.Text;
namespace HoseRenderer
{
    /// <summary>
    /// Powershell was having a stroke with types when generating the long number string as for a seed so im adding it to the engine as an extra tool
    /// </summary>
    public class SeedGenerator
    {
        /// <summary>
        /// A function that can take in a string to generate a seed for uses to make their own terrarian generators or other things that need a seed
        /// </summary>
        /// <param name="StringSeedStarter"></param>
        /// <returns>A string that is a long number representing a seed</returns>
        public static string GenerateSeed(string? StringSeedStarter)
        {
            string seed = "";
            string intermediate_data = "";
            //just a few more strings that hold data for a few more seeded randoms to better use the input data and actually allow seeds to make more unique seeds
            string intermediate_data_0_9 = "";
            string intermediate_data_9_18 = "";
            string intermediate_data_18_27 = "";
            string intermediate_data_27_36 = "";
            int num_randoms = 0;
            //this random is seeded with 69696969 to make sure if the string is somehow less then 20 that it will still generate the same seed value
            Random random = new Random(69696969);
            Random? seeded_random = null;
            Random? seeded_random_2 = null;
            Random? seeded_random_3 = null ;
            Random? seeded_random_4 = null;
            if (StringSeedStarter == null)
            {
                Console.WriteLine("SeedStarted not provided will generate a random number");
                for (int i = 0; i < 20; i++)
                {
                    string value = random.Next(00, 99).ToString();
                    Console.Write(value);
                    intermediate_data += value;
                }
            }
            else
            {
                if (StringSeedStarter.Length >= 20) {
                    Console.WriteLine("String Long enough Making seed");
                    foreach (byte charbyte in Encoding.UTF8.GetBytes(StringSeedStarter))
                    {
                        Console.Write(Convert.ToUInt32(charbyte));
                        intermediate_data += Convert.ToInt32(charbyte).ToString();
                    }
                }
                else
                {
                    byte[] current_string_bytes_awaiting_random = Encoding.UTF8.GetBytes(StringSeedStarter);
                    int randon_numbers_ammount = 20 - current_string_bytes_awaiting_random.Length;
                    byte[] random_garbage = new byte[randon_numbers_ammount];
                    random.NextBytes(random_garbage);
                    for (int i = 0; i < current_string_bytes_awaiting_random.Length; i++)
                    {
                        Console.Write(Convert.ToInt32(current_string_bytes_awaiting_random[i]));
                        intermediate_data += Convert.ToInt32(current_string_bytes_awaiting_random[i]).ToString();
                    }
                    for (int i = 0; i < random_garbage.Length; i++)
                    {
                        Console.Write(Convert.ToInt32(random_garbage[i]));
                        intermediate_data += Convert.ToInt32(random_garbage[i]).ToString();
                    }
                }
                if (intermediate_data.Length > int.MaxValue.ToString().Length)
                {
                    //gotta add in here the ability to offset by 9s and have 4 different Seeded randoms to better use the value allowing different strings to better generate unique seeds as for now if you enter enough data it
                    //gets trimmed down to the same value because int32.maxvalue length is 10 digits long
                    Console.WriteLine($"{Environment.NewLine}Shaving some length on the seed");
                    for (int j = 0; j < Math.Floor((double)(intermediate_data.Length / 9)); j++)
                    {
                        Console.WriteLine($"Processing {j} Total length is {intermediate_data.Length}");
                        if (j == 0)
                        {
                            intermediate_data_0_9 = intermediate_data.Substring(0, 9);
                            num_randoms++;
                            Console.WriteLine("pos 0-9 are processed");
                        }
                        if (j == 1)
                        {
                            intermediate_data_9_18 = intermediate_data.Substring(9, 9);
                            num_randoms++;
                            Console.WriteLine("pos 10-18");
                        }
                        if (j == 2)
                        {
                            intermediate_data_18_27 = intermediate_data.Substring(18, 9);
                            num_randoms++;
                            Console.WriteLine("pos 19-27");
                        }
                        if (j == 3)
                        {
                            intermediate_data_27_36 = intermediate_data.Substring(27,9);
                            num_randoms++;
                            Console.WriteLine("post 28-36");
                        }
                    }
                }
            }
            Console.WriteLine($"{intermediate_data_0_9} starting of {num_randoms} number of keys");
            Console.WriteLine();
            seeded_random = new Random(Convert.ToInt32(intermediate_data_0_9));
            if (intermediate_data_18_27 != "")
            {
                seeded_random_3 = new Random(Convert.ToInt32(intermediate_data_18_27));
            }
            if (intermediate_data_9_18 != "")
            {
                seeded_random_2 = new Random(Convert.ToInt32(intermediate_data_9_18));
            }
            if (intermediate_data_27_36 != "") 
            {
                seeded_random_4 = new Random(Convert.ToInt32(intermediate_data_27_36));
            }
            //block 1 so I can leverage numbers bigger then int32:maxsize
            if (num_randoms == 1)
            {
                Console.WriteLine("number block size was 1 so 100 characters of 1 seed that might cause collisions");
                for (int i = 0; i < 100; i++)
                {
                    seed += seeded_random.Next().ToString();
                }
            }
            else
            {
                if (seeded_random != null) {
                    for (int i = 0; i < 25; i++)
                    {
                        seed += seeded_random.Next().ToString();
                    }
                }
                if (seeded_random_2 != null) {
                    for (int i = 0; i < 25; i++)
                    {
                        seed += seeded_random_2.Next().ToString();
                    }
                }
                if (seeded_random_3 != null) {
                    for (int i = 0; i < 25; i++)
                    {
                        seed += seeded_random_3.Next().ToString();
                    } 
                }
                if (seeded_random_4 != null) {
                    for (int i = 0; i < 25; i++)
                    {
                        seed += seeded_random_4.Next().ToString();
                    }
                }
            }
            int multiple_of_8 = (int)Math.Floor((double)(seed.Length / 8));
            Console.WriteLine($"Multiple of 8:{multiple_of_8}");
            seed = seed.Substring(0, multiple_of_8 * 8);
            return seed;
        }
    }
}
