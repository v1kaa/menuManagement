using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

class Dish
{
    [JsonProperty("name")] /// The JsonProperty attribute specifies that the Name field will be mapped to the "name" key in JSON format.
    public string Name { get; set; }

    private decimal price;

    [JsonProperty("category")] 
    public string Category { get; set; }

    [JsonProperty("price")]
    public decimal Price
    {
        get { return price; }
        set
        {
            if (value < 0) //Checking if the price is not negative.
            {
                throw new ArgumentException("price cannot be negative");
            }
            price = value;
        }
    }

    [JsonProperty("description")]
    public string Description { get; set; }

    [JsonProperty("spiciness")] 
    public virtual string Spiciness { get; set; } // A virtual Spiciness property that can be overridden by inheriting classes.

    public Dish(string name, decimal price, string description, string category) // Constructor of the Dish class that initializes the basic properties of the dish.
    {
        Name = name;
        Price = price;
        Description = description;
        Category = category;
    }

    public virtual string Display() //A virtual method for displaying information about a dish.
    {
        return $"{Name}: {Price} zł\ndescription: {Description}\nCategory: {Category}\nSpiciness: {Spiciness}";
    }
}

class Soup : Dish
{
    [JsonProperty("spiciness")]
    public override string Spiciness { get; set; } // Add this line

    public Soup(string name, decimal price, string description, string spiciness) : base(name, price, description, "Soup") //Soup class constructor
    {
        Spiciness = spiciness;
    }

    public override string Display()//Overridden Display method for the Soup class.
    {
        return $"Soup: {base.Display()}\nspiciness: {Spiciness}";/// Calls the Display method from the base class (Dish) and adds information about the spiciness of the soup.
    }
}

class MainDish : Dish
{
    public MainDish(string name, decimal price, string description) : base(name, price, description, "Main Dish")// Constructor for MainDish
    {
    }

    public override string Display()
    {         
        return $"MAin Dish: {base.Display()}"; //// Returns the category "Main Course" and information from the base class.
    }
}

class Dessert : Dish
{
    public bool HasToppings { get; set; }

    public Dessert(string name, decimal price, string description/*, bool hasToppings*/) : base(name, price, description, "Dessert")
    {
        //HasToppings = hasToppings;
    }

    public override string Display()
    {
        // string toppingsInfo = HasToppings ? "Z dodatkami" : "Bez dodatków";
        return $"Dessert: {base.Display()}";
       // return $"Deser: {base.Display()}\nDodatki: {toppingsInfo}";
    }
}

class MenuCreator
{
    private List<Dish> menu; // List representing the current menu.

    public MenuCreator(List<Dish> initialMenu)
    {
        menu = initialMenu;
    }


    public void AddDish()
    {
        Console.WriteLine("Adding new dishes to the menu:");// Message informing 

        try
        {
            // input information about a new dish from the user
            Console.Write("Podaj nazwę dania: ");
            string name = Console.ReadLine();

            Console.Write("Podaj cenę dania: ");
            decimal price = Convert.ToDecimal(Console.ReadLine());

            Console.Write("Podaj opis dania: ");
            string description = Console.ReadLine();

            Console.Write("Podaj kategorię dania (1 - Zupa, 2 - Danie Główne, 3 - Deser): ");
            int dishType = Convert.ToInt32(Console.ReadLine());

            Dish newDish;//variable for new dish

            switch (dishType)
            {
                case 1://add soup
                    Console.Write("Podaj ostrość zupy: ");
                    string spiciness = Console.ReadLine();
                    newDish = new Soup(name, price, description, spiciness);
                    break;

                case 2://main dish
                    newDish = new MainDish(name, price, description);
                    break;

                case 3://desert
                   // Console.Write("Czy deser ma dodatki? (true/false): ");
                   // bool hasToppings = Convert.ToBoolean(Console.ReadLine());
                    newDish = new Dessert(name, price, description/*, hasToppings*/);
                    break;

                default://if invalid category
                    Console.WriteLine("Nieprawidłowy typ dania. Dodawanie anulowane.");
                    return;
            }

            menu.Add(newDish);
            Console.WriteLine($"Danie '{name}' zostało dodane do menu.");
        }
        catch (FormatException)//// Exception handling in case of data format error
        {
            Console.WriteLine("Błąd formatu danych. Sprawdź, czy wprowadziłeś poprawne liczby.");
        }
        catch (ArgumentException ex)//argument error
        {
            Console.WriteLine($"Błąd: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Wystąpił błąd: {ex.Message}");
        }
    }

