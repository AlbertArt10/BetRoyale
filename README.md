# BetRoyale.API

Backend-ul unei platforme de sports analytics construit cu ASP.NET Core Web API și PostgreSQL.

Proiectul este dezvoltat incremental și acoperă în prezent autentificare JWT, management de utilizatori și roluri, meciuri, articole, comentarii, like-uri, predicții cu punctaj și subscriptions între utilizatori și analiști.

## Stack
- .NET 8
- ASP.NET Core Web API
- Entity Framework Core
- PostgreSQL
- JWT Bearer Authentication
- Swagger / OpenAPI

## Roluri
- `Admin`
- `Analyst`
- `User`

## Funcționalități implementate

### Auth și acces
- register
- login
- generare și validare JWT
- `/api/auth/me`
- autorizare pe roluri
- integrare Swagger cu Bearer auth

### Useri și profil
- management admin pentru utilizatori
- `/api/profile/me`
- update profil pentru `FullName` și `Bio`
- `UserProfile.TotalPoints`

### Matches
- CRUD pe meciuri
- `GET` public pentru listă și detalii
- create / update / delete doar pentru `Admin`
- setare rezultat final:
  - `PUT /api/matches/{id}/result`
- scoruri:
  - `HomeScore`
  - `AwayScore`

### Articles
- listare publică articole
- detalii articol
- filtrare după match
- filtrare după autor
- create / update / delete pentru `Analyst` și `Admin`
- `Admin` poate interveni peste articolele altor analiști

### Comments
- listare publică pentru comentariile unui articol
- detalii comentariu
- create pentru orice utilizator autentificat
- update / delete pentru autor
- `Admin` poate modifica sau șterge orice comentariu

### ArticleLikes
- like
- unlike
- summary public per articol
- prevenire duplicate la nivel de bază de date și service

### Predictions și scoring
- creare predicții pentru orice utilizator autentificat
- o singură predicție per utilizator per meci
- valori suportate:
  - `Home = 1`
  - `Away = 2`
  - `Draw = 3`
- `Draw` permis doar pentru `Football`
- evaluare automată a predicțiilor la setarea rezultatului meciului
- actualizare automată `UserProfile.TotalPoints`

### Subscriptions
- subscribe la un `Analyst`
- unsubscribe
- listă cu analyst-ii urmăriți de utilizatorul curent
- listă cu subscriberii unui analyst
- prevenire duplicate
- interzis subscribe la tine însuți

## Structură proiect
- `Controllers` - HTTP endpoints
- `DTOs` - request / response contracts
- `Entities` - modele persistate
- `Data` - `AppDbContext`, migrații, seed
- `Services` - logică de business
- `Configurations` - JWT, EF Core, Swagger și alte configurări
- `Enums` - valori controlate pentru domeniu
- `Middleware` - cross-cutting concerns
- `Repositories` - rezervat pentru acces la date separat, dacă va fi introdus ulterior

## Baza de date
Proiectul folosește PostgreSQL.

Pentru dezvoltare locală există un `docker-compose.yml` care pornește containerul de PostgreSQL:
- host: `127.0.0.1`
- port: `55432`
- database: `betroyale_db`
- user: `betroyale`

Pornire DB:

```bash
docker compose up -d postgres
```

## Configurare
Nu păstra secrete reale în `appsettings.json` sau `appsettings.Development.json`.

Pentru rulare locală, setează connection string-ul prin variabilă de mediu sau user secrets.

Exemplu pentru EF Core migrations / database update:

```bash
ConnectionStrings__DefaultConnection="Host=127.0.0.1;Port=55432;Database=betroyale_db;Username=betroyale;Password=betroyale_dev_password" dotnet ef database update --project BetRoyale.API.csproj
```

Pentru JWT, în dezvoltare trebuie să ai valori valide pentru:
- `Jwt:Issuer`
- `Jwt:Audience`
- `Jwt:SecretKey`
- `Jwt:AccessTokenExpirationMinutes`

## Setup local

### 1. Restore
```bash
dotnet restore
```

### 2. Pornește PostgreSQL
```bash
docker compose up -d postgres
```

### 3. Aplică migrațiile
```bash
ConnectionStrings__DefaultConnection="Host=127.0.0.1;Port=55432;Database=betroyale_db;Username=betroyale;Password=betroyale_dev_password" dotnet ef database update --project BetRoyale.API.csproj
```

### 4. Build
```bash
dotnet build BetRoyale.API.sln
```

### 5. Run
```bash
dotnet run --project BetRoyale.API.csproj
```

Sau:

```bash
dotnet watch run
```

## Swagger
După pornirea aplicației, Swagger poate fi folosit pentru testare manuală.

Aplicația are suport pentru Bearer auth în Swagger UI, deci endpoint-urile protejate pot fi testate direct cu tokenul obținut din login.

Flux recomandat:
1. `POST /api/auth/login`
2. copiază `accessToken`
3. apasă `Authorize`
4. folosește `Bearer <token>`

## Comenzi utile
```bash
dotnet restore
dotnet build BetRoyale.API.sln
dotnet run --project BetRoyale.API.csproj
dotnet watch run
dotnet ef migrations add <MigrationName>
dotnet ef database update --project BetRoyale.API.csproj
```

## Status curent al roadmap-ului
Implementat:
- auth și acces
- user/profile management de bază
- matches
- articles
- comments
- article likes
- match result flow
- predictions și scoring
- subscriptions

În curs / următorii pași naturali:
- email notifications pentru subscriptions
- global exception middleware
- polish final și testare mai serioasă
