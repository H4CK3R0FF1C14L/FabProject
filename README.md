# FabProject
This project is created to add all the free Quixel Megascans assets to your library 

# Build
- Install [.NET 8](https://dotnet.microsoft.com/en-us/download/)
- Than open CMD in project folder and paste this commands:
```
dotnet build
```

# How to use
- Open Browser
- Authorize on [FAB](https://fab.com)
- Open Browser Inspector -> Applications and copy all variables to the program startup parameters

![image](..\FabProject\Images\image.png)

```
FabProject.exe "{sb_csrftoken}" "{sb_sessionid}" "{OptanonAlertBoxClosed}" "{OptanonConsent}" "{cf_clearance}" "{__cf_bm}"
```