namespace IoTBay.Models.Views
{
    // CartItemViewModel class represents an individual product in the cart.
    public class CartItemViewModel
    {
        public int ProductId { get; set; }  // The ID of the product
        public string ProductName { get; set; } = string.Empty;  // The name of the product
        public int Quantity { get; set; }  // Quantity of the product in the cart
        public int Stock { get; set; }  // Available stock of the product
        public double Price { get; set; }  // Price per unit of the product

        // Calculated property for total price for this cart item (Price * Quantity)
        public double TotalPrice => Price * Quantity;  
    }

    // CartViewModel class represents the entire cart, including a list of cart items and the total price.
    public class CartViewModel
    {
        // List of CartItemViewModel objects, each representing an individual cart item.
        public List<CartItemViewModel> Items { get; set; } = new List<CartItemViewModel>();

        // Total price for all items in the cart.
        public double TotalPrice { get; set; }

        // Constructor that calculates the total price of all items in the cart.
        public CartViewModel()
        {
            TotalPrice = 0;
        }

        // Optionally, you could add a method to calculate the total price for all items
        public void CalculateTotalPrice()
        {
            TotalPrice = Items.Sum(item => item.TotalPrice);
        }
    }
}