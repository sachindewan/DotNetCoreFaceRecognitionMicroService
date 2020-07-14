using EmailService;
using MassTransit;
using MassTransit.ConsumeConfigurators;
using Messaging.InterfacesConstants.Events;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace notificationService.Consumer
{
    class OrderProcessedEventConsumer : IConsumer<IOrderProcessedEvent>
    {
        private readonly IEmailSender _emailSender;
        public OrderProcessedEventConsumer(IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }
        public async Task Consume(ConsumeContext<IOrderProcessedEvent> context)
        {
            try
            {
                var rootFolder = AppContext.BaseDirectory.Substring(0, AppContext.BaseDirectory.IndexOf("bin"));
                var result = context.Message;
                var facesData = result.Faces;
                if (facesData.Count < 1)
                {
                    await Console.Out.WriteLineAsync($"no faces dtected");
                }
                else
                {
                    int j = 0;
                    foreach (var face in facesData)
                    {
                        MemoryStream ms = new MemoryStream(face);
                        var image = Image.FromStream(ms);
                        //image.Save(rootFolder + "/Images/face" + j + ".jpg", ImageFormat.Jpeg);
                        j++;
                    }
                }
                    // Here we will add the Email Sending code

                    string[] mailAddress = { result.UserEmail };

                    await _emailSender.SendEmailAsync(new Message(mailAddress, "your order" + result.OrderId,
                         "From FacesAndFaces", facesData));
                    await context.Publish<IOrderDispatchedEvent>(new
                    {
                        context.Message.OrderId,
                        DispatchDateTime = DateTime.UtcNow
                    });
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.InnerException.Message);
            }
        }
    }
}
