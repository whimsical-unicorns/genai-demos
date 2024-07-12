using Microsoft.SemanticKernel;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace WalletSense
{
	public class AccountPlugin
	{
		// mock data for single account, ballance and transactions
		private readonly AccountModel account = new()
		{
			Id = 1,
			Name = "Main",
			Balance = 258.80M, // assume $
			Transactions = new List<TransactionModel>
			{
				new TransactionModel { Id = 4873, Amount = 100, Type = "Deposit" },
				new TransactionModel { Id = 4874, Amount = -51.20M, Type = "Withdrawal", Tags = ["games", "entertainment"] },
				new TransactionModel { Id = 4875, Amount = -10.16M, Type = "Withdrawal", Tags = ["books", "entertainment"] },
				new TransactionModel { Id = 4876, Amount = -19.35M, Type = "Withdrawal", Tags = ["food", "groceries"] },
				new TransactionModel { Id = 4877, Amount = -20.50M, Type = "Withdrawal", Tags = ["games", "entertainment"] },
				new TransactionModel { Id = 4878, Amount = -100, Type = "Withdrawal", Tags = ["rent", "utilities"] },
				new TransactionModel { Id = 4879, Amount = -50, Type = "Withdrawal", Tags = ["transportation"] },
				new TransactionModel { Id = 4880, Amount = -30, Type = "Withdrawal", Tags = ["health"] },
				new TransactionModel { Id = 4881, Amount = -20, Type = "Withdrawal", Tags = ["insurance"] },
				new TransactionModel { Id = 4882, Amount = -10, Type = "Withdrawal", Tags = ["taxes"] },
				new TransactionModel { Id = 4883, Amount = -5, Type = "Withdrawal", Tags = ["savings", "investments"] },
			}
		};

		[KernelFunction("get_balance")]
		[Description("Gets the current balance of current user's main account")]
		[return: Description("The current balance of the account")]
		public async Task<decimal> GetBalanceAsync()
		{
			return account.Balance;
		}

		[KernelFunction("get_transactions")]
		[Description("Gets the list of recent transactions of the current user's main account from this month")]
		[return: Description("An array of transactions with amount spent and tags")]
		public async Task<List<TransactionModel>> GetTransactionsAsync()
		{
			return account.Transactions;
		}

		[KernelFunction("add_transaction")]
		[Description("Adds a new transaction to the current user's main account")]
		[return: Description("'OK' if the transaction was added successfully")]
		public async Task<string> AddTransactionAsync(decimal amount, string type, string[] tags)
		{
			var newTransaction = new TransactionModel
			{
				Id = account.Transactions.Max(t => t.Id) + 1,
				Amount = amount,
				Type = type,
				Tags = tags
			};

			account.Transactions.Add(newTransaction);
			account.Balance += amount;

			return "OK";
		}

		[KernelFunction("predict_spending")]
		[Description("Predicts the total spending for the rest of the month based on the current spending pattern")]
		[return: Description("The predicted total spending for the rest of the month")]
		public async Task<decimal> PredictSpendingAsync()
		{
			return 200;
		}
	}

	public class AccountModel
	{
		[JsonPropertyName("id")]
		public int Id { get; set; }

		[JsonPropertyName("name")]
		public required string Name { get; set; }

		[JsonPropertyName("balance")]
		public decimal Balance { get; set; }

		[JsonPropertyName("transactions")]
		public List<TransactionModel> Transactions { get; set; } = [];
	}

	public class TransactionModel
	{
		[JsonPropertyName("id")]
		public int Id { get; set; }

		[JsonPropertyName("amount")]
		public decimal Amount { get; set; }

		[JsonPropertyName("description")]
		public required string Type { get; set; }

		[JsonPropertyName("tags")]
		public string[] Tags { get; set; } = [];
	}
}
