namespace PustokApp.Services
{
    public class BankAccountService
    {
        public int Balance { get; set; }
        public void AddBalance()
        {
            Balance *= 100;
        }
    }
}
