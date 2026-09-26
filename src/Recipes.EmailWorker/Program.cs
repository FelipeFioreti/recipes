using Recipes.EmailWorker;
using Recipes.EmailWorker.Smtp;
using Recipes.Messaging;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.Configure<RabbitMqSettings>(builder.Configuration.GetSection("RabbitMq"));
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("Smtp"));

builder.Services.AddSingleton<IEmailSender, SmtpEmailSender>();
builder.Services.AddSingleton(new EmailRetrySettings());
builder.Services.AddSingleton<EmailMessageHandler>();
builder.Services.AddHostedService<EmailConsumer>();

builder.Build().Run();
