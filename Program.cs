using DVD;

namespace RG35XX.DVD
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            DVDBounce store = new();

            store.Execute();
        }
    }
}