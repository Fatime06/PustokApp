using Microsoft.AspNetCore.Mvc;

namespace PustokApp.Services
{
    public class BankManagerService
    {
        private readonly BankAccountService _bankAccountService;
        public int Count => _bankAccountService.Balance;
        public BankManagerService(BankAccountService bankAccountService)
        {
            _bankAccountService = bankAccountService;
        }

        public void Add()
        {
            _bankAccountService.AddBalance();
        }
    }
}
