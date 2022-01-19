namespace shopapp.webui.Models
{
    public class OrderModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public CartModel CartModel { get; set; }

        public string CardName{get;set;}
        public string CardNumber{get;set;}
        public string ExpMonth{get;set;}
        public string ExpYear{get;set;}
        public string Cvc{get;set;}
        public string Note { get; set; }


    }
}