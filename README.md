# Recipes

API de receitas em .NET 10 com frontend Angular. O backend segue Clean Architecture, com cada camada
em um projeto separado e as regras de dependencia verificadas pelo compilador.

## Estrutura

```
Recipes.sln
src/
  Recipes.Domain/          entidades, excecoes e contratos    -> sem dependencias
  Recipes.Application/     casos de uso, DTOs e mapeamentos   -> Domain
  Recipes.Infrastructure/  EF Core, repositorios, jobs        -> Application, Domain
  Recipes.Api/             controllers, middlewares, host     -> Infrastructure, Application, Domain
```

A seta aponta sempre para dentro. O `Recipes.Domain` nao referencia nenhum projeto e nao tem nenhum
pacote NuGet: se algo la dentro precisar de EF Core ou de ASP.NET, esta na camada errada.

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

Este repositorio publica as imagens `chefarchive-api` e `chefarchive-api-migrations` no GHCR via CI
(`.github/workflows/publish-images.yml`, contexto `.`, dockerfiles `src/Recipes.Api/Dockerfile` e
`src/Recipes.Api/Dockerfile.migrations`).

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

Requer os secrets `HOSTINGER_API_KEY`, `DATABASE_CONNECTION_STRING`, `JWT_SECRET` e
`ASPNETCORE_ENVIRONMENT` configurados no repositorio.

## Repositorios relacionados

- [`FelipeFioreti/chefarchive-web`](https://github.com/FelipeFioreti/chefarchive-web) — frontend Angular
- [`FelipeFioreti/chefarchive-infra`](https://github.com/FelipeFioreti/chefarchive-infra) — compose de producao, nginx de borda e runbook (privado)
