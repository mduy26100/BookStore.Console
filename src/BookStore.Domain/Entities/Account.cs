namespace BookStore.Domain.Entities
{
    public class Account
    {
        public int AccountID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string Role { get; set; }

        public ICollection<Order> Orders { get; set; }
        public ICollection<ShoppingCart> ShoppingCarts { get; set; }
    }
}
