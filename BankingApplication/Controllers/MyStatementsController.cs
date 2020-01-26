using System.Threading.Tasks;
using BankingApplication.Data;
using BankingApplication.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using X.PagedList;
using System.Collections.Generic;
using System;

namespace BankingApplication.Controllers
{
    public class MyStatementsController : Controller
    {
        private readonly BankAppContext _context;
        public MyStatementsController(BankAppContext context) => _context = context;
        private int CustomerID => HttpContext.Session.GetInt32(nameof(Customer.CustomerID)).Value;
        private List<Transaction> transactions;
        private int selectedAccountNumber;
        public async Task<IActionResult> SelectAccount() 
        {
            var accounts = await _context.Account.Where(x => x.CustomerID == CustomerID).ToListAsync();

            return View(accounts);
        } 
        
        
         public async Task<IActionResult> IndexToMyDetails(int accountNumber) 
        {
            // const int pageSize = 3;
            // var transactions = await _context.Transaction.Where(x => x.AccountNumber == accountNumber).ToPagedListAsync(page, pageSize);

            // return PartialView(transactions);

            transactions = await _context.Transaction.Where(x => x.AccountNumber == accountNumber).ToListAsync();

             HttpContext.Session.SetInt32("selectedAccountNumber", accountNumber);

            return RedirectToAction(nameof(MyDetails));
        } 

        public async Task<IActionResult> MyDetails(int page = 1)
        {
            const int pageSize = 4;
            var selectedAccountNumber = HttpContext.Session.GetInt32("selectedAccountNumber");
            Console.WriteLine(selectedAccountNumber);
            var pagedList = await _context.Transaction.Where(x => x.AccountNumber == selectedAccountNumber).ToPagedListAsync(page, pageSize);

            return View(pagedList);
        }

        public async Task<IActionResult> SeeMyBalance(int id) 
        {
            var account = await _context.Account.FindAsync(id);
            return PartialView(account);
        } 



        
    }

    

}