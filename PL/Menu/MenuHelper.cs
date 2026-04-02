namespace PL.Menu
{
    public static class MenuHelper
    {
        public static void ShowHeader(string title, string user = "Гість")
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("====================================================");
            Console.WriteLine($"  {title.ToUpper().PadRight(30)} | Користувач: {user}");
            Console.WriteLine("====================================================");
            Console.ResetColor();
        }

        public static void ShowError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\n[ПОМИЛКА]: {message}");
            Console.ResetColor();
            Wait();
        }

        public static void ShowSuccess(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\n[УСПІХ]: {message}");
            Console.ResetColor();
            Wait();
        }

        public static void Wait()
        {
            Console.WriteLine("\nНатисніть для продовження");
            Console.ReadKey();
        }
    }
}