# Recipes

API de receitas em .NET 10. Segue Clean Architecture, com cada camada em um projeto separado e as
regras de dependencia verificadas pelo compilador. O frontend Angular vive em repositorio proprio,
[`chefarchive-web`](https://github.com/FelipeFioreti/chefarchive-web).

Para entender como o ChefArchive inteiro funciona (os 3 repositorios, infra na VPS, deploy, redes),
veja [`ARCHITECTURE.md`](ARCHITECTURE.md).

## Estrutura

```
Recipes.sln
src/
  Recipes.Domain/          entidades, excecoes e contratos    -> sem dependencias
  Recipes.Application/     casos de uso, DTOs e mapeamentos   -> Domain
  Recipes.Infrastructure/  EF Core, repositorios, jobs        -> Application, Domain
  Recipes.Api/             controllers, middlewares, host     -> Infrastructure, Application, Domain
  Recipes.Messaging/       contrato e filas do RabbitMQ       -> sem projetos (so RabbitMQ.Client)
  Recipes.EmailWorker/     worker que envia os emails (SMTP)  -> Messaging
```

A seta aponta sempre para dentro. O `Recipes.Domain` nao referencia nenhum projeto e nao tem nenhum
pacote NuGet: se algo la dentro precisar de EF Core ou de ASP.NET, esta na camada errada.

`Recipes.EmailWorker` e um processo separado (container proprio): consome a fila `email.send` e envia por
SMTP. A API so publica na fila (`Recipes.Infrastructure/Email/RabbitMqEmailPublisher.cs`); Domain e
Application nao conhecem RabbitMQ, so a interface `IEmailService`.

### O que vai onde

| Precisa adicionar | Vai para |
|---|---|
| Entidade, regra de negocio, excecao de dominio | `Recipes.Domain/Entities`, `Exceptions` |
| Contrato que o dominio exige de fora (repositorio, dado de entrada) | `Recipes.Domain/Interfaces` |
| Caso de uso, DTO de request/response, mapeamento | `Recipes.Application/Services`, `DTOs`, `Mappings` |
| Implementacao de repositorio, configuracao EF, job | `Recipes.Infrastructure/Repositories`, `Data`, `Jobs` |
| Endpoint, middleware, configuracao do host | `Recipes.Api/Controllers`, `Middlewares`, `Program.cs` |

O registro no container fica no projeto dono do servico, nao no host: `AddApplication()` em
`Recipes.Application/DependencyInjection.cs` e `AddInfrastructure()` em
`Recipes.Infrastructure/DependencyInjection.cs`. O `Program.cs` so os chama.

## Rodando local

### Com Docker (recomendado)

O `docker-compose.dev.yml` sobe migrations, API e frontend com hot reload. Requer o
[`chefarchive-web`](https://github.com/FelipeFioreti/chefarchive-web) clonado ao lado deste repositorio
(`../chefarchive-web`).

Toda a configuracao de dev da API (connection string, `AppSettings:Secret`) fica em
`src/Recipes.Api/appsettings.Development.json`, que esta no `.gitignore` e e montado nos containers da API
e das migrations. Crie o arquivo a partir do exemplo versionado e preencha:

```bash
cp src/Recipes.Api/appsettings.Development.example.json src/Recipes.Api/appsettings.Development.json
```

O compose nao sobe banco: use a connection string de um banco de desenvolvimento. As migrations sao
aplicadas nele a cada subida.

```bash
docker compose -f docker-compose.dev.yml up --build
```

| Servico | Endereco |
|---|---|
| Frontend (`ng serve`) | http://localhost:4200 |
| API (`dotnet watch`) | http://localhost:5184 |

Alteracoes em `.cs`/`.ts` recarregam sozinhas. Migrations novas exigem rodar o servico de novo:
`docker compose -f docker-compose.dev.yml up --build migration`. Mudou o `package-lock.json` do frontend?
Recrie o volume de dependencias:
`docker compose -f docker-compose.dev.yml rm -sf web && docker volume rm chefarchive-dev_web-node-modules`.

### Sem Docker

Configuracao sensivel vem de user secrets ou variaveis de ambiente — nao de `appsettings.json`:

```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=recipes;Username=postgres;Password=postgres" --project src/Recipes.Api
dotnet user-secrets set "AppSettings:Secret" "<segredo com pelo menos 32 caracteres>" --project src/Recipes.Api
```

```bash
dotnet run --project src/Recipes.Api --launch-profile http
```

O perfil `http` sobe em `http://localhost:5184`, que e o alvo do `proxy.conf.json` do frontend
([`chefarchive-web`](https://github.com/FelipeFioreti/chefarchive-web)) e do `environment.development.ts`.
O redirecionamento para HTTPS fica desligado em Development.

### Email (recuperacao de senha)

A API publica os emails no RabbitMQ e o `Recipes.EmailWorker` envia por SMTP (MailKit). Para
desenvolvimento, suba o RabbitMQ e o [Mailpit](https://mailpit.axllent.org/), que captura os emails sem
enviar nada de verdade:

```bash
docker run -d --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:4-management
docker run -d --name mailpit -p 1025:1025 -p 8025:8025 axllent/mailpit
dotnet user-secrets set "Smtp:Host" "localhost" --project src/Recipes.EmailWorker
dotnet user-secrets set "Smtp:Port" "1025" --project src/Recipes.EmailWorker
dotnet user-secrets set "Smtp:FromAddress" "no-reply@chefarchive.local" --project src/Recipes.EmailWorker
dotnet run --project src/Recipes.EmailWorker
```

Localmente os dois lados usam o usuario padrao `guest`/`guest` do RabbitMQ (`RabbitMq:*` nos `appsettings.json`).
As filas aparecem em `http://localhost:15672` e os emails em `http://localhost:8025`. Com um provedor real,
configure tambem `Smtp:Username` e `Smtp:Password` no worker (`Smtp:Security` aceita `Auto`, `StartTls`,
`SslOnConnect` ou `None`; o padrao `Auto` escolhe pela porta). O link do email usa `AppSettings:PasswordResetUrl`
da API.

## Migrations

As migrations vivem no `Recipes.Infrastructure`, junto do `ApplicationDbContext`, mas o host continua
sendo o startup project:

```bash
dotnet ef migrations add <Nome> --project src/Recipes.Infrastructure --startup-project src/Recipes.Api
```

```bash
dotnet ef database update --project src/Recipes.Infrastructure --startup-project src/Recipes.Api
```

## Docker

Este repositorio publica as imagens `chefarchive-api`, `chefarchive-api-migrations` e
`chefarchive-email-worker` no GHCR via CI (`.github/workflows/publish-images.yml`, contexto `.`, dockerfiles
`src/Recipes.Api/Dockerfile`, `src/Recipes.Api/Dockerfile.migrations` e `src/Recipes.EmailWorker/Dockerfile`).

Para buildar a imagem da API isoladamente:

```bash
docker build -f src/Recipes.Api/Dockerfile .
```

## Deploy

Toda tag `vX.Y.Z` empurrada nesse repositorio builda e publica as imagens, depois chama a
[Action da Hostinger](https://github.com/hostinger/deploy-on-vps) para atualizar so o projeto
`chefarchive-api` na VPS (`deploy/docker-compose.yml`), sem tocar em `web`/`proxy`. O restante da
producao (proxy de borda com TLS, runbook, redes compartilhadas) vive em
[`chefarchive-infra`](https://github.com/FelipeFioreti/chefarchive-infra) (privado).

O projeto `chefarchive-api` sobe `api`, `migration`, `email-worker` e `rabbitmq`.

Requer os secrets `HOSTINGER_API_KEY`, `DATABASE_CONNECTION_STRING`, `JWT_SECRET`,
`ASPNETCORE_ENVIRONMENT`, `SMTP_HOST`, `SMTP_PORT`, `SMTP_USERNAME`, `SMTP_PASSWORD`, `SMTP_FROM_ADDRESS`,
`RABBITMQ_USERNAME` e `RABBITMQ_PASSWORD` configurados no repositorio.

## Repositorios relacionados

- [`FelipeFioreti/chefarchive-web`](https://github.com/FelipeFioreti/chefarchive-web) — frontend Angular
- [`FelipeFioreti/chefarchive-infra`](https://github.com/FelipeFioreti/chefarchive-infra) — compose de producao, nginx de borda e runbook (privado)