    public void DeleteDish()
    {
        Console.WriteLine("Usuwanie dania z menu:");

        DisplayMenu();

        try
        {
            Console.Write("Podaj numer dania do usunięcia: ");//Prompt the user to enter the number of the dish to be deleted
            int dishNumber = Convert.ToInt32(Console.ReadLine());

            if (dishNumber >= 0 && dishNumber < menu.Count)// Check if the entered dish number is within valid range
            {
                Dish removedDish = menu[dishNumber];
                menu.RemoveAt(dishNumber);
                Console.WriteLine($"Danie '{removedDish.Name}' zostało usunięte z menu.");
            }
            else // Display a message if the entered dish number is invalid
            {
                Console.WriteLine("Nieprawidłowy numer dania. Usuwanie anulowane.");
            }
        }
        catch (FormatException) //if the user enters a non-numeric value
        {
            Console.WriteLine("Błąd formatu danych. Sprawdź, czy wprowadziłeś poprawny numer.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Wystąpił błąd: {ex.Message}"); //another errors
        }
    }

    public void ModifyDish()
    {
        Console.WriteLine("Modifying a dish on the menu:");

        DisplayMenu(); //current menu

        try
        {
            Console.Write("Enter the number of the dish to be modified: "); // input number for modification
            int dishNumber = Convert.ToInt32(Console.ReadLine());
            // Check if the entered dish number is within valid range
            if (dishNumber >= 0 && dishNumber < menu.Count)
            {
                Dish selectedDish = menu[dishNumber];  //selected dish from the menu

                Console.WriteLine($"Modifying a dish: {selectedDish.Display()}");//details of the selected dish

                Console.Write("Enter a new name for the dish: ");
                selectedDish.Name = Console.ReadLine();

                Console.Write("Enter the new price of the dish: ");
                selectedDish.Price = Convert.ToDecimal(Console.ReadLine());

                Console.Write("Enter a new description of the dish: ");
                selectedDish.Description = Console.ReadLine();

                if (selectedDish is Soup)  // If the selected dish is of type Soup, prompt for spiciness modification
                {
                    Console.Write("Enter a new soup spiciness: ");
                    ((Soup)selectedDish).Spiciness = Console.ReadLine();
                }
               /* else if (selectedDish is Dessert)
                {
                    Console.Write("Czy deser ma dodatki? (true/false): ");
                    ((Dessert)selectedDish).HasToppings = Convert.ToBoolean(Console.ReadLine());
                }*/

                Console.WriteLine($"dish '{selectedDish.Name}' modified successfully.");
            }
            else
            {
                Console.WriteLine("Incorrect dish number. Modification canceled.");
            }
        }
        catch (FormatException)
        {
            Console.WriteLine("Data format error. Check if the number is correct.");
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"error: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occured: {ex.Message}");
        }
    }

    public void DisplayMenu() //current menu
    {
        Console.WriteLine("Current menu:");

        for (int i = 0; i < menu.Count; i++)
        {
            Console.WriteLine($"[{i}] {menu[i].Display()}");
            Console.WriteLine();
        }
    }

    public void SaveMenuToJson(string filePath) //serializes the menu list to JSON format and writes it to a file
    {
        string json = JsonConvert.SerializeObject(menu, Formatting.Indented);
        File.WriteAllText(filePath, json);
        Console.WriteLine($"Menu saved to file: {filePath}");
    }
}

class Program
{
    static void Main()
    {
        string filePath = @"G:\restaurant\menu.json"; //file path for the menu JSON file

        try
        {
            //  Initialize the men
            List<Dish> initialMenu = LoadInitialMenu(filePath);

            // Create a MenuCreator with the loaded menu
            MenuCreator menuCreator = new MenuCreator(initialMenu);


            while (true)  // Main program loop
            {
                Console.WriteLine("Available operations:");
                Console.WriteLine("1. Add a new dish");
                Console.WriteLine("2. Delete the dish");
                Console.WriteLine("3. Modify the dish");
                Console.WriteLine("4. Display the menu");
                Console.WriteLine("6. Finish and save all modifications");
                Console.WriteLine("Select operation (enter number):"); //choose an operation
                int choice = Convert.ToInt32(Console.ReadLine());

                switch (choice)  // Switch based on the user's choice
                {
                    case 1:
                        menuCreator.AddDish();
                        break;

                    case 2:
                        menuCreator.DeleteDish();
                        break;

                    case 3:
                        menuCreator.ModifyDish();
                        break;

                    case 4:
                        menuCreator.DisplayMenu();
                        break;
                    case 6:
                        menuCreator.SaveMenuToJson(filePath);
                        Environment.Exit(0);
                        break;

                    default:
                        Console.WriteLine("Invalid operation number. try again.");
                        break;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occured: {ex.Message}");
        }
    }

    static List<Dish> LoadInitialMenu(string filePath)
    {
        try
        {
            // Wczytaj dane z pliku JSON
            string json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<List<Dish>>(json);
        }
        catch (FileNotFoundException)
        {
            Console.WriteLine($"File '{filePath}' has not been found. Creating a new menu.");
            return new List<Dish>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while loading the menu: {ex.Message}");
            return new List<Dish>();
        }
    }
}
