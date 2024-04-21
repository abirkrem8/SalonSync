namespace SalonSync.MVC.Models
{
    public class ClientListViewModel
    {
        public List<ClientListViewModelItem> ClientList { get; set; }
    }

    public class ClientListViewModelItem
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
    }
}
