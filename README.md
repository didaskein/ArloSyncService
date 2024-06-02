# ArloSyncService
Allow to get videos from Arlo Basestations to be sync in the filesystem with a Windows Service in C#
When you run the services, it will get the last videos (max DayToSyncBack) from the Arlo Basestations and sync them in the filesystem.
After it use MQTT events (new file on the basestations) to get the new videos from the Arlo Basestations.

# Configure the appsettings.json :

## DeviceId
Generate a new Guid and put it in the DeviceId field (https://www.guidgenerator.com/)

## Arlo Account
Set AccountArloLogin & AccountArloPassword with the MAIN Arlo Account (It"s not possible to use a secondary account as they do not have access to basestations...)
In the example "xxxxxxxxxx@hotmail.com" is the mail account, you can create a redirection rule to send MFA to another mailbox "yyyyyARLO@gmail.com".

## Email MFA
Set AccountMailMFALogin, AccountMailMFAPassword, AccountMailMFAImapServer, AccountMailMFAImapPort
If you are using a Gmail account you need to create an App Password with https://myaccount.google.com/apppasswords

MailMFAFormEmail : it"s the email sender of the MFA code, in the example it"s "xxxxxxxxxx@outlook.com", without mail redirection "do_not_reply@arlo.com"
MailMFASubjectEmail : the sentence is in french for me, you need to change it to your language
MailMFARegexOtpToMatch :
 - With a email forward it's : "\\s*(\\d{6})\\s*</h1>"
 - Without email forward it's : "^\W*(\d{6})\W*$"


## Certfificates & Paths
 - FolderForCertificate : The folder where the certificates will be generated & stored
 - FolderForVideos : The Folder where the videos will be stored (on sub folder per date YYYYMMDD), you can set a OneDriver folder
 - DayToSyncBack: Max is 30 days, it's the number of days to sync back when you start the service