using CasinoApi.DTO;
using Dapper;
using System.Data;

namespace CasinoApi.Repositories;

public class CasinoRepository : ICasinoRepository
{
    private readonly IDbConnection _dbConnection;

    public CasinoRepository(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    public async Task<CurrentBalanceResponse> GetBalanceAsync(GetBalanceRequest getBalance)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@Token", getBalance.Token, DbType.String, size: 450);

        parameters.Add("@StatusCode", dbType: DbType.Int32, direction: ParameterDirection.Output);
        parameters.Add("@ErrorMessage", dbType: DbType.String, size: 4000, direction: ParameterDirection.Output);
        parameters.Add("@CurrentBalance", dbType: DbType.Decimal, direction: ParameterDirection.Output);

        await _dbConnection.ExecuteAsync(
            "GetCurrentBalance",
            parameters,
            commandType: CommandType.StoredProcedure);

        var resultResponse = new CurrentBalanceResponse
        {
            Data = new CurrentBalanceModel
            {
                CurrentBalance = parameters.Get<decimal?>("@CurrentBalance")
            },
            StatusCode = parameters.Get<int>("@StatusCode"),
            ErrorMessage = parameters.Get<string?>("@ErrorMessage")
        };

        return resultResponse;
    }

    public async Task<UserInfoResponse> GetPlayerInfoAsync(GetUserInfoRequest userInfo)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@Token", userInfo.Token, DbType.String, size: 450);

        parameters.Add("@UserId", dbType: DbType.String, size: 450, direction: ParameterDirection.Output);
        parameters.Add("@UserName", dbType: DbType.String, size: 256, direction: ParameterDirection.Output);
        parameters.Add("@Email", dbType: DbType.String, size: 256, direction: ParameterDirection.Output);
        parameters.Add("@Currency", dbType: DbType.String, size: 3, direction: ParameterDirection.Output);
        parameters.Add("@CurrentBalance", dbType: DbType.Decimal, direction: ParameterDirection.Output);
        parameters.Add("@StatusCode", dbType: DbType.Int32, direction: ParameterDirection.Output);
        parameters.Add("@ErrorMessage", dbType: DbType.String, size: 4000, direction: ParameterDirection.Output);

        await _dbConnection.ExecuteAsync(
            "GetPlayerInfo",
            parameters,
            commandType: CommandType.StoredProcedure);

        var resultResponse = new UserInfoResponse
        {
            Data = new UserInfoModel
            {
                UserId = parameters.Get<string>("@UserId"),
                UserName = parameters.Get<string>("@UserName"),
                Email = parameters.Get<string>("@Email"),
                Currency = parameters.Get<string>("@Currency"),
                CurrentBalance = parameters.Get<decimal?>("@CurrentBalance")
            },
            StatusCode = parameters.Get<int>("@StatusCode"),
            ErrorMessage = parameters.Get<string?>("@ErrorMessage")
        };

