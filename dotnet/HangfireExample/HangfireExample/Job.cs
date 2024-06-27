namespace HangfireExample
{
    public class Job
    {
        public void Run()
        {
            Console.WriteLine($"Running at: {DateTime.Now}");
        }
    }
}
