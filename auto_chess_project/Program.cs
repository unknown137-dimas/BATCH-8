using Spectre.Console;
class Program
{
    static void Main()
    {
        // var panel = new Panel("");
        // panel.Width = 3;
        // panel.Height = 2;
        // panel.Padding = new Padding(0,0,0,0);
        // var column = new Columns(panel, panel, panel);
        // column.Expand = false;
        // column.Padding = new Padding(0,0,0,0);
        // var row = new Rows(column, column, column);
        // AnsiConsole.Write(row);
        var mainMenu = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
            .Title("Welcome to AutoChess")
            .PageSize(5)
            // .MoreChoicesText("[grey](Move up and down to reveal more fruits)[/]")
            .AddChoices(
                new[] {
                "Start", "Exit"
                }
            )
        );
        if(mainMenu == "Start")
        {
            // Pick menu basic logic
            // TODO :
            // 1. How to edit/remove item from user (if user want to change item that already picked)
            // 2. Pick menu layout (Live display?)
            List<int> optionsList = new();
            var picked = new List<int>();
            while(picked.Count < 5)
            {
                GenerateRandomPick(ref optionsList);
                var options = AnsiConsole.Prompt(
                    new MultiSelectionPrompt<int>()
                    .Title("Pick Number")
                    .NotRequired()
                    .PageSize(5)
                    .AddChoices(optionsList)
                );
                foreach(var pick in options)
                {
                    picked.Add(pick);
                }
            }
            // Keep first 5 item picked by user
            if(picked.Count > 5)
            {
                picked = picked[0..5];
            }
            // Display the result
            foreach(var pick in picked)
            {
                AnsiConsole.WriteLine(pick);
            }
        }
    }
    // Generate random options
    static public void GenerateRandomPick(ref List<int> options)
    {
        var random = new Random();
        options.Clear();
        int n = 5;
        while(n > 0)
        {
            options.Add(random.Next(0,10));
            n--;
        }
    }
}