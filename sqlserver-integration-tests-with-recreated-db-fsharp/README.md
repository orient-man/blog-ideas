Testy integracyjne wymagają SQL Server (może być Express).

Dodatkowo uruchomienie ich z TeamCity wymagało konfiguracji uprawnień (dla agenta).
W trybie *single user* (przełącznik `-m` w ustawieniach startowych SQL Server), należy:

    > sqlcmd -S <machine name>\SQLEXPRESS
    > CREATE USER [NT AUTHORITY\SYSTEM] FOR LOGIN [NT AUTHORITY\SYSTEM] WITH DEFAULT_SCHEMA=[master];
    > GO
    > EXEC sp_addsrvrolemember 'NT AUTHORITY\SYSTEM', 'sysadmin';
    > GO
