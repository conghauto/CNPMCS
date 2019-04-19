using System.Threading.Tasks;
using Abp.AutoMapper;
using Abp.Dependency;
using MyCompanyName.AbpZeroTemplate.ApiClient;
using MyCompanyName.AbpZeroTemplate.ApiClient.Models;
using MyCompanyName.AbpZeroTemplate.Core.DataStorage;
using MyCompanyName.AbpZeroTemplate.Models.Common;
using MyCompanyName.AbpZeroTemplate.Sessions.Dto;

namespace MyCompanyName.AbpZeroTemplate.Services.Storage
{
    public class DataStorageService : IDataStorageService, ISingletonDependency
    {
        private readonly IDataStorageManager _dataStorageManager;

        public DataStorageService(IDataStorageManager dataStorageManager)
        {
            _dataStorageManager = dataStorageManager;
        }

        public async Task StoreAccessTokenAsync(string newAccessToken)
        {
            var authenticateResult = _dataStorageManager.Retrieve<AuthenticateResultPersistanceModel>(DataStorageKey.CurrentSession_TokenInfo);

            authenticateResult.AccessToken = newAccessToken;

            await _dataStorageManager.StoreAsync(DataStorageKey.CurrentSession_TokenInfo, authenticateResult);
        }

        public AbpAuthenticateResultModel RetrieveAuthenticateResult()
        {
            return _dataStorageManager.Retrieve<AuthenticateResultPersistanceModel>(
                DataStorageKey.CurrentSession_TokenInfo).MapTo<AbpAuthenticateResultModel>();
        }

        public async Task StoreAuthenticateResultAsync(AbpAuthenticateResultModel authenticateResultModel)
        {
            await _dataStorageManager.StoreAsync(DataStorageKey.CurrentSession_TokenInfo,
                authenticateResultModel.MapTo<AuthenticateResultPersistanceModel>());
        }

        public TenantInformation RetrieveTenantInfo()
        {
            return _dataStorageManager.Retrieve<TenantInformationPersistanceModel>(DataStorageKey.CurrentSession_TenantInfo)
                 .MapTo<TenantInformation>();
        }

        public async Task StoreTenantInfoAsync(TenantInformation tenantInfo)
        {
            await _dataStorageManager.StoreAsync(DataStorageKey.CurrentSession_TenantInfo, tenantInfo.MapTo<TenantInformationPersistanceModel>());
        }

        public GetCurrentLoginInformationsOutput RetrieveLoginInfo()
        {
            return _dataStorageManager.Retrieve<CurrentLoginInformationPersistanceModel>(DataStorageKey.CurrentSession_LoginInfo)
                .MapTo<GetCurrentLoginInformationsOutput>();
        }

        public async Task StoreLoginInformationAsync(GetCurrentLoginInformationsOutput loginInfo)
        {
            await _dataStorageManager.StoreAsync(DataStorageKey.CurrentSession_LoginInfo, loginInfo.MapTo<CurrentLoginInformationPersistanceModel>());
        }

        public void ClearSessionPersistance()
        {
            _dataStorageManager.RemoveIfExists(DataStorageKey.CurrentSession_TokenInfo);
            _dataStorageManager.RemoveIfExists(DataStorageKey.CurrentSession_TenantInfo);
            _dataStorageManager.RemoveIfExists(DataStorageKey.CurrentSession_LoginInfo);
        }
    }
}