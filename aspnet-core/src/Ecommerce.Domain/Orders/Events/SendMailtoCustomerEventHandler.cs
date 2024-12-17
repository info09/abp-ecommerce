using Ecommerce.Emailing;
using System;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Emailing;
using Volo.Abp.EventBus;
using Volo.Abp.TextTemplating;

namespace Ecommerce.Orders.Events
{
    public class SendMailtoCustomerEventHandler : ILocalEventHandler<NewOrderCreatedEvent>, ITransientDependency
    {
        private readonly IEmailSender _emailSender;
        private readonly ITemplateRenderer _templateRenderer;

        public SendMailtoCustomerEventHandler(IEmailSender emailSender, ITemplateRenderer templateRenderer)
        {
            _emailSender = emailSender;
            _templateRenderer = templateRenderer;
        }

        public async Task HandleEventAsync(NewOrderCreatedEvent eventData)
        {
            try
            {
                var emailBody = await _templateRenderer.RenderAsync(EmailTemplates.CreateOrderEmail, new { message = eventData.Message });
                await _emailSender.SendAsync(eventData.CustomerEmail, "Tạo đơn hàng thành công", emailBody);
            }
            catch (Exception ex)
            {

            }
        }
    }
}
