--BACKUP
BACKUP DATABASE Gringotts
TO DISK = 'C:\src\github\Gringotts\src\Gringotts.bak'
   WITH FORMAT,
      MEDIANAME = 'c_backups',
      NAME = 'Gringotts';

--RESTORE
ALTER DATABASE Gringotts SET SINGLE_USER WITH ROLLBACK IMMEDIATE
GO
RESTORE DATABASE Gringotts 
FROM DISK = 'C:\src\github\Gringots\src\Gringots.bak' 
   WITH FILE=1;