using Ecommerce.IdentitySettings;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;

namespace Ecommerce.Orders
{
    public class OrderCodeGenerator : ITransientDependency
    {
        private readonly IRepository<IdentitySetting, string> _identitySettingRepository;

        public OrderCodeGenerator(IRepository<IdentitySetting, string> identitySettingRepository)
        {
            _identitySettingRepository = identitySettingRepository;
        }

        public async Task<string> GenerateAsync()
        {
            string newCode;
            var identitySetting = await _identitySettingRepository.FindAsync(EcommerceConsts.OrderIdentitySettingId);
            if (identitySetting == null)
            {
                identitySetting = await _identitySettingRepository.InsertAsync(new IdentitySetting(EcommerceConsts.OrderIdentitySettingId, "Đặt hàng", EcommerceConsts.OrderIdentitySettingPrefix, 1, 1));
                newCode = identitySetting.Prefix + identitySetting.StepNumber;
            }
            else
            {
                identitySetting.CurrentNumber += identitySetting.StepNumber;
                newCode = identitySetting.Prefix + identitySetting.CurrentNumber;

                await _identitySettingRepository.UpdateAsync(identitySetting);
            }
            return newCode;
        }
    }
}
