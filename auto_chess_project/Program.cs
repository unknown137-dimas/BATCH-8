using Spectre.Console;
class Program
{
    static void Main()
    {
        AnsiConsole.Markup("[underline red]Hello[/] World!");
        AnsiConsole.Write(new BreakdownChart()
        .Width(60)
        // Add item is in the order of label, value, then color.
        .AddItem("SCSS", 80, Color.Red)
        .AddItem("HTML", 28.3, Color.Blue)
        .AddItem("C#", 22.6, Color.Green)
        .AddItem("JavaScript", 6, Color.Yellow)
        .AddItem("Ruby", 6, Color.LightGreen)
        .AddItem("Shell", 0.1, Color.Aqua));
    }
}