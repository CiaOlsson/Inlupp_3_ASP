﻿using Inlämning_Bank.Core.Interfaces;
using Inlämning_Bank.Core.Services;
using Inlämning_Bank.Domain.DTO;
using Inlämning_Bank.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using System.Security.Claims;

namespace Inlämning_Bank.Api.Controllers
{
    //[Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Customer")]
    public class CustomerAccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly ITransactionService _transactionService;
        private readonly IUserService _userService;
        public CustomerAccountController(IAccountService accountService, ITransactionService transactionService, IUserService userService)
        {
            _accountService = accountService;
            _transactionService = transactionService;
            _userService = userService;
        }

        [Route("/api/myaccounts")]
        [HttpGet]  
        public async Task<IActionResult> ViewCustomerAccounts()
        {
            try
            {
                var accounts = await _accountService.GetCustomerAccounts(User);

                if (accounts == null)
                {
                    return Ok("Kunden har inga konton ännu.");
                }

                return Ok(accounts);
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }

        }

        [Route("/api/myaccounts/{accountId}")]
        [HttpGet]
        public async Task<IActionResult> ViewAccountDetails(int accountId)
        {
            try
            {
                //metod för att hitta den inloggade kundens id.
                var accounts = await _accountService.GetCustomerAccounts(User);

                //För uppgiftens skull måste man dubbelkolla så att kunden försöker se sitt eget konto.
                var account = accounts.SingleOrDefault(a => a.AccountNumber == accountId);

                if (account == null)
                {
                    return Unauthorized("Det är inte kundens konto");
                }

                // sedan en metod för att ta fram alla transaktioner. 
                var transactions = await _transactionService.GetTransactionsOnAccount(account.AccountNumber);

                return Ok(transactions);
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }
        }

        [Route("/api/opennewaccount/{accountType}")]
        [HttpPost]
        public async Task<IActionResult> OpenNewAccount(int accountType)
        {
            try
            {
                var customerId = await _userService.RetrieveCustomerId(User);

                int accountId = await _accountService.AddAccount(accountType, customerId);

                return Ok($"Ditt nya konto är skapat, kontonummer: {accountId}");

            }
            catch (Exception ex)
            {
                return Conflict(ex.Message );
            }
        }

        [Route("/api/maketransaction")]
        [HttpPost]
        public async Task<IActionResult> MakeTransaction([FromBody] MakeTransactionDTO transaction)
        {
            if (transaction.FromAccount == transaction.ToAccount)
            {
                return BadRequest("Det går inte att föra över pengar till samma konto som från-kontot");
            }

            try
            {
                await _transactionService.TransferMoney(transaction);
               
                return Ok($"Förde över {transaction.Amount}kr. \nFrån kontonummer: {transaction.FromAccount} \nTill kontonummer: {transaction.ToAccount}");

            }
            catch (Exception ex)
            {
                return Conflict($"Det gick inte att föra över pengar. \n{ex.Message}");
            }
        }
    }
}
