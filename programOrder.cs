using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using System.Linq;

public class MenuItem
{
    // Properties to represent attributes of a menu item
    public string Name { get; set; }
    public string Category { get; set; }
    public double Price { get; set; }
    public string Description { get; set; }
    public int? Spiciness { get; set; }
    //public bool HasToppings { get; set; }

    // Constructor to initialize the properties of the menu item
    public MenuItem(string name, string category, double price, string description, int? spiciness, bool hasToppings)
    {
        Name = name;
        Category = category;
        Price = price;
        Description = description;
        Spiciness = spiciness;
       // HasToppings = hasToppings;
    }

    public override string ToString() // Override of the ToString() method to provide a string representation of the menu item
    {
        string spicinessInfo = Spiciness.HasValue ? $"Spiciness: {Spiciness}\n" : "";
       // string toppingsInfo = HasToppings ? "Has Toppings: True\n" : "";

       // return $"{Name} ({Category}) - {Price:C2}\n{Description}\n{spicinessInfo}{toppingsInfo}";
        return $"{Name} ({Category}) - {Price:C2}\n{Description}"; // Return a formatted string with the menu item details
    }
}

/*public class Dessert : MenuItem
{
    public Dessert(string name, string category, double price, string description)
        : base(name, category, price, description, null, true)
    {
    }

    public override string ToString()
    {
        return $"{Name} ({Category}) - {Price:C2}\n{Description}\n";
    }
}*/

public class OrderItem
{
    public string Name { get; set; } //// Properties to represent attributes of an order item
    public int Quantity { get; set; }
    public double Price { get; set; }
}

public class Order
{
    private List<MenuItem> menu; // available menu items
    private List<OrderItem> items; // List representing the items in the orde

    public Order(List<MenuItem> menu) //// Constructor for the Order class, initializes the menu and order items list
    {
        this.menu = menu;
        items = new List<OrderItem>();
    }

    public void DisplayMenu()
    {
        Console.WriteLine("MENU:");
        for (int i = 0; i < menu.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {menu[i].Name} - {menu[i].Price:C2}");
        }
        Console.WriteLine();
    }

    public void AddToOrder(int menuItemIndex)
    {
        if (menuItemIndex >= 0 && menuItemIndex < menu.Count)
        {
            MenuItem selectedItem = menu[menuItemIndex];

            // Check if the selected item already exists in the order
            OrderItem existingItem = items.FirstOrDefault(item => item.Name == selectedItem.Name);

            if (existingItem != null)
            {
                // If the item exists, increase the quantity
                existingItem.Quantity++;
            }
            else
            {
                // If the item does not exist, add a new item to the order
                items.Add(new OrderItem
                {
                    Name = selectedItem.Name,
                    Quantity = 1,
                    Price = selectedItem.Price
                });
            }

            Console.WriteLine($"Added to order: {selectedItem.Name}");
            DisplayOrder();
        }
        else // Handle the case when an invalid index is provided
        {
            Console.WriteLine("Incorrect index of a dish from the menu.");
        }
    }

    private double CalculateTotalPrice() //total price of all items in the order
    {
        return items.Sum(item => item.Price * item.Quantity);
    }

    public void DisplayOrder()
    {
        Console.WriteLine("---------------------------------------------------------------");
        Console.WriteLine("CURRENT ORDER:");
        foreach (var item in items)
        {
            Console.WriteLine($"{item.Name} - quantity: {item.Quantity} - Price: {item.Price:C2}");
        }
        Console.WriteLine($"Total Price: {CalculateTotalPrice():C2}\n");
        Console.WriteLine("---------------------------------------------------------------");
    }

    public override string ToString()
    {
        return $"CURRENT ORDER:\n{string.Join("\n", items.Select(item => $"{item.Name} - Quantity: {item.Quantity} - Price: {item.Price:C2}"))}\nTotal Price: {CalculateTotalPrice():C2}\n";
    }
}



class Program
{
    static void Main()
    {
        // Path to the JSON file 
        string jsonPath = @"G:\restaurant\menu.json";
         List<MenuItem> menuItems;

        // Read the content of the JSON file
        try
        {
            // try to read the JSON file and deserialize it into a list of MenuItems
            string json = File.ReadAllText(jsonPath);
            menuItems = JsonConvert.DeserializeObject<List<MenuItem>>(json);
        }
        catch (FileNotFoundException)
        {
            //  the case when the specified file is not found
            Console.WriteLine($"File '{jsonPath}' cannot be found.");
            return;
        }
        catch (Exception ex)
        {
            //  other exceptions that might occur during file reading or deserialization
            Console.WriteLine($"An error occurred while reading the JSON file: {ex.Message}");
            return;
        }

        // Create an Order instance with the loaded menu items
        Order order = new Order(menuItems);

        int choice;
        do
        {
            // Display menu options and prompt the user for their choice
            Console.WriteLine("Select a dish from the menu (enter number) to add to your order.");
            Console.WriteLine("Enter 0 to complete ordering.");
            Console.WriteLine("Type -1 to see the description of the dish.");
            order.DisplayMenu();

            if (int.TryParse(Console.ReadLine(), out choice))
            {
                switch (choice)
                {
                    case 0:
                        // complete the ordering process
                        break;
                    case -1:
                        //see the description of a dish
                        Console.WriteLine("Enter the dish number to see the description:");
                        int itemIndex;
                        if (int.TryParse(Console.ReadLine(), out itemIndex) && itemIndex >= 1 && itemIndex <= menuItems.Count)
                        {
                            Console.WriteLine(menuItems[itemIndex - 1].Description);
                        }
                        else
                        {
                            Console.WriteLine("Invalid dish number.");
                        }
                        break;
                    default:
                        // User chose a dish to add to the order
                        if (choice >= 1 && choice <= menuItems.Count)
                        {
                            order.AddToOrder(choice - 1);
                        }
                        else
                        {
                            Console.WriteLine("Incorrect selection. Try again.");
                        }
                        break;
                }
            }
            else
            {
                // Handle the case when the user enters an invalid input
                Console.WriteLine("Invalid input. Try again.");
            }
        } while (choice != 0); // Continue the loop until the user chooses to complete the ordering process

        // Display the final details of the order
        Console.WriteLine(order.ToString());

        Console.WriteLine("Thank you for your order!");

        // Wait for any key press before terminating the program
        Console.WriteLine("Press any key to end...");
        Console.ReadKey();
    }

}