        return resultResponse;
    }

    public async Task<UpdatedBalanceResponse> MakeBetAsync(BetRequest bet)
    {
        var parameters = new DynamicParameters();
        var transactionId = Guid.NewGuid().ToString();

        parameters.Add("@TransactionId", transactionId, DbType.String, size: 450);
        parameters.Add("@Token", bet.Token, DbType.String, size: 36);
        parameters.Add("@Amount", bet.Amount, DbType.Decimal);
        parameters.Add("@BetTypeId", bet.BetTypeId, DbType.Int32);
        parameters.Add("@GameId", bet.GameId, DbType.Int32);
        parameters.Add("@ProductId", bet.ProductId, DbType.Int32);
        parameters.Add("@RoundId", bet.RoundId, DbType.Int32);
        parameters.Add("@Hash", bet.Hash, DbType.String, size: -1);
        parameters.Add("@Currency", bet.Currency, DbType.String, size: 3);
        parameters.Add("@CampaignId", bet.CampaignId, DbType.Int32);
        parameters.Add("@CampaignName", bet.CampaignName, DbType.String);

        parameters.Add("@StatusCode", dbType: DbType.Int32, direction: ParameterDirection.Output);
        parameters.Add("@ErrorMessage", dbType: DbType.String, size: 4000, direction: ParameterDirection.Output);
        parameters.Add("@UpdatedBalance", dbType: DbType.Decimal, direction: ParameterDirection.Output);

        await _dbConnection.ExecuteAsync(
            "MakeBet",
            parameters,
            commandType: CommandType.StoredProcedure);

        var response = new UpdatedBalanceResponse
        {
            Data = new UpdatedBalanceModel
            {
                TransactionId = transactionId,
                CurrentBalance = parameters.Get<decimal?>("@UpdatedBalance"),
            },
            StatusCode = parameters.Get<int>("@StatusCode"),
            ErrorMessage = parameters.Get<string?>("@ErrorMessage")
        };

        return response;
    }

    public async Task<UpdatedBalanceResponse> WinAsync(WinRequest win)
    {
        var parameters = new DynamicParameters();
        var transactionId = Guid.NewGuid().ToString();

        parameters.Add("@TransactionId", transactionId, DbType.String, size: 450);
        parameters.Add("@Token", win.Token, DbType.String, size: 36);
        parameters.Add("@Amount", win.Amount, DbType.Decimal);
        parameters.Add("@BetTypeId", win.WinTypeId, DbType.Int32);
        parameters.Add("@GameId", win.GameId, DbType.Int32);
        parameters.Add("@ProductId", win.ProductId, DbType.Int32);
        parameters.Add("@RoundId", win.RoundId, DbType.Int32);
        parameters.Add("@Hash", win.Hash, DbType.String, size: -1);
        parameters.Add("@Currency", win.Currency, DbType.String, size: 3);
        parameters.Add("@CampaignId", win.CampaignId, DbType.Int32);
        parameters.Add("@CampaignName", win.CampaignName, DbType.String);

        parameters.Add("@StatusCode", dbType: DbType.Int32, direction: ParameterDirection.Output);
        parameters.Add("@ErrorMessage", dbType: DbType.String, size: 4000, direction: ParameterDirection.Output);
        parameters.Add("@UpdatedBalance", dbType: DbType.Decimal, direction: ParameterDirection.Output);

        await _dbConnection.ExecuteAsync(
            "Win",
            parameters,
            commandType: CommandType.StoredProcedure);

        var resultResponse = new UpdatedBalanceResponse
        {
            Data = new UpdatedBalanceModel
            {
                TransactionId = transactionId,
                CurrentBalance = parameters.Get<decimal?>("@UpdatedBalance")
            },
            StatusCode = parameters.Get<int>("@StatusCode"),
            ErrorMessage = parameters.Get<string?>("@ErrorMessage")
        };

        return resultResponse;
    }

    public async Task<UpdatedBalanceResponse> CancelBetAsync(CancelBetRequest cancelBet)
    {
        var parameters = new DynamicParameters();
        var transactionId = Guid.NewGuid().ToString();

        parameters.Add("@TransactionId", transactionId, DbType.String, size: 450);
        parameters.Add("@Token", cancelBet.Token, DbType.String, size: 36);
        parameters.Add("@Amount", cancelBet.Amount, DbType.Decimal);
        parameters.Add("@BetTypeId", cancelBet.BetTypeId, DbType.Int32);
        parameters.Add("@GameId", cancelBet.GameId, DbType.Int32);
        parameters.Add("@ProductId", cancelBet.ProductId, DbType.Int32);
        parameters.Add("@RoundId", cancelBet.RoundId, DbType.Int32);
        parameters.Add("@Hash", cancelBet.Hash, DbType.String, size: -1);
        parameters.Add("BetTransactionId", cancelBet.BetTransactionId);
        parameters.Add("@Currency", cancelBet.Currency, DbType.String, size: 3);
        parameters.Add("@CampaignId", cancelBet.CampaignId, DbType.Int32);
        parameters.Add("@CampaignName", cancelBet.CampaignName, DbType.String, size: 500);

        parameters.Add("@StatusCode", dbType: DbType.Int32, direction: ParameterDirection.Output);
        parameters.Add("@ErrorMessage", dbType: DbType.String, size: 4000, direction: ParameterDirection.Output);
        parameters.Add("@UpdatedBalance", dbType: DbType.Decimal, direction: ParameterDirection.Output);

        await _dbConnection.ExecuteAsync(
            "CancelBet",
            parameters,
            commandType: CommandType.StoredProcedure);

        var response = new UpdatedBalanceResponse
        {
            Data = new UpdatedBalanceModel
            {
                TransactionId = transactionId,
                CurrentBalance = parameters.Get<decimal?>("@UpdatedBalance"),
            },
            StatusCode = parameters.Get<int>("@StatusCode"),
            ErrorMessage = parameters.Get<string?>("@ErrorMessage")
        };

        return response;
    }

    public async Task<UpdatedBalanceResponse> ChangeWinAsync(ChangeWinRequest changeWin)
    {
        var parameters = new DynamicParameters();
        var transactionId = Guid.NewGuid().ToString();

        parameters.Add("@TransactionId", transactionId, DbType.String, size: 450);
        parameters.Add("@Token", changeWin.Token, DbType.String, size: 36);
        parameters.Add("@Amount", changeWin.Amount, DbType.Decimal);
        parameters.Add("@PreviousAmount", changeWin.PreviousAmount, DbType.Decimal);
        parameters.Add("@PreviousTransactionId", changeWin.PreviousTransactionId, DbType.String, size: 36);
        parameters.Add("@BetTypeId", changeWin.ChangeWinTypeId, DbType.Int32);
        parameters.Add("@GameId", changeWin.GameId, DbType.Int32);
        parameters.Add("@ProductId", changeWin.ProductId, DbType.Int32);
        parameters.Add("@RoundId", changeWin.RoundId, DbType.Int32);
        parameters.Add("@Hash", changeWin.Hash, DbType.String, size: -1);
        parameters.Add("@Currency", changeWin.Currency, DbType.String, size: 3);

        parameters.Add("@StatusCode", dbType: DbType.Int32, direction: ParameterDirection.Output);
        parameters.Add("@ErrorMessage", dbType: DbType.String, size: 4000, direction: ParameterDirection.Output);
        parameters.Add("@UpdatedBalance", dbType: DbType.Decimal, direction: ParameterDirection.Output);

        await _dbConnection.ExecuteAsync(
            "ChangeWin",
            parameters,
            commandType: CommandType.StoredProcedure);

        var resultResponse = new UpdatedBalanceResponse
        {
            Data = new UpdatedBalanceModel
            {
                TransactionId = transactionId,
                CurrentBalance = parameters.Get<decimal?>("@UpdatedBalance")
            },
            StatusCode = parameters.Get<int>("@StatusCode"),
            ErrorMessage = parameters.Get<string?>("@ErrorMessage")
        };

        return resultResponse;
    }
}
