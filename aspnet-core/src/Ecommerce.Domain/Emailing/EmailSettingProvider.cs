using Volo.Abp.Settings;

namespace Ecommerce.Emailing
{
    public class EmailSettingProvider : SettingDefinitionProvider
    {
        private readonly ISettingEncryptionService _encryptionService;

        public EmailSettingProvider(ISettingEncryptionService encryptionService)
        {
            _encryptionService = encryptionService;
        }

        public override void Define(ISettingDefinitionContext context)
        {
            var passSettings = context.GetOrNull("Abp.Mailing.Smtp.Password");
            if (passSettings != null)
            {
                string debug = _encryptionService.Encrypt(passSettings, "rzov dikz lonr pwes");
            }
        }
    }
}
