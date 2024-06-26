# progettoMicroservizi

Repository dedicata allo sviluppo del progetto del corso _Programmazione orientata ai microservizi 2023/2024_.

## Premesse

È utilizzato .NET versione 8.0 e Visual Studio Code come IDE. I pacchetti vengono quindi aggiunti manualmente tramite

```bash
dotnet add package [PACKAGE_NAME] -v 8.0
```

Ogni microservizio è sviluppato nella propria cartella e contiene un proprio database indipendente dagli altri; ognuno di questi viene avviato su un container dedicato.

## Scopo del progetto

L'insieme dei microservizi gestisce una __libreria virtuale__ di spartiti musicali in formato `pdf`. Gli spartiti sono scaricabili da chiunque mentre l'upload è associato ad un utente registrato. Ad ogni utente registrato è associata anche un'immagine di profilo ugualmente scaricabile.

Di seguito l'elenco dei microservizi e una breve descrizione:

1. `AuthorsHandler`: gestisce gli autori degli spartiti musicali e le informazioni associate; viene comunicata in maniera asincrona eventuale aggiunta/aggiornamento/eliminazione di un autore.
2. `UsersHandler`: gestisce gli utenti registrati e le informazioni associate; viene comunicata in maniera asincrona eventuale aggiunta/aggiornamento/eliminazione di un utente.
3. `MusicalScoresHandler`: gestisce gli spartiti musicali e i files `pdf` a loro associati. Implementa un sistema di caching automatico sfruttando __Kafka__ e __ClientHttp__ per gestire internamente una lista di autori e utenti esistenti nel sistema; se non viene trovata una corrispondenza locale al microservizio, viene effettuato un check tramite una chiamata _http_ al rispettivo microservizio.

## Per eseguire

Nella cartella `progettoMicroservizi` eseguire da terminale il comando:

```bash
docker compose up -d
```

per avviare tutti i microservizi e relativi DBs.

Di seguito gli URL per connettersi alle API tramite browser web:

- [adminer](http://localhost:8080)

  Per connettersi utilizzare i parametri:
  - Sistema: `PostgreSQL`
  - Server: `[ authors_dbms | users_dbms | scores_dbms ]`
  - Utente: `postgres`
  - Password: `password`
  - Database: `postgres`

- [authors_API](http://localhost:5000/swagger/index.html)
- [users_API](http://localhost:5001/swagger/index.html)
- [musicalScores_API](http://localhost:5002/swagger/index.html)

## Infrastruttura di rete

Segue la mappatura di rete utilizzata:

- adminer 8080:8080
- authors_dbms 10000:5432
- authors_microservice 5000:5082
- users_dbms 10001:5432
- users_microservice 5001:5082
- scores_dbms 10002:5432
- scores_microservice 5002:5082

> N.B.
> Una volta che un microservizio è lanciato in maniera containerizzata, la __connectionString__ cambia in quanto non si usa più `Host=localhost` ma il nome del microservizio, ad es. `Host=authors_dbms` (si occupa docker di fare da DNS). Per questo motivo ci sono due diversi `appsettings.json` per ogni _Api_.

## Database

### Link utili

- [.NET 6.0 - Connect to PostgreSQL Database with Entity Framework Core](https://jasonwatmore.com/post/2022/06/23/net-6-connect-to-postgresql-database-with-entity-framework-core)
- [Official PostgreSQL docker image](https://hub.docker.com/_/postgres)

### How to

È necessario come prima cosa installare il pacchetto NuGet che permette di utilizzare Entity Framework con PostgreSQL:

```bash
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL -v 8.0
```

#### Aggiungere query iniziali

Si possono aggiungere scripts `*.sql`, `*.sql.gz`, o `*.sh` nella cartella `/docker-entrypoint-initdb.d`.

> **Nota Bene**
>
> Il contenuto di questa cartella viene eseguito solo se il container viene lanciato con una cartella dati vuota (ossia `/var/lib/postgresql/data`); quindi se una query di uno script non va a buon fine e provate a rilanciare il container, gli scripts verranno saltati.

#### Connessione PGAdminer

Nel campo _Server_ bisogna indicare il nome di un servizio scelto nel docker-compose della solution (`authors_dbms` ad esempio); infatti viene creata automaticamente una rete __nomeSolution_database__ di tipo bridge a cui sono collegati i vari container.
