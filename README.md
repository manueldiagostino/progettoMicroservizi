# progettoMicroservizi
Repository dedicata allo sviluppo del progetto del corso _Programmazione orientata ai microservizi 2023/2024_.

## Premesse
È utilizzato .NET versione 8.0 e Visual Studio Code come IDE. I pacchetti vengono quindi aggiunti manualmente tramite
```bash
dotnet add package [PACKAGE_NAME] -v 8.0
```

Ogni microservizio è sviluppato nella propria cartella e contiene un proprio database indipendente dagli altri; ognuno di questi viene avviato su un container dedicato. 

Per connettersi con PGAdminer bisogna digitare come nome del server il nome del servizio indicato sul `docker-compose.yaml` (viene creata un'apposita network di tipo _bridge_, denominata __database_default__ dall'immagine contenente PostgreSQL).

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

---

## Infrastruttura di rete
Segue la mappatura di rete utilizzata:
- adminer 8080:8080
- authors_dbms 10000:5432
- authors_microservice 5000:5082

> N.B.
> Una volta che un microservizio è lanciato in maniera containerizzata, la __connectionString__ cambia in quanto non si usa più `Host=localhost` ma il nome del microservizio, ad es. `Host=authors_dbms` (si occupa docker di fare da DNS). Per questo motivo ci sono due diversi `appsettings.json` per ogni _Api_.

---

## Per eseguire
Nella cartella `progettoMicroservizi` eseguire da terminale il comando:
```bash
docker compose up -d
```
per lanciare tutti i microservizi e relativi DBMS.

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